using System;
using System.Collections;
using Luna.Common;
using Luna.Core.Extensions;
using Luna.Infrastructure.Domain;
using System.Collections.Generic;
using System.Linq;

namespace Luna.Statistic.Domain
{
    public class ServiceQueueContainer : Dictionary<int, IServiceQueueStatistic>, IServiceQueueContainer
    {
        private Action<double[][], int> _calculateForecastStaffing;
        //xprivate Action<double[]> _sumForecastStaffing;
        private Action<int, double?> _calculateAssignedServiceLevel;
        private readonly Dictionary<Skill, IServiceQueueStatistic> _skillMapping;
        private readonly IDictionary<DateTime, int> _dateIndexer;
        private readonly List<StatisticRaw> _list;
        //private readonly List<IVisibleLinerData> _lines;
        private readonly IEnumerable _entities;
        private Func<double[], int, string, object, IVisibleLinerData> _itemContstruction;

        //xprivate readonly IList<IStaffingStatistic> _staffingStatistics;

        public ServiceQueueContainer(IEnumerable serviceQueues, ITerm dateRange, Func<double[], int, string, object, IVisibleLinerData> itemContstruction)
            : base(serviceQueues.Count())
        {
            _entities = serviceQueues;
            _itemContstruction = itemContstruction;
            _skillMapping = new Dictionary<Skill, IServiceQueueStatistic>(Count);
            _list = new List<StatisticRaw>(Count);
            //_lines = new List<IVisibleLinerData>();

            //x_staffingStatistics = new List<IStaffingStatistic>(); // estimation

            _calculateForecastStaffing = (shrinkage, startWeekDayIndex) => { };
            _calculateAssignedServiceLevel = (i, staffing) => { };
            //x_sumForecastStaffing = d => { };

            //使用非排班期起始, 用的是前後扩展過的日期范围
            _dateIndexer = Common.Extensions.DateTimeExt.CreateDateIndexer(dateRange.Start, dateRange.End);

            foreach (ServiceQueue item in serviceQueues)
            {
                var mappedSkill = item.MappedSkill;

                new ServiceQueueStatistic(item, _dateIndexer).Self(t =>
                {
                    var x = new StatisticRaw(t, _itemContstruction);

                    _calculateForecastStaffing += (shrinkage, startWeekDayIndex) =>
                    {
                        t.CalculateForecastStaffing(shrinkage, startWeekDayIndex);
                        x.SetForceastValues();
                    };

                    // check service is single mode
                    //x_sumForecastStaffing += result => { Sum(t.ForceastStaffing, ref result); };

                    _calculateAssignedServiceLevel += (index, staffing) =>
                                                          {
                                                              if (staffing != null)
                                                                  t.AssignedStaffing[index] = staffing.Value; // value always zero

                                                              t.CalculateAssignedServiceLevel(index);
                                                              //t.CalculateStaffingRequirement(index);
                                                              t.CalculateDailyMisc(index / 96);
                                                              x.SetAssignedValues(index);
                                                          };
                    _skillMapping[mappedSkill] = t;
                    this[t.GetHashCode()] = t;
                    _list.Add(x);
                });
            }
        }

        public IEnumerable GetEntities()
        {
            return _entities;
        }

        public IEnumerable GetStaffingStatistics()
        {
            return _list.Select(o => new StaffingStatistic(o.Source.Entity, o.Source.ForceastStaffing, _itemContstruction)).ToList();
        }

        public double[] GetStaffingDemanded(int startIndex, int length, IDictionary<Skill, double> skills)
        {
            var sumOfStaffingDemanded = new double[length]; // 人力需求量
          
            var qtStartIndex = startIndex / 3; //quarterIndex
            for (var i = 0; i < length; i++)
            {
                foreach (var skill in skills)
                {
                    var q = this[skill.Key];
                    if (q == null) continue;
                    sumOfStaffingDemanded[i] += q.ForceastStaffing[(i/3) + qtStartIndex];//consider skill productivity
                }
            }
            return sumOfStaffingDemanded;
        }

        public IServiceQueueStatistic this[Skill skill]
        {
            get { return _skillMapping.ContainsKey(skill) ? _skillMapping[skill] : null; }
        }

        public int CoverageDays { get { return _dateIndexer.Count; } }

        //xprivate void Sum(double[] source, ref double[] destination)
        //x{
        //xvar count = destination.Length;
        //xfor (var i = 0; i < count; i++)
        //x{
        //xdestination[i] += source[i];
        //x}
        //x}

        ///// <summary>
        ///// Sum AssignedStaffing
        ///// </summary>
        //public void Method3(ref double[] sumOfForecastStaffing)
        //{
        //    _sumForecastStaffing(sumOfForecastStaffing);
        //}

        public void CalculateForecastStatistics(double[][] shrinkage, int startWeekDayIndex)
        {
            //预测人力计算,只会执行一次
            _calculateForecastStaffing(shrinkage, startWeekDayIndex);
            //xvar sumOfForecastStaffing = new double[_dateIndexer.Count * 96];
            //x_sumForecastStaffing(sumOfForecastStaffing);
            //x_staffingStatistics.Add(new StaffingStatistic(sumOfForecastStaffing, _itemContstruction));
        }

        /// <summary>
        /// CalculateAssignedServiceLevel
        /// </summary>
        /// <param name="index">15min index</param>
        /// <param name="value">Staffing</param>
        public void CalculateAssignedStatistics(int index, double? value)
        {
            _calculateAssignedServiceLevel(index, value);
        }


        //估算用
        //public void Output2(out IEnumerable lines, ref Action callback)
        //{
        //    callback = _staffingStatistics.Aggregate(callback, (current, o) => current + o.Output());
        //    lines = _staffingStatistics;
        //}

        public void Output(Action<StatisticRaw> action, out IEnumerable list)
        {
            Values.ForEach((q, i) =>
                               {
                                   q.Output();
                                   _list[i].Output();
                                   action(_list[i]);
                               });
            list = _list;
        }

        public void Dispose()
        {
            _itemContstruction = null;
            foreach (var statisticRaw in _list)
            {
                statisticRaw.Dispose();
            }
            _list.Clear();
            //x_staffingStatistics.Clear();
            _calculateAssignedServiceLevel = null;
        }
    }
}