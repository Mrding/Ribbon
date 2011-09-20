using System;
using System.Collections;
using System.Collections.Generic;
using Luna.Common;
using Luna.Common.Constants;
using Luna.Common.Extensions;
using Luna.Core;
using Luna.Data;
using Luna.Data.Transform;
using Luna.Shifts.Data.Listeners;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System.Diagnostics;
using Luna.Core.Extensions;

namespace Luna.Shifts.Data.Impl.Repositories
{
    public class TimeBoxRepository : Repository<TimeBox>, ITimeBoxRepository
    {
        public IList<TimeBox> GetTimeBoxes(string[] seatIds, Guid[] excludedEmployeeIds, DateTime start, DateTime end)
        {
            if (seatIds.If(o => o.Length > 0) == true)
            {
                var sw = Stopwatch.StartNew();

                Session.EnableFilter("LargeRange").SetParameter("start", start.AddDays(-1))
                                                  .SetParameter("end", end.AddDays(1));

                var query = Session.CreateQuery(
                    string.Format(@"from TimeBox t inner join fetch t.Agent where t.Id in ( 
                                                    select distinct t.EmployeeId 
                                                    from Term t 
                                                    where t.Seat in (:seatIds) and {0}t.End >:start and t.Start <:end))",
                    excludedEmployeeIds.Length > 0 ? "t.EmployeeId not in (:excludedEmployeeIds) and " : string.Empty))
                    .SetParameterList("seatIds", seatIds)
                    .SetDateTime("start", start)
                    .SetDateTime("end", end);

                if (excludedEmployeeIds.Length > 0)
                    query = query.SetParameterList("excludedEmployeeIds", excludedEmployeeIds);

                var results = query.List<TimeBox>();

                Session.DisableFilter("LargeRange");

                sw.Stop();

