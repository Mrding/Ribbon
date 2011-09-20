using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Data;
using System.Windows.Input;
using Luna.Common;
using Luna.Common.Constants;
using Luna.Common.Domain;
using Luna.Common.Extensions;
using Luna.Core;
using Luna.Core.Extensions;
using Luna.Shifts.Domain;
using Luna.Statistic.Domain;
using Luna.WPF.ApplicationFramework.Threads;
using Luna.Infrastructure.Domain;

namespace Luna.Shifts.Presenters
{
    public partial class ShiftComposerPresenter
    {
        private Thread _estimateShiftThread;
        //xprivate IDictionary<int, IList<string>> _prioritiedAssignmentTypeGroup;

        private int[] _selectionDataRowRange2;
        public object SelectionDataRowRange2
        {
            get { return _selectionDataRowRange2; }
            set
            {
                _selectionDataRowRange2 = (int[])value;

                var yCount = (_selectionDataRowRange2[1] - _selectionDataRowRange2[0]) + 1;
                var xCount = SelectionTimeRange2.GetLength().Days + 1;
                SelectionTypeRange = string.Format("{0:} x {1:}", yCount, xCount);
            }
        }

        public TimeRange SelectionTimeRange2 { get; set; }

        private string _selectionTypeRange = "0 x 0";
        public string SelectionTypeRange
        {
            get { return _selectionTypeRange; }
            set { _selectionTypeRange = value; NotifyOfPropertyChange(() => SelectionTypeRange); }
        }

        private void AnalyisComplete(object obj, CanExecuteRoutedEventArgs e) { e.CanExecute = _staffingCalculatorService != null && _staffingCalculatorService.Ready; }

        private void EstimateShift(object sender, ExecutedRoutedEventArgs e)
        {
            _estimateShiftThread = new Thread(() =>
                                                  {
                                                      var q = _staffingCalculatorService.BeginEstimation();
                                                      NewF(q);
                                                      var action = q.Output();
                                                      UIThread.BeginInvoke(action);
                                                      _estimateShiftThread.Abort();
                                                  }) { IsBackground = true }.Self(t => t.Start());
        }

        private void SendParameter(object sender, ExecutedRoutedEventArgs e)
        {
        _switchView(e.Parameter.ToString());
        }



