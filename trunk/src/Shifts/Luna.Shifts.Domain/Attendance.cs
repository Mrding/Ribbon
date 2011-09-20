using System;
using System.Collections.Generic;
using Luna.Common;
using Luna.Infrastructure.Domain;

namespace Luna.Shifts.Domain
{
    public static class ValueLimitationExt
    {
        public static bool IsOverlap(this double value, double max, double min)
        {
            return value > max || value < min;
        }
    }


    public class Attendance : AbstractEntity<int>, IAttendance, ITerm
    {
        public virtual T SetProperties<T>(Employee agent, Entity campaign, DateTime start, DateTime end) where T : Attendance
        {
            _agent = agent;
            if (_agent.EnrollmentDate >= end || _agent.LaborRule == null)
            {
                CanNotEnroll = true;
                return this as T;
            }

            _campaign = campaign;
            Start = _agent.EnrollmentDate > start ? _agent.EnrollmentDate : start;
            End = end;


            return this as T;
        }

        public virtual string LaborRule { get { return Agent.LaborRule == null ? string.Empty : Agent.LaborRule.Name; } }

        public virtual SchedulingPayload SchedulingPayload { get; set; }

        private TimeSpan _duration;
        public virtual double Duration
        {
            get
            {
                if (_duration == default(TimeSpan))
                    _duration = _end.Subtract(_start);

                return _duration.TotalDays;
            }
        }

        private bool _joined = true;

        public virtual bool Joined
        {
            get { return _joined; }
            set { _joined = value; }
        }

        public virtual bool CanNotEnroll { get; set; }

        public virtual void MarkAsNew()
        {
            Id = 0;
        }

        public override int Id { get; protected set; }

        private ISimpleEmployee _agent;
        public virtual ISimpleEmployee Agent
        {
            get { return _agent; }
            set { _agent = value; }
        }

        private DateTime _start;
        public virtual DateTime Start
        {
            get { return _start; }
            set { _start = value; }
        }

        private DateTime _end;
        public virtual DateTime End
        {
            get { return _end; }
            set { _end = value; }
        }



        private Entity _campaign;
        public virtual Entity Campaign
        {
            get { return _campaign; }
            set { _campaign = value; }
        }

        public virtual TimeSpan OvertimeTotals { get; set; }

        public virtual TimeSpan ShrinkedTotals { get; set; }

        public virtual TimeSpan MaxOvertimeThreshold { get; set; }

        public virtual TimeSpan MaxShrinkedThreshold { get; set; }

        public virtual DayOffRule DayOffRule { get; set; }

        private int _amountDayOff;
        
        public virtual int AmountDayOff
        {
            get { return _amountDayOff; }
            set
            {
                if (DayOffRule != null && value < DayOffRule.MinimumTotalDayOff)
                    _amountDayOff = DayOffRule.MinimumTotalDayOff;
                else
                    _amountDayOff = value;

                WorkingDayCount = (int)Duration - _amountDayOff;
            }
        }

    

        /// <summary>
        /// 最大连续休假日数
        /// </summary>
        public virtual int MCDO { get; set; }
        /// <summary>
        /// 最大连续上班天数
        /// </summary>
        public virtual int MCWD { get; set; }
        /// <summary>
        /// 最小班距
        /// </summary>
        public virtual TimeSpan MinIdleGap { get; set; }
        /// <summary>
        /// 每日标准工时
        /// </summary>
        public virtual TimeSpan StdDailyLaborHour { get; set; }

        private TimeSpan _maxLaborHour;

        /// <summary>
        /// 最大工时
        /// </summary>
        public virtual TimeSpan MaxLaborHour
        {
            get { return _maxLaborHour; }
            set
            {
                if (value > _minLaborHour)
                    _maxLaborHour = value;
            }
        }

        private TimeSpan _minLaborHour;

        /// <summary>
        /// 最小工时
        /// </summary>
        public virtual TimeSpan MinLaborHour
        {
            get { return _minLaborHour; }
            set
            {
                if (value < _maxLaborHour)
                    _minLaborHour = value;
            }
        }

        public virtual TimeSpan LaborHourTotals { get; set; }

        public virtual int MaxSwapTimes { get; set; }

        /// <summary>
        /// 参与排班期天数
        /// </summary>
        public virtual int WorkingDayCount { get; set; }

        /// <summary>
        /// 没有限定到职日期
        /// </summary>
        public virtual bool EnrolmentDateNotQualify
        {
            get
            {
                return Agent.EnrollmentDate >= End;
            }
        }

        private double? _score;
        public virtual double? Score
        {
            get { return _score; }
            set { _score = value; }
        }

        public virtual int FullWeekendTotals { get; set; }

        public virtual int PartialWeekendTotals { get; set; }

        private ICollection<Exception> _exceptions;

        public virtual ICollection<Exception> Exceptions
        {
            get { return _exceptions; }
            set
            {
                _exceptions = value;
                HasError = !HasError;
            }
        }



        //public override bool Equals(IGenericEntity<int> obj)
        //{
        //    var other = obj as Attendance;
        //    if(other == null) return false;
        //    return other.Id == Id && other.TimeEquals(this) && other.Campaign.Equals(Campaign) && other.Agent.Equals(Agent);
        //}



        public virtual bool HasError
        {
            get
            {
                var hasError = (_exceptions != null && _exceptions.Count > 0) ||
                LaborHourTotals.TotalMinutes.IsOverlap(MaxLaborHour.TotalMinutes, MinLaborHour.TotalMinutes) ||
                OvertimeTotals.TotalMinutes.IsOverlap(MaxOvertimeThreshold.TotalMinutes, -1)
                || ShrinkedTotals.TotalMinutes.IsOverlap(MaxShrinkedThreshold.TotalMinutes, -1);
                return hasError;
            }
            set
            {
                //do nothing, just only for notification
            }
        }
    }
}
