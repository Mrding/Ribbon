using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luna.Common
{
    public interface IEntityFilter
    {
        bool Filter(object obj);
    }
}
