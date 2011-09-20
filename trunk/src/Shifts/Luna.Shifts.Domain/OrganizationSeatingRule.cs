using Luna.Common;

namespace Luna.Shifts.Domain
{
    public interface IArrangeSeatRule : IIndexable
    {
        ArrangeSeatMethodology Methodology { get; set; }

        ISeat TargetSeat { get; set; }
    }

    
}
