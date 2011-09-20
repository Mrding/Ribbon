using System;

namespace Luna.Common.Interfaces
{
    public interface IInitializeNotification
    {
        event EventHandler Initialized;

        bool IsInitialized { get; }
    }
}
