using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Common.Attributes;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain.Model;
using uNhAddIns.Adapters;
using Luna.Common.Extensions;
using Luna.Core.Extensions;

namespace Luna.Shifts.Domain.Impl
{
    [Aop]
    [PersistenceConversational(MethodsIncludeMode = MethodsIncludeMode.Explicit)]
    public class SeatingServiceModel : ISeatingServiceModel
    {
        private readonly ISeatConsolidationRepository _seatConsolidationRepository;
        private bool _isMatchingRank;
        private int _bufferLength;
        private IList<TimeBox> _participaters;
        private IList<TimeBox> _agentsOnSeat;
        private SeatingEngineStatus _status;
        private Dictionary<TimeBox, List<Fork>> _employeeAssignmentRegister;
        private Dictionary<ISimpleEmployee, List<List<SeatArrangement>>> _employeeSeatOccupationRegister;
        private IDictionary<ISeatingSeat, List<Occupation>> _seatOccupationRegister;
        private Dictionary<ISeatingTerm, List<List<SeatArrangement>>> _termOccupationRegister;
        private readonly ISeatBoxRepository _seatBoxRepository;
        private readonly ITimeBoxRepository _timeBoxRepository;
        private readonly IAreaRepository _areaRepository;

        public SeatingServiceModel(ISeatConsolidationRepository seatConsolidationRepository, ISeatBoxRepository seatBoxRepository,
            ITimeBoxRepository timeBoxRepository, IAreaRepository areaRepository)
        {
            _seatConsolidationRepository = seatConsolidationRepository;
            _seatBoxRepository = seatBoxRepository;
            _timeBoxRepository = timeBoxRepository;
            _areaRepository = areaRepository;
        }

        public event EventHandler<SeatingEngineStatus> EngineStatusChanged;

        #region Methods

        [PersistenceConversation(ConversationEndMode = EndMode.CommitAndContinue)]
        public ISeatingSite<Entity>[] GetSites(Entity campaign)
        {
            var areas = _areaRepository.GetAreaByCampaign(campaign);
            var q = from a in areas
                    group a by a.Site
                        into g
                        select new CampaignSite
                                   {
                                       Campaign = campaign,
                                       Site = g.Key,
                                       Areas = g.ToRebuildPriorityList<Area, Entity>(true, null)
                                   };
            return q.ToArray();
        }

        private Dictionary<Entity, OrganizationSeatingArea[]> _organizationSeatingAreaSet;
        private Dictionary<ISeat, List<PriorityEmployee>> _priorityEmployeeSet;
        private Dictionary<Entity, IEnumerable<ISeat>> _seatsMap;
        private Dictionary<string, SeatBox> _seatBoxSet;


        [PersistenceConversation(ConversationEndMode = EndMode.CommitAndContinue)]
        public IList<SeatBox> Preparing(Entity campaign, Entity[] areas, DateTime start, DateTime end)
        {
            _organizationSeatingAreaSet = new Dictionary<Entity, OrganizationSeatingArea[]>();
            _seatsMap = new Dictionary<Entity, IEnumerable<ISeat>>();

            foreach (var area in areas)
            {
                _organizationSeatingAreaSet[area] =
                    _areaRepository.GetOrganizationSeatingArea(area.Id).Where(o => o.IsSelected == true && o.TargetSeat != null).OrderBy(o => o.Index).ToArray();
                _seatsMap[area] = ((IArea)area).Seats;
            }

            _priorityEmployeeSet = _areaRepository.GetPriorityEmployees(areas).ToUsefulDictionary();

            var seatBoxList = _seatBoxRepository.Search(areas, start, end, false);

            var seatIds = new string[seatBoxList.Count];
            var counter = 0;
            _seatBoxSet = seatBoxList.ToDictionary(o =>
            {
                o.Initial();
                var key = o.Seat.Id.ToString();
                seatIds[counter] = key;
                counter++;
                return key;
            });

            //TODO: need filter with OccupyStatus ? do not load labourRule only timebox object
            _participaters = _timeBoxRepository.GetTimeBoxesFrom(campaign, start, end);
            _agentsOnSeat = _timeBoxRepository.GetTimeBoxes(seatIds, _participaters.Select(o => o.Id).ToArray(), start, end);

            return seatBoxList;
        }

