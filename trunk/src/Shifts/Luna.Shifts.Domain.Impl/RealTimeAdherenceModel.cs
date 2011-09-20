using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Globalization;
using Luna.Infrastructure.Data.Repositories;
using Luna.Infrastructure.Domain;
using Luna.Infrastructure.Domain.Model;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain.Model;
using Microsoft.Practices.ServiceLocation;
using uNhAddIns.Adapters;
using System.Diagnostics;
using System.Windows;
using Luna.Core.Extensions;

namespace Luna.Shifts.Domain.Impl
{
    [PersistenceConversational(MethodsIncludeMode = MethodsIncludeMode.Explicit)]
    public partial class RealTimeAdherenceModel : IRealTimeAdherenceModel
    {
        private int DelayOffLineBuffer = 0;
        private int PrematureOnLineBuffer = 0;
        private int DelayOnLineBuffer = 0;
        private int PrematureOffLineBuffer = 0;



        private Dictionary<Guid, IEnumerable<string>> _agentAcdidsCache;

        private static Dictionary<Guid, int> _employeeIdDicCache;
        private static Guid[] _employeeIdsCache;

     

        private readonly Func<Term, bool> _shiftsFilter = new Func<Term, bool>(o =>
        {
            if (o is DayOff || o is TimeOff)
                return false;
            return true;
        });

        private IDictionary<ISimpleEmployee, List<IAgentStatus>> _agentStatusCache;


        private readonly IAgentStatusRepository _agentStatusRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAdherenceEventRepository _adherenceEventRepository;

        private IDictionary<string, Seat> _allSeats;

        public RealTimeAdherenceModel(IAgentStatusRepository agentStatusRepository, ISeatRepository seatRepository,
            IEmployeeRepository employeeRepository, IAdherenceEventRepository adherenceEventRepository)
        {
            _agentStatusRepository = agentStatusRepository;
            _adherenceEventRepository = adherenceEventRepository;
            _seatRepository = seatRepository;
            _employeeRepository = employeeRepository;
            _agentAcdidsCache = new Dictionary<Guid, IEnumerable<string>>(500);
        }

