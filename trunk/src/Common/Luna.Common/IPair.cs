using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luna.Common
{
    public interface IPair<T>
    {
        T First { get; set; }
        T Second { get; set; }
    }
}