        private void NewF(IStaffingStatistic staffingStatistic)
        {
            var prioritiedAssignmentTypeGroup = new Dictionary<int, IList<string>>(10);
            var validAssignmentTypes = new Dictionary<string, HeaderContainer<AssignmentType, DailyCounter<AssignmentType>, DateTime>>();

            foreach (var item in _assignmentTypes)
            {
                var termStyle = item.Value.Header;

                if (!termStyle.ServiceQueue.Equals(staffingStatistic.Entity)) continue;

                //优先级分组
                var priority = termStyle.EstimationPriority;
                if (!prioritiedAssignmentTypeGroup.ContainsKey(priority))
                    prioritiedAssignmentTypeGroup.Add(priority, new List<string>());
                prioritiedAssignmentTypeGroup[priority].Add(item.Key);

                validAssignmentTypes[item.Key] = item.Value;
            }

            var dCells = new double[Convert.ToInt32(_enquiryRange.GetLength().TotalMinutes) / CellUnit]; //班别分布量

            var assignmentTypeCoverage = new Tuple<int, int>(int.MaxValue, int.MinValue);

            //整月班表贡献,分布统计(循环次数 = 天 x 班表数量)
            Loop((dateKey, dateIndex, isHoliday) =>
            {
                foreach (var style in validAssignmentTypes)
                {
                    var termStyle = style.Value.Header;

                    if (!termStyle.WorkingDayMask.CanWork(dateKey, isHoliday))
                        continue;

                    if(!style.Value.Header.Locked)
                        style.Value[dateKey].Max = 0; // reset

                    var sensedTermStyle = termStyle.Sense(dateKey);

                    var assignmentStartIndex = (sensedTermStyle.TimeRange.StartValue / CellUnit) + dateIndex;
                    var endIndex = assignmentStartIndex + (sensedTermStyle.TimeRange.Length / CellUnit);

                    #region 求所有AssignmentType时间含盖范围

                    var hi = assignmentTypeCoverage.Item2;
                    var lo = assignmentTypeCoverage.Item1;

                    var range = sensedTermStyle.TimeRange;
                    if (range.StartValue < lo)
                        lo = range.StartValue;

                    if (range.EndValue > hi)
                        hi = range.EndValue;

                    assignmentTypeCoverage.Item1 = lo / CellUnit; //5分钟计量
                    assignmentTypeCoverage.Item2 = hi / CellUnit; //5分钟计量
                    #endregion

                    // 將cell補滿預設為 online = true , 大循环注意
                    for (var i = assignmentStartIndex; i < endIndex; i++)
                        dCells[i] += 1; //班别分布量

                    #region 扣除离线事件位移产生的相交位置(不管怎排都无法贡献的格子位置,一定会造成离线)
                    FindSubEventInsertRuleIntersections(sensedTermStyle.SubEventInsertRules, i =>
                    {
                        dCells[i + assignmentStartIndex] -= 1;//绝对位置使用, 减去无法贡献的位置
                    });
                    #endregion
                }
            });

            //开始一天一天估算
            Loop((dateKey, dateIndex, isHoliday) =>
                     {
                         var pushed = new Dictionary<AssignmentType, List<List<Luna.Core.Tuple<int, int, double>>>>();
                         foreach (var g in prioritiedAssignmentTypeGroup)
                         {
                             var typeNames = g.Value.Select(t => _assignmentTypes[t].Header).Where(t => t.WorkingDayMask.CanWork(dateKey, isHoliday));
                             var fixedEstimation = new Dictionary<string, AssignmentType>();

                             Loop(20, k =>
                                          {
                                              foreach (var termStyle in typeNames) // enumerate assignment type
                                              {
                                                  if(fixedEstimation.ContainsKey(termStyle.Text))
                                                      continue;

                                                  var sensedTermStyle = termStyle.Sense(dateKey);
                                                  var startIndex = (sensedTermStyle.TimeRange.StartValue / CellUnit) + dateIndex;
                                                  var length = sensedTermStyle.TimeRange.Length / CellUnit;
                                                  var endIndex = startIndex + length;
                                                  var unContributionPosition = new bool[length];
                                                  
                                                  var required = 0;

                                                  if(termStyle.Locked)
                                                  {
                                                      required = (int)_assignmentTypes[termStyle.Text][dateKey].Max;
                                                      fixedEstimation[termStyle.Text] = termStyle;
                                                  }
                                                  else
                                                  {
                                                      #region 算法公式
                                                      //找出离线事件永远为离线交集
                                                      FindSubEventInsertRuleIntersections(sensedTermStyle.SubEventInsertRules, i =>
                                                      {
                                                          unContributionPosition[i] = true; //使用相对位置记录, 注意true意思为无法贡献 
                                                      });

                                                      var contributableCells = new List<double>(length);// 可贡献的格子

                                                      var validContributionPositionCount = 0;
                                                      var sumOfRequired = 0d;

                                                      // required(班次) = Sum(Cell班次不足量) / 可贡献的Cell数
                                                      for (var i = startIndex; i < endIndex; i++)
                                                      {
                                                          if (unContributionPosition[i - startIndex])//相對位置
                                                              continue; // 此格无法贡献,因为有离线交集

                                                          //计算Cell班次不足量
                                                          sumOfRequired += staffingStatistic.M1(i, dCells[i]).Self(contributableCells.Add);
                                                          validContributionPositionCount++; // 可贡献的Cell数量
                                                      }

                                                      var avg = (sumOfRequired / validContributionPositionCount);

                                                      (contributableCells.OrderBy(v => Math.Abs(v - avg)).Self(difference =>
                                                      {
                                                          if (MathLib.StandardDeviation(difference.ToArray()) < 1.0) return;
                                                          validContributionPositionCount = (int)(contributableCells.Count * 0.8); // take 80% 

                                                      }).Take(validContributionPositionCount).Sum() / validContributionPositionCount).Self(v =>
                                                      {
                                                          if (v < 0)
                                                              required = (int)(v / 1);
                                                          else if (0 < v)
                                                              required = (int)Math.Ceiling(v);
                                                      });

                                                      var max = _assignmentTypes[termStyle.Text][dateKey].Max + required;

                                                      _assignmentTypes[termStyle.Text][dateKey].Max = Math.Max(0, max);

                                                      #endregion
                                                  }

                                                  if (!pushed.ContainsKey(termStyle))
                                                      pushed[termStyle] = new List<List<Tuple<int, int, double>>>(Math.Abs(required)); // index, length, score

                                                  Contribute(staffingStatistic, required, pushed[termStyle], sensedTermStyle.SubEventInsertRules, startIndex, length); // 相当于 startIndex - endIndex
                                              }
                                              return staffingStatistic.GetDeviation(assignmentTypeCoverage.Item1 + dateIndex, assignmentTypeCoverage.Item2 + dateIndex, dateIndex); //resultOfScore 天离散系数
                                          });
                         }
                         SumOfDailyShiftEstimation(dateKey, dateKey.IndexOf(Schedule.Start));
                     });

            //班次估算值发生变化
            _shiftEstimatesChanged = true;
            NotifyOfPropertyChange(() => AssignmentTypes);
            NotifyOfPropertyChange(() => StatisticItems);//不可移除, 次通知需要响应给staffingChartview
            CellChanged = true;
        }

