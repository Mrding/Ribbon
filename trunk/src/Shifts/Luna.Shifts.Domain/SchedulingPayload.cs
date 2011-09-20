using System.Collections.Generic;
using Luna.Infrastructure.Domain;

namespace Luna.Shifts.Domain
{
    public class SchedulingPayload
    {
        protected virtual int Id { get; set; }

        //补充---------------------
        public virtual int AmountParticipateDay { get; set; }
        public virtual int AmountWorkDay { get; set; }
        public virtual int AmountDayOff { get; set; }

        //添加的四个属性
        public virtual MaskOfDay DayOffMask { get; set; }
       //x public virtual GroupingArrangeShift GroupingArrangeShift { get; set; }

    }
}