                return results;
            }
            return new List<TimeBox>(0);
        }

        public override IList<TimeBox> GetAll()
        {
            var results = Session.CreateQuery("select t from TimeBox t")
               .SetResultTransformer(new DistinctRootEntityResultTransformer())
               .List<TimeBox>();

            return results;
        }

        

        private IQuery GetAgentsBySchedule(Entity campaign, DateTime start, DateTime end)
        {
            var query = Session.CreateQuery(
               @"select new Agent(t,a) from TimeBox t inner join fetch t.Agent ,
                 Attendance a where a.Agent = t.Agent and a.Campaign = :campaign and a.Start >= :st and a.End <= :end order by t.Agent.Name")
               .SetDateTime("st", start)
               .SetDateTime("end", end)
              .SetGuid("campaign", campaign.Id);
            return query;
        }

        private IQuery GetAgentsBySchedule(Entity campaign, DateTime start, DateTime end, Type instance)
        {
            var query = Session.CreateQuery(
               string.Format(@"select new {0}(t,a) from TimeBox t inner join fetch t.Agent ,
                 Attendance a where a.Agent = t.Agent and a.Campaign = :campaign and a.Start >= :st and a.End <= :end order by t.Agent.Name", instance.Name))
               .SetDateTime("st", start)
               .SetDateTime("end", end)
               .SetGuid("campaign", campaign.Id);
            return query;
        }

        private IQuery GetTimeBoxesBySchedule(Entity campaign, DateTime start, DateTime end)
        {
            var query = Session.CreateQuery(
               @"select t from TimeBox t inner join fetch t.Agent ,
                 Attendance a where a.Agent = t.Agent and a.Campaign = :campaign and ((a.End >:st and a.Start <:end) or (a.Start =:st and a.End =:end)) order by t.Agent.Name")
               .SetDateTime("st", start)
               .SetDateTime("end", end)
               .SetGuid("campaign", campaign.Id);

            return query;
        }

        private void EnableRangeFilter(DateTime start, DateTime end, int offSetDay)
        {
            Session.EnableFilter("LargeRange").SetParameter("start", start.AddDays(-offSetDay))
                           .SetParameter("end", end.AddDays(offSetDay));
        }

        public void DeletedTerms(string[] employees, DateTime from, DateTime to)
        {

        }

        public IList<TimeBox> GetTimeBoxesFrom(Entity campaign, DateTime start, DateTime end)
        {
            EnableRangeFilter(start, end, 1);
            var results = GetTimeBoxesBySchedule(campaign, start, end).List<TimeBox>();
            Session.DisableFilter("LargeRange");

            return results;
        }

        public IList<IAgent> GetAgentsFrom<TAgent>(Entity campaign, DateTime start, DateTime end)
        {
            var sw = Stopwatch.StartNew();

            var results = new List<IAgent>();
            var range = new DateRange(start, end);
            //var dateStorage = range.CreateDateIndexer();
            EnableRangeFilter(start, end, 7);
            GetAgentsBySchedule(campaign, start, end, typeof(TAgent))
                .SetResultTransformer(new ResultTransformer<TAgent>(
                            tuple => new Dictionary<string, object>
                                          {
                                              { "timeBox", tuple[0] }, 
                                              { "attendance", tuple[1] }, 
                                              { "enquiryRange", range }
                }))
                .List(new ActionableList<IAgent>(o =>
                            {
                                results.Add(o);
                            }));
            Session.DisableFilter("LargeRange");

            sw.Stop();
            Console.WriteLine("GetAgentsFrom row({0}) Elapsed: {1}s", results.Count, sw.Elapsed.TotalSeconds);

            return results;
        }

        public TAgent GetAgentFrom<TAgent>(string[] text, DateTime start, DateTime end) where TAgent : IAgent
        {
            //var sw = Stopwatch.StartNew();

            IAgent foundAgent = null;
            var range = new DateRange(start, end);
            //var dateStorage = range.CreateDateIndexer();
            EnableRangeFilter(start, end, 7);
            Session.CreateQuery(string.Format(@"select new {0}(t) from TimeBox t inner join fetch t.Agent a where a.AgentId in (:text)", typeof(TAgent).Name))
                .SetParameterList("text", text)
                .SetResultTransformer(new ResultTransformer<TAgent>(
                tuple => new Dictionary<string, object>
                              {
                                  { "timeBox", tuple[0] },
                                  { "start", start },
                                  { "end", end }
                              }))
                .List(new ActionableList<IAgent>(o =>
                        {
                            if (!o.Schedule.IsNew())
                                foundAgent = o;
                        }));

            Session.DisableFilter("LargeRange");

            //sw.Stop();

            return foundAgent.As<TAgent>();
        }

        public void Detach(object entity)
        {
            //Session.Clear();
            Session.Evict(entity);
        }

        

        public void SaveLog(Term term, TimeBox timeBox)
        {
            Session.Save(new DeletedTerm(term, timeBox.Id));
        }

        public IAgent GetAgent(Attendance attendance, Type agentInstanceType)
        {
            var type = agentInstanceType.BaseType;

            EnableRangeFilter(attendance.Start, attendance.End, 7);
            var query = Session.CreateQuery(string.Format(@"select new {0}(t,a) from TimeBox t inner join fetch t.Agent ,
                                              Attendance a where a.Id =:attendanceId and a.Agent = t.Agent", type.Name))
                .SetInt32("attendanceId", attendance.Id);

            var agent = query.SetResultTransformer(new ResultTransformer(
                tuple => new Dictionary<string, object>
                              {
                                  { "timeBox", tuple[0] }, 
                                  { "attendance", tuple[1] }
                              }, type)).UniqueResult<IAgent>();

            Session.DisableFilter("LargeRange");

            return agent;
        }

        public IList<TimeBox> GetTimeBoxesByRange(string[] agentAcdids, DateTime start, DateTime end)
        {
            if (agentAcdids.Length == 0)
                return new List<TimeBox>();
            EnableRangeFilter(start, end, 1);

            var employeeIds = Session.CreateSQLQuery(
                 string.Format("select distinct EmployeeId from EmployeeSkill where AgentAcdid in ('{0}')",
                 string.Join("','", agentAcdids))).List();
            if (employeeIds.Count == 0)
                return new List<TimeBox>();

            var results = Session.CreateQuery("from TimeBox t left outer join fetch t.TermSet term where t.Agent.Id in (:employeeIds) order by t.Agent.Name")
                .SetParameterList("employeeIds", employeeIds)
                .SetResultTransformer(new DistinctRootEntityResultTransformer())
                .List<TimeBox>();

            Session.DisableFilter("LargeRange");

            return results;
        }

        public IList<TimeBox> GetTimeBoxesByRange(Guid[] employeeIds, DateTime start, DateTime end)
        {
            if (employeeIds.Length == 0)
                return new List<TimeBox>();
            EnableRangeFilter(start, end, 1);

            var results = Session.CreateQuery("from TimeBox t inner join fetch t.Agent where t.Agent.Id in (:employeeIds) order by t.Agent.Name")
                .SetParameterList("employeeIds", employeeIds)
                .SetResultTransformer(new DistinctRootEntityResultTransformer())
                .List<TimeBox>();

            Session.DisableFilter("LargeRange");

            return results;
        }

        public TimeBox GetTermsWithEndSibling(Guid timeBoxId, DateTime start, DateTime end)
        {
            //大于End中最小的endValue
            var endValue = GetCriticalEndTime(timeBoxId, end);
            return TeamFilter(timeBoxId, start, endValue, "Range");
        }

        private DateTime GetCriticalEndTime(Guid timeBoxId, DateTime time)
        {
            //大于End中最小的endValue
            var endValue = Session.CreateCriteria<TimeBox>()
                .Add(Restrictions.IdEq(timeBoxId))
                .CreateAlias("TermSet", "t")
                .Add(Restrictions.Eq("t.Level", 0))
                .Add(Restrictions.Gt("t.End", time))
                .SetProjection(Projections.Min("t.End"))
                .UniqueResult<DateTime>();
            if (endValue == DateTime.MinValue)
            {
                endValue = time;
            }
            return endValue;
        }

        private Luna.Core.Tuple<DateTime, DateTime> GetTime(Guid timeBoxId, DateTime start, DateTime end)
        {
            //小于start的最大的startValue
            var startValue = Session.CreateCriteria<TimeBox>()
                .Add(Restrictions.IdEq(timeBoxId))
                .CreateAlias("TermSet", "t")
                .Add(Restrictions.Eq("t.Level", 0))
                .Add(Restrictions.Lt("t.Start", start))
                .SetProjection(Projections.Max("t.Start"));
            //大于End中最小的endValue
            var endValue = Session.CreateCriteria<TimeBox>()
                .Add(Restrictions.IdEq(timeBoxId))
                .CreateAlias("TermSet", "t")
                .Add(Restrictions.Eq("t.Level", 0))
                .Add(Restrictions.Gt("t.End", end))
                .SetProjection(Projections.Min("t.End"));

            var query = Session.CreateMultiCriteria().Add(startValue).Add(endValue).List();
            var a = ((IList)(query[0]))[0] == null ? start : (DateTime)((IList)(query[0]))[0];
            var b = ((IList)(query[1]))[0] == null ? end : (DateTime)((IList)(query[1]))[0];


            return new Core.Tuple<DateTime, DateTime>(a, b);
        }

        public TimeBox GetTermsWithSiblings(Guid timeBoxId, DateTime start, DateTime end)
        {
            var value = GetTime(timeBoxId, start, end);
            return TeamFilter(timeBoxId, value.Item1, value.Item2, "MiddleRange");
        }

        public TimeBox GetTermsWithSiblings(Guid timeBoxId, DateTime time)
        {
            return GetTermsWithSiblings(timeBoxId, time, time);
        }

        public IList<TimeBox> Find(DateRange watchRange, DateRange editRange)
        {
            EnableRangeFilter(watchRange.Start, watchRange.End, 1);

            var results = Session.CreateQuery("from TimeBox").List<TimeBox>();

            Session.DisableFilter("LargeRange");

            var resultsCount = results.Count;
            for (var i = 0; i < resultsCount; i++)
                results[i].Boundary = editRange;

            return results;
        }

        public TimeBox GetTimeBoxByTermId(long termId)
        {
            var term = Session.Get<Term>(termId);
            if (term == null)
            {
                return null;
            }
            var timeBoxId = Session.CreateCriteria<TimeBox>()
                 .CreateCriteria("TermSet", "t")
                .Add(Restrictions.IdEq(termId))
                .SetProjection(Projections.Property("Id"))
                .SetResultTransformer(new DistinctRootEntityResultTransformer())
                .UniqueResult<Guid>();


            Session.EnableFilter("MiddleRange").SetParameter("start", term.Start)
                             .SetParameter("end", term.End);

            var timeBox = Session.CreateQuery("from TimeBox where Id=:timeboxId")
                .SetGuid("timeboxId", timeBoxId)
                .UniqueResult<TimeBox>();
            timeBox.Boundary = new DateRange(term.Start, term.End);
            Session.DisableFilter("MiddleRange");

            return timeBox;
        }

        public TimeBox GetTermsWithMiddleRange(Guid employeeId, DateTime start, DateTime end)
        {
            return TeamFilter(employeeId, start, end, "MiddleRange");
        }
        public TimeBox GetTermsWithTouchRange(Guid employeeId, DateTime start, DateTime end)
        {
            return TeamFilter(employeeId, start, end, "Range");
        }
        public TimeBox GetTermsWithCurrentDay(Guid employeeId, DateTime date)
        {
            return TeamFilter(employeeId, date, DateTime.MaxValue, "CurrentDate");
        }

        private TimeBox TeamFilter(Guid employeeId, DateTime start, DateTime end, string filtername)
        {
            if (end == DateTime.MaxValue)
            {
                Session.EnableFilter(filtername).SetParameter("start", start);
            }
            else
            {
                Session.EnableFilter(filtername).SetParameter("start", start).SetParameter("end", end);
            }
            var timeBox = Session.CreateQuery("from TimeBox where Id=:timeboxId")
                .SetGuid("timeboxId", employeeId)
                .UniqueResult<TimeBox>();

            Session.DisableFilter(filtername);
            timeBox.Boundary = new DateRange(start, end);
            return timeBox;
        }

        public IList<TimeBox> GetTermsWithCurrentDay(Guid employeeAId, Guid employeeBId, DateTime date)
        {
            Session.EnableFilter("CurrentDate").SetParameter("start", date);

            var timeBox = Session.CreateQuery("from TimeBox where Id in (:timeboxId)")
               .SetParameterList("timeboxId", new[] { employeeAId, employeeBId })
               .List<TimeBox>();

            Session.DisableFilter("CurrentDate");
            return timeBox;
        }

        public IList<long> GetExsitTerms(List<long> ids)
        {
            var result = Session.CreateSQLQuery("select termid from Term where termid in (:ids) and timeboxid is not null")
                .SetParameterList("ids", ids)
                .List<long>();

            return result;
        }

        public void EnableBackup(bool value)
        {
            if (value)
                TermBackupListener.Enable();
            else
                TermBackupListener.Disable();
        }

        public string BatchDeleteTerm(TimeBox timeBox, string @operator, long[] ids)
        {
            if (ids.Length == 0) return string.Empty;

            using (var s = Factory.OpenStatelessSession())
            {
                using (var tx = s.BeginTransaction())
                {
                    var resultCount = s.CreateQuery(@"select count(o) from TimeBox o where o.Id = :timeBoxId")
                        //and (o.Operator = :operator or o.Operator is null)")
                        .SetGuid("timeBoxId", timeBox.Id)
                        //.SetString("operator", @operator)
                        .UniqueResult<Int64>();

                    if (resultCount > 0)
                    {
                        var alterBy = string.Format("{0}@{1:yyyy/MM/dd HH:mm:ss.fff}",
                                                 ApplicationCache.Get<string>(Global.LoggerId),
                                                 DateTime.Now);
                        s.CreateQuery(@"delete from Term o where o.Id IN (:ids)")
                                .SetParameterList("ids", ids)
                                .ExecuteUpdate();
                        //s.CreateQuery("update TimeBox o set o.Operator = :alterBy where o.Id = :timeBoxId")
                        //    .SetGuid("timeBoxId", timeBox.Id)
                        //    .SetString("alterBy", alterBy)
                        //    .ExecuteUpdate();
                        tx.Commit();
                        return alterBy;
                    }
                    return string.Empty;
                }
            }
        }
    }
}
