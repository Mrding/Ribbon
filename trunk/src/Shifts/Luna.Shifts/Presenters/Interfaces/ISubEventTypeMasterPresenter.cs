using Caliburn.PresentationFramework.ApplicationModel;
using Luna.Shifts.Domain;
using Luna.WPF.ApplicationFramework.Interfaces;

namespace Luna.Shifts.Presenters.Interfaces
{
    public interface ISubEventTypeMasterPresenter : IPresenterManager
    {
    }

    public interface ISubEventTypeDetailPresenter : IDetailPresenter<TermStyle>
    {
    }
}