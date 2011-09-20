using Iesi.Collections.Generic;
using Luna.Common;
using Luna.Infrastructure.Domain;
using Iesi.Collections;

namespace Luna.Shifts.Domain
{
    public interface IShiftGroup : IIndexable
    {
        ISet AssignmentTypes { get; }
    }

    public class ShiftGroup : Entity, IShiftGroup
    {
        public ShiftGroup()
        {
        }

        public virtual ShiftGroup SetDefaultProperty()
        {
            AssignmentTypes = new HashedSet<AssignmentType>();
            WorkingDayMask = new MaskOfDay();
            return this;
        }
        

        public ShiftGroup(ShiftGroup shiftGroup)
        {
            WorkingDayMask = shiftGroup.WorkingDayMask;
            IsOneAssignmentTypeOnly = shiftGroup.IsOneAssignmentTypeOnly;
            MaxGroupTimes = shiftGroup.MaxGroupTimes;
            MinGroupTimes = shiftGroup.MinGroupTimes;
            MaxContinueGroupTimes = shiftGroup.MaxContinueGroupTimes;
            MinContinueGroupTimes = shiftGroup.MinContinueGroupTimes;
            MinDayOffTimeSpan = shiftGroup.MinDayOffTimeSpan;
            Priority = shiftGroup.Priority;
            InUse = shiftGroup.InUse;
            AssignmentTypes = shiftGroup.AssignmentTypes;
            Index = shiftGroup.Index;
            Mode = shiftGroup.Mode;
            Name = shiftGroup.Name;
        }

        public virtual MaskOfDay WorkingDayMask { get; set; }

        public virtual bool IsOneAssignmentTypeOnly { get; set; }

        private int _maxGroupTimes;
        public virtual int MaxGroupTimes
        {
            get { return _maxGroupTimes; }
            set { _maxGroupTimes = value; }
        }

        public virtual int MinGroupTimes { get; set; }

        public virtual int MaxContinueGroupTimes { get; set; }

        public virtual int MinContinueGroupTimes { get; set; }

        public virtual int MinDayOffTimeSpan { get; set; }

        public virtual int Priority { get; set; }

        public virtual bool InUse { get; set; }

        public virtual ISet AssignmentTypes { get; set; }

    
        #region IIndexable Members

        public virtual int Index
        {
            get
            {
                return Priority;
            }
            set
            {
                Priority = value;
            }
        }

        public virtual IndexingMode Mode { get; set; }
       

        #endregion
    }
}