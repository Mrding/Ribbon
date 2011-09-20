using System;
using System.Windows;
using System.Windows.Media;
using Luna.Core.Extensions;
using Luna.WPF.ApplicationFramework.Extensions;

namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    public class MarkBlock : AxisControl
    {
        protected override void AddToPanel(IAxisPanel axisPanel)
        {
            axisPanel.AddHorizontalControl(this);
        }

        public DateTime StartTime
        {
            get { return (DateTime)GetValue(StartTimeProperty); }
            set { SetValue(StartTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartTimeProperty =
            DependencyProperty.Register("StartTime", typeof(DateTime), typeof(MarkBlock), new UIPropertyMetadata(default(DateTime)));

        public DateTime EndTime
        {
            get { return (DateTime)GetValue(EndTimeProperty); }
            set { SetValue(EndTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndTimeProperty =
            DependencyProperty.Register("EndTime", typeof(DateTime), typeof(DateTimeNumberLine), new UIPropertyMetadata(default(DateTime)));

        public Brush ColorRender
        {
            get { return (Brush)GetValue(ColorRenderProperty); }
            set { SetValue(ColorRenderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColorRender.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorRenderProperty =
            DependencyProperty.Register("ColorRender", typeof(Brush), typeof(DateTimeNumberLine), new PropertyMetadata(Brushes.Transparent));



        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Foreground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(MarkBlock), new UIPropertyMetadata(Brushes.Black));



        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(DateTimeNumberLine), new UIPropertyMetadata(null));



        protected override void OnRender(DrawingContext drawingContext)
        {
            if (StartTime == (DateTime)StartTimeProperty.DefaultMetadata.DefaultValue || EndTime == (DateTime)EndTimeProperty.DefaultMetadata.DefaultValue || AxisPanel == null)
                return;

            var left = AxisXConverter.DataToScreen(StartTime);
            var right = Math.Min(AxisXConverter.DataToScreen(EndTime), RenderSize.Width);
            var width = right - left;

            var rect = new Rect(Math.Max(0, left), RenderSize.Height - 14, Math.Min(right >= 0 ? right : 0, width > 0 ? width : 0), 14);//14 is Line Height
            drawingContext.DrawRectangle(ColorRender, null, rect);
            if (Text == null)
                return;
            Text.ToFormattedText(Foreground).Self(fTxt =>
                                                         {
                                                             if (AxisXConverter.DataToScreen(EndTime) - Text.GetTtextSize().Width < 0 || RenderSize.Width < left)
                                                                 return;
                                                             var topMargin = 0;
                                                             drawingContext.DrawText(fTxt, new Point(left>=0?left:0, topMargin));
                                                         });
          
        }
    }
}