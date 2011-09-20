using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Luna.WPF.ApplicationFramework.Controls.Brick;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public static class DrawingContextExtension
    {
        #region linescontrol
        //for linescontrol

        public static void DrawColumnLines(this FrameworkElement element, DrawingContext drawingContext, 
            double RowWidth, double xOffset,double offset,Pen pen)
        {
            double halfPenHeight = pen.Thickness / 2;
            Point start = new Point(0, 0);
            Point end = new Point(0, element.RenderSize.Height);
            
            GuidelineSet guidelines = new GuidelineSet();
            //draw column
            var count = Convert.ToInt32(element.RenderSize.Width / RowWidth);
            for (int i = 1; i <= count; i++)
            {
                start.X = xOffset - offset;
                end.X = xOffset - offset;
                guidelines.GuidelinesX = new DoubleCollection(new[] { xOffset - halfPenHeight, xOffset + halfPenHeight });
                drawingContext.PushGuidelineSet(guidelines);
                drawingContext.DrawLine(pen, start, end);
                drawingContext.Pop();
                xOffset += RowWidth;
            }
        }

       

        #endregion

        public static Pen GetDashPen(this FrameworkElement element, Brush brush)
        {
            return element.GetDashPen(brush, 1);
        }

        public static Pen GetDashPen(this FrameworkElement element, Brush brush, double thickness)
        {
            var pen = new Pen(brush, thickness) { DashStyle = new DashStyle() };
            pen.DashStyle.Dashes.Add(1);
            pen.DashStyle.Dashes.Add(3);
            pen.Freeze();
            return pen;
        }

        public static Pen GetDashPen(this FrameworkElement element, Brush brush, double thickness, DashStyle dashStyle)
        {
            var pen = new Pen(brush, thickness) { DashStyle = dashStyle };
            pen.Freeze();
            return pen;
        }

        //for ScheduleGrid

        //public static void InvalidateLayerVisual(this DependencyObject element, Orientation oriengation, Type type)
        //{
        //    //var sg = (ScheduleGrid)element;
        //    //switch (oriengation)
        //    //{
        //    //    case Orientation.Vertical:
        //    //        var verticalControl = sg.VerticalControls.Find(c => c.GetType().Equals(type));
        //    //        if (verticalControl != null)
        //    //            ((UIElement)verticalControl).InvalidateVisual();
        //    //        break;
        //    //    case Orientation.Horizontal:
        //    //        var horizontalControl = sg.HorizontalControls.Find(c => c.GetType().Equals(type));
        //    //        if (horizontalControl != null)
        //    //            ((UIElement)horizontalControl).InvalidateVisual();
        //    //        break;
        //    //}
        //}
    }
}
