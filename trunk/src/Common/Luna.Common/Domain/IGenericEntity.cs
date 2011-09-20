using System;

namespace Luna.Common
{
    public interface IGenericEntity<TIdentity> : IEquatable<IGenericEntity<TIdentity>>
    {
        TIdentity Id { get; }
    }
}
