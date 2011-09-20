using System;

namespace Luna.Shifts.Domain
{
    [Flags]
    public enum WorkHourType
    {
        Paid = 0,
        ExtraPaid = 1,
        Unpaid = -1,
        Shrink = -2,
    }
}