        [PersistenceConversation(ConversationEndMode = EndMode.Continue)]
        public IEnumerable<TimeBox> ArrangeSeat(Entity site, int bufferLength, bool isMatchingRank,
            IgnoreAgentPriority agentPriorityMethodology, DateTime start, DateTime end, SeatingEngineStatus engineStatus)
        {
            _status = engineStatus;
            _isMatchingRank = isMatchingRank;
            _bufferLength = bufferLength;

            _employeeSeatOccupationRegister = new Dictionary<ISimpleEmployee, List<List<SeatArrangement>>>();
            _employeeAssignmentRegister = new Dictionary<TimeBox, List<Fork>>();
            _termOccupationRegister = new Dictionary<ISeatingTerm, List<List<SeatArrangement>>>();

            IEnumerable<List<SeatArrangement>> occupationList;
            IEnumerable<ISeat> seatList;

            #region StatusMessage

            int temp1 = 0;
            int[] occupationData;
            _status.TextMessage += "\n";
            _status.TextMessage += string.Format("{0} : EngineGeatherInfo", DateTime.Now);
            #endregion

            #region // Prepare Data //

            var getSeat =
                new Func<string, Seat>(seat => _seatBoxSet.ContainsKey(seat) ? _seatBoxSet[seat].Seat : default(Seat));
            var addSeatArrangement =
                new Func<string, SeatArrangement, bool>(
                    (seat, seatArrangement) => _seatBoxSet[seat].AddOccupation(seatArrangement));

            foreach (var timeBox in _agentsOnSeat)
                timeBox.GetAllTerm<Term>(start, end).GenSeatArrangements(timeBox.Agent, getSeat, addSeatArrangement);

            foreach (var timeBox in _participaters)
            {
                var filteredTerms = timeBox.GetAllTermWithoutOffWork<Term>(start, end, o =>
                   {
                       return o.GetLowestTerm().If<ISeatingTerm>(x=> x.ArrangeSeatYet() && x.StartIsCoverd(start, end));
                      
                   }).ToArray();

                timeBox.GetAllTerm<Term>(start, end).GenSeatArrangements(timeBox.Agent, getSeat, addSeatArrangement);

                var prv = default(Term);
                var seatArrangementGroup = new List<SeatArrangement>();
                var occupationGroup = new List<List<SeatArrangement>>();
                Action finalStep = () => { };

                var agent = timeBox.Agent;

                filteredTerms.SliceOccupied((dateRange, term) =>
                {
                    if (term == null || !term.IsNeedSeat)
                    {
                        prv = term;
                        return default(SeatArrangement);
                    }

                    if (prv != null && !ReferenceEquals(prv, term) && prv.Level == 0 && term.Level == 0)
                    {
                        prv = null;
                    }

                    var source = default(Term);
                    TermExt.X(prv, term, ref source);

                    var instance = default(SeatArrangement);

                    if (source.If(o => o.SeatIsEmpty()) == true)
                    {
                        instance = new SeatArrangement(source, agent, dateRange.Start, dateRange.End) { Remark = "ByEngine" };

                        var isOccupiedUnlaoredSubevent = new Func<Term, bool>(o => o.GetIsNeedSeatField() && o is UnlaboredSubEvent);

                        if (isOccupiedUnlaoredSubevent(source) || (source.Level > 1 && source.SeatIsEmpty() && isOccupiedUnlaoredSubevent(source.Bottom)))
                            seatArrangementGroup.Add(instance);
                        else
                        {
                            var key = (Term)term.GetLowestTerm();
                            if (!_termOccupationRegister.ContainsKey(key))
                                _termOccupationRegister[key] = new List<List<SeatArrangement>>();
                            _termOccupationRegister[key].Add(seatArrangementGroup);

                            if (seatArrangementGroup.Count > 0)
                                occupationGroup.Add(seatArrangementGroup);

                            if (term is IAssignment)
                                _status.AssignmentAmount += 1;
                            seatArrangementGroup = new List<SeatArrangement>(new[] { instance });
                            finalStep = () =>
                            {
                                _termOccupationRegister[key].Add(seatArrangementGroup);
                                occupationGroup.Add(seatArrangementGroup);
                            };
                        }
                    }

                    prv = term;
                    return instance;
                }, t => t.IsNeedSeat);

                finalStep();

                _employeeSeatOccupationRegister.Add(timeBox.Agent, occupationGroup);
                _employeeAssignmentRegister.Add(timeBox, filteredTerms.GetFork());
            }

            //TODO: load exist occupations

            _seatOccupationRegister = _seatBoxSet.Values
                .ToDictionary(o => o.Seat as ISeatingSeat, o => o.Occupations.ToList());

            #region StatusMessage
            _status.EmployeeAmount = _employeeAssignmentRegister.Count;
            //foreach (List<Fork> fkList in _employeeAssignmentRegister.Values)
            //    temp1 += fkList.Count;
            //_status.AssignmentAmount = 0;
            temp1 = 0;
            foreach (List<List<SeatArrangement>> soList in _employeeSeatOccupationRegister.Values)
            {
                foreach (List<SeatArrangement> sooList in soList)
                    temp1 += sooList.Count;
            }
            _status.SeatArrangementAmount = temp1;
            _status.TextMessage += "\n";
            _status.TextMessage += string.Format("{0} : EngineStartSeating", DateTime.Now);
            #endregion
            #endregion

            #region // Consolidation Rule  Active //

            foreach (var consolidationRule in _seatConsolidationRepository.GetByStie(site))
            {
                if (!_seatsMap.Keys.Contains(consolidationRule.TargetSeat.Area))
                    continue;
                seatList = GetSeatByRule(consolidationRule, _seatsMap[consolidationRule.TargetSeat.Area], true);
                occupationList = GetOccupationByConsolidationRule(consolidationRule);
                foreach (var seat in seatList)
                {
                    foreach (var seatArrangement in occupationList)
                        SetOccupationSeat(seatArrangement, seat);
                }
            }
            // debug
            //Console.WriteLine("=============================");
            // debug
            #region StatusMessage
            _status.Process = 0.25;
            occupationData = getState();

            _status.Stage1ArrangedAmount = occupationData[1] - _status.ArrangedSeatArrangementAmount;
            _status.Stage1ArrangePercentage = _status.SeatArrangementAmount == 0 ? 0 : 100 * (double)_status.Stage1ArrangedAmount / (double)_status.SeatArrangementAmount;

            _status.TotalContinueAssignmentAmount = occupationData[2];
            _status.TotalContinueAssignmentPercentage = _status.AssignmentAmount == 0 ? 0 : 100 * (double)_status.TotalContinueAssignmentAmount / (double)_status.AssignmentAmount;
            _status.ArrangedSeatArrangementAmount = occupationData[1];
            _status.ArrangedSeatArrangementPercentage = _status.SeatArrangementAmount == 0 ? 0 : 100 * (double)_status.ArrangedSeatArrangementAmount / (double)_status.SeatArrangementAmount;
            _status.UnArrangedSeatArrangementAmount = _status.SeatArrangementAmount - _status.ArrangedSeatArrangementAmount;
            _status.UnArrangedSeatArrangementPercentage = _status.SeatArrangementAmount == 0 ? 0 : 100 * (double)_status.UnArrangedSeatArrangementAmount / (double)_status.SeatArrangementAmount;
            _status.TextMessage += "\n";
            _status.TextMessage += String.Format("{0} : EngineConsolidationFinish", DateTime.Now);

            #endregion

            #endregion

            #region // Seat Priority Employee Active //

            int priorityEmployeeIndexMax = Math.Min(4 - (int)agentPriorityMethodology, 3);
            for (int priorityEmployeeIndex = 0; priorityEmployeeIndex < priorityEmployeeIndexMax; priorityEmployeeIndex++)
            {
                foreach (var item in _priorityEmployeeSet)
                {
                    var priorityEmployeeConut = item.Value.Count;

                    if (priorityEmployeeIndex >= priorityEmployeeConut)
                        continue;

                    occupationList = GetOccupationByEmployee(item.Value[priorityEmployeeIndex].Object);
                    foreach (List<SeatArrangement> iso in occupationList)
                    {
                        SetOccupationSeat(iso, item.Key);
                    }
                }

                #region StatusMessage
                _status.Process = 0.25 + 0.25 * priorityEmployeeIndex / 3;
                occupationData = getState();
                _status.Stage2ArrangedAmount[priorityEmployeeIndex] = occupationData[1] - _status.ArrangedSeatArrangementAmount;
                _status.Stage2ArrangedPercentage[priorityEmployeeIndex] = 100 * (double)_status.Stage2ArrangedAmount[priorityEmployeeIndex] / (double)_status.SeatArrangementAmount;
                //_status.Stage2ArrangedAmount = _status.Stage2ArrangedAmount;
                //_status.Stage2ArrangedPercentage = _status.Stage2ArrangedPercentage;

                _status.TotalContinueAssignmentAmount = occupationData[2];
                _status.TotalContinueAssignmentPercentage = _status.AssignmentAmount == 0 ? 0 : 100 * (double)_status.TotalContinueAssignmentAmount / (double)_status.AssignmentAmount;
                _status.ArrangedSeatArrangementAmount = occupationData[1];
                _status.ArrangedSeatArrangementPercentage = _status.SeatArrangementAmount == 0 ? 0 : 100 * (double)_status.ArrangedSeatArrangementAmount / (double)_status.SeatArrangementAmount;
                _status.UnArrangedSeatArrangementAmount = _status.SeatArrangementAmount - _status.ArrangedSeatArrangementAmount;
                _status.UnArrangedSeatArrangementPercentage = _status.SeatArrangementAmount == 0 ? 0 : 100 * (double)_status.UnArrangedSeatArrangementAmount / (double)_status.SeatArrangementAmount;

                _status.TextMessage += "\n";
                _status.TextMessage += string.Format("{0} : EnginePriorityEmployee {1} Finish", DateTime.Now, priorityEmployeeIndex);

                #endregion
            }
            #endregion

            #region // Seat Priority Organization Active //

            foreach (var area in _seatsMap)
            {
                foreach (var seat in area.Value)
                {
                    if (seat.PriorityOrganization == null)
                        continue;

                    //var seatRank = seat.Rank;
                    occupationList = GetOccupationByOrganization(seat.PriorityOrganization,
                                                                   rank => true, skills => true, o => true);

                    foreach (List<SeatArrangement> occupation in occupationList)
                        SetOccupationSeat(occupation, seat);
                }
            }

            #region StatusMessage
            _status.Process = 0.75;
            occupationData = getState();
            _status.Stage3ArrangedAmount = occupationData[1] - _status.ArrangedSeatArrangementAmount;
            _status.Stage3ArrangePercentage = _status.SeatArrangementAmount == 0 ? 0 : 100 * (double)_status.Stage3ArrangedAmount / (double)_status.SeatArrangementAmount;

            _status.TotalContinueAssignmentAmount = occupationData[2];
            _status.TotalContinueAssignmentPercentage = _status.AssignmentAmount == 0 ? 0 : 100 * (double)_status.TotalContinueAssignmentAmount / (double)_status.AssignmentAmount;
            _status.ArrangedSeatArrangementAmount = occupationData[1];
            _status.ArrangedSeatArrangementPercentage = _status.SeatArrangementAmount == 0 ? 0 : 100 * (double)_status.ArrangedSeatArrangementAmount / (double)_status.SeatArrangementAmount;
            _status.UnArrangedSeatArrangementAmount = _status.SeatArrangementAmount - _status.ArrangedSeatArrangementAmount;
            _status.UnArrangedSeatArrangementPercentage = _status.SeatArrangementAmount == 0 ? 0 : 100 * (double)_status.UnArrangedSeatArrangementAmount / (double)_status.SeatArrangementAmount;
            _status.TextMessage += "\n";
            _status.TextMessage += string.Format("{0} : EnginePriorityOrgFinish", DateTime.Now);

            #endregion
            #endregion

            #region // Area Priority Organization Active //

            OrganizationSeatingArea organizationSeatingRule;

            int priorityOrganizationIndexMax = _organizationSeatingAreaSet.Values.Max(o => o.Length);


            for (int index = 0; index < priorityOrganizationIndexMax; index++)
            {
                foreach (var item in _organizationSeatingAreaSet.OrderBy(o=> o.Key.SaftyGetProperty<int,IIndexable>(i=> i.Index)))
                {
                    if (index >= item.Value.Length)
                        continue;
                    organizationSeatingRule = item.Value[index];
                    seatList = GetSeatByRule(organizationSeatingRule, _seatsMap[item.Key], false);

                    occupationList = GetOccupationByOrganization(organizationSeatingRule.Object,
                                                            rank => true,
                                                            skill => true, o => true);
                    foreach (var seat in seatList)
                    {
                        //var seatRank = seat.Rank;
                        foreach (List<SeatArrangement> iso in occupationList)
                            SetOccupationSeat(iso, seat);
                    }
                }
            }
            #endregion

            #region // Clear Partial Assignment and ReAssign AssignmentWise //
            //foreach (var kvp in _termOccupationRegister)
            //{
            //    if (CheckAssignmentState(kvp.Key) == AssignmentSeatingState.Partial)
            //    {
            //        foreach (var iso in kvp.Value)
            //        {
            //            if (iso.Seat != default(ISeat))
            //            {
            //                _seatOccupationRegister[iso.Seat].Remove(iso);
            //                iso.Seat = default(ISeat);
            //            }
            //        }
            //    }
            //}
            //for (int index = 0; index < priorityOrganizationIndexMax; index++)
            //{
            //    foreach (var item in _seatsMap)
            //    {
            //        var area = item.Key;

            //        if (index >= _organizationSeatingAreaSet[area].Length)
            //            continue;

            //        var orderedServiceOrganizations = _organizationSeatingAreaSet[area];

            //        organizationSeatingRule = orderedServiceOrganizations[index];

            //        // try arrange Assignment-wise
            //        foreach (ISeatingTerm isa in GetAssignmentByOrganization(organizationSeatingRule.Object, rank => true))
            //        {
            //            var seatArrangementList = _termOccupationRegister.ContainsKey(isa) ? _termOccupationRegister[isa] : null;

            //            if (seatArrangementList == null)
            //                continue;

            //            bool isAssignmentSuccess = true;
            //            foreach (var iso in seatArrangementList)
            //            {
            //                bool isOccupationSuccess = false;
            //                seatList = GetSeatByRule(organizationSeatingRule, item.Value, false);
            //                foreach (var iss in seatList.Where(st => _isMatchingRank ? st.Rank <= iso.Agent.Rank : true))
            //                {
            //                    if (SetOccupationSeat(iso, iss))
            //                    {
            //                        isOccupationSuccess = true;
            //                        break;
            //                    }
            //                }
            //                if (!isOccupationSuccess)
            //                {
            //                    isAssignmentSuccess = false;
            //                    break;
            //                }
            //            }
            //            // fail to Arrange Whole Assignment cancel Assignment
            //            if (!isAssignmentSuccess)
            //            {
            //                foreach (SeatArrangement iso in seatArrangementList)
            //                {
            //                    if (iso.Seat != default(ISeat))
            //                    {
            //                        _seatOccupationRegister[iso.Seat].Remove(iso);
            //                        iso.Seat = default(ISeat);
            //                    }
            //                }
            //            }
            //        }

            //    }
            //}
            #endregion

            #region StatusMessage
            _status.Process = 1;
            occupationData = getState();
            _status.Stage4ArrangedAmount = occupationData[1] - _status.ArrangedSeatArrangementAmount;
            _status.Stage4ArrangePercentage = _status.SeatArrangementAmount == 0 ? 0 : 100 * (double)_status.Stage4ArrangedAmount / (double)_status.SeatArrangementAmount;

            _status.TotalContinueAssignmentAmount = occupationData[2];
            _status.TotalContinueAssignmentPercentage = _status.AssignmentAmount == 0 ? 0 : 100 * (double)_status.TotalContinueAssignmentAmount / (double)_status.AssignmentAmount;
            _status.ArrangedSeatArrangementAmount = occupationData[1];
            _status.ArrangedSeatArrangementPercentage = _status.SeatArrangementAmount == 0 ? 0 : 100 * (double)_status.ArrangedSeatArrangementAmount / (double)_status.SeatArrangementAmount;
            _status.UnArrangedSeatArrangementAmount = _status.SeatArrangementAmount - _status.ArrangedSeatArrangementAmount;
            _status.UnArrangedSeatArrangementPercentage = _status.SeatArrangementAmount == 0 ? 0 : 100 * (double)_status.UnArrangedSeatArrangementAmount / (double)_status.SeatArrangementAmount;

            _status.SeatUsageRate = GetSeatUsageRate((end - start).TotalMinutes);
            _status.TextMessage += "\n";
            _status.TextMessage += string.Format("{0} : EngineAreaOrgFinsish", DateTime.Now);

            #endregion

            foreach (var kvp in _termOccupationRegister)
            {
                foreach (var arrangementList in kvp.Value)
                {
                    foreach (var arrangement in arrangementList.Where(o => o.Seat != null))
                    {
                        _seatBoxSet[arrangement.Seat.Id.ToString()].AddOccupation(arrangement);
                    }
                }
            }

            //foreach (var kvp in _termOccupationRegister)
            //{
            //    var assignment = ((AssignmentBase)kvp.Key);

            //    AssignmentSeatingState checkState = CheckAssignmentState(assignment);
            //    if (checkState == AssignmentSeatingState.Empty)
            //    {
            //        assignment.OccupyStatus = "W";
            //    }
            //    else if (checkState == AssignmentSeatingState.Full)
            //    {
            //        assignment.OccupyStatus = "C";
            //    }
            //    else if (checkState == AssignmentSeatingState.FullButSeprate)
            //    {
            //        assignment.OccupyStatus = "S";
            //    }
            //    else
            //    {
            //        //throw new Exception("SeatingEngine inner exception: There is partial assignment in result.");
            //    }
            //}
            //_timeBoxRepository.Clear();
            return _employeeAssignmentRegister.Keys;
        }

