using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Collection;
using NHibernate.Collection.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Type;
using NHibernate.UserTypes;
using Luna.Common;
using System.Diagnostics;

namespace Luna.Common
{
    public static class TermSetExt
    {
        private static TermSet<T> GetContainer<T>(this IEnumerable source) where T : ITerm
        {
            var pTermSet = source as PersistentTermSet<T>;

            var termSet = pTermSet == null ? source as TermSet<T> : pTermSet.GetContainer();

            return termSet;
        }

        public static IEnumerable<T> Retrieve<T>(this IEnumerable source, DateTime start, DateTime end) where T : ITerm
        {
            return source.Retrieve(start, end, default(Func<T, bool>));
        }

        public static IEnumerable<T> Retrieve<T>(this IEnumerable source, DateTime start, DateTime end, Func<T, bool> predicate) where T : ITerm
        {
            var termSet = source.GetContainer<T>();
            if (termSet != null)
            {
                var query = termSet.PickTerms(start, end);

                if (predicate != null)
                    query = query.Where(predicate);

                return query;
            }

            return new List<T>();
        }

        public static void Rebuild<T>(this IEnumerable source) where T : ITerm
        {
            var termSet = source.GetContainer<T>();
            if (termSet != null)
                termSet.Rebuild();
        }

        //public static void AddToCache<T>(this IEnumerable source, T[] terms) where T : ITerm
        //{
        //    var termSet = source.GetContainer<T>();
        //    if (termSet != null)
        //        foreach (var item in terms)
        //            termSet.AddToContainer(item);
        //}

        //public static void RemoveFromCache<T>(this IEnumerable source, T[] terms) where T : ITerm
        //{
        //    var termSet = source.GetContainer<T>();
        //    if (termSet != null)
        //        foreach (var item in terms)
        //            termSet.RemoveFromContainer(item);
        //}

        //public static void UpdateCache<T>(this IEnumerable source, T[] terms) where T : ITerm
        //{
        //    var termSet = source.GetContainer<T>();
        //    if (termSet != null)
        //        foreach (var item in terms)
        //            termSet.UpdateCacheIndex(item);
        //}

    }

    public class PersistentTermSet<T> : PersistentGenericSet<T> where T : ITerm
    {
        public PersistentTermSet(ISessionImplementor sessionImplementor)
            : base(sessionImplementor)
        {
        }

        public PersistentTermSet(ISessionImplementor sessionImplementor, Iesi.Collections.Generic.ISet<T> coll)
            : base(sessionImplementor, coll)
        {
            //CaptureEventHandlers(coll);
        }

        public PersistentTermSet() { }

        public TermSet<T> GetContainer()
        {
            return gset as TermSet<T>;
        }

    }
}
