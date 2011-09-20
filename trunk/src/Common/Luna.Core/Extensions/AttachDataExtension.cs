using System.Linq;
using System.Reflection;
using Luna.Core.Attributes;

namespace Luna.Core.Extensions
{
    public static class AttachDataExtension
    {
        public static object GetAttachData(this ICustomAttributeProvider provider, object key)
        {
            if (!provider.IsDefined(typeof(AttachDataAttribute), true))
                return null;

            var attributes = provider.GetCustomAttributes(typeof(AttachDataAttribute), true);
            if (attributes.IsNullOrEmpty())
                return null;
            else
                return attributes.As<AttachDataAttribute[]>().First(a => a.Key.Equals(key)).Value;
        }

        public static T GetAttachData<T>(this ICustomAttributeProvider provider, object key)
        {
            return provider.GetAttachData(key).As<T>();
        }
    }
}
