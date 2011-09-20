using Luna.Common;
using Luna.WPF.ApplicationFramework;

namespace Luna.Shifts.Presenters.Interfaces
{
    

    public interface IShiftComposerPresenter : IDockablePresenter
    {
        ITerm Schedule { get; }

        int WorkingDays { get; }

        int Holidays { get; }

        int AttendancesCount { get; }
    }
}