        [PersistenceConversation(ConversationEndMode = EndMode.Continue)]
        public IList<IEnumerable> GetAdherenceBlocks(IEnumerable agents, Func<object, TimeBox> cast, DateRange monitoringRange, bool doNotFetchFromCaches, Action<object, Seat> resultLooping)
        {
            if (_allSeats == null)
                _allSeats = _seatRepository.GetAll().ToDictionary(o => o.ExtNo);

            var agentAdherences = new List<IEnumerable>();

            if (agents == null) return agentAdherences;

            var agentsCount = agents.Count();
           
            _employeeIdDicCache = new Dictionary<Guid, int>(agentsCount);
            _employeeIdsCache = new Guid[agentsCount];
            var allAgentAcdids = new List<string>(agentsCount * 2);

            var now = ServiceLocator.Current.GetInstance<IBackendModel>().GetUniversialTime();

            if (doNotFetchFromCaches)
            {
                var timeboxacdid = new Dictionary<string, TimeBox>(3);

                foreach (var item in agents)
                {
                    var timeBox = cast(item);
                    var agentAcdids = _agentAcdidsCache.GetValue(timeBox.Id, key => _employeeRepository.GetAgentAcdid(key));

                    allAgentAcdids.AddRange(agentAcdids);
                    foreach (var acdid in agentAcdids)
                    {
                        if (!timeboxacdid.ContainsKey(acdid))
                            timeboxacdid.Add(acdid, timeBox);
                    }
                }

                _agentStatusCache = new Dictionary<ISimpleEmployee, List<IAgentStatus>>(agentsCount);

                //AgentStatus fetching
                _agentStatusRepository.FastSearch(allAgentAcdids.ToArray(), monitoringRange.Start.AddHours(-8), monitoringRange.End,
                     agentStatus =>
                     {
                         if (!timeboxacdid.ContainsKey(agentStatus.AgentAcdid)) return;

                         var key = timeboxacdid[agentStatus.AgentAcdid].Agent;
                         if (!_agentStatusCache.ContainsKey(key))
                             _agentStatusCache[key] = new List<IAgentStatus>(100);
                         _agentStatusCache[key].Add(agentStatus);
                     });
            }

            var agentsLoopingIndex = 0;
            foreach (var item in agents)
            {
                var timeBox = cast(item);

                _employeeIdsCache[agentsLoopingIndex] = timeBox.Id;
                _employeeIdDicCache[timeBox.Id] = agentsLoopingIndex;
                agentsLoopingIndex++;

                var status = _agentStatusCache.ContainsKey(timeBox.Agent) ? _agentStatusCache[timeBox.Agent] : new List<IAgentStatus>();

                var shrinkedTermSet = timeBox.TermSet.Retrieve<Term>(monitoringRange.Start, monitoringRange.End);

                var slicedTerms = FillTerminalGap(SliceIntoPieces(shrinkedTermSet, _shiftsFilter), monitoringRange.Start, monitoringRange.End);

                var adTerms = Method1(monitoringRange.Start,
                                      now > monitoringRange.Start ? monitoringRange.End > now ? now : monitoringRange.End : monitoringRange.End,
                                      slicedTerms, status, now);

                var typeTerms = FillTerminalGap(SliceIntoPiecesByType(shrinkedTermSet, _shiftsFilter), monitoringRange.Start, monitoringRange.End);
                agentAdherences.Add(new AgentAdherence(timeBox.Agent) { AdherenceTerms = adTerms, SlicedTerms = typeTerms });

                if (resultLooping != null)
                {
                    var crossedStatus = status.LastOrDefault(o => o.TimeStamp <= monitoringRange.End);
                    var extNo = crossedStatus == null || !crossedStatus.OnService ? string.Empty : crossedStatus.ExtNo.Trim();

                    var seat = !string.IsNullOrEmpty(extNo) && _allSeats.ContainsKey(extNo) ? _allSeats[extNo] : default(Seat);
                    resultLooping(item, seat);
                }
            }
           return agentAdherences;
        }

        public string GapCheckAdherence(RtaaSlicedTerm leftBlock, RtaaSlicedTerm centerBlock, RtaaSlicedTerm rightBlock,
            DateRangeRef leftDateRange, DateRangeRef centerDateRange, DateRangeRef rightDateRange)
        {
            DateTime start = centerDateRange.Start;
            DateTime end = centerDateRange.End;

            if (leftBlock.OnService)
            {
                DateTime temp = leftDateRange.End.AddSeconds(DelayOffLineBuffer);
                if (start < temp)
                    start = temp;
            }

            if (rightBlock.OnService)
            {
                DateTime temp = rightDateRange.Start.AddSeconds(-PrematureOnLineBuffer);
                if (end > temp)
                    end = temp;
            }

            return start < end ? "LateToLeave" : string.Empty;
        }


        public string AbsentCheckAdherence(RtaaSlicedTerm leftBlock, RtaaSlicedTerm centerBlock, RtaaSlicedTerm rightBlock,
            DateRangeRef leftDateRange, DateRangeRef centerDateRange, DateRangeRef rightDateRange)
        {
            DateTime start = centerDateRange.Start;
            DateTime end = centerDateRange.End;

            if (leftBlock.OnService)
            {
                DateTime temp = leftDateRange.End.AddSeconds(DelayOffLineBuffer);
                if (start < temp)
                    start = temp;
            }

            if (leftBlock.OnService)
            {
                DateTime temp = rightDateRange.Start.AddSeconds(-PrematureOnLineBuffer);
                if (end > temp)
                    end = temp;
            }

            return start < end ? "LateToLeave" : string.Empty;
        }

