using System;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public interface IHorizontalControl
    {
        /// <summary>
        /// Relative
        /// </summary>
        Func<double, DateTime> ScreenToData { get; }
        Func<DateTime, double> DataToScreen { get; }
        Func<double> Step { get; set; }

        Func<double, DateTime> ToData { get; }

        //IAxisConverter<DateTime> AxisConverter { get; }
        //void RefreshX();

        /// <summary>
        /// Between StartTime & EndTime
        /// </summary>
        /// <typeparam name="T">DateTime, Axis(double)</typeparam>
        /// <param name="data">Screen value</param>
        /// <param name="absolutPosition"></param>
        /// <returns></returns>
        bool IsInDataRagne<T>(T data, bool absolutPosition);

        bool IsInViewRagne(double value);
    }

    public interface IVerticalControl
    {
        /// <summary>
        /// Relative
        /// </summary>
        Func<double, int> ScreenToData { get; }
        Func<int, double> DataToScreen { get; }

        Func<double, int> ToData { get; }

        bool IsInViewRagne<T>(T value);
        //IAxisConverter<int> AxisConverter { get; }
        //void RefreshY();
    }
}
