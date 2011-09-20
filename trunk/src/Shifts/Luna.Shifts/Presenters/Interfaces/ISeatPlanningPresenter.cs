using Caliburn.PresentationFramework.ApplicationModel;
using Luna.Shifts.Domain;

namespace Luna.Shifts.Presenters.Interfaces
{
    public interface ISeatPlanningPresenter : IPresenter
    {
        Area OpenedArea { get; }
    }
}
