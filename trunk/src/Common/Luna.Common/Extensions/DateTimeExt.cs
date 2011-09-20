using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Luna.Common.Constants;
using Luna.Core;

namespace Luna.Common.Extensions
{
    public static class DateTimeExt
    {
        public static DateRange Extend(this DateRange value, double hours, DateRange limitation)
        {
            //var cantExtendedHours = 0;
            DateTime start;
            if (value.Start.AddHours(-hours).TryCorrectIfLess(limitation.Start, out start))
            {
                //cantExtendedHours = 6;
            }
            DateTime end;
            if (value.End.AddHours(hours).TryCorrectIfOver(limitation.End, out end))
            {

            }
            return new DateRange(start, end);

        }

        /// <summary>
        /// prevention beyond the boundaries
        /// </summary>
        public static bool TryCorrectIfLess(this DateTime dateTime, DateTime min, out DateTime result)
        {
            if (dateTime < min)
            {
                result = min;
                return true;
            }
            else
            {
                result = dateTime;
                return false;
            }
        }

        /// <summary>
        /// prevention beyond the boundaries
        /// </summary>
        public static bool TryCorrectIfOver(this DateTime dateTime, DateTime max, out DateTime result)
        {
            if (dateTime > max)
            {
                result = max;
                return true;
            }
            else
            {
                result = dateTime;
                return false;
            }
        }

        public static int IndexOf(this DateTime date, Dictionary<DateTime, int> sources)
        {
            if (!sources.ContainsKey(date))
                return -1;
            return sources[date];
        }

        public static int IndexOf(this DateTime date, DateTime start)
        {
            if (date < start)
                return -1;
            return (int) date.Subtract(start).TotalDays;
        }


        private static bool Is(this DateTime dateTime, string key)
        {
            var globalCalendar = ApplicationCache.Get<Dictionary<DateTime, Dictionary<string, bool>>>(Global.GlobalCalendar);
            if (globalCalendar == null || !globalCalendar.ContainsKey(dateTime) || string.IsNullOrEmpty(key))
                return false;

            return globalCalendar[dateTime].ContainsKey(key) && globalCalendar[dateTime][key];
        }

        //public static bool IsHoliday(this DateTime date)
        //{
        //    return Is(date, Country.);
        //}
        
        public static bool IsHoliday(this DateTime date, string country)
        {
            return Is(date, country);
        } 

        public static bool IsDaylightSaving(this DateTime date, TimeZoneInfo timeZoneInfo)
        {
            if (timeZoneInfo == default(TimeZoneInfo)) return false;
            return Is(date, timeZoneInfo.Id);
        }

        public static bool IsInTheRange(this DateRange range, ITerm term)
        {
            return range.Start >= term.Start && range.End <= term.End;
        }

        public static bool IsInTheRange(this DateRange range, DateRange value)
        {
            return range.Start >= value.Start && range.End <= value.End;
        }



        public static bool IsInTheRange(this DateTime dateTime, ITerm term)
        {
            return dateTime.IsInTheRange(term.Start, term.End);
        }

        public static bool IsInTheRange(this DateTime dateTime, DateTime from, DateTime end)
        {
            return dateTime >= from && dateTime < end;
        }




        public static DateTime TurnToMultiplesOf5(this DateTime value)
        {
            if (value.Second > 0)
                value = DateTime.Parse(value.ToString("yyyy/MM/dd HH:mm:00"));
            var mod = -value.Minute % 5;
            return mod == 0 ? value : value.AddMinutes(mod);
        }

        public static int TurnToMultiplesOf5(this int value)
        {
            var mod = -value % 5;
            if (mod == 0)
                return value;
            return value + mod;
        }


        /// <summary>
        /// Get index avoid index out of range
        /// </summary>
        /// <param name="termStart"></param>
        /// <param name="boundaryStart"></param>
        /// <param name="unit"></param>
        /// <param name="boundaryCapacity"></param>
        /// <param name="outOfIndexValue"></param>
        /// <returns></returns>
        public static int GetIndex(this DateTime termStart, DateTime boundaryStart, int unit, int boundaryCapacity, int outOfIndexValue)
        {
            var index = Convert.ToInt32(termStart.Subtract(boundaryStart).TotalMinutes) / unit; //usually is 5
            index = index < 0 ? outOfIndexValue : index >= boundaryCapacity ? boundaryCapacity - 1 : index;
            return index;
        }

        /// <summary>
        /// Unsafely Get Index
        /// </summary>
        /// <param name="termStart"></param>
        /// <param name="boundaryStart"></param>
        /// <param name="unit"></param>
        /// <param name="boundaryCapacity"></param>
        /// <returns></returns>
        public static int GetIndex(this DateTime termStart, DateTime boundaryStart, int unit, int boundaryCapacity)
        {
            var index = Convert.ToInt32(termStart.Subtract(boundaryStart).TotalMinutes) / unit; //usually is 5

            return index;
        }

        public static Dictionary<DateTime, int> CreateDateIndexer(DateTime start, DateTime end)
        {
            var startDate = start.Date;

            var viewDays = Convert.ToInt32(end.Date.Subtract(startDate).TotalDays);

            var dateToIndexTable = new Dictionary<DateTime, int>(viewDays);

            for (var i = 0; i < viewDays; i++)
            {
                var date = startDate.AddDays(i);
                dateToIndexTable[date] = i;
            }

            return dateToIndexTable;
        }
    }
}
