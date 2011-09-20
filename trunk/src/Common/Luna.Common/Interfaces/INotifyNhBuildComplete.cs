using System;

namespace Luna.Common.Interfaces
{
    public interface INotifyNhBuildComplete
    {
        event EventHandler<EventArgs> NHibernateBuildCompleted;

        void AfterBuildComplete(Action action);

        bool IsBuildCompleted { get; }
    }
}
