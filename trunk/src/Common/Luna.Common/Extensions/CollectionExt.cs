using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Iesi.Collections;

namespace Luna.Common.Extensions
{
    public static class CollectionExt
    {

        public static T FirstOrDefault<T>(this ISet set)
        {
            if (set != null && set.Count > 0)
            {
                T firtItem = default(T);
                foreach (var item in set)
                {
                    if (item is T)
                    {
                        firtItem = (T)item;
                        break;
                    }
                }
                return firtItem;
            }
            return default(T);
        }

        public static IList<TEntity> ToRebuildPriorityList<T, TEntity>(this IEnumerable<T> items, bool sort, Action<TEntity> beforeChangeIndex)
            where T : IIndexable
            where TEntity : class
        {
            var results = new List<TEntity>(items.Count());

            var priority = 0;
            foreach (var item in sort ? items.OrderBy(o => o.Index) : items)
            {
                var entity = item as TEntity;

                if (beforeChangeIndex != null && entity != null)
                {
                    beforeChangeIndex(entity);
                }

                item.Index = priority;
                priority++;

                if (entity != null)
                    results.Add(entity);
            }
            return results;
        }

        public static void RebuildPriority<T, TEntity>(this IEnumerable<T> items, bool sort, Action<TEntity> beforeChangeIndex)
            where T : IIndexable
            where TEntity : class
        {

            var priority = 0;
            foreach (var item in sort ? items.OrderBy(o => o.Index) : items)
            {
                var entity = item as TEntity;

                if (beforeChangeIndex != null && entity != null)
                {
                    beforeChangeIndex(entity);
                }

                item.Index = priority;
                priority++;
            }
        }
    }
}
