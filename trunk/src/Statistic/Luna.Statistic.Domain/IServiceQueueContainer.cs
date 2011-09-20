using System;
using System.Collections;
using System.Collections.Generic;
using Luna.Infrastructure.Domain;

namespace Luna.Statistic.Domain
{
    public interface IServiceQueueContainer : IDictionary<int, IServiceQueueStatistic>, IDisposable
    {
        void CalculateForecastStatistics(double[][] shrinkage, int startWeekDayIndex);
        void CalculateAssignedStatistics(int index, double? value);
        //void Method3(ref double[] sumOfForecastStaffing);
        void Output(Action<StatisticRaw> action, out IEnumerable list);

        //本為班次估算用(合併所有的serviceQueues)
        //xvoid Output2(out IEnumerable lines, ref Action callback);

        IServiceQueueStatistic this[Skill skill] { get; }
        IEnumerable GetEntities();

        IEnumerable GetStaffingStatistics();

        double[] GetStaffingDemanded(int startIndex, int length, IDictionary<Skill, double> skills);

        int CoverageDays { get; }
    }
}