using System.Collections.Generic;
using System.Collections;

namespace Luna.Common.Interfaces
{
    public interface IHierarchical
    {
        IHierarchical Parent { get; set; }
        ICollection<IHierarchical> Children { get; }
    }
}