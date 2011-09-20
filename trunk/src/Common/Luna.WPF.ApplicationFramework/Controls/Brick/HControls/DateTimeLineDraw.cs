using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Luna.WPF.ApplicationFramework.Graphics;
using System.Windows;

namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    public class DateTimeLineDraw : CalibrationControlDraw<DateTime>
    {
        protected override void InternalDraw(DrawingContext dc)
        {
            DrawSelectedBlockLine(dc);
            base.InternalDraw(dc);
        }

        private void DrawSelectedBlockLine(DrawingContext dc)
        {
            if(Element.BlockLine == null || !Element.ShowSelectedLine) 
                return;

            var selectedLineHeight = Element.GetSelectedLineHeight();
            var timeDrawConverter = Element.TimeDrawConverter;
            var axisConverter = Element.AxisXConverter;
            var xStart = axisConverter.DataToScreen(Element.TimeDrawConverter.GetStart(Element.BlockLine));
            var xEnd = axisConverter.DataToScreen(timeDrawConverter.GetEnd(Element.BlockLine));
            
            var rect = new Rect(xStart, GetY(), xEnd - xStart, selectedLineHeight);

            var brush = timeDrawConverter.GetBackground(Element.BlockLine);
            brush.Freeze();
            if (Element.IsNewBlockLine)
            {
                var myAnimation = new RectAnimation
                    (new Rect(rect.X, rect.Y, 1, rect.Height), rect, new Duration(TimeSpan.FromMilliseconds(300)));
                myAnimation.Freeze();
              
                dc.DrawRectangle(brush, null, rect, myAnimation.CreateClock());
                Element.IsNewBlockLine = false;
            }
            else
                dc.DrawRectangle(brush, null, rect);
        }
    }
}
