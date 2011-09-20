using Luna.Common;
using System.Collections;
using Luna.Infrastructure.Domain;

namespace Luna.Statistic.Domain
{
    public interface IServiceQueueStatistic
    {
        //int ServiceLevelSecond { get; }
        //double AbadonRate { get; }
        double[] AHT { get; }
        double[] CV { get; }

        double[] DailyCV { get; }

        //Calculate from (ForecastLibrary) erlang-C
        double[] ForceastStaffing { get; }

        double[] AssignedStaffing { get; }
        double[] AssignedMaxStaffing { get; }

        double[] ServiceLevelGoal { get; }
        double[] AssignedServiceLevel { get; }

        double[] DailyAssignedServiceLevel { get; }

        void Concat(IDailyObject dailyObject);

        object Entity { get; }

        void Output();
        void Reset(int index);

        void CalculateDailyMisc(int dayOfIndex);
    }
}