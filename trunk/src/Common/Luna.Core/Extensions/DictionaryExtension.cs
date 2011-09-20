using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luna.Core.Extensions
{
    public static class DictionaryExtension
    {
        public static TValue GetValue<TKey,TValue>(this Dictionary<TKey,TValue> dictionary, TKey key, Func<TKey,TValue> func)
        {
            if(!dictionary.ContainsKey(key))
            {
                dictionary[key] = func(key);
            }
            return dictionary[key];
        }
    }
}