        public string SubEventCheckAdherence(RtaaSlicedTerm leftBlock, RtaaSlicedTerm centerBlock, RtaaSlicedTerm rightBlock,
            DateRangeRef leftDateRange, DateRangeRef centerDateRange, DateRangeRef rightDateRange)
        {
            DateTime start = centerDateRange.Start;
            DateTime end = centerDateRange.End;

            if (leftBlock.OnService != centerBlock.OnService)
            {
                DateTime temp = leftDateRange.End.AddSeconds(DelayOnLineBuffer);
                if (start < temp)
                    start = temp;
            }

            if (leftBlock.OnService != centerBlock.OnService)
            {
                DateTime temp = rightDateRange.Start.AddSeconds(-PrematureOffLineBuffer);
                if (end > temp)
                    end = temp;
            }

            return start < end ? centerBlock.OnService ? "LateToWork" : "LateToLeave" : string.Empty;
        }

        public string AssignmentCheckAdherence(RtaaSlicedTerm leftBlock, RtaaSlicedTerm centerBlock, RtaaSlicedTerm rightBlock,
            DateRangeRef leftDateRange, DateRangeRef centerDateRange, DateRangeRef rightDateRange)
        {
            DateTime start = centerDateRange.Start;
            DateTime end = centerDateRange.End;

            if (!leftBlock.OnService)
            {
                DateTime temp = leftDateRange.End.AddSeconds(DelayOnLineBuffer);
                if (start < temp)
                    start = temp;
            }

            if (!rightBlock.OnService)
            {
                DateTime temp = rightDateRange.Start.AddSeconds(-PrematureOffLineBuffer);
                if (end > temp)
                    end = temp;
            }

            return start < end ? "LateToWork" : string.Empty;
        }

        private string ShiftCheckAdherence(RtaaSlicedTerm leftBlock, RtaaSlicedTerm centerBlock, RtaaSlicedTerm rightBlock,
            DateRangeRef leftDateRange, DateRangeRef centerDateRange, DateRangeRef rightDateRange)
        {
            if (centerBlock.Text == "Assignment")
                return AssignmentCheckAdherence(leftBlock, centerBlock, rightBlock, leftDateRange, centerDateRange, rightDateRange);

            if (centerBlock.Text == "SubEvent")
                return SubEventCheckAdherence(leftBlock, centerBlock, rightBlock, leftDateRange, centerDateRange, rightDateRange);

            if (centerBlock.Text == "Gap")
                return GapCheckAdherence(leftBlock, centerBlock, rightBlock, leftDateRange, centerDateRange, rightDateRange);

            return AbsentCheckAdherence(leftBlock, centerBlock, rightBlock, leftDateRange, centerDateRange, rightDateRange);

        }

