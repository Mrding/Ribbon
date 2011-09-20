using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Luna.GUI.UserControls
{
    public partial class DateRangeSelector
    {

        public static readonly DependencyProperty DateFromProperty = DependencyProperty
            .Register("DateFrom", typeof(DateTime), typeof(DateRangeSelector),
                new FrameworkPropertyMetadata(DateTime.Today,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, 
                    new PropertyChangedCallback(OnDateFromChanged)));

        public static readonly DependencyProperty DateToProperty = DependencyProperty
            .Register("DateTo", typeof(DateTime), typeof(DateRangeSelector), 
                            new FrameworkPropertyMetadata(DateTime.Today.AddMonths(1),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, 
                    new PropertyChangedCallback(OnDateToChanged)));

        public static readonly DependencyProperty DateTypeProperty = DependencyProperty.
            Register("Interval", typeof(string), typeof(DateRangeSelector), new PropertyMetadata("Month", OnIntervalChanged));


        public static readonly RoutedEvent RangeChangedEvent = EventManager.RegisterRoutedEvent(
                                    "RangeChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler),
                                    typeof(DateRangeSelector));

        public event RoutedEventHandler RangeChanged
        {
            add
            {
                
                AddHandler(RangeChangedEvent, value);
            }
            remove { RemoveHandler(RangeChangedEvent, value); }
        }


        public DateRangeSelector()
        {
            InitializeComponent();
        }

        public DateTime DateFrom
        {
            get { return (DateTime)GetValue(DateFromProperty); }
            set { SetValue(DateFromProperty, value); }
        }

        public DateTime DateTo
        {
            get { return (DateTime)GetValue(DateToProperty); }
            set { SetValue(DateToProperty, value); }
        }

        public string Interval
        {
            get { return (string)GetValue(DateTypeProperty); }
            set { SetValue(DateTypeProperty, value); }
        }

        public bool Handled { get; set; }

        private int _dateValueChangeTimes;
        internal int DateValueChangeTimes
        {
            get { return _dateValueChangeTimes; }
            set
            {
                _dateValueChangeTimes = value;
                if (_dateValueChangeTimes > 1)
                    RaiseRangeChangedEvent();
            }
        }


        private static void OnDateFromChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selector = (DateRangeSelector)d;
            var newType = (DateTime)e.NewValue;
            if (selector.Handled)
            {
                selector.Handled = false;
                selector.DateValueChangeTimes += 1;
                return;
            }
            selector.Handled = true;
            switch (selector.Interval)
            {
                case "Day":
                    selector.DateTo = newType.AddDays(1);
                    break;
                case "Week":
                    selector.DateTo = newType.AddDays(7);
                    break;
                case "Month":
                    selector.DateTo = newType.AddMonths(1);
                    break;
                case "Year":
                    selector.DateTo = newType.AddYears(1);
                    break;
            }
            selector.DateValueChangeTimes += 1;
        }

        private static void OnDateToChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selector = (DateRangeSelector)d;
            var newType = (DateTime)e.NewValue;
            if (selector.Handled)
            {
                selector.Handled = false;
                selector.DateValueChangeTimes += 1;
                return;
            }
            selector.Handled = true;
            switch (selector.Interval)
            {
                case "Day":
                    selector.DateFrom = newType.AddDays(-1);
                    break;
                case "Week":
                    selector.DateFrom = newType.AddDays(-7);
                    break;
                case "Month":
                    selector.DateFrom = newType.AddMonths(-1);
                    break;
                case "Year":
                    selector.DateFrom = newType.AddYears(-1);
                    break;
            }
            selector.DateValueChangeTimes += 1;
        }

        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selector = (DateRangeSelector)d;
            var dateType = e.NewValue.ToString();
            switch (dateType)
            {
                case "Day":
                    selector.DateTo = selector.DateFrom.AddDays(1);
                    break;
                case "Week":
                    selector.DateTo = selector.DateFrom.AddDays(7);
                    break;
                case "Month":
                    selector.DateTo = selector.DateFrom.AddMonths(1);
                    break;
                case "Year":
                    selector.DateTo = selector.DateFrom.AddYears(1);
                    break;
            }
            selector.RaiseRangeChangedEvent();
            selector.Handled = false;
        }

        private void Backward(object sender, RoutedEventArgs e)
        {
            switch (Interval)
            {
                case "Day":
                    DateFrom = DateFrom.AddDays(-1);
                    break;
                case "Week":
                    DateFrom = DateFrom.AddDays(-7);
                    break;
                case "Month":
                    DateFrom = DateFrom.AddMonths(-1);
                    break;
                case "Year":
                    DateFrom = DateFrom.AddYears(-1);
                    break;
            }
        }

        private void Forward(object sender, RoutedEventArgs e)
        {
            switch (Interval)
            {
                case "Day":
                    DateFrom = DateFrom.AddDays(1);
                    break;
                case "Week":
                    DateFrom = DateFrom.AddDays(7);
                    break;
                case "Month":
                    DateFrom = DateFrom.AddMonths(1);
                    break;
                case "Year":
                    DateFrom = DateFrom.AddYears(1);
                    break;
            }
        }

        internal void RaiseRangeChangedEvent()
        {
            DateValueChangeTimes = 0;
            var newEventArgs = new RoutedEventArgs(RangeChangedEvent);
            RaiseEvent(newEventArgs);
        }
    }

}