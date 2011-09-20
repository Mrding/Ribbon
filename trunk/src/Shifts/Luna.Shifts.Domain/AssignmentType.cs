using System;
using Luna.Common.Extensions;
using Luna.Infrastructure.Domain;

namespace Luna.Shifts.Domain
{
    public class AssignmentType : BasicAssignmentType
    {
        public static AssignmentType DayOff = new AssignmentType { Name = "DO" };

        public virtual MaskOfDay WorkingDayMask { get; set; }



        /// <summary>
        /// 非数据映射属性
        /// </summary>
        public virtual TimeZoneInfo TimeZone
        {
            get { return string.IsNullOrEmpty(TimeZoneInfoId) ? default(TimeZoneInfo) : TimeZoneInfo.FindSystemTimeZoneById(TimeZoneInfoId); }
            set { TimeZoneInfoId = value.Id; }
        }

        private string _timeZoneInfoId = TimeZoneInfo.Local.Id;
        public virtual string TimeZoneInfoId
        {
            get { return _timeZoneInfoId; }
            set { _timeZoneInfoId = value; }
        }

        private ServiceQueue _serviceQueue;
        public virtual ServiceQueue ServiceQueue
        {
            get { return _serviceQueue; }
            set
            {
                if (value != null && value.IsNew()) return;
                _serviceQueue = value;
            }
        }

        private int _estimationPriority = 1;
        public virtual int EstimationPriority
        {
            get { return _estimationPriority; }
            set { _estimationPriority = value; }
        }

        private BasicAssignmentType _template1;

        /// <summary>
        ///  meet holiday event
        /// </summary>
        public virtual BasicAssignmentType Template1
        {
            get { return _template1; }
            set
            {
                //if (value == null || value.Value == null) return;
                //if (value != null && value.Value == null)
                //{
                //    _template1 = null;
                //    return;
                //}
                if (value != null && value.IsNew()) return;
                _template1 = value;
            }
        }

        private BasicAssignmentType _template2;

        /// <summary>
        /// meet daylight saving
        /// </summary>
        public virtual BasicAssignmentType Template2
        {
            get { return _template2; }
            set
            {
                if (value != null && value.IsNew()) return;
                _template2 = value;
            }
        }

        private BasicAssignmentType _template3;

        /// <summary>
        /// >meet daylight saving holiday
        /// </summary>
        public virtual BasicAssignmentType Template3
        {
            get { return _template3; }
            set
            {
                if (value != null && value.IsNew()) return;
                _template3 = value;
            }
        }

        public virtual bool Locked { get; set; }

        public virtual string CustomTag { get; set; }

        public override string Purpose
        {
            get { return "scheduling"; }
        }

        public virtual string Country { get; set; }

        public virtual BasicAssignmentType Sense(DateTime date)
        {
            var applyType = default(BasicAssignmentType);

            var destinationStartTime = TimeZoneInfo.ConvertTime(date.AddMinutes(TimeRange.StartValue), TimeZoneInfo.Local, TimeZone);

            var isDaylightSaving = destinationStartTime.Date.IsDaylightSaving(TimeZone);
            var isHoliday = destinationStartTime.Date.IsHoliday(Country);

            if (isHoliday)
                applyType = Template1;
            if (isDaylightSaving)
                applyType = Template2;
            if (isHoliday && isDaylightSaving)
                applyType = Template3;


            if (applyType == null || applyType.Value == null)
                applyType = this;

            return applyType;
        }
    }
}