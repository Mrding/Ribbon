using System;
using Iesi.Collections;
using Luna.Common;
using Luna.Common.Constants;

namespace Luna.Infrastructure.Domain
{
    public class LaborRule : Entity
    {
        public LaborRule()
        {

            this.StdDailyLaborHour = new TimeSpan(0, 60, 0);
            this.MinIdleGap = new TimeSpan(0, 60, 0);
            this.MaxOvertime = new TimeSpan(0, 0, 0);
            this.MaxShrinked = new TimeSpan(0, 0, 0);
            this.MaxSwapTimes = 0;
            this.MCWD = 2;
            this.MCDO = 2;
        }

        public virtual LaborRule SetDefaultProperty()
        {
            //TODO:Verify who set DefaultMaxLaborHour value to cache
            this.MaxLaborHour = new TimeSpan(ApplicationCache.Get<int>(Global.DefaultMaxLaborHour), 0, 0);
            this.MinLaborHour = new TimeSpan(ApplicationCache.Get<int>(Global.DefaultMinLaborHour), 0, 0);
            //xGroupingArrangeShift = new GroupingArrangeShift();
            DayOffRule = new DayOffRule();
            DayOffMask = new MaskOfDay();
            return this;
        }

        private TimeSpan _maxOvertime;

        /// <summary>
        /// 最大加班工时
        /// </summary>
        public virtual TimeSpan MaxOvertime
        {
            get { return _maxOvertime; }
            set
            {
                if (value < TimeSpan.FromHours(1))
                {
                    _maxOvertime = TimeSpan.FromHours(1);
                }
                if (value > TimeSpan.FromMinutes(1920))
                {
                    _maxOvertime = TimeSpan.FromMinutes(1920);
                }
                else
                {
                    _maxOvertime = value;
                }
            }
        }

        private TimeSpan _maxShrinked;

        /// <summary>
        /// 最大减班工时
        /// </summary>
        public virtual TimeSpan MaxShrinked
        {
            get { return _maxShrinked; }
            set
            {
                if (value < TimeSpan.FromMinutes(60))
                {
                    _maxShrinked = TimeSpan.FromMinutes(60);
                }
                if (value > TimeSpan.FromMinutes(1920))
                {
                    _maxShrinked = TimeSpan.FromMinutes(1920);
                }
                else
                {
                    _maxShrinked = value;
                }
            }
        }

        private int _mcdo = 2;

        /// <summary>
        /// 最大连续休假日数
        /// </summary>
        public virtual int MCDO
        {
            get { return _mcdo; }
            set
            {
                if (value < 2)
                {
                    _mcdo = 2;
                }
                if (value > 5)
                {
                    _mcdo = 5;
                }
                else
                {
                    _mcdo = value;
                }
            }
        }

        private int _mcwd = 2;

        /// <summary>
        /// 最大连续上班天数
        /// </summary>
        public virtual int MCWD
        {
            get { return _mcwd; }
            set
            {
                if (value < 2)
                {
                    _mcwd = 2;
                }
                if (value > 15)
                {
                    _mcwd = 15;
                }
                else
                {
                    _mcwd = value;
                }
            }
        }

        private TimeSpan _minIdleGap = new TimeSpan(0, 60, 0);

        /// <summary>
        /// 最小班距
        /// </summary>
        public virtual TimeSpan MinIdleGap
        {
            get { return _minIdleGap; }
            set
            {
                if (value < TimeSpan.FromMinutes(60))
                {
                    _minIdleGap = TimeSpan.FromMinutes(60);
                }
                if (value > TimeSpan.FromMinutes(960))
                {
                    _minIdleGap = TimeSpan.FromMinutes(960);
                }
                else
                {
                    _minIdleGap = value;
                }
            }
        }

        private TimeSpan _stdDailyLaborHour = new TimeSpan(0, 60, 0);

        /// <summary>
        /// 每日标准工时
        /// </summary>
        public virtual TimeSpan StdDailyLaborHour
        {
            get { return _stdDailyLaborHour; }
            set
            {
                if (value < TimeSpan.FromMinutes(60))
                {
                    _stdDailyLaborHour = TimeSpan.FromMinutes(60);
                }
                if (value > TimeSpan.FromMinutes(1440))
                {
                    _stdDailyLaborHour = TimeSpan.FromMinutes(1440);
                }
                else
                {
                    _stdDailyLaborHour = value;
                }
            }
        }

        /// <summary>
        /// 最大工时
        /// </summary>
        public virtual TimeSpan MaxLaborHour { get; set; }
        /// <summary>
        /// 最小工时
        /// </summary>
        public virtual TimeSpan MinLaborHour { get; set; }

        public override string GetUniqueKey()
        {
            return Name;
        }

        //增加的属性
        private int _maxSwapTimes;

        /// <summary>
        /// 代换班次数上限
        /// </summary>
        public virtual int MaxSwapTimes
        {
            get { return _maxSwapTimes; }
            set
            {
                if (value < 0)
                {
                    _maxSwapTimes = 0;
                }
                if (value > 30)
                {
                    _maxSwapTimes = 30;
                }
                else
                {
                    _maxSwapTimes = value;
                }
            }
        }

        public virtual DayOffRule DayOffRule { get; set; }
        public virtual MaskOfDay DayOffMask { get; set; }
        
        /*
        /// <summary>
        /// 組排班規則
        /// </summary>
        public virtual GroupingArrangeShift GroupingArrangeShift { get; set; }


        private ISet _shiftGroups;

        
        /// <summary>
        /// 班群組,非優先的priority= -1,優先的群組 priority從0開始(0為最優先)
        /// </summary>
        public virtual ISet ShiftGroups
        {
            get { return _shiftGroups; }
            set { _shiftGroups = value; }
        }*/

        //public virtual ISet<Organization> Organizations { get; set; }
    }
}
