using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Luna.Common;
using Luna.Common.Constants;
using Luna.Common.Extensions;
using Luna.Core;
using Luna.Core.Extensions;
using Luna.Shifts.Domain;
using Luna.Shifts.Presenters.Extentions;
using System.Windows;
using Luna.Common.Domain;

namespace Luna.Shifts.Presenters
{
    using Luna.WPF.ApplicationFramework.Extensions;

    public partial class ShiftComposerPresenter
    {
        private IList<IAgent> _changedAgentList;
        private IList<string[]> _shiftPainterTemplate;

        #region ICanSupportAgentFinder

        public System.Func<bool> FullyRefresh { get; set; }

        public TResult Transform<TResult>(object item) where TResult : class
        {
            return ((IAgent)item).Profile as TResult;
        }

        public bool CanAddTo(object item)
        {
            return true;
        }

        public bool UnselectedAfterUpateQueryResult
        {
            get { return true; }
        }

        #endregion

        private ITerm _initalDrawBlock;
        public object InitalDrawBlock
        {
            get { return _initalDrawBlock; }
            set { _initalDrawBlock = (ITerm)value; }
        }

        private bool _paintShiftIsEnabled;
        public bool PaintShiftIsEnabled
        {
            get { return _paintShiftIsEnabled; }
            set { _paintShiftIsEnabled = value; NotifyOfPropertyChange(() => PaintShiftIsEnabled); }
        }

        private int[] _selectionDataRowRange;
        public object SelectionDataRowRange
        {
            get { return _selectionDataRowRange; }
            set
            {
                _selectionDataRowRange = (int[])value;

                var yCount = (_selectionDataRowRange[1] - _selectionDataRowRange[0]) + 1;
                var l = SelectionTimeRange.GetLength();

                var xCount = (int)(l.TotalDays + 0.9);
                SelectionAgentRange = string.Format("{0:} x {1:}", yCount, xCount);
            }
        }

        private TimeRange _selectionTimeRange;
        public TimeRange SelectionTimeRange
        {
            get { return _selectionTimeRange; }
            set
            {
                _selectionTimeRange = value;
                if (value == null) return;

                var clickDate = value.Start.Date;

                _hourViewScreenStart = clickDate;
                SelectedColumnDate = clickDate;
                NotifyOfPropertyChange(() => CurrentDateAssignmentTypes);
            }
        }

        private string _selectionAgentRange = "0 x 0";
        public string SelectionAgentRange
        {
            get { return _selectionAgentRange; }
            set { _selectionAgentRange = value; NotifyOfPropertyChange(() => SelectionAgentRange); }
        }

        public IEnumerable Agents
        {
            get { return _attendances; }
            set
            {
                BindableAgents = new List<IEnumerable>(_attendances.Count);
                FullyRefresh = () => true;
                this.NotifyOfPropertyChange("AttendancesCount");
            }
        }

        public int AttendancesCount
        {
            get { return _attendances == null ? 0 : _attendances.Count; }
        }

        public IList BindableAgents
        {
            get { return _bindableAgents; }
            set
            {
                if (value == null) return;
                _bindableAgents = value;

                for (var i = 0; i < _attendances.Count; i++)
                {
                    var agent = _attendances[i];
                    _laborHoursCountingModel.SummarizeLaborRule(agent);

                    _bindableAgents.Insert(i, agent);
                }
                this.NotifyOfPropertyChange("BindableAgents");
            }
        }

        private IDictionary<DateTime, IList> _currentDateAssignmentTypes;

        /// <summary>
        /// Popup avaliable assignmentTypes list
        /// </summary>
        public IList CurrentDateAssignmentTypes
        {
            get { return _currentDateAssignmentTypes[SelectionTimeRange.Start.Date]; }
        }

        public void CopyShiftForPainting(object sender, ExecutedRoutedEventArgs e)
        {
            PaintShiftIsEnabled = !PaintShiftIsEnabled;
            RetrieveSelectionRange(BindableAgents);
        }

        public void PaintShift()
        {
            if (!PaintShiftIsEnabled) return;

            CellTraversal<PlanningAgent>(ResetAgentWhenCellEditing, WhenAgentDirty, (agent, dataRowIndex, templateItem, indexOfDate) =>
            {
                var existTerm = agent[indexOfDate] as IAssignment;

                if (existTerm == null || _dateIndexer.Count <= indexOfDate) return false;

                var changed = false;
                var changedDate = default(DateTime?);
                if (templateItem == "DO")
                    changed = !existTerm.Is<UnknowAssignment>() && (changedDate = TryDeleteTerm(agent, existTerm)) != null;
                else
                {
                    if (!_assignmentTypes.ContainsKey(templateItem)) return false;
                    changedDate = ReplaceAssignment(_assignmentTypes[templateItem].Header, agent, agent[indexOfDate]);
                }

                if (changed || changedDate != null)
                {
                    MarkAsChanged(agent, changedDate.Value);
                    return true;
                }
                return false;
            }, BindableAgents, _shiftPainterTemplate, AnyShiftCellValueChanged);
        }

