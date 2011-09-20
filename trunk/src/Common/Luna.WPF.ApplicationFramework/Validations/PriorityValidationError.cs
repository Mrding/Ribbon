using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luna.WPF.ApplicationFramework.Validations
{
    public class PriorityValidationError
    {

        public int Priority { get; set; }

        public Guid ValidatorKey { get; set; }

        public string ErrorContent { get; set; }
    }
}
