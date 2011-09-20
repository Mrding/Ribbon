//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Luna.Core.Extensions
//{
//    public static class ParallelCollectionExtension
//    {
//        public static IEnumerable<T> ParallelForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
//        {
//            Parallel.ForEach(enumerable, action);
//            return enumerable;
//        }

//        public static IEnumerable<T> ParallelForEach<T>(
//            this IEnumerable<T> enumerable, Action<T> action, Predicate<T> condition)
//        {
//            Parallel.ForEach(enumerable, i =>
//            {
//                if (condition(i))
//                    action(i);
//            });
//            return enumerable;
//        }

//        public static IEnumerable<T> ParallelFor<T>(this IEnumerable<T> enumerable, Action<int> action)
//        {
//            Parallel.For(0, enumerable.Count(), action);
//            return enumerable;
//        }

//        public static IDictionary<TKey, TSource> ToDistinctDictionaryParallel<TSource, TKey>(
//            this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
//        {
//            Dictionary<TKey, TSource> dictionary = new Dictionary<TKey, TSource>(source.Count());
//            Parallel.ForEach(source, obj => dictionary[keySelector(obj)] = obj);
//            return dictionary;
//        }
//    }
//}