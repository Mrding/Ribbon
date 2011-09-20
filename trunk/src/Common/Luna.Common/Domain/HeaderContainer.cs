using System;
using System.Collections;
using System.Collections.Generic;
using Luna.Common.Interfaces;

namespace Luna.Common.Domain
{
    public class HeaderContainer<T, TItem, TIndex> : IEnumerable, IIntIndexer<TItem>, IDisposable
    {
        private Func<TIndex, int> _indexResolver;

        public HeaderContainer(T entity, Func<TIndex, int> indexResolver)
        {
            Header = entity;
            _indexResolver = indexResolver;
        }

        public virtual T Header { get; private set; }

        public virtual IList<TItem> Items { get; set; }

        public virtual TItem this[TIndex dateTime]
        {
            get
            {
                var i = _indexResolver(dateTime);
                if (i < 0)
                    return default(TItem);
                return Items[i];
            }
            set
            {
                var i = _indexResolver(dateTime);
                if (i >= 0)
                    Items[i] = value;
            }
        }

        public virtual IEnumerator GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public TItem this[int index]
        {
            get { return Items[index]; }
        }

        public object GetItem(int index)
        {
            return (this as IIntIndexer<TItem>)[index];
        }

        public void Dispose()
        {
            Header = default(T);
            Items.Clear();
            Items = null;
            _indexResolver = null;
        }
    }
}