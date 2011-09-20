using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luna.WPF.ApplicationFramework
{
    /// <summary>
    /// Defines the interface a control implements to get or set its priority content
    /// </summary>
    public interface IPriority
    {
        int Priority { get; set; }
    }
}