        private DateTime? TryDeleteTerm(IAgent agent, IIndependenceTerm term)
        {
            if (term is UnknowAssignment)
            {
                return null;
                //xthrow new Exception("UnknowAssignment can't delete from TimeBox");
            }

            // 因为删除term之后会导致班表的工时无法参考到离线, 所以请勿变更次行顺序
            var workingHours = term.SaftyGetProperty<double, IAssignment>(o => o.WorkingTotals.TotalHours);

            DateTime? result = null;
            if (_shiftDispatcherModel.DeleteShift(agent, (Term)term))
            {  //以下为删除成功后续处理
                var dateKey = term.SaftyGetHrDate(); // 未来采用HRDate
                term.SaftyInvoke<IAssignment>(o=> Statistic(o.NativeName, o, dateKey, -1));
                result = dateKey;
            }
            return result;
        }

        public void DeleteAssignments(object layer, Luna.WPF.ApplicationFramework.Controls.CellEditRoutedEventArgs e)
        {
            if (layer == null) return;

            CellTraversal(SelectionTimeRange, _selectionDataRowRange, AssignmentType.DayOff, ResetAgentWhenCellEditing, WhenAgentDirty, BindableAgents, (value, item) => item.Is<UnknowAssignment>(),
                (AssignmentType choosedTermStyle, IAgent agent, ITerm term) => TryDeleteTerm(agent, (IIndependenceTerm)term), AnyShiftCellValueChanged, e);
        }

        public void SetAsDayOff()
        {
            CellTraversal(SelectionTimeRange, _selectionDataRowRange, "DO", ResetAgentWhenCellEditing, WhenAgentDirty, BindableAgents, Equals, (string offWorkType, IAgent agent, ITerm term) =>
            {
                bool? dirty = null;
                if (term.IsNot<UnknowAssignment>())
                {
                    if (TryDeleteTerm(agent, (IIndependenceTerm)term) == null)
                        return null; // 该天已存在班表无法删除, 也就无需再创建 (break point)
                    dirty = true;
                }

                var dateKey = term.SaftyGetProperty<DateTime, IAssignment>(t => t.SaftyGetHrDate()); // 未来改用 HRDate
                agent.Schedule.Create(new DayOff(dateKey, TimeSpan.FromDays(1)),
                    (t, success) =>
                    {
                        if (!success)
                            agent.OperationFail = true;
                        else
                            dirty = true;
                    }, true);

                return dirty == true ? dateKey : default(DateTime?);

            }, AnyShiftCellValueChanged, null);
        }

        public void ChangeShfitCells(object newValue, Luna.WPF.ApplicationFramework.Controls.CellEditRoutedEventArgs e)
        {
            IEnumerable<SubEventInsertRule> subeventInsertRules = null;

            CellTraversal(SelectionTimeRange, _selectionDataRowRange, newValue, ResetAgentWhenCellEditing, WhenAgentDirty, BindableAgents, Equals, (AssignmentType choosedTermStyle, IAgent agent, ITerm term) =>
            {
                if (subeventInsertRules == null)
                    subeventInsertRules = _shiftDispatcherModel.LoadSubEventInsertRules(choosedTermStyle);
                return ReplaceAssignment(choosedTermStyle, agent, term);

            }, AnyShiftCellValueChanged, e);
        }

