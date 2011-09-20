using System;
using Luna.Core.Extensions;

namespace Luna.Infrastructure.Domain
{
    public class MaskOfDay
    {
        private int _weekdays2 = 0;
        private int _weekdays = 127;
        private bool[] _weekDayMask, _weekDayMask2, _monthDayMask; // 缓存加速用

        public MaskOfDay()
        {
            //Weekdays = WEEKDAYS;
            //Monthdays = MONTHDAYS;
        }

        /// <summary>
        /// Dayoff marking
        /// </summary>

        public virtual int Weekdays
        {
            get { return _weekdays; }
            set
            {
                _weekdays = value;
                _weekDayMask = null;
            }
        }

        /// <summary>
        /// Workingday marking
        /// </summary>

        public virtual int Weekdays2
        {
            get { return _weekdays2; }
            set
            {
                _weekdays2 = value;
                _weekDayMask2 = null;
            }
        }

        //private bool[] _weekDayMask;
        public bool[] WeekDayMask
        {
            get
            {
                return _weekdays.ToBooleanArray(7);
            }
            //set { _weekDayMask = value; }
        }

        public bool[] WeekDayMask2
        {
            get
            {
                return _weekdays2.ToBooleanArray(7);
            }
            //set { _weekDayMask = value; }
        }

        private int _monthdays = int.MaxValue;
        public virtual int Monthdays
        {
            get { return _monthdays; }
            set
            {
                _monthdays = value;
                _monthDayMask = null;
            }
        }

        //private bool[] _monthDayMask;
        public bool[] MonthDayMask
        {
            get
            {
                return _monthdays.ToBooleanArray(31);
            }
        }

        public bool CanWork(DateTime day, bool isHoliday)
        {
            var dayOfWeek = (int)day.DayOfWeek;

            if (_weekDayMask == null)
                _weekDayMask = WeekDayMask;

            if (_weekDayMask2 == null)
                _weekDayMask2 = WeekDayMask2;

            if (_monthDayMask == null)
                _monthDayMask = MonthDayMask;

            return (isHoliday ? _weekDayMask2[dayOfWeek] : _weekDayMask[dayOfWeek]) && _monthDayMask[day.Day-1]; //注意day.Day只有1~31而且是1起算所以要减去1
        }
    }
}