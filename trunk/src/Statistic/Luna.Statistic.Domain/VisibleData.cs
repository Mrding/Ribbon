using System;
using System.Linq;
using Luna.Common;
using Luna.Common.Interfaces;

namespace Luna.Statistic.Domain
{
    public class VisibleData : IVisibleLinerData, IIntIndexer
    {
        public virtual string Color { get; set; }

        public virtual double ZeroBaseValue { get; set; }

        public virtual double MaxValue { get; set; }

        public virtual double[] Values { get; set; }

        public string Text { get; set; }

        public string Format { get; set; }

        public object Source { get; set; }

        public int Category { get; set; }


        public object GetItem(int index)
        {
            if (Values.Length <= index)
                return 0;
            return Values[index];
        }
    }
}