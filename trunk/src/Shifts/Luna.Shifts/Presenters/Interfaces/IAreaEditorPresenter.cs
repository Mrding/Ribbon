using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Luna.Common;
using Luna.Shifts.Domain;

namespace Luna.Shifts.Presenters.Interfaces
{
    public interface IAreaEditorPresenter : IPresenter, IAction<Site>, IPredicate<Site>
    {
        BindableCollection<Site> Sites { get; }

        Entity SelectedSite { get; set; }
    }
}