        /// <summary>
        /// Push/Pull 人力
        /// </summary>
        /// <param name="staffingStatistic">标的的SQ</param>
        /// <param name="required">短缺量,或过剩量有正负数</param>
        /// <param name="contributionHistory">记录贡献位置</param>
        /// <param name="insertRules">可安排的离线规则</param>
        /// <param name="startIndex">班表(Assignment)的起始</param>
        /// <param name="contributionLength">班表(Assignment)有效贡献长度</param>
        private void Contribute(IStaffingStatistic staffingStatistic, int required, List<List<Tuple<int, int, double>>> contributionHistory,
            IEnumerable<SubEventInsertRule> insertRules, int startIndex, int contributionLength)
        {
            var isOverStaff = required < 0;
            var times = Math.Abs(required);

            for (var i = 0; i < times; i++)
            {
                if (isOverStaff)
                {
                    staffingStatistic.Push(startIndex, contributionLength, -1);

                    if (contributionHistory.Count <= i)
                        continue;
                    foreach (var e in contributionHistory[i])
                        staffingStatistic.Push(e.Item1, e.Item2, 1);
                    contributionHistory.RemoveAt(i);
                }
                else // underStaff
                {
                    staffingStatistic.Push(startIndex, contributionLength, 1);

                    contributionHistory.Add(new List<Tuple<int, int, double>>());

                    foreach (var rule in insertRules.Where(r => !r.SubEvent.OnService))
                    {
                        //设定一个初始最大比对变量
                        var lowestScore = new Tuple<int, int, double>(-1, 0, int.MaxValue); // (离线事件startIndex),(离线事件长度),(离线事件在此时间短缺率)

                        var length = rule.SubEventLength / 5; //离线事件长度

                        var rounds = rule.GetPossibleMovementTimes(); // 离线事件可以安排的可能位置
                        do
                        {
                            var occurIndex = startIndex + ((rule.TimeRange.StartValue + (rounds * rule.OccurScale)) / CellUnit); // 离线事件startIndex
                            var score = staffingStatistic.GetShortfall(occurIndex, length); // 离线事件在此时间短缺率
                            if (score < lowestScore.Item3) // 愈小愈好
                                lowestScore = new Tuple<int, int, double>(occurIndex, length, score);
                            rounds--;
                        } while (rounds >= 0);

                        staffingStatistic.Push(lowestScore.Item1, length, -1); //lowestSorce.Item1 意思是最后胜出的离线事件startIndex
                        contributionHistory[i].Add(lowestScore);
                    }
                }
            }
        }

        private void Loop(System.Action<DateTime, int, bool> func)
        {
            foreach (var d in _dateIndexer)
            {
                var dateKey = d.Key;
                var dailyIndex = dateKey.IndexOf(_enquiryRange.Start);
                var dateIndex = dailyIndex * 288; // 5分钟单位计算 1440/5=288
                var isHoliday = dateKey.IsHoliday(Country.Local);

                func(dateKey, dateIndex, isHoliday);
            }
        }

        private static void FindSubEventInsertRuleIntersections(IEnumerable<SubEventInsertRule> rules, Action<int> found)
        {
            foreach (var rule in rules)
            {
                if (rule.SubEvent.OnService)
                    return;
                var reletiveStart = (rule.GetPossibleMovementTimes() * rule.OccurScale) + rule.TimeRange.StartValue;
                var relativeEnd = rule.TimeRange.StartValue + rule.SubEventLength;

                if (relativeEnd > reletiveStart) //发生交集
                {
                    var st = (reletiveStart / CellUnit);
                    var en = (relativeEnd / CellUnit);

                    for (var i = st; i < en; i++)
                        found(i);
                }
            }
        }

        private static void Loop(int times, System.Func<int, double> func)
        {
            double? lastScore = null;
            for (var i = 0; i < times; i++)
            {
                var score = func(i);

                if (lastScore.HasValue && (Math.Abs(lastScore.Value - score) / score) <= 0.02d)
                    break;

                lastScore = score;
            }
        }
    }
}
