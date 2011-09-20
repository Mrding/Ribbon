using Caliburn.PresentationFramework.ApplicationModel;
using Luna.Common;

namespace Luna.Infrastructure.Presenters.Interfaces
{
    public interface IAgentFinderPresenter : IPresenter
    {
        //ICanSupportAgentFinder Invoker { get; set; }
        AgentSearchMode SearchMode { get; }
    }
    public enum AgentSearchMode
    {
        Add,
        Replace
    }
}
