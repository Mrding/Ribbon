using System.Collections.Specialized;

namespace Luna.Common
{
    public static class ApplicationCache
    {
        private static readonly HybridDictionary Dictionary = new HybridDictionary();

        public static T Get<T>(object key)
        {
            if (Dictionary.Contains(key))
                return (T)Dictionary[key];
            return default(T);
        }

        public static object Get(object key)
        {
            return Dictionary[key];
        }

        public static void Set(object key, object value)
        {
            Dictionary[key] = value;
        }

        public static void Set<T>(object key, T value)
        {
            Dictionary[key] = value;
        }

        public static void Add(object key, object value)
        {
            Dictionary.Add(key, value);
        }

        public static void Remove(object key)
        {
            Dictionary.Remove(key);
        }

        public static bool Contains(object key)
        {
            return Dictionary.Contains(key);
        }

        public static void Clear()
        {
            Dictionary.Clear();
        }
    }
}