        private DateTime? ReplaceAssignment(AssignmentType choosedTermStyle, IAgent agent, ITerm term)
        {

            bool? dirty = null;
            if (term.IsNot<UnknowAssignment>())
            {
                if (TryDeleteTerm(agent, (IIndependenceTerm)term) == null)
                    return null; // 该天已存在班表无法删除, 也就无需再创建 (break point)
                dirty = true;
            }

            var dateKey = term.SaftyGetProperty<DateTime, IIndependenceTerm>(t => t.SaftyGetHrDate()); // 未来改用 HRDate
            
            var newAssignment = _shiftDispatcherModel.CreateAssignmentWithSenser(dateKey, choosedTermStyle);
            agent.Schedule.Create(newAssignment,
                (t, success) =>
                {
                    if (!success)
                        agent.OperationFail = true;
                    else
                    {
                        dirty = true;
                        t.SaftyInvoke<IAssignment>(a =>
                                                       {
                                                           var assignmentStartIndex = (int)(a.Start.Subtract(_enquiryRange.Start).TotalMinutes / CellUnit);
                                                           //xvar assignmentStartIndex = a.Start.IndexOf(_enquiryRange.Start);


                                                           var staffingDemanded = _serviceQueueContainer.GetStaffingDemanded(
                                                                assignmentStartIndex, (int)a.GetLength().TotalMinutes / CellUnit, agent.Profile.Skills);


                                                           foreach (var rule in choosedTermStyle.SubEventInsertRules)
                                                           {
                                                               var ruleStartIndex = rule.TimeRange.StartValue / CellUnit;

                                                               var eventCellLength = rule.SubEvent.TimeRange.Length / CellUnit;

                                                               var availableOccurTimes = rule.GetAmountOfAvailableOccurPoints();
                                                               var balls = new Dictionary<int, double>();
                                                               for (var i = 0; i < availableOccurTimes; i++)
                                                               {
                                                                   var score = 0.0;
                                                                   for (var j = 0; j < eventCellLength; j++)
                                                                       score += staffingDemanded[i + ruleStartIndex + j];

                                                                   balls[i] = int.MaxValue - score;
                                                               }
                                                               var x = MathLib.CreateRatioSelector(balls).Pick();

                                                               agent.Schedule.ArrangeSubEvent(a, rule, x);
                                                           }


                                                           //agent.Schedule.ArrangeSubEvent(a, choosedTermStyle.GetSubEventInsertRules(), null);
                                                           Statistic(a.NativeName, a, dateKey, 1);
                                                       });
                    }
                }, true);

            return dirty == true ? dateKey : default(DateTime?);
        }

        private void ResetAgentWhenCellEditing(IAgent agent)
        {
            agent.OperationFail = false;
        }

        private void WhenAgentDirty(IAgent agent)
        {
            agent.BuildOnlines();
            _laborHoursCountingModel.SummarizeLaborRule(agent);

            if (_changedAgentList == null)
                _changedAgentList = new List<IAgent>(BindableAgents.Count);

            if (!_changedAgentList.Contains(agent))
                _changedAgentList.Add(agent);
        }

        private void AnyShiftCellValueChanged()
        {
            // update assignment fqc estimate
            NotifyOfPropertyChange(() => AssignmentTypes);
            NotifyOfPropertyChange(() => BindableAgents);
            NotifyOfPropertyChange(() => StatisticItems);
            IsDirty = true; // do not change this, 触发StaffingChartView需要
            CellChanged = true;

            SummarizeLaborHours();
        }

        /// <summary>
        /// 根据改动的范围重新向后端加载人员班表数据, 并且重新统计
        /// </summary>
        private void ReloadChangedAgents()
        {
            if (_changedAgentList == null || _changedAgentList.Count == 0) return;


            _changedAgentList.ReAssignAgents(_bindableAgents, _attendances, (originalSource =>
                       {
                           //var dirtyAgent = originalSource;
                           _shiftDispatcherModel.Reload(ref originalSource);

                           _laborHoursCountingModel.SummarizeLaborRule(originalSource);
                           return originalSource;
                       }));

            _changedAgentList.Clear();

            if (!_shiftEstimatesChanged) // 因为估算数据没有发生任何变化, 所以只要更新范围更改范围内统计就可以
                _alterDateRange.Foreach(date =>
                {
                    if (!_dateIndexer.ContainsKey(date)) return;
                    RecalculateDailyStatistic(_dateIndexer[date], true);
                });

            ResetAlterDateRange();
            NotifyOfPropertyChange(() => BindableAgents);
            IsDirty = false; // 因为会导致UI变化所以UI发生重绘,而自动将正确的数据呈现了, 如果未来UI没发生变化,就必须手动进行UI更新通知
        }


        private string _sorting;
        /// <summary>
        /// asc, des
        /// </summary>
        public string Sorting
        {
            get { return _sorting; }
            set { _sorting = value; NotifyOfPropertyChange(() => Sorting); }
        }

