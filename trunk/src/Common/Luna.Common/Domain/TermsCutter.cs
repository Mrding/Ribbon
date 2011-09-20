using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Luna.Common
{
    public class TermsCutter<T, TResult> where T : ITerm
    {
        public delegate TResult CreateInstance(DateTime start, DateTime end, T startTarget);
        public delegate bool CanCreateInstance(T begin, T current);
        public delegate TResult CreateInstancePre(DateTime start, DateTime end,T previous, T startTarget);
        public delegate bool CanCreateInstancePre(T begin, T previous, T current);


        [DebuggerDisplay("{Time}")]
        protected class Dot
        {
            public Dot ParentDot { get; set; }
            public bool IsStart { get; private set; }
            public Dot Next { get; set; }

            public DateTime Time { get; private set; }
            public T Target { get; private set; }

            public Dot(DateTime time, T term, bool isStart)
            {
                IsStart = isStart;
                Time = time;
                Target = term;
            }

            public T GetActualTarget()
            {
                if (IsStart)
                    return Target;
                Dot parent = ParentDot;
                //while(parent!=null&&parent.Time==Time)
                //{
                //    parent = parent.ParentDot;
                //}

                return parent != null ? parent.Target : default(T);
            }
        }

        private readonly CreateInstance _instanceFunc;
        private readonly Dot _headDot;
        private int _cutCount;
        public TermsCutter(IEnumerable<T> terms, Func<T, bool> termsFilter, CreateInstance instanceFunc)
        {
            _instanceFunc = instanceFunc;
            _headDot = new Dot(DateTime.MinValue, default(T), true);
            int count = 0;
            foreach (var term in terms)
            {
                if(!termsFilter(term)) continue;

                Dot startDot = new Dot(term.Start, term, true);
                Dot endDot = new Dot(term.End, term, false);
                startDot.Next = endDot;

                FillStartDot(ref startDot);
                count++;
            }

            _cutCount = count*2;
        }

        protected void FillStartDot(ref Dot startDot)
        {
            Dot tempDot = _headDot;
            while (tempDot.Next != null && tempDot.Next.Time <= startDot.Time)
            {
                tempDot = tempDot.Next;
            }

            FillEndDot(startDot.Next, tempDot.Next);

            if (tempDot.IsStart)
                startDot.ParentDot = tempDot;
            else
                startDot.ParentDot = tempDot.ParentDot;

            tempDot.Next = startDot;
        }

        protected void FillEndDot(Dot endDot, Dot beginDot)
        {
            while (beginDot != null && beginDot.Time < endDot.Time)
            {
                beginDot = beginDot.Next;
            }

            endDot.Next = beginDot;
            if (beginDot != null)
            {
                if (beginDot.IsStart)
                    endDot.ParentDot = beginDot.ParentDot;
                else
                    endDot.ParentDot = beginDot;
            }
        }

        public IList<TResult> ToList(CanCreateInstance canCreateInstance)
        {
            return ToList(canCreateInstance, null, null);
        }

        public IList<TResult> ToList(CreateInstancePre createInstancePre, CanCreateInstancePre canCreateInstancePre)
        {
            return ToList(null, createInstancePre, canCreateInstancePre);
        }

        public IList<TResult> ToList(CanCreateInstance canCreateInstance,
            CreateInstancePre createInstancePre, CanCreateInstancePre canCreateInstancePre)
        {
            List<TResult> list = new List<TResult>(_cutCount);

            if (_headDot.Next == null)
                return list;

            Dot currentDot = _headDot.Next;
            Dot beginDot = _headDot;
            T previousT = default(T);

            var m1 = new Action<DateTime, DateTime,T, T>((t1, t2,previous, target) =>
            {
                var newInstance =createInstancePre!=null?createInstancePre(t1,t2,previous,target): _instanceFunc(t1, t2, target);
                if (!ReferenceEquals(newInstance, default(TResult)))
                    list.Add(newInstance);
            });

            while (currentDot != null)
            {
                if (currentDot.Next == null)
                {
                    m1(beginDot.Time, currentDot.Time, previousT, beginDot.GetActualTarget());
                    break;
                }

                if (currentDot.Time != currentDot.Next.Time)
                {
                    if (beginDot == _headDot)
                    {
                        beginDot = currentDot;
                        continue;
                    }

                    var target = beginDot.GetActualTarget();

                    bool canAdd = canCreateInstancePre != null
                                      ? canCreateInstancePre(target, previousT, currentDot.GetActualTarget())
                                      : canCreateInstance(target, currentDot.GetActualTarget());
                    if (canAdd)
                    {
                        m1(beginDot.Time, currentDot.Time,previousT, target);
                        beginDot = currentDot;
                    }

                    previousT = target;
                }
                currentDot = currentDot.Next;
            }

            return list;
        }
    }
}