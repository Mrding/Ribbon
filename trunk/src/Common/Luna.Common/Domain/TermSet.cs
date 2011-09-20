using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Iesi.Collections.Generic;

namespace Luna.Common
{
    public class TermSet<T> : OrderedSet<T> where T : ITerm
    {


        private readonly Dictionary<DateTime, List<T>> _container = new Dictionary<DateTime, List<T>>();

        private bool _isInXAllOperation;

        public TermSet()
            : base()
        {
        }

        public TermSet(ICollection<T> initialValues)
            : base(initialValues)
        {

        }

        public override bool Add(T o)
        {
            bool add = base.Add(o);

            if (add && !_isInXAllOperation)
            {
                //OnPropertyChanged(ISEMPTY_PROPERTY_NAME);
                //OnPropertyChanged(COUNT_PROPERTY_NAME);
                //OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, o));
            }

            if (add)
            {
                AddToContainer(o);
            }
            return add;
        }

        public void AddToContainer(T o)
        {
            //fixed part support long term
            var from = o.Start.Date;
            var end = o.End.Date;

            while (from <= end)
            {
                if (!_container.ContainsKey(from))
                    _container[from] = new List<T>(10);

                _container[from].Add(o);
                from = from.AddDays(1);
            }
        }


        public override bool Remove(T o)
        {
            var itemIndex = 0;
            foreach (var obj in this)
            {
                if (obj.Equals(o)) break;
                itemIndex++;
            }

            bool result = base.Remove(o);
            if (result && !_isInXAllOperation)
            {

                //OnPropertyChanged(ISEMPTY_PROPERTY_NAME);
                //OnPropertyChanged(COUNT_PROPERTY_NAME);
                //OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, o, itemIndex));    
            }

            if (result)
            {
                //fixed part support long term
                var from = o.Start.Date;
                var end = o.End.Date;

                while (from <= end)
                {
                    if (_container.ContainsKey(from))
                        _container[from].Remove(o);
                    from = from.AddDays(1);
                }
            }
            return result;
        }

        /// <summary>
        /// Remove all the specified elements from this set, if they exist in this set.
        /// </summary>
        /// <param name="c">A collection of elements to remove.</param>
        /// <returns>
        /// <see langword="true"/> if the set was modified as a result of this operation.
        /// </returns>
        public override bool RemoveAll(ICollection<T> c)
        {
            bool flag = false;
            _isInXAllOperation = true;
            var itemsRemoved = new List<T>();
            foreach (var item in c)
            {
                bool operationResult = Remove(item);
                flag |= operationResult;
                if (operationResult)
                    itemsRemoved.Add(item);
            }
            if (flag)
            {
                //OnPropertyChanged(ISEMPTY_PROPERTY_NAME);
                //OnPropertyChanged(COUNT_PROPERTY_NAME);
                //OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, itemsRemoved));
            }
            _isInXAllOperation = false;

            return flag;
        }

        public override bool RetainAll(ICollection<T> c)
        {
            bool flag = false;
            _isInXAllOperation = true;
            var itemsRemoved = new List<T>();
            foreach (T item in (IEnumerable)Clone())
            {
                if (c.Contains(item))
                    continue;

                bool operationResult = Remove(item);
                flag |= operationResult;
                if (operationResult)
                    itemsRemoved.Add(item);
            }
            if (flag)
            {
                //OnPropertyChanged(ISEMPTY_PROPERTY_NAME);
                //OnPropertyChanged(COUNT_PROPERTY_NAME);
                //OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, itemsRemoved));
            }
            _isInXAllOperation = false;

            return flag;
        }

        /// <summary>
        /// Adds all the elements in the specified collection to the set if they are not already present.
        /// </summary>
        /// <param name="c">A collection of objects to add to the set.</param>
        /// <returns>
        /// <see langword="true"/> is the set changed as a result of this operation, <see langword="false"/> if not.
        /// </returns>
        public override bool AddAll(ICollection<T> c)
        {
            bool flag = false;
            _isInXAllOperation = true;
            var itemsAdded = new List<T>();
            foreach (T item in c)
            {
                bool operationResult = Add(item);
                flag |= operationResult;
                if (operationResult)
                    itemsAdded.Add(item);
            }
            if (flag)
            {
                //OnPropertyChanged(ISEMPTY_PROPERTY_NAME);
                //OnPropertyChanged(COUNT_PROPERTY_NAME);
                //OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, itemsAdded));
            }
            _isInXAllOperation = false;
            return flag;
        }


        public IEnumerable<T> PickTerms(DateTime start, DateTime end)
        {
            //var sw = Stopwatch.StartNew();
            var duration = Convert.ToInt32(end.Date.Subtract(start.Date).TotalDays);

            if (duration == 0)
                return _container.ContainsKey(start.Date) ? _container[start.Date] : new List<T>();

            var container = new List<T>((duration + 1) * 10); // 假設一天有10個term

            var tempDate = start.Date;
            while (tempDate <= end.Date)
            {
                if (_container.ContainsKey(tempDate))
                    container.AddRange(_container[tempDate]);
                tempDate = tempDate.AddDays(1);
            }

            var result = container.Distinct();

            //sw.Stop();
            //Console.WriteLine("PickTerms Elapsed {0}s", sw.Elapsed.TotalSeconds);
            return result;
        }

        public void RemoveFromContainer(T targetTerm)
        {
            if (_container.ContainsKey(targetTerm.Start.Date))
                _container[targetTerm.Start.Date].Remove(targetTerm);

            if (targetTerm.Start.Date != targetTerm.End.Date)
            {
                if (_container.ContainsKey(targetTerm.End.Date))
                    _container[targetTerm.End.Date].Remove(targetTerm);
            }
        }

        public void Rebuild()
        {
            _container.Clear();

            foreach (var item in this)
            {
                this.AddToContainer((T)item);
            }
        }

    }
}