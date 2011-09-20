using Luna.Common;

namespace Luna.Statistic.Domain
{
    /// <summary>
    /// 目前班次估算使用
    /// </summary>
    public interface IStaffingStatistic
    {
        //double[] Forceast { get; }
        // double[] Estimate { get; }
        double M1(int index, double value);
        void Push(int index, int length, double value);
        double GetShortfall(int index, int length);

        double GetDeviation(int startIndex, int endIndex, int dateIndex);

        void Reset();
        System.Action Output();

        //void Pull(int index, double value);

        IVisibleLinerData Difference { get; }

        object Entity { get; }

        //IVisibleLinerData OverStaff { get; }
        //IVisibleLinerData UnderStaff { get; }
    }
}