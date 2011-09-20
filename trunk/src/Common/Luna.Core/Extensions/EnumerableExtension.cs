using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;


namespace Luna.Core.Extensions
{
    public static class EnumerableExtension
    {
        public static void Foreach(this DateTime[] range, Action<DateTime> action)
        {
            var t = range[0];
            while (t < range[1])
            {
                action(t);
                t = t.Add(TimeSpan.FromDays(1));
            }
        }

        public static IEnumerable ForEach<T>(this IEnumerable source, Action<T> action)
        {
            foreach (T item in source)
            {
                action(item);
            }
            return source;
        }

        public static void ForEach<T>(this IEnumerable source, Action<T, int> action)
        {
            var i = 0;
            foreach (var item in source)
            {
                if (item is T)
                {
                    action((T)item, i);
                    i++;
                }
            }
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
            return source;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            var i = 0;
            foreach (var item in source)
            {
                action(item, i);
                i++;
            }
            return source;
        }

        public static IEnumerable<T> ForEach<T>(
            this IEnumerable<T> source, Action<T> action, Predicate<T> condition)
        {
            foreach (var item in source)
            {
                if (condition(item))
                    action(item);
            }
            return source;
        }

        public static IEnumerable<T> For<T>(this IEnumerable<T> source, Action<int> action)
        {
            for (int i = 0; i < source.Count(); i++)
            {
                action(i);
            }
            return source;
        }

        public static bool EqualValue<T>(this IEnumerable<T> source1, IEnumerable<T> source2)
        {
            if (source1 == source2)
                return true;
            if (source1.Count() != source2.Count())
                return false;
            for (int i = 0; i < source1.Count(); i++)
            {
                if (!source1.ElementAt(i).Equals(source2.ElementAt(i)))
                    return false;
            }
            return true;
        }

        public static IEnumerable<T> Add<T>(this IEnumerable<T> source, T item)
        {
            foreach (var obj in source)
            {
                yield return obj;
            }
            yield return item;
        }

        public static IEnumerable<T> Remove<T>(this IEnumerable<T> source, T item)
        {
            foreach (var obj in source)
            {
                if (!obj.Equals(item))
                    yield return obj;
            }
        }

        public static IEnumerable<T> GetRange<T>(this IEnumerable<T> source, int start, int end)
        {
            int index = 0;
            int count = end - start + 1;
            bool begin = false;
            foreach (var item in source)
            {
                if (index == start)
                    begin = true;
                if (begin)
                {
                    if (count > 0)
                    {
                        yield return item;
                        count--;
                    }
                }
                index++;
            }
        }

        public static IDictionary<TKey, TSource> ToDistinctDictionary<TSource, TKey>(
            this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            Dictionary<TKey, TSource> dictionary = new Dictionary<TKey, TSource>(source.Count());
            foreach (var obj in source)
            {
                dictionary[keySelector(obj)] = obj;
            }
            return dictionary;
        }

        public static IEnumerable<TResult> TransformTree<TSource, TResult>(
         this IEnumerable<TSource> enumerable,
         Func<TSource, IEnumerable, TResult> transformFunction,
         Func<TSource, IEnumerable<TSource>> getChildrenFunction)
        {
            foreach (var item in enumerable)
            {
                yield return transformFunction.Invoke(item,
                    getChildrenFunction.Invoke(item)
                                       .TransformTree(transformFunction, getChildrenFunction)
                    );
            }
        }

        public static int Count(this IEnumerable source)
        {
            return Enumerable.Count(source.Cast<object>());
        }

        public static int IndexOf(this IEnumerable source, object value)
        {
            int index = -1;
            foreach (var o in source)
            {
                if (Equals(o, value))
                    return index + 1;
                index++;
            }
            return index;
        }

        public static IEnumerable<T> ToEnumerable<T>(this IEnumerable source)
        {
            return from object o in source select (T)o;
        }

        public static List<object> ToList(this IEnumerable source)
        {
            var enumerable = source.ToEnumerable<object>();
            return enumerable.ToList<object>();
        }

        public static bool Contains<T>(this IEnumerable<T> source, T item, Func<T, object> keyExtractor)
        {
            return source.Contains(item, new KeyEqualityComparer<T>(keyExtractor));
        }

        public static bool Contains<T>(this IEnumerable<T> source, T item, Func<T, T, bool> comparer)
        {
            return source.Contains(item, new KeyEqualityComparer<T>(comparer));
        }
    }
    public class KeyEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _comparer;
        private readonly Func<T, object> _keyExtractor;

        // Allows us to simply specify the key to compare with: y => y.Id
        public KeyEqualityComparer(Func<T, object> keyExtractor) : this(keyExtractor, null) { }

        // Allows us to tell if two objects are equal: (x, y) => y.Id == x.Id
        public KeyEqualityComparer(Func<T, T, bool> comparer) : this(null, comparer) { }

        public KeyEqualityComparer(Func<T, object> keyExtractor, Func<T, T, bool> comparer)
        {
            _keyExtractor = keyExtractor;
            _comparer = comparer;
        }

        public bool Equals(T x, T y)
        {
            if (_comparer != null)
                return _comparer(x, y);

            var valX = _keyExtractor(x);
            if (valX is IEnumerable<object>) // The special case where we pass a list of keys
                return ((IEnumerable<object>)valX).SequenceEqual((IEnumerable<object>)_keyExtractor(y));
            return valX.Equals(_keyExtractor(y));
        }

        public int GetHashCode(T obj)
        {
            if (_keyExtractor == null)
                return obj.ToString().ToLower().GetHashCode();
            var val = _keyExtractor(obj);
            if (val is IEnumerable<object>) // The special case where we pass a list of keys
                return (int)((IEnumerable<object>)val).Aggregate((x, y) => x.GetHashCode() ^ y.GetHashCode());
            return val.GetHashCode();
        }
    }
}