        private Dictionary<string, System.Func<PlanningAgent, PlanningAgent, int>> _sortingFuncs = new Dictionary<string, System.Func<PlanningAgent, PlanningAgent, int>>
                    {
                        { "PW", (x,y) =>
                            {
                                return x.LaborRule.PartialWeekendTotals.CompareTo(y.LaborRule.PartialWeekendTotals);
                            } }, 
                        { "FW", (x,y) =>
                            {
                                return x.LaborRule.FullWeekendTotals.CompareTo(y.LaborRule.FullWeekendTotals);
                            } },
                        {"DO",(x,y)=>
                            {
                                return x.DayOffRemains.CompareTo(y.DayOffRemains);
                            }},
                        {"LR",(x,y)=>
                            {
                                return x.LaborRule.LaborRule.CompareTo(y.LaborRule.LaborRule);
                            }},
                        {"WorkingTotals",(x,y)=> x.LaborRule.LaborHourTotals.TotalHours.CompareTo(y.LaborRule.LaborHourTotals.TotalHours)}
                    };


        private int OrderByAgentName(int compareResult, IAgent x, IAgent y)
        {
            if (compareResult == 0)
                compareResult = x.Profile.Name.CompareTo(y.Profile.Name);
            if (Sorting == "des")
                compareResult = -compareResult;

            return compareResult;
        }



        public void Sort()
        {
            var screenStart = CellMode ? _selectedColumnDate : ScreenStart;

            var screenStartIndex = screenStart.IndexOf(Schedule.Start);


            ((List<IEnumerable>)BindableAgents).Sort(delegate(IEnumerable x, IEnumerable y)
            {
                var a = (PlanningAgent)x;
                var b = (PlanningAgent)y;

                var term1 = CellMode ? a[screenStartIndex] as AssignmentBase : a.Schedule.TermSet.FirstOrDefault(o => o.Level == 0 && ((screenStart < o.End && screenStart <= o.Start) || screenStart.IsInTheRange(o)));
                var term2 = CellMode ? b[screenStartIndex] as AssignmentBase : b.Schedule.TermSet.FirstOrDefault(o => o.Level == 0 && ((screenStart < o.End && screenStart <= o.Start) || screenStart.IsInTheRange(o)));

                if ((term2 == null && term1 == null) || (term1.Is<UnknowAssignment>() && term2.Is<UnknowAssignment>()))
                    return 0;
                if (term1 == null || term1.Is<UnknowAssignment>())
                    return 1;
                if (term2 == null || term2.Is<UnknowAssignment>())
                    return -1;

                var result = term1.Start.CompareTo(term2.Start); // 先比开始时间

                if (result == 0)
                {
                    result = term1.Text.CompareTo(term2.Text); // 开始时间相同比班名
                    return result == 0 ? a.Profile.Name.CompareTo(b.Profile.Name) : result;
                }
                return result;
            });

            this.QuietlyReload(ref _bindableAgents, "BindableAgents");
        }

        private string _lastSortingProperty;
        public void Sort(string property)
        {
            if (_lastSortingProperty != property)
                Sorting = "asc"; // 默認使用 asc
            else
                if (Sorting == "asc") // 反向給值 asc -> des
                    Sorting = "des";
                else
                    Sorting = "asc"; // des -> asc

            ((List<IEnumerable>)BindableAgents).Sort((x, y) =>
            {
                var a = x.As<PlanningAgent>();
                var b = y.As<PlanningAgent>();
                return OrderByAgentName(_sortingFuncs[property](a, b), a, b);
            });

            _lastSortingProperty = property;

            this.QuietlyReload(ref _bindableAgents, "BindableAgents");
        }

        private double _averageWorkingHours;
        public double AverageWorkingHours
        {
            get { return _averageWorkingHours; }
            set { _averageWorkingHours = value; NotifyOfPropertyChange(() => AverageWorkingHours); }
        }

        private double _maxWorkingHours;
        public double MaxWorkingHours
        {
            get { return _maxWorkingHours; }
            set { _maxWorkingHours = value; NotifyOfPropertyChange(() => MaxWorkingHours); }
        }

        private double _minWorkingHours;
        public double MinWorkingHours
        {
            get { return _minWorkingHours; }
            set { _minWorkingHours = value; NotifyOfPropertyChange(() => MinWorkingHours); }
        }

        private void SummarizeLaborHours()
        {
            var min = double.MaxValue;
            var max = double.MinValue;
            var sum = 0.0;
            foreach (var a in _attendances)
            {
                var hours = a.LaborRule.LaborHourTotals.TotalHours;
                if (hours < min)
                    min = hours;
                if (max < hours)
                    max = hours;
                sum += hours;
            }
            AverageWorkingHours = (sum == 0 && _attendances.Count == 0) ? 0 : sum / _attendances.Count;
            MinWorkingHours = min == double.MaxValue ? 0 : min;
            MaxWorkingHours = max == double.MinValue ? 0 : max;
        }
    }
}
