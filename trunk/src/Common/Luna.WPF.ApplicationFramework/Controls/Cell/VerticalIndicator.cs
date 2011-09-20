using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Luna.Common;
using Luna.Common.Constants;
using Luna.Common.Interfaces;
using Luna.Core.Extensions;
using System.Linq;
using Luna.WPF.ApplicationFramework.Extensions;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public class VerticalIndicator : AxisControl
    {
        static VerticalIndicator()
        {
            SnapsToDevicePixelsProperty.OverrideMetadata(typeof(VerticalIndicator), new FrameworkPropertyMetadata(true));
        }

        private Pen _pen;
        private DateTime _startTime;
        private DateTime _enquiryStartTime;
        private readonly BitmapEffect _effect;

        private int _valueIndex;
        private DateTime _pointDateTime;
        private IList<IVisibleLinerData> _lines;

        public VerticalIndicator()
        {
            _effect = new OuterGlowBitmapEffect { GlowColor = Colors.White, GlowSize = 20, Opacity = 1 };
            _effect.Freeze();
        }


        public override void Initialize()
        {
            base.Initialize();
            _startTime = AxisXConverter.ScreenToData(Math.Min(0, -AxisPanel.HorizontalOffset));//AxisXConverter.ViewportToData(0);
            _enquiryStartTime = _startTime.AddDays(Global.HeadDayAmount);

            base.OnLoaded();
        }

        public object FilterParams
        {
            get { return (object)GetValue(FilterParamsProperty); }
            set { SetValue(FilterParamsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilterParams.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterParamsProperty =
            DependencyProperty.Register("FilterParams", typeof(object), typeof(VerticalIndicator), new FrameworkPropertyMetadata(null, (d, e) =>
            {
                d.SaftyInvoke<VerticalIndicator>(o => o.Update());
            }));

        private double _max;

        internal void Update()
        {
            if (ItemsSource == null)
            {
                if (_lines != null)
                    _lines.Clear();
                _lines = null;

                return;
            }


            var category = FilterParams == null ? 0 : System.Convert.ToInt32(FilterParams);
            _lines = ItemsSource.SaftyGetProperty<IList<IVisibleLinerData>, IEnumerable>(l => l.OfType<IVisibleLinerData>().Where(o => Equals(o.Category, category)).ToList()
                , () => new List<IVisibleLinerData> { ItemsSource.As<IVisibleLinerData>() });

            _max = _lines.Max(l => l.MaxValue);

        }


        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(VerticalIndicator), new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) =>
            {
                if (e.NewValue != null)
                    d.SaftyInvoke<VerticalIndicator>(o => o.Update());
            }));

        public double CorrectionValue
        {
            get { return (double)GetValue(CorrectionValueProperty); }
            set { SetValue(CorrectionValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CorrectionValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CorrectionValueProperty =
            DependencyProperty.Register("CorrectionValue", typeof(double), typeof(VerticalIndicator), new UIPropertyMetadata(0.0));

        protected override void AddToPanel(IAxisPanel axisPanel)
        {
            axisPanel.AddHorizontalControl(this);
        }

        private int GetCurrentIndex(Point point)
        {
            if (AxisPanel == null) return -1;

            var datetime = AxisXConverter.ToData(point.X + AxisPanel.ViewportRangeX.ViewMin);

            datetime = datetime.AddSeconds(-datetime.Second);
            _pointDateTime = datetime;

            var remains = datetime.Minute % 15;
            if (remains < 3)
            {
                datetime = datetime.AddMinutes(-remains);
            }
            else if (12 < remains)
            {
                datetime = datetime.AddMinutes(15 - remains);
            }
            else if (remains == 0)
            {
            }
            else
                datetime = datetime.AddMinutes(-remains);

            return (int)(datetime.Subtract(_enquiryStartTime).TotalHours * 4); // 4 = perHourCount 一小時4格, 15分钟数据单位
        }

        private double CalibrateValue(double value)
        {
            if (CorrectionValue == 0.0) return value;

            return -(value - CorrectionValue);
        }

        private double CalibrateAxisY(double value, double height)
        {
            if (CorrectionValue == 0.0)
                return ((value / _max) * height);

            var halfHeight = height / 2;

            if (value < 0)
                return (((CorrectionValue + value) / CorrectionValue) * halfHeight);

            if (value == 0.0)
                return (height / 2);

            return (((Math.Abs(value) / CorrectionValue) * halfHeight) + halfHeight);
        }


        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (_lines != null && 0 < _lines.Count)
            {
                _currentPoint = null;
                InvalidateVisual();
            }
            base.OnMouseLeave(e);
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_lines != null && 0 < _lines.Count)
            {
                _currentPoint = Mouse.GetPosition(this);
                InvalidateVisual();
            }
        }

        private Point? _currentPoint;

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, RenderSize.Width, RenderSize.Height));

            if (_currentPoint == null) return;

            if (_lines == null || 0 == _lines.Count) return;

            var count = _lines.Count;

            var mouse = _currentPoint.Value;

            _valueIndex = GetCurrentIndex(mouse);

            if (_valueIndex < 0) return;

            if (_pen == null)
            {
                _pen = this.GetDashPen(Brushes.DarkGray);
                _pen.Freeze();
            }

            var height = RenderSize.Height - 20; // 20 上方日期占位

            var values = new Dictionary<double, string>();

            for (var i = 0; i < count; i++)
            {
                if (_lines[i].SaftyGetProperty<bool, ISelectable>(l => l.IsSelected != true))
                    continue;

                var value = CalibrateValue(_lines[i].Values[_valueIndex]);
                if (value == 0) continue;

                if (values.ContainsKey(value))
                    continue;

                var y = CalibrateAxisY(value, height);

                values[value] = string.Format(_lines[i].Format, value);

                values[value].ToFormattedText(Brushes.Black).Self(fTxt =>
                {
                    fTxt.SetFontSize(10);
                    dc.PushEffect(_effect, null);
                    dc.DrawText(fTxt, new Point(mouse.X + 3, height - y));
                    dc.Pop();
                });
            }

            if (values.Count == 0) return;

            //mouse.X + .5 校正
            dc.DrawLine(_pen, new Point(mouse.X + .5, 0), new Point(mouse.X + .5, this.RenderSize.Height));

            _pointDateTime.ToString("hh:mm").ToFormattedText(Brushes.Black).Self(fTxt =>
            {
                dc.DrawText(fTxt, new Point(mouse.X - 32, RenderSize.Height - 18));
            });
        }

        protected override void OnDispose()
        {
            ItemsSource = null;
        }
    }
}