        [PersistenceConversation(ConversationEndMode = EndMode.End)]
        public void SubmitArrangement()
        {
            //foreach (var timeBox in _employeeAssignmentRegister.Keys)
            //{
            //    _timeBoxRepository.MakePersistent(timeBox);
            //}
        }

        [PersistenceConversation(ConversationEndMode = EndMode.DoNothing)]
        public virtual void RegisterSynchronization(Action<bool> complete)
        {
            _timeBoxRepository.RegisterSynchronization(complete);
        }


        public void ClearSession()
        {
            _seatConsolidationRepository.Clear();
        }

        private double GetSeatUsageRate(double totalMin)
        {
            double totalArrangedMin = _termOccupationRegister.Sum(kvp => kvp.Value.
                //Where(sa => sa.Any(o => o.Seat != null)).
                Select(o =>
                {
                    TimeSpan result = new TimeSpan();
                    foreach (var arran in o)
                    {
                        if (arran.Seat != null)
                            result += arran.End - arran.Start;
                    }
                    return result;
                }
                    ).Sum(ts => ts.TotalMinutes));
            double totalSeatNumber = _seatOccupationRegister.Count;
            return totalArrangedMin / (totalSeatNumber * totalMin) * 100;
        }

        private AssignmentSeatingState CheckAssignmentState(ISeatingTerm assignment)
        {
            var firstSeat = default(ISeatingSeat);
            bool hasEmpty = false;
            bool hasSeat = false;
            bool hasDiffSeat = false;
            if (!_termOccupationRegister.Keys.Contains(assignment))
            {
                return AssignmentSeatingState.Empty;
            }
            foreach (List<SeatArrangement> seatArrList in _termOccupationRegister[assignment])
            {
                foreach (var iso in seatArrList)
                {
                    if (iso.Seat == default(ISeatingSeat))
                    {
                        hasEmpty = true;
                    }
                    else
                    {
                        hasSeat = true;
                        if (firstSeat == default(ISeatingSeat))
                        {
                            firstSeat = iso.Seat;
                        }
                        else if (firstSeat != iso.Seat)
                        {
                            hasDiffSeat = true;
                        }
                    }
                }
            }
            if (hasEmpty && !hasSeat)
            {
                return AssignmentSeatingState.Empty;
            }
            else if (hasEmpty && hasSeat)
            {
                return AssignmentSeatingState.Partial;
            }
            else if (!hasEmpty && hasSeat && !hasDiffSeat)
            {
                return AssignmentSeatingState.Full;
            }
            else if (!hasEmpty && hasSeat && hasDiffSeat)
            {
                return AssignmentSeatingState.FullButSeprate;
            }
            return AssignmentSeatingState.Empty;
        }

