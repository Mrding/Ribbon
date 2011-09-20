using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Luna.WPF.ApplicationFramework.Controls
{

    public class BackgroundColor : AxisControl
    {
        public override void Initialize()
        {
            ElementDraws.Add(new BackgroundColorDraw());
            base.Initialize();
        }

        protected override void AddToPanel(IAxisPanel axisPanel)
        {
            axisPanel.AddHorizontalControl(this);
        }

        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            return new RectangleGeometry(new Rect(RenderSize));
        }

        public bool ShowDateText
        {
            get { return (bool)GetValue(ShowDateTextProperty); }
            set { SetValue(ShowDateTextProperty, value); }
        }

        public static readonly DependencyProperty ShowDateTextProperty =
            DependencyProperty.Register("ShowDateText", typeof(bool), typeof(BackgroundColor),
            new UIPropertyMetadata(false));

        public IValueConverter DateTimeConverter
        {
            get { return (IValueConverter)GetValue(DateTimeConverterProperty); }
            set { SetValue(DateTimeConverterProperty, value); }
        }

        public static readonly DependencyProperty DateTimeConverterProperty =
            DependencyProperty.Register("DateTimeConverter", typeof(IValueConverter), typeof(BackgroundColor),
            new UIPropertyMetadata());
    }
}