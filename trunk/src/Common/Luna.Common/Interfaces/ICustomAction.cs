using System;

namespace Luna.Common
{
    public interface ICustomAction : IDisposable
    {
        string ResourceKey { get; set; }

        object Model { get; set; }

        MulticastDelegate Action { get; }
    }
}