using System.Collections;
using System.Collections.Generic;

namespace Luna.Statistic.Domain
{
    public class Group : IGroup
    {
        private readonly IDictionary<object, double> _inner;
        private int _count;

        public Group()
        {
            _inner = new Dictionary<object, double>();
        }

        public void Add()
        {
            _count++;
        }

        public void Add(object keyEntity, double value)
        {
            if (!_inner.ContainsKey(keyEntity))
                _inner[keyEntity] = value;
            else
                _inner[keyEntity] += value;
        }

        public IEnumerable GetKeyList()
        {
            return _inner.Keys;
        }

        public double GetE(object keyEntity)
        {
            return _inner[keyEntity] / Totals;
        }

        public void BuildF(ref double[,] mtx, int position, ref int hCounter)
        {
            var count = _inner.Count;

            for (var i = 0; i < count; i++)
            {
                mtx[position, hCounter] = 1; // mark
                hCounter++;
            }
        }

        public int Totals { get { return _count; } }
    }
}