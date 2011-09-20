using System;

namespace Luna.Core.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class AttachDataAttribute : Attribute
    {
        public AttachDataAttribute(object key, object value)
        {
            Key = key;
            Value = value;
        }

        public object Key { get; private set; }

        public object Value { get; private set; }
    }
}
