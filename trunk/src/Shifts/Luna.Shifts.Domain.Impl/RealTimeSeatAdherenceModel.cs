using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Core.Extensions;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain.Model;
using uNhAddIns.Adapters;
using Luna.Common.Extensions;

namespace Luna.Shifts.Domain.Impl
{
    [PersistenceConversational(MethodsIncludeMode = MethodsIncludeMode.Implicit)]
    public class RealTimeSeatAdherenceModel : IRealTimeSeatAdherenceModel
    {
        private readonly IAgentStatusRepository _agentStatusRepository;
        private readonly ISeatBoxRepository _seatBoxRepository;
        private readonly IAreaRepository _areaRepository;
        private readonly ITimeBoxRepository _timeBoxRepository;
        private readonly ITermStyleRepository _termStyleRepository;

        private DateRange _watchRange;
        private DateTime _watchPoint;
        private IDictionary<string, AgentStatus> _agentStatusRecords;
        private IDictionary<string, Occupation> _planned;
        private IDictionary<string, TimeBox> _foundAgents;

        public RealTimeSeatAdherenceModel(IAgentStatusRepository agentStatusRepository, ISeatBoxRepository seatBoxRepository,
            IAreaRepository areaRepository, ITimeBoxRepository timeBoxRepository, ITermStyleRepository termStyleRepository)
        {
            _agentStatusRepository = agentStatusRepository;
            _seatBoxRepository = seatBoxRepository;
            _areaRepository = areaRepository;
            _timeBoxRepository = timeBoxRepository;
            _termStyleRepository = termStyleRepository;
        }

        public IEnumerable<TermStyle> GetAllEventTypes()
        {
            return _termStyleRepository.GetEventTypes();
        }

        [PersistenceConversation(ConversationEndMode = EndMode.Continue)]
        public void BuildSeats(Entity area, Action<IEnumerable<ISeat>> buildSeats)
        {
            buildSeats(_areaRepository.Get(area.Id).Seats);

        }

        [PersistenceConversation(ConversationEndMode = EndMode.Continue)]
        public IList<TimeBox> Save(IList<TimeBox> timeBoxs)
        {
            _timeBoxRepository.Clear();
            return _timeBoxRepository.GetTimeBoxesByRange(timeBoxs.Select(o => o.Agent.Id).ToArray(), _watchRange.Start,
                                                   _watchRange.End);

        }

        [PersistenceConversation(ConversationEndMode = EndMode.Continue)]
        public IList<AgentStatus> GetAgentStatusHistory(string extNo, DateTime start, DateTime end)
        {
            return _agentStatusRepository.Search(extNo, start, end);
        }

        public LastStatusDto GetLastStatus(Employee agent, DateTime currentTime)
        {
            return _agentStatusRepository.GetLastStatus(agent, currentTime);
        }

        [PersistenceConversation(ConversationEndMode = EndMode.Continue)]
        public void SetMonitoringArea(Entity area, DateTime watchPoint, Dictionary<string, AgentStatusType> agentStatusTypes)
        {
            _watchPoint = watchPoint;
            _planned = new Dictionary<string, Occupation>();
            _watchRange = new DateRange(_watchPoint.AddHours(-12), _watchPoint.AddHours(12));

            _agentStatusRecords = _agentStatusRepository.Search(area, watchPoint, agentStatusTypes);
            _seatBoxRepository.Search(area, watchPoint, (extNo, o) => { _planned[extNo] = o; });

            _foundAgents = _timeBoxRepository.GetTimeBoxesByRange(_agentStatusRecords.Values.Where(o => !o.AgentStatusType.IsLogout).Select(o => o.AgentAcdid).ToArray(),
                 _watchRange.Start, _watchRange.End).ToDistinctDictionary(o =>
                 {
                     var key = _agentStatusRecords.Values.FirstOrDefault(v => o.Agent.AgentAcdids.Contains(v.AgentAcdid));
                     if (key != null)
                         return key.AgentAcdid;
                     return o.Agent.AgentId; // TODO: when agentStatus's agentAcdid is invalid not completed yet
                 });
        }

        [PersistenceConversation(Exclude = true)]
        public void LoadData(IAgentSeatModel agentSeat)
        {
            var extNo = agentSeat.Profile.ExtNo;

            if (_agentStatusRecords.ContainsKey(extNo))
            {
                agentSeat.AgentStatus = _agentStatusRecords[extNo];

                if (_foundAgents.ContainsKey(agentSeat.AgentStatus.AgentAcdid))
                {
                    agentSeat.CurrentAgent = _foundAgents[agentSeat.AgentStatus.AgentAcdid];

                    //var sliceTerms = agentSeat.CurrentAgent.TermSet.SliceIntoPieces();
                    //FillTerminalGap(sliceTerms, _watchRange.Start, _watchRange.End);
                    //agentSeat.CurrentActivity =
                    //    filledTerminalGapTerms.LastOrDefault(o => o.Start <= _watchPoint && o.End > _watchPoint);

                    var found = agentSeat.CurrentAgent.TermSet.Retrieve<Term>(_watchPoint, _watchPoint)
                        .LastOrDefault(o => _watchPoint.IsInTheRange(o));

                    if (found != null)
                    {
                        agentSeat.CurrentActivity = new RtaaSlicedTerm()
                        {
                            Start = found.Start,
                            End = found.End,
                            Level = found.Level,
                            OnService = found.OnService,
                            Text = found.Text
                        };
                    }
                    else
                    {
                        agentSeat.CurrentActivity = new RtaaSlicedTerm()
                        {
                            Start = _watchRange.Start,
                            End = _watchRange.End,
                            Level = -1,
                            OnService = false
                        };
                    }
                }
                else
                {
                    //anonymous timebox 
                    agentSeat.CurrentAgent = new TimeBox(new DateRange(_watchRange.Start, _watchRange.End), new Employee()); // TODO:rewrite anonymous timebox instance
                    agentSeat.CurrentActivity = null;
                }

                agentSeat.ShowOn = _agentStatusRepository.GetLastStatus;
            }
            else
                agentSeat.StatusNotFound();

            var occupation = _planned.ContainsKey(extNo) ? _planned[extNo] : default(Occupation);

            agentSeat.Arrangement = occupation as SeatArrangement;//_seatBoxRepository.GetPlannedSeatArrangement(_plannedSeat[extNo].Seat, _watchPoint);
            agentSeat.SeatEvent = occupation as SeatEvent;

        }


        private static IEnumerable<RtaaSlicedTerm> FillTerminalGap(IEnumerable<RtaaSlicedTerm> slicedTerms, DateTime start, DateTime end)
        {
            var slicedTermList = new List<RtaaSlicedTerm>(slicedTerms);

            if (slicedTermList.Count == 0)
            {
                slicedTermList.Insert(0, new RtaaSlicedTerm()
                {
                    Start = start,
                    End = end,
                    OnService = false,
                    Level = -1
                });

                return slicedTermList;
            }

            var first = slicedTermList.FirstOrDefault();
            var last = slicedTermList.LastOrDefault();

            if (first.Start > start)
            {
                slicedTermList.Insert(0, new RtaaSlicedTerm()
                {
                    Start = start,
                    End = first.Start,
                    OnService = false,
                    Level = -1
                });
            }

            if (last.End < end)
            {
                slicedTermList.Add(new RtaaSlicedTerm()
                {
                    Start = last.End,
                    End = end,
                    OnService = false,
                    Level = -1
                });
            }

            return slicedTermList;
        }

    }
}
