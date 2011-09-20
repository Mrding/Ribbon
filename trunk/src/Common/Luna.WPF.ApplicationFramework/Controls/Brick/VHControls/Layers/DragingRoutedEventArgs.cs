using System;
using System.Windows;
using Luna.Common;

namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    public class DragingRoutedEventArgs : RoutedEventArgs
    {
        private readonly object _drageItemData;
        private readonly DateTime _pointTime;

        public DragingRoutedEventArgs(DateTime pointTime, object dragData, RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
            _pointTime = pointTime;
            _drageItemData = dragData;
        }

        
        public object DrageItemData
        {
            get { return _drageItemData; }
        }

        public DateTime PointTime { get { return _pointTime; } }

        public ITerm Target { get; set; }
    }
}