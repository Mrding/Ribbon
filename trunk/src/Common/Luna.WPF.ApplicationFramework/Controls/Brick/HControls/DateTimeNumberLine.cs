using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    public class DateTimeNumberLine : AxisControl
    {
        private readonly List<ContentPresenter> _contentPresenterList = new List<ContentPresenter>(36); //以1440解像度的螢幕求得 1440 / hourWidth(40) 約為 36

        public override void Initialize()
        {
            base.Initialize();
            //RefreshChildElement();
        }

        protected override void AddToPanel(IAxisPanel axisPanel)
        {
            axisPanel.AddHorizontalControl(this);
        }

        public double HourWidth
        {
            get { return (double)GetValue(HourWidthProperty); }
            set { SetValue(HourWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HourWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HourWidthProperty =
            Controls.ScheduleGrid.HourWidthProperty.AddOwner(typeof(DateTimeNumberLine), new FrameworkPropertyMetadata(Controls.ScheduleGrid.HourWidthProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.AffectsArrange));

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(DateTimeNumberLine), new UIPropertyMetadata());

        private void VisibleChildLooping(Size availableSize, Action<ContentPresenter> action)
        {
            var onScreenCount = (int)(availableSize.Width / HourWidth + 0.9);

            for (var i = _contentPresenterList.Count; i < onScreenCount; i++) // 不足
            {
                var contentPresenter = new ContentPresenter { ContentTemplate = ItemTemplate, Width = HourWidth };
                _contentPresenterList.Add(contentPresenter);
                AddLogicalChild(contentPresenter);
                AddVisualChild(contentPresenter);
                action(contentPresenter);
            }

            //foreach (var child in _contentPresenterList)
            //    action(child);
        }

        private void Fillin(Size availableSize, out bool elementCountChanged)
        {

            VisibleChildLooping(availableSize, (child) =>
            {
                child.Measure(availableSize);
            });

            var onScreenCount = (int)(availableSize.Width / HourWidth + 0.9);

            elementCountChanged = _contentPresenterList.Count < onScreenCount;

            var excess = _contentPresenterList.Count - onScreenCount; // 過剩

            for (var i = 0; i < excess; i++)
            {
                var lastIndex = _contentPresenterList.Count - 1;
                if (lastIndex < 0) break;
                var contentPresenter = _contentPresenterList[lastIndex];
                _contentPresenterList.RemoveAt(lastIndex);
                RemoveLogicalChild(contentPresenter);
                RemoveVisualChild(contentPresenter);
            }

            elementCountChanged = excess > 0 || elementCountChanged;
        }

        private void RefreshChildElement()
        {
            if (AxisPanel == null) return;

            //得到相对于大容器的位置
            var viewPortWidth = AxisPanel.ViewportRangeX.ViewMin;//AxisXConverter.ScreenToViewport(0);
            //用段的模求出偏移量
            var xTranslateTransform = viewPortWidth % HourWidth;
            var firstSectionX = viewPortWidth - xTranslateTransform;

            for (var i = 0; i < VisualChildrenCount; i++)
            {
                var x = i * HourWidth - xTranslateTransform;

                var date = AxisXConverter.ToData(firstSectionX + (i * HourWidth));

                // Debug.Print(date.ToString());


                _contentPresenterList[i].Content = date;
                _contentPresenterList[i].Arrange(new Rect(new Point(x, 0), _contentPresenterList[i].DesiredSize));
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Dispatcher.BeginInvoke(() =>
                                       {
                                           RefreshChildElement();
                                       }, DispatcherPriority.Render);

        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            bool elementCountChanged;
            Fillin(finalSize, out elementCountChanged);
            //RefreshChildElement();

            return base.ArrangeOverride(finalSize);
        }

        protected override int VisualChildrenCount
        {
            get { return _contentPresenterList.Count; }
        }

        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            return new RectangleGeometry(new Rect(RenderSize));
        }

        protected override Visual GetVisualChild(int index)
        {
            return _contentPresenterList[index];
        }

        protected override void OnDispose()
        {
            if (_contentPresenterList != null)
                _contentPresenterList.Clear();

            base.OnDispose();
        }
    }
}