        private IEnumerable<ITerm> Method1(DateTime start, DateTime end, IList<RtaaSlicedTerm> blocks, IList<IAgentStatus> statuses, DateTime watchPoint)
        {
            List<ITerm> results = new List<ITerm>();
            var agentAdherenceList = GetACDEvents(statuses, start, end, 0, watchPoint);
            int minAbnormalBufferSeconds;

            if (Application.Current == null)
            {
                // 晚下班
                DelayOffLineBuffer = LanguageReader.GetValue<int>("OffLineLaterBufferSeconds");
                // 早上線
                PrematureOnLineBuffer = LanguageReader.GetValue<int>("OnLineEarlierBufferSeconds");
                // 晚上線
                DelayOnLineBuffer = LanguageReader.GetValue<int>("OnLineLaterBufferSeconds");
                // 早下線
                PrematureOffLineBuffer = LanguageReader.GetValue<int>("OffLineEarlierBufferSeconds");
                // 通用
                minAbnormalBufferSeconds = LanguageReader.GetValue<int>("MinAbnormalBufferSeconds");
            }
            else
            {
                // 晚下班
                DelayOffLineBuffer = Convert.ToInt32(Application.Current.Resources["OffLineLaterBufferSeconds"]);
                // 早上線
                PrematureOnLineBuffer = Convert.ToInt32(Application.Current.Resources["OnLineEarlierBufferSeconds"]);
                // 晚上線
                DelayOnLineBuffer = Convert.ToInt32(Application.Current.Resources["OnLineLaterBufferSeconds"]);
                // 早下線
                PrematureOffLineBuffer = Convert.ToInt32(Application.Current.Resources["OffLineEarlierBufferSeconds"]);
                // 通用
                minAbnormalBufferSeconds = Convert.ToInt32(Application.Current.Resources["MinAbnormalBufferSeconds"]);
            }

            int j = 0;
            for (int i = 0; i < agentAdherenceList.Count; i++)
            {
                //并交行走
                for (; j < blocks.Count && i < agentAdherenceList.Count; j++)
                {//两个相交才确定
                    if (blocks[j].Start <= agentAdherenceList[i].End && agentAdherenceList[i].Start <= blocks[j].End)
                    {
                        #region CrossOver

                        // if 相交部分OnService的情況相同 Or 這個班塊是Ignore
                        if (blocks[j].OnService == agentAdherenceList[i].OnService || blocks[j].Ignore)
                        {
                            //               i
                            //  Adh    |===========|----------
                            //  Block      |===========|------
                            //                   j
                            // Block 已經比 Adh 更晚了
                            // Block idx不變 Adh 換到下一個
                            if (agentAdherenceList[i].End <= blocks[j].End)
                            {
                                j--;
                                i++;
                            }
                            continue;
                        }

                        DateRangeRef centerDateRange = new DateRangeRef();

                        // 找出相交部分的區域
                        centerDateRange.Start = blocks[j].Start < agentAdherenceList[i].Start ? agentAdherenceList[i].Start : blocks[j].Start;
                        centerDateRange.End = blocks[j].End > agentAdherenceList[i].End ? agentAdherenceList[i].End : blocks[j].End;


                        //if (exceptionBlock.End.Subtract(exceptionBlock.Start).TotalMinutes < unitmin) continue;

                        #region 找出leftBlock, leftDateRange, rightBlock, rightDateRange
                        // 找出前一個Block  如果是第一個(inx = 0)Block ,建立一個Gap,時間是從DateTime.Min~第一個Block的開始
                        RtaaSlicedTerm leftBlock = j > 0 ? blocks[j - 1] :
                            new RtaaSlicedTerm()
                            {
                                Start = DateTime.MinValue,
                                End = blocks[j].Start,
                                OnService = false,
                                Text = "Gap"
                            };//Gap
                        DateRangeRef leftDateRange = new DateRangeRef(leftBlock.Start, leftBlock.End);
                        if (j > 0)
                        {
                            leftDateRange.Start = blocks[j - 1].Start;
                            leftDateRange.End = blocks[j - 1].End;
                        }

                        // 找出下一個Block 如果是最後一個Block ,建立一個Gap,時間是最後一個Block結束~DateTime.Max
                        RtaaSlicedTerm rightBlock = j < (blocks.Count - 1) ? blocks[j + 1] :
                            new RtaaSlicedTerm()
                            {
                                Start = blocks[j].End,
                                End = DateTime.MaxValue,
                                OnService = false,
                                Text = "Gap"
                            };//Gap
                        DateRangeRef rightDateRange = new DateRangeRef(rightBlock.Start, rightBlock.End);
                        if (j < (blocks.Count - 1))
                        {
                            rightDateRange.Start = blocks[j + 1].Start;
                            rightDateRange.End = blocks[j + 1].End;
                        }
                        #endregion

                        // 決定這個相交段是 "LateToWork"(應上未上) Or "LateToLeave"(應下未下) Or ""(被Buff忽略)
                        string style = ShiftCheckAdherence(leftBlock, blocks[j], rightBlock, leftDateRange, centerDateRange, rightDateRange);

                        // if 是 "LateToWork"(應上未上) Or "LateToLeave"(應下未下)
                        if (style.Length > 0)
                        {
                            // 如果是不在start~end間, 換看下一個Block
                            if (centerDateRange.End <= start || centerDateRange.Start >= end)
                            {
                                //               i          i+1
                                //  Adh    |===========|----------|
                                //  Block      |===========|-------
                                //                   j
                                // Block 已經比 Adh 更晚了
                                // Block idx不變 Adh 換到下一個
                                if (agentAdherenceList[i].End <= blocks[j].End)
                                {
                                    j--;
                                    i++;
                                }
                                //                          i          i+1
                                //  Adh               |===========|----------|
                                //  Block      |===========|---------|--------
                                //                   j         J+1
                                // Adh 已經比 Block 更晚了
                                // Adh idx不變 Block 換到下一個
                                //else { }
                                continue;
                            }


                            var exceptionBlock = new AdherenceTerm(centerDateRange.Start, centerDateRange.End, new DateRange(start, end)) { Text = style };
                            //exceptionBlock.BackgroundColor = blockBrushColors[style];

                            // 檢查產生的長度是否大過 minAbnormalBufferSeconds,否則就不加入Result
                            if (exceptionBlock.End.Subtract(exceptionBlock.Start).TotalSeconds >= minAbnormalBufferSeconds)
                                results.Add(exceptionBlock);
                        }

                        //               i          i+1
                        //  Adh    |===========|----------|
                        //  Block      |===========|-------
                        //                   j
                        // Block 已經比 Adh 更晚了
                        // Block idx不變 Adh 換到下一個
                        if (agentAdherenceList[i].End <= blocks[j].End)
                        {
                            j--;
                            i++;
                        }
                        //                          i          i+1
                        //  Adh               |===========|----------|
                        //  Block      |===========|---------|--------
                        //                   j         J+1
                        // Adh 已經比 Block 更晚了
                        // Adh idx不變 Block 換到下一個
                        //else { }
                        #endregion
                    }
                    else
                    {
                        //               i            i+1
                        //  Adh    |===========|--------------------|
                        //  Block                |===========|------
                        //                             j
                        // Block 已經比 Adh 更晚了
                        // Block idx不變 Adh 換到下一個
                        if (blocks[j].Start > agentAdherenceList[i].End)
                        {
                            if (j > 0)
                                j--;
                            break;
                        }
                        //                                 i
                        //  Adh                      |===========|----------
                        //  Block      |===========|------------|------------
                        //                   j           j+1
                        // Adh 已經比 Block 更晚了
                        // Adh idx不變 Block 換到下一個
                        //else { }
                    }
                }
            }


            // 切掉超出start和end之外的片段
            if (results.Count > 0 && results[0].Start < start) //Henry: modified @ 2010/3/31 13:50 if (results.Count > 0 && results[0].Start <= start)
            {
                results.RemoveAt(0);
            }

            if (results.Count > 0 && results[results.Count - 1].End > end)
            {
                results[results.Count - 1].SaftyInvoke<AdherenceTerm>(o =>
                {
                    o.End = end;
                    if (o.End.Subtract(o.Start).TotalSeconds < minAbnormalBufferSeconds)
                        results.Remove(o);
                });
            }

            return results;
        }

