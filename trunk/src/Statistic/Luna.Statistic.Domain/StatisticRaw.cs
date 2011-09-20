using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Luna.Core.Extensions;
using Luna.Common;
using System.Linq;

namespace Luna.Statistic.Domain
{
    public class StatisticRaw : ISelectable
    {
        private readonly string _displayText;
        private IServiceQueueStatistic _queue;
        private Func<double[], int, string, object, IVisibleLinerData> _itemContstruction;
        private IList<IVisibleLinerData> _items;

        public StatisticRaw(IServiceQueueStatistic queue, Func<double[], int, string, object, IVisibleLinerData> itemContstruction)
        {
            _displayText = queue.ToString();
            _queue = queue;
            _items = new List<IVisibleLinerData>(10);
            _itemContstruction = itemContstruction;
            IsOpened = true;
        }

        public IServiceQueueStatistic Source { get { return _queue; } }

        /// <summary>
        /// ICollectionView
        /// </summary>
        public IEnumerable Items3 { get; set; }

        public IEnumerable<IVisibleLinerData> Items { get { return _items; } }

        public void SetForceastValues()
        {
            _items.Add(_itemContstruction(_queue.ForceastStaffing, 0, "ForecastStaffing", _queue));//0
            _items.Add(_itemContstruction(_queue.AssignedStaffing, 0, "AssignedStaffing", _queue));//1
            _items.Add(_itemContstruction(_queue.AssignedMaxStaffing, 0, "AssignedMaxStaffing", _queue)
                .Self<ISelectable>(o => o.IsSelected = false).As<IVisibleLinerData>()); //2
            _items.Add(_itemContstruction(_queue.ServiceLevelGoal, 1, "ServiceLevelGoal", _queue));//3
            _items.Add(_itemContstruction(_queue.AssignedServiceLevel, 1, "AssignedServiceLevel", _queue)); //4

            _items.Add(_itemContstruction(_queue.CV, 2, "ForecastCallVolume", _queue)); //5
            _items.Add(_itemContstruction(_queue.AHT, 3, "ForecastAverageHandlingTime", _queue)); //6

        }

        public void SetAssignedValues(int index)
        {
            _items[1].Values[index] = _queue.AssignedStaffing[index];
            _items[2].Values[index] = _queue.AssignedMaxStaffing[index];
            _items[4].Values[index] = _queue.AssignedServiceLevel[index];
            //Items[2].Values = Items[2].Values;
        }

        public void Output()
        {
            var staffingMaxValue = new[] { _items[0].Values.Max(), _items[1].Values.Max(), _items[2].Values.Max() }.Max() * 1.1;
            _items[0].MaxValue = staffingMaxValue;
            _items[1].MaxValue = staffingMaxValue;
            _items[2].MaxValue = staffingMaxValue;

            var cvMaxValue = new[] { _items[5].Values.Max() }.Max() * 1.1;
            _items[5].MaxValue = cvMaxValue;

            var ahtMaxValue = new[] { _items[6].Values.Max() }.Max() * 1.1;
            _items[6].MaxValue = ahtMaxValue;
        }

        public bool? IsSelected
        {
            get { return _queue.SaftyGetProperty<bool?, ISelectable>(o => o.IsSelected); }
            set { _queue.SaftyInvoke<ISelectable>(o => o.IsSelected = value ); }
        }

        public bool IsOpened { get; set; }

        public void Dispose()
        {
            _items.Clear();
            _items = null;
            _itemContstruction = null;
            _queue = null;
            Items3 = null;

        }

        public override string ToString()
        {
            return _displayText;
        }
    }
}