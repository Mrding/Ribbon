using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luna.Core.Extensions
{
    public static class NullExtention
    {
        public static bool Any(params object [] objs)
        {
           return objs.Any(o => o == null);
        }
    }
}
