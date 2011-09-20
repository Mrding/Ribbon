using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Luna.Common;
using Luna.Common.Constants;
using Luna.Common.Domain;
using Luna.Core;
using Luna.Core.Extensions;
using Luna.Shifts.Domain;
using Luna.Statistic.Domain;
using Luna.Statistic.Domain.Service;
using Luna.WPF.ApplicationFramework.Threads;

namespace Luna.Shifts.Presenters
{
    public partial class ShiftComposerPresenter
    {
        private const int CellUnit = 5;
        private IStaffingCalculatorService _staffingCalculatorService;
        private IServiceQueueContainer _serviceQueueContainer;
        private Action<string> _switchView;

        private IDictionary _staffingCalculatorArgs;

        //xprivate Tuple<int, int> _assignmentTypeCoverage; //5分钟计算单位
        //xprivate Dictionary<AssignmentType, IList<Tuple<int, int>>> _assignmentInsertRuleIntersectionPoints;
        //xprivate Dictionary<AssignmentType, bool[]> _assignmentContributionPositions;
        private bool _shiftEstimatesChanged;

        private IList<Tuple<string, IList>> _statisticItems;
        public ICollectionView StatisticItems
        {
            get { return CollectionViewSource.GetDefaultView(_statisticItems); }
        }

        /// <summary>
        /// 手动指定班次估算值
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="newValue"></param>
        /// <param name="e"></param>
        public void ChangeAssignmentTypeCells(object layer, object newValue, WPF.ApplicationFramework.Controls.CellEditRoutedEventArgs e)
        {
            if(layer == null) return;

            var timeRange = new Core.Reflector(layer).Property<TimeRange>("TimeRange");
            var dataRowRange = new Core.Reflector(layer).Property<int[]>("DataRowRange");

            // HeaderContainer<AssignmentType, DailyCounter<AssignmentType>, DateTime>
            CellTraversal<int, HeaderContainer<AssignmentType, DailyCounter<AssignmentType>, DateTime>>(timeRange, dataRowRange, newValue,
                assignmentType => { }, WhenShiftEstimatesChanged, AssignmentTypes,

                (value, item) => item.SaftyGetProperty<double, DailyCounter<AssignmentType>>(c => c.Max) == value,
                (value, header, item) =>
                {
                    DateTime? effectDate = null;
                    item.SaftyInvoke<DailyCounter<AssignmentType>>(c =>
                    {
                        if (value == c.Max) return;

                        AlterShiftEstimation(c, value); // don't change order
                        //c.Max = value; // don't change order -> move to AlterShiftEstimation method
                        c.IsDirty = true;
                        effectDate = c.Date;
                    });
                    return effectDate;
                }, () =>
                {
                    CellChanged = true;
                }, e);
        }

        private void WhenShiftEstimatesChanged(HeaderContainer<AssignmentType, DailyCounter<AssignmentType>, DateTime> o)
        {
            _shiftEstimatesChanged = true;
        }

        public void RegisterRunStaffingChart(FrameworkElement el)
        {
            _staffingCalculatorArgs = new Dictionary<string, object> {
                                              {"contentView", el},
                                              {"getAgents", new System.Func<IEnumerable<IAgent>>(() => _attendances)},
                                              {"getViewRange", new System.Func<DateRange>(() => new DateRange(SelectionTimeRange.Start.Date.AddDays(-1), SelectionTimeRange.End.Date.AddDays(1)))},
                                              {"schedule", _schedule},
                                              {"Invoker", this},
                                              {"WhenReady", new Action<IServiceQueueContainer, Action<string>>((c, switchView)=>
                                                                                                   {
                                                                                                       _switchView = switchView;
                                                                                                       _serviceQueueContainer = c;
                                                                                                   })},
                                              {"WhenCurrentQueueSelected", new Action<IServiceQueueStatistic>(q=>
                                                                                                  {
                                                                                                      if( q== null) return;

                                                                                                      SelectedServiceQueue = q.ToString();
                                                                                                      //NotifyOfPropertyChange(()=> AssignmentTypes);
                                                                                                      new Thread(() => {
                                                                                                                var days = _dateIndexer.Count;
                                                                                                                for (var i = 0; i < days; i++)
                                                                                                                    SumOfDailyPlanningServiceLevel(q,i);
                                                                                                                NotifyOfPropertyChange(() => StatisticItems);
                                                                                                        }).Start();
                                                                                                  })}
                                          };
            _staffingCalculatorService = Container.Resolve<IStaffingCalculatorService>(_staffingCalculatorArgs);
            _autoReset.Set();
        }

        private string _selectedServiceQueue;
        public string SelectedServiceQueue
        {
            get { return _selectedServiceQueue; }
            set { _selectedServiceQueue = value; NotifyOfPropertyChange(() => SelectedServiceQueue); }
        }

        protected DailyCounter<DateTime>[] PlanningShifts { get { return (DailyCounter<DateTime>[])_statisticItems[3].Item2; } }

        protected DailyCounter[] PlanningWorkingHours { get { return (DailyCounter[])_statisticItems[0].Item2; } }