        // --- Assignment --- //
        private IEnumerable<ISeatingTerm> GetAssignmentByOrganization(Entity org, Func<int, bool> rankFiter)
        {
            foreach (var item in _employeeAssignmentRegister.OrderBy(o => o.Key.Agent.Rank))
            {
                if (item.Key.Agent.Organization.Equals(org) && (!_isMatchingRank || rankFiter(item.Key.Agent.Rank)))
                {
                    foreach (var fork in item.Value)
                    {
                        if (CheckAssignmentState(fork.Self) == AssignmentSeatingState.Empty)
                            yield return fork.Self;
                    }
                }
            }
        }

        // --- Occupation --- //
        private IEnumerable<List<SeatArrangement>> GetOccupationByConsolidationRule(SeatConsolidationRule rule)
        {
            List<List<SeatArrangement>> result = new List<List<SeatArrangement>>();

            Func<int, bool> rankFiter = rank => rule.MaxRank >= rank && rule.MinRank <= rank;

            Func<IEnumerable<Skill>, bool> skillsFiter = agentSkills =>
            {
                var agnetSkillsCount = agentSkills.Count();
                if (rule.MatchWholeSkills && rule.Skills.Count() != agnetSkillsCount)
                    return false;
                //return rule.Skills.Intersect(agentSkills).Count() == agnetSkillsCount;
                return agentSkills.Intersect(rule.Skills).Count() == rule.Skills.Count;
            };

            Func<ISeatingTerm, bool> assignmentTypeFilter = seatingTerm =>
            {
                var assignment = (AssignmentBase)seatingTerm;
                if (rule.AssignmentTypes.Count == 0)
                    return true;
                return rule.AssignmentTypes.Any(o => o.Name == assignment.Text);

                //return rule.AssignmentTypes.Contains(assignment.Style as AssignmentType);
            };

            foreach (var org in rule.Organizations)
            {
                foreach (var seatArrList in GetOccupationByOrganization(org, rankFiter, skillsFiter, assignmentTypeFilter))
                {
                    bool isAdded = false;
                    foreach (var seatArrangement in seatArrList)
                    {
                        var baseLineTime = new DateTime(seatArrangement.Start.Year, seatArrangement.Start.Month, seatArrangement.Start.Day);
                        double min = (seatArrangement.Start - baseLineTime).TotalMinutes;
                        if (min >= rule.StartValue && min <= rule.EndValue
                            && rule.AppliedDayOfWeeks[(int)baseLineTime.DayOfWeek])
                        {
                            isAdded = true;
                            break;
                        }
                        baseLineTime = baseLineTime.AddDays(-1.0);
                        min = (seatArrangement.Start - baseLineTime).TotalMinutes;
                        if (rule.EndValue >= 1440 && min >= rule.StartValue && min <= rule.EndValue
                            && rule.AppliedDayOfWeeks[(int)baseLineTime.DayOfWeek])
                        {
                            isAdded = true;
                            break;
                        }
                    }
                    if (isAdded)
                        result.Add(seatArrList);
                }
            }
            return result.OrderBy(o => o[0].Start);
        }

