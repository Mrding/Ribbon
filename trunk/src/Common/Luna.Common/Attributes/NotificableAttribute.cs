using System;

namespace Luna.Common
{
    public class NotificableAttribute : Attribute
    {
        public Type AdditionalType { get; set; }
    }
}
