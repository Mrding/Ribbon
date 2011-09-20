using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Luna.Common;
using Luna.Data;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain;

namespace Luna.Shifts.Data.Impl.Repositories
{
    public class AgentStatusRepository : Repository<AgentStatus>, IAgentStatusRepository
    {
        private readonly IEntityFactory _entityFactory;

        public AgentStatusRepository(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public Dictionary<string, AgentStatus> Search(Entity area, DateTime time, Dictionary<string, AgentStatusType> agentStatusTypes)
        {
            var sqlStatement = string.Format(
                @"select AgentStatus.[TimeStamp], AgentStatus.ExtNo, AgentStatus.AgentACDId ,AgentStatus.AgentStatusTypeCode
                  from AgentStatus INNER JOIN Seat 
                  ON AgentStatus.ExtNo = Seat.ExtNo INNER JOIN AgentStatusType 
                  ON AgentStatus.AgentStatusTypeCode = AgentStatusType.AgentStatusTypeCode
                                                         and Seat.AreaId = '{0}'
                                                         and AgentStatus.AgentStatusId in (
                                                                       SELECT AgentStatusId
                                                                             FROM AgentStatus a,(SELECT MAX(TimeStamp) as TimeStamp, AgentACDId
                                                                             FROM AgentStatus 
                                                                             WHERE TimeStamp >= '{1:yyyy/MM/dd HH:mm:ss}' and TimeStamp <= '{2:yyyy/MM/dd HH:mm:ss}'                                                                              
                                                                             GROUP BY AgentACDId) b
                                                                             WHERE a.TimeStamp = b.TimeStamp and a.AgentACDId = b.AgentACDId)
                                                order by  AgentStatus.TimeStamp desc", area.Id, time.AddHours(-12), time);

            var agentLastActivity = new Dictionary<string, IAgentStatus>();
            var seatLastActivity = new Dictionary<string, AgentStatus>();

            ToLightweightAgentStatus(sqlStatement, o =>
                                                       {
                                                           agentLastActivity[o.AgentAcdid] = o;
                                                       }, () => _entityFactory.Create<AgentStatus>());

            var removeLogoutStatus = new Action(() => { });

            foreach (var item in agentLastActivity)
            {
                var agentStatus = ((AgentStatus)item.Value).SetAgentStatusType(agentStatusTypes);
                if (agentStatus.IsLogout())
                {
                    var key = item.Key;
                    removeLogoutStatus += () => { agentLastActivity[key] = default(IAgentStatus); };
                    continue;
                }
                seatLastActivity[item.Value.ExtNo] = agentStatus;
            }
            removeLogoutStatus();

            return seatLastActivity;
        }

        public IList<AgentStatus> Search(string extNo, DateTime start, DateTime end)
        {
            return Session.CreateQuery(@"from AgentStatus a where 
                                         a.ExtNo =:extNo and a.TimeStamp >= :start and a.TimeStamp <= :end order by a.TimeStamp desc")
                          .SetDateTime("start", start)
                          .SetDateTime("end", end)
                          .SetString("extNo", extNo)
                          .List<AgentStatus>();
        }

        public IEnumerable<AgentStatus> Search(string[] agentAcdIds, DateTime start, DateTime end)
        {
            var sw = Stopwatch.StartNew();

            if (agentAcdIds == null || agentAcdIds.Length == 0) return new List<AgentStatus>();

            var results = Session.CreateQuery(@"from AgentStatus a where a.TimeStamp >= :start and a.TimeStamp <= :end and a.AgentAcdid in (:agentAcdids) 
                                               ")
                .SetParameterList("agentAcdids", agentAcdIds)
                .SetDateTime("start", start)
                .SetDateTime("end", end)
                .List<AgentStatus>();

            sw.Stop();
            Console.WriteLine("Get AgentStatus Elapsed {0}s", sw.Elapsed.TotalSeconds);

            return results;
        }

        public void FastSearch(string[] agentAcdIds, DateTime start, DateTime end, Action<IAgentStatus> loopingDelegate)
        {
#if DEBUG
            var sw = Stopwatch.StartNew();
#endif

            if (agentAcdIds == null || agentAcdIds.Length == 0) return;

            var condition = string.Format(@" from AgentStatus inner join AgentStatusType 
                                                on AgentStatus.AgentStatusTypeCode = AgentStatusType.AgentStatusTypeCode
                                                where AgentAcdid in ('{0}') 
                                                and [TimeStamp] >= '{1:yyyy/MM/dd HH:mm}' and [TimeStamp] <= '{2:yyyy/MM/dd HH:mm}'"
                    , string.Join("','", agentAcdIds), start, end);

            var selectSql = string.Format(@"; select [TimeStamp], ExtNo, AgentACDId, AgentStatusType.OnService {0} 
                                                                      order by  [TimeStamp], AgentStatus.AgentStatusId", condition);


            ToLightweightAgentStatus(selectSql, loopingDelegate, () => new LightweightAgentStatus());

#if DEBUG
            sw.Stop();
            Console.WriteLine("Get LightweightAgentStatus Elapsed {0}s", sw.Elapsed.TotalSeconds);
#endif

        }

        private static void ToLightweightAgentStatus<T>(string sqlStatement, Action<IAgentStatus> loopingDelegate, Func<T> newInstance) where T : IAgentStatus
        {
            var reader = NHibernateManager.ExecuteReader(sqlStatement);
            while (reader.Read())
            {
                var time = reader.GetDateTime(0);
                var extNo = reader.GetString(1);
                var agentAcdid = reader.GetString(2);
                var option = reader[3];

                //loopingDelegate(new T().SetProperties(time, extNo, agentAcdid, option)); old
                loopingDelegate(newInstance().SetProperties(time, extNo, agentAcdid, option));
            }

            NHibernateManager.CloseReader(reader);
        }



        public LastStatusDto GetLastStatus(Employee agent, DateTime currentTime)
        {
            var lastStatusDto = new LastStatusDto
                                    {
                                        Employee = agent,
                                        AgentAcdid = string.Empty,
                                        TimeStamp = DateTime.MinValue,
                                        StatusName = string.Empty,
                                        ExtNo = string.Empty,
                                        AreaName = string.Empty,
                                        SeatNo = String.Empty
                                    };

            var r = Session.BeginTransaction();
            var agentStatus = Session.CreateQuery(
                "from AgentStatus a where a.AgentAcdid in (:acdid) and a.TimeStamp<=:currenttime order by a.TimeStamp desc")
                .SetParameterList("acdid", agent.AgentAcdids.ToArray())
                .SetDateTime("currenttime", currentTime)
                .SetMaxResults(1)
                .UniqueResult<AgentStatus>();
            if (agentStatus != null)
            {
                lastStatusDto.AgentAcdid = agentStatus.AgentAcdid;
                lastStatusDto.TimeStamp = agentStatus.TimeStamp;
                lastStatusDto.StatusName = agentStatus.AgentStatusType.StatusName;
                lastStatusDto.ExtNo = agentStatus.ExtNo;
                var seat =
                    Session.CreateQuery("from Seat s inner join fetch s.Area where s.ExtNo=:extNo").SetString("extNo", agentStatus.ExtNo).
                        UniqueResult<Seat>();
                if (seat != null)
                {
                    lastStatusDto.AreaName = seat.Area.Name;
                    lastStatusDto.SeatNo = seat.Number;
                }
            }
            r.Commit();
            return lastStatusDto;
        }
    }
}