        private IEnumerable<List<SeatArrangement>> GetOccupationByEmployee(ISimpleEmployee employee)
        {
            return _employeeSeatOccupationRegister.Keys.Contains(employee) ?
                _employeeSeatOccupationRegister[employee].Where(so => so.Any(o => o.Seat == null)) :
                new List<List<SeatArrangement>>().AsEnumerable();
        }

        private IEnumerable<List<SeatArrangement>> GetOccupationByOrganization(Entity org, Func<int, bool> rankFiter, Func<IEnumerable<Skill>, bool> skillsFiter,
           Func<ISeatingTerm, bool> assignmentTypeFilter)
        {
            List<List<SeatArrangement>> result = new List<List<SeatArrangement>>();
            foreach (var employee in _employeeSeatOccupationRegister.Keys.OrderBy(emp => emp.Rank))
            {
                if (employee.Organization.Equals(org) &&
                    (rankFiter(employee.Rank)) && skillsFiter(employee.Skills.Keys))
                {
                    //if (_employeeSeatOccupationRegister.Keys.Contains(employee))
                    //{

                    //foreach (var seatArrangement in _employeeSeatOccupationRegister[employee].Where(o => assignmentTypeFilter(o.GetSourceParent())))
                    foreach (List<SeatArrangement> seatArrList in _employeeSeatOccupationRegister[employee])
                    {
                        bool isUnseated = false;
                        foreach (SeatArrangement seatArrangement in seatArrList.Where(o => assignmentTypeFilter(o.GetSourceParent())))
                        {
                            if (seatArrangement.Seat == null)
                            {
                                isUnseated = true;
                                break;
                            }
                            //yield return seatArrangement;
                        }
                        if (isUnseated)
                            result.Add(seatArrList);
                        //yield return seatArrList;
                    }
                    //}
                }
            }
            return result.OrderBy(o => o[0].Start);
        }

