using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luna.Common
{
    
    public interface IQuestion
    {
        string Text { get; set; }

        Answer Answer { get; set; }
    }
}
