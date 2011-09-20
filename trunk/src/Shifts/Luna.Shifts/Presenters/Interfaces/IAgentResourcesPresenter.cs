using Caliburn.PresentationFramework.ApplicationModel;
using Luna.Infrastructure.Domain;

namespace Luna.Shifts.Presenters.Interfaces
{
    public interface IAgentResourcesPresenter : IPresenter
    {
        Schedule Schedule { get; set; }
    }
}