        // --- Seat --- //
        private static IEnumerable<ISeat> GetSeatByRule(IArrangeSeatRule organizationSeatingRule, IEnumerable<ISeat> targetAreaSeats, bool includeUnOpenSeat)
        {
            if (organizationSeatingRule.Methodology == ArrangeSeatMethodology.Sequence)
            {
                List<ISeat> targetAreaSeatsArray = targetAreaSeats.OrderBy(o => o.Index).ToList();
                int seatAmount = targetAreaSeatsArray.Count;
                int currentIndex = targetAreaSeatsArray.IndexOf(organizationSeatingRule.TargetSeat);
                for (int i = 0; i < seatAmount; i++)
                {
                    if (currentIndex >= seatAmount)
                        currentIndex = 0;
                    if (includeUnOpenSeat)
                    {
                        yield return targetAreaSeatsArray[currentIndex];
                    }
                    else if (targetAreaSeatsArray[currentIndex].IsOpen)
                    {
                        yield return targetAreaSeatsArray[currentIndex];
                    }
                    currentIndex++;
                }
            }
            else //if(organizationSeatingRule.Methodology == ArrangeSeatMethodology.Centralize)
            {
                var distance = targetAreaSeats.ToDictionary<ISeat, ISeat, double>
                    (iss =>
                        iss,
                        iss => (iss.XCord - organizationSeatingRule.TargetSeat.XCord) * (iss.XCord - organizationSeatingRule.TargetSeat.XCord)
                            + (iss.XCord - organizationSeatingRule.TargetSeat.YCord) * (iss.XCord - organizationSeatingRule.TargetSeat.YCord));

                foreach (var kvp in distance.OrderBy(dn => dn.Value))
                {
                    if (includeUnOpenSeat)
                        yield return kvp.Key;
                    else if (kvp.Key.IsOpen)
                        yield return kvp.Key;
                }
            }
        }