        protected DailyCounter[] PlanningOtHours { get { return (DailyCounter[])_statisticItems[1].Item2; } }

        protected DailyCounter[] PlanningServiceLevel { get { return (DailyCounter[])_statisticItems[2].Item2; } }

        /// <summary>
        /// 结算一天的班次统计(已排的班表次数,和工时, 不包含估算内容)
        /// </summary>
        private void RecalculateDailyStatistic(int indexOfDate, bool isInitialized)
        {
            var dailyWorkingHours = PlanningWorkingHours[indexOfDate];
            dailyWorkingHours.Value = 0; //归零每日总工时

            var dailyShifts = PlanningShifts[indexOfDate];
            dailyShifts.Value = 0; //归零每日总班次

            if (isInitialized)
                _assignmentTypes.Values.ForEach(o => o[indexOfDate].Value = 0); //将某日所有班型归零

            foreach (PlanningAgent agent in _attendances)
            {
                agent[indexOfDate].SaftyInvoke<IVisibleTerm>(t =>
                {
                    if (t.Is<UnknowAssignment>() || t.IsNot<AssignmentBase>())
                        return;

                    var typeName = t.SaftyGetProperty<string, AssignmentBase>(o => o.NativeName);

                    if (!_assignmentTypes.ContainsKey(typeName))
                        return;

                    dailyWorkingHours.Value += t.SaftyGetProperty<double, AssignmentBase>(s => s.WorkingTotals.TotalHours);
                    dailyShifts.Value++;
                    _assignmentTypes[typeName][indexOfDate].Value++;
                });
            }
        }

        //单日班次估算加总
        private void DailySetAssignmentTypeEval(DateTerm value)
        {
            //班次统计
            var dailyStatistic = PlanningShifts;
            _currentDateAssignmentTypes[value.Date] = new List<DailyCounter<AssignmentType>>(_assignmentTypes.Count);

            foreach (var assignmentType in _assignmentTypes.Values)
            {
                dailyStatistic[value.Index].Max += assignmentType[value.Index].Max; //加上单日班次估算值;
                _currentDateAssignmentTypes[value.Date].Add(assignmentType[value.Index]);
            }
        }

        private void SumOfDailyPlanningServiceLevel(IServiceQueueStatistic serviceQueueStatistic, int index)
        {
            PlanningServiceLevel[index].Value = serviceQueueStatistic.DailyAssignedServiceLevel[index - Global.HeadDayAmount];
        }

        private void Statistic(string assignmentType, IAssignment term, DateTime dateKey, int adding)
        {
            if (!_dateIndexer.ContainsKey(dateKey))
            {

            }

            var indexOfDate = _dateIndexer[Convert.ToDateTime(dateKey)];

            if (_assignmentTypes.ContainsKey(assignmentType))
            {
                _assignmentTypes[assignmentType][indexOfDate].SaftyInvoke(o => o.Value += adding); // 增加某天结算班次 
                PlanningShifts[indexOfDate].Value += adding;

                if (term.Is<OvertimeAssignment>())
                {
                    if (adding < 0)
                        PlanningOtHours[indexOfDate].Value -= term.OvertimeTotals.TotalHours;
                    else
                        PlanningOtHours[indexOfDate].Value += term.OvertimeTotals.TotalHours;
                }
                else if (term.Is<Assignment>())
                {
                    if (adding < 0)
                        PlanningWorkingHours[indexOfDate].Value -= term.WorkingTotals.TotalHours;
                    else
                        PlanningWorkingHours[indexOfDate].Value += term.WorkingTotals.TotalHours;
                }
            }
            //以下如出现unknownAssignement, 则workingHours和adding都会是负数(减去)
        }

        /// <summary>
        /// 修改班次估算统计值
        /// </summary>
        /// <param name="a">班别</param>
        /// <param name="value">班次</param>
        private void AlterShiftEstimation(DailyCounter<AssignmentType> a, int value)
        {
            var indexOfDate = _dateIndexer[a.Date]; // 未来可能会用 a.Date -> HRDate
            var diff = value - a.Max;
            PlanningShifts[indexOfDate].Max += diff;
            a.Max = value;
        }

        private void SumOfDailyShiftEstimation(DateTime date, int index)
        {
            var oldValue = PlanningShifts[index].Max;
            var newValue = _assignmentTypes.Values.Sum(t => t[date].Max);

            if (oldValue == newValue) return;

            PlanningShifts[index].Max = newValue;
            _shiftEstimatesChanged = true;
        }

        /// <summary>
        /// 全部重新向加载后端加载班次预估数据
        /// </summary>
        private void ReloadChangedShiftEstimations()
        {
            if (!_shiftEstimatesChanged) return;

            _assignmentTypes = _shiftDispatcherModel.GetAssignmentTypeDailyCounter(_assignmentTypes.Values, _schedule);
            _bindableAssignmentTypes = null;

            _dateIndexer.ForEach(pair =>
                        {
                            RecalculateDailyStatistic(pair.Value, true);
                            SumOfDailyShiftEstimation(pair.Key, pair.Value);
                        });
            _shiftEstimatesChanged = false;
        }
    }
}