        private List<AgentStatusTerm> GetACDEvents(IList<IAgentStatus> acdList, DateTime start, DateTime end, double blockResolution, DateTime watchPoint)
        {
            List<AgentStatusTerm> results = new List<AgentStatusTerm>();

            if (acdList == null || acdList.Count == 0)
            {
                results.Add(new AgentStatusTerm()
                {
                    Start = DateTime.MinValue,
                    End = watchPoint > end ? end : watchPoint,
                    OnService = false
                });
                return results;
            }

            if (acdList[0].TimeStamp > start)
            {
                acdList.Insert(0,
                               new AgentStatus()
                               {
                                   TimeStamp = start,
                                   AgentStatusType = new AgentStatusType() { OnService = false }
                               });
            }

            int i = 0;
            IAgentStatus preACDEvent = acdList[i];

            for (i = 1; i < acdList.Count; i++)
            {
                if (acdList[i].OnService != preACDEvent.OnService)
                {
                    double length = acdList[i].TimeStamp.Subtract(preACDEvent.TimeStamp).TotalSeconds;
                    if (blockResolution > length)
                    {
                        if (results.Count > 0)
                        {
                            results.RemoveAt(results.Count - 1);
                        }
                    }
                    else
                    {
                        AgentStatusTerm acdBlock = new AgentStatusTerm()
                        {
                            Start = preACDEvent.TimeStamp,
                            End = preACDEvent.TimeStamp.AddSeconds(length),
                            OnService = preACDEvent.OnService
                        };
                        results.Add(acdBlock);

                        preACDEvent = acdList[i];
                    }
                }
            }

            results.Add(new AgentStatusTerm()
            {
                Start = preACDEvent.TimeStamp,
                End = end,
                OnService = preACDEvent.OnService
            });
            return results;
        }

