using System;
using System.Windows;
using System.Windows.Media;
using Luna.WPF.ApplicationFramework.Graphics;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public class RowLinesControlDraw : ElementDraw<RowLineControl>
    {
        /// <summary>
        /// GuideLineSet参考
        /// http://www.wpftutorial.net/DrawOnPhysicalDevicePixels.html
        /// http://www.cnblogs.com/AaronLu/archive/2009/11/13/1602332.html
        /// </summary>
        /// <param name="drawingContext"></param>
        protected override void InternalDraw(DrawingContext drawingContext)
        {
            DrawRowLines(drawingContext, Element.AxisPanel.GetScreenTopRowIndex(), Element.RowCount, false, Element.LinePen);
        }

        /// <summary>
        /// DrawRowLines
        /// </summary>
        /// <param name="drawingContext"></param>
        /// <param name="startIndex"></param>
        /// <param name="rowCount"></param>
        /// <param name="plusOffset">True 为+pen.Thickness/2 False为-pen.Thickness/2</param>
        /// <param name="pen"></param>
        private void DrawRowLines(DrawingContext drawingContext, int startIndex, int rowCount, bool plusOffset, Pen pen)
        {
            var halfPenWidth = pen.Thickness / 2;

            var displayCount = Convert.ToInt32(Math.Floor(Element.RenderSize.Height / Element.RowHeight));

            var point0 = new Point(0, 0);
            var point1 = new Point(Element.RenderSize.Width, 0);
          
            var guidelines = new GuidelineSet();
            
            //根据HeadListBox来对齐，减去halfPenHeight
            
            var y = plusOffset ? halfPenWidth : -(halfPenWidth);

            for (var i = 0; 0 <= startIndex && i < displayCount && startIndex < rowCount; startIndex++, i++)
            {
                y += Element.RowHeight;
                point0.Y = y;
                point1.Y = y;

                guidelines.GuidelinesY = new DoubleCollection(new[] { y - halfPenWidth, y + halfPenWidth });
                drawingContext.PushGuidelineSet(guidelines);
                drawingContext.DrawLine(pen, point0, point1);
                drawingContext.Pop();
            }
        }
    }
}