        // --- Private Method --- //
        private bool SetOccupationSeat(List<SeatArrangement> occupations, ISeat targetSeat)
        {
            if (occupations.Any(o => o.Seat != default(ISeat)) || !targetSeat.InUse)
                return false;

            if (_isMatchingRank && occupations.Any(o => o.Agent.Rank < targetSeat.Rank))
                return false;

            bool success = true;
            foreach (var occ in occupations)
            {
                DateTime extendStart = occ.Start.AddMinutes(-_bufferLength);
                DateTime extendEnd = occ.End.AddMinutes(_bufferLength);

                if (!_seatOccupationRegister.ContainsKey(targetSeat))
                    return false;

                foreach (var o in _seatOccupationRegister[targetSeat])
                {
                    if (extendStart < o.End && extendEnd > o.Start)
                        success = false;
                }
            }
            if (success)
            {
                foreach (var occ in occupations)
                {
                    _seatOccupationRegister[targetSeat].Add(occ);
                    occ.Seat = targetSeat;
                }

            }
            return success;
        }

        //private static SeatArrangement[] SortSeatOccupationArray(IEnumerable<SeatArrangement> occupationList)
        //{
        //    SeatArrangement[] soArray = occupationList.ToArray();
        //    Array.Sort(soArray, (oa, ob) =>
        //    {
        //        if (oa.Start < ob.Start)
        //            return -1;
        //        else if (oa.Start == ob.Start)
        //            return 0;
        //        else
        //            return 1;
        //    }
        //        );
        //    return soArray;
        //}

