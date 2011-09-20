using System;
using Luna.Infrastructure.Domain;

namespace Luna.Statistic.Domain.Impl
{
    static class DateTimeExtension
    {
        internal static bool IsTimeDiveBy15(this DateTime time)
        {
            double totalMinofDay = (time - time.Date).TotalMinutes;
            if (totalMinofDay % 15.0 == 0.0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static DateTime ToNext15mins(this DateTime time)
        {
            if (time.IsTimeDiveBy15())
            {
                return time;
            }
            else
            {
                // 這邊使用特別給15用的算法,較快
                if (time.Minute < 15)
                {
                    return new DateTime(time.Year, time.Month, time.Day, time.Hour, 15, 0);
                }
                else if (time.Minute < 30)
                {
                    return new DateTime(time.Year, time.Month, time.Day, time.Hour, 30, 0);
                }
                else if (time.Minute < 45)
                {
                    return new DateTime(time.Year, time.Month, time.Day, time.Hour, 45, 0);
                }
                else
                {
                    return new DateTime(time.Year, time.Month, time.Day, time.Hour + 1, 0, 0);
                }
            }
        }

        internal static bool IsTimeDiveByN(this DateTime time, int n)
        {
            if (1440 % n != 0)
            {
                throw new Exception("Error: n cant dived 1440 (mins of a day).");
            }
            double totalMinofDay = (time - time.Date).TotalMinutes;
            if (totalMinofDay % n == 0.0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // 方法可以推到可以整除1440的n,但是還沒空去弄,改天再改吧
        /// <summary>
        ///   [0]       [1]       [2]       [3]       [4]       [5]       [6]
        ///    |---------|---------|---------|---------|---------|---------|----
        ///  00:00     00:15     00:30     00:45     01:00     01:15     01:30
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        internal static int dateTime2Index15(this DateTime time, Schedule campaignSchedule)
        {
            // 可以多檢查是否是15分的節點,但是這邊我會另外Check,不要寫在這裡
            double totalMins = (time - campaignSchedule.Start).TotalMinutes;
            return Convert.ToInt32(totalMins / 15);
        }
    }
}
