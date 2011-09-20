using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Luna.Core.Extensions
{
    public static class IListExtention
    {
        public static int AddNotContains(this IList list, object item)
        {
            if (list.Contains(item)) return -1;

            return list.Add(item);
        }
    }

    public static class ArrayExtension
    {
        public static bool DvideTo5Min(this int index, bool[] source)
        {
            var baseIndex = index * 3;
            if (baseIndex+2 >= source.Length) return false;
            
            return source[baseIndex] || source[baseIndex + 1] || source[baseIndex + 2];
        }

        public static IEnumerable<T> SliceElements<T>(this T[] values, int start, int count)
        {
            int end = start + count - 1;

            if (start < 0)
            {
                start = 0;
            }
            if (end >= values.Length)
            {
                end = values.Length - 1;
            }
            for (int i = start; i <= end; i++)
            {
                yield return values[i];
            }
        }

        public static IEnumerable<T> GetFromEnd<T>(this T[] array, int count)
        {
            for (int i = array.Length - 1; i > array.Length - 1 - count; i--)
            {
                yield return array[i];
            }
        }

        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            return array == null || array.Length == 0;
        }

        public static bool[] ConvertToArray(this string booleanArrayString, int defaultCapcity)
        {
            if (string.IsNullOrEmpty(booleanArrayString)) return new bool[defaultCapcity];
            return booleanArrayString.Split(new[] { ';' }).Select(o => System.Convert.ToBoolean(o.Trim())).ToArray();
        }
    }
}