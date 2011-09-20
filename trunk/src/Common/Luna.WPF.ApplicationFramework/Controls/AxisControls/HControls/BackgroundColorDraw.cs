using System;
using Luna.WPF.ApplicationFramework.Extensions;
using Luna.WPF.ApplicationFramework.Graphics;
using System.Windows.Media;
using System.Windows;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public class BackgroundColorDraw : ElementDraw<BackgroundColor>
    {
        private Pen _pen;

        protected override void InternalDraw(DrawingContext dc)
        {
            if (_pen == null)
            {
                _pen = new Pen("#FFCCD9EA".ToBrush(1), 1);
                _pen.Freeze();
            }

            var axisConverter = Element.AxisXConverter;
            var start = axisConverter.ScreenToData(0).Date;
            var end = axisConverter.ScreenToData(Element.RenderSize.Width);

            var startPoint = 0d;
            var count = 1;
            while (start <= end)
            {
                var nextDay = start.AddDays(1);
                var nextPoint = Math.Min(axisConverter.DataToScreen(nextDay), Element.RenderSize.Width);

                var rect = new Rect(startPoint, 0, nextPoint, Element.RenderSize.Height);
                //xdc.DrawRectangle(GetBrush(start, count), null, rect);
                
                if (Element.ShowDateText)
                    dc.DrawText(start.ToString("yyyy/M/dd"), Brushes.Black, new Thickness(10,10,0,0), rect);

                dc.DrawLine(_pen, new Point(startPoint + 0.5, 0), new Point(startPoint +0.5, Element.RenderSize.Height));
                

                startPoint = nextPoint;
                start = nextDay;
                count++;
            }
            base.InternalDraw(dc);
        }

        private Brush GetBrush(DateTime current, int count)
        {
            if (Element.DateTimeConverter == null) return Brushes.Transparent;

            return Element.DateTimeConverter.Convert(current, typeof(Brush), count, null) as Brush;
        }
    }
}
