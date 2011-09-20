using System;
using System.Linq;
using System.Collections.Generic;
using Luna.Common;
using Luna.Core.Extensions;
using Luna.Infrastructure.Domain;

namespace Luna.Statistic.Domain
{
    public class StaffingStatistic : IStaffingStatistic
    {
        private readonly double[] _forecastValues;

        private double[] _estimateValues;//暫排

        private double[] _difference;
        private double[] _quarterDifference;
        //private readonly double[] _adjustedDiffValues;

        private double _forecastMaxValue;

        private readonly int _quarterCellCapacity;
        private readonly object _entity;

        private readonly IList<IVisibleLinerData> _items;
        private readonly Func<double[], int, string, object, IVisibleLinerData> _itemContstruction;

        public StaffingStatistic(object entity, double[] forecastValue,
            Func<double[], int, string, object, IVisibleLinerData> itemContstruction)
        {
            _entity = entity;
            _items = new List<IVisibleLinerData>(1);
            _forecastValues = forecastValue;
            _itemContstruction = itemContstruction;
            _forecastMaxValue = _forecastValues.Max() / 1; // 放大縮小比例 越高越大

            _quarterCellCapacity = _forecastValues.Length;
            _quarterDifference = new double[_quarterCellCapacity];
            _items.Add(_itemContstruction(new double[_quarterCellCapacity], 0, "StaffingDifference", this).Self(l =>
            {
                l.ZeroBaseValue = _forecastMaxValue;
                l.MaxValue = _forecastMaxValue * 2; // 正負(overstaff, understaff)
            }));
            Output().Invoke();
        }

        public void Reset()
        {
            (_quarterCellCapacity * 3).Self(c =>
            {
                _estimateValues = new double[c];
                _difference = new double[c];
            });
            _quarterDifference = new double[_quarterCellCapacity];
        }

        public IVisibleLinerData Difference { get { return _items[0]; } }

        public object Entity { get { return _entity; } }


        public Action Output()
        {
            var values = new double[_quarterDifference.Length];

            //var m = _quarterDifference.Max();
            //if(0 < m)
            //{
            //    _forecastMaxValue = m/2.5;
            //    Difference.ZeroBaseValue = _forecastMaxValue;
            //    Difference.MaxValue = _forecastMaxValue*2;
            //}

            for (var i = 0; i < values.Length; i++)
            {
                values[i] = _quarterDifference[i] + _forecastMaxValue;
            }
            

            return () => { _items[0].Values = values; };
        }


        public double GetDeviation(int startIndex, int endIndex, int dateIndex)
        {
            //dateIndex 注意前期抓取

            var deviation = 0d;

            var length = dateIndex + 288; // 預設為一天
            if (endIndex > length) // 半夜需要延長
                length = endIndex;

            for (var i = dateIndex; i < length; i++)
            {
                var qIndex = i / 3; // quarterIndex
                var r = _forecastValues[qIndex] - _estimateValues[i];

                if (i % 3 == 0)
                    _quarterDifference[qIndex] = r;

                _difference[i] = r;
                if (i >= startIndex && i < endIndex)
                    deviation += Math.Abs(r);
            }

            return deviation;
        }

        public double M1(int index, double value)
        {
            if (value == 0) return 0;
            return (_forecastValues[index / 3] - _estimateValues[index]) / value;
        }

        public double GetShortfall(int index, int length)
        {
            var shortfall = 0d;
            for (var i = 0; i < length; i++)
            {
                var position = i + index;
                shortfall += _forecastValues[position / 3] - _estimateValues[position];
            }
            return shortfall;
        }

        public void Push(int index, int length, double value)
        {
            for (var i = 0; i < length; i++)
            {
                _estimateValues[i + index] += value;
            }
        }

        public override string ToString()
        {
            return "MultiQueues";
        }
    }
}