        public IList<RtaaSlicedTerm> FillTerminalGap(IEnumerable<RtaaSlicedTerm> slicedTerms, DateTime start, DateTime end)
        {
            List<RtaaSlicedTerm> slicedTermList = new List<RtaaSlicedTerm>(slicedTerms);

            if (slicedTermList.Count == 0)
            {
                slicedTermList.Insert(0, new RtaaSlicedTerm()
                {
                    Start = start,
                    End = end,
                    OnService = false,
                    Text = "Gap"
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
                    Text = "Gap"
                });
            }

            if (last.End < end)
            {
                slicedTermList.Add(new RtaaSlicedTerm()
                {
                    Start = last.End,
                    End = end,
                    OnService = false,
                    Text = "Gap"
                });
            }

            return slicedTermList;
        }

        protected IEnumerable<RtaaSlicedTerm> SliceIntoPiecesByType(IEnumerable<Term> terms, Func<Term, bool> termsFilter)
        {
            var termsCutter = new TermsCutter<Term, RtaaSlicedTerm>
                (terms, termsFilter, (s, e, t) =>
                {
                    var slicedTerm = new RtaaSlicedTerm
                    {
                        Start = s,
                        End = e,
                        OnService = t == null ? false : t.OnService,
                        Text = t == null ? "Gap" : t.Level > 0 ? t.Level > 2 ? "Absent" : "SubEvent" : "Assignment"
                    };

                    return slicedTerm;
                });

            return termsCutter.ToList(((begin, current) =>
            {
                return !Equals(begin, current);

            }));

        }

        protected IEnumerable<RtaaSlicedTerm> SliceIntoPieces(IEnumerable<Term> terms, Func<Term, bool> termsFilter)
        {
            var termsCutter = new TermsCutter<Term, RtaaSlicedTerm>
                (terms, termsFilter, (s, e, t) =>
                {
                    bool ignore = false;
                    if (t != null)
                    {
                        var assignment = t.GetLowestTerm();
                        if (assignment != null)
                            ignore = assignment.IgnoreAdherence();

                    }
                    var slicedTerm = new RtaaSlicedTerm
                    {
                        Ignore = ignore,
                        Start = s,
                        End = e,
                        OnService = t == null ? false : t.OnService,
                        Text = t == null ? "Gap" : t.Level > 0 ? t.Level > 2 ? "Absent" : "SubEvent" : "Assignment"
                    };

                    return slicedTerm;
                });

            return termsCutter.ToList(((begin, current) =>
            {
                bool currentOnService = current != null ? current.OnService : false;
                bool beginOnService = begin != null ? begin.OnService : false;

                bool beginIgnore = false;
                if (begin != null)
                {
                    var assignment = begin.GetLowestTerm();
                    if (assignment != null)
                        beginIgnore = assignment.IgnoreAdherence();

                }

                bool currentIgnore = false;
                if (current != null)
                {
                    var assignment = current.GetLowestTerm();
                    if (assignment != null)
                        currentIgnore = assignment.IgnoreAdherence();

                }


                if (beginOnService != currentOnService || beginIgnore != currentIgnore)
                    return true;
                return false;
            }));

        }
    }
}
