using System;
using System.Collections.Generic;

namespace Luna.Core.Extensions
{
    public static class DateTimeExtension
    {
        public static DateTime ConvertToMultiplesOfFive(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute - value.Minute % 5, 0);
        }

        public static int IntDays(this DateTime start, DateTime end)
        {
            //End日期正好为整天，1日12:00--3日00:00，结果为3-1=2
            if (end == end.Date)
                //return end.DayOfYear - start.DayOfYear;
                return (int)(end - start).TotalDays;
            //日期补1天。1日01:00--3日23:00，结果为3-1+1=3，同一天则是1-1+1=1
            else
                //return end.DayOfYear - start.DayOfYear + 1;
                return (int)(end - start).TotalDays +1;
        }

        public static TimeSpan Round(this TimeSpan timeSpan)
        {
            var hour = (int)timeSpan.TotalHours;
            var minute = (int)Math.Round(timeSpan.TotalMinutes % 60d);
            if (minute == 60)
            {
                hour++;
                minute = 0;
            }
            return new TimeSpan(hour, minute, 0);
        }

        public static DateTime Round(this DateTime dateTime)
        {
            var hour = dateTime.Hour;
            var minute = (int)Math.Round(dateTime.TimeOfDay.TotalMinutes % 60d);
            if (minute == 60)
            {
                hour++;
                minute = 0;
            }
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hour, minute, 0);
        }

        public static DateTime RemoveSeconds(this DateTime dateTime)
        {
            if (dateTime.Second != 0 || dateTime.Millisecond != 0)
                return new DateTime(dateTime.Year,dateTime.Month,dateTime.Day,dateTime.Hour,dateTime.Minute,0);
            return dateTime;
        }
    }
}