        private int[] getState()
        {
            AssignmentSeatingState checkResult;
            int completeAssAmount = 0;
            int completeOccAmount = 0;
            int seprateOccAmount = 0;
            int partialOccAmount = 0;
            foreach (var agent in _participaters)
            {
                if (_employeeAssignmentRegister.Keys.Contains(agent))
                {
                    foreach (var fork in _employeeAssignmentRegister[agent])
                    {
                        checkResult = CheckAssignmentState(fork.Self);
                        if (checkResult == AssignmentSeatingState.Full)
                        {
                            completeAssAmount++;
                            foreach (var termList in _termOccupationRegister[fork.Self])
                            {
                                completeOccAmount += termList.Count;
                            }
                            //completeOccAmount += _termOccupationRegister[fork.Self].Count;
                        }
                        else if (checkResult == AssignmentSeatingState.FullButSeprate)
                        {
                            foreach (var termList in _termOccupationRegister[fork.Self])
                            {
                                seprateOccAmount += termList.Count;
                            }
                            //seprateOccAmount += _termOccupationRegister[fork.Self].Count;
                        }
                        else if (checkResult == AssignmentSeatingState.Partial)
                        {
                            foreach (var termList in _termOccupationRegister[fork.Self])
                            {
                                partialOccAmount += termList.Where(so => so.Seat != default(ISeatingSeat)).Count();
                            }
                            //partialOccAmount +=
                            //    _termOccupationRegister[fork.Self].Where(so => so.Any(o=>o.Seat != default(ISeatingSeat))).Count();
                        }
                    }
                }
            }
            return new int[]
                      { completeOccAmount, 
                        completeOccAmount + seprateOccAmount + partialOccAmount,
                        completeAssAmount };
        }

        #endregion Methods
    }
}
