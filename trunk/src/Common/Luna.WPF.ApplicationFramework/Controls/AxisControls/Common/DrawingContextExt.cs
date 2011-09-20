using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.ComponentModel;

namespace Luna.WPF.ApplicationFramework.Extensions
{
    public static class DrawingContextExt
    {
        public static Pen GetGuidePen(this FrameworkElement element, Brush brush)
        {
            var ps = PresentationSource.FromVisual(element);
            if (ps == null) throw new Exception("GetGuidePen fail");

            var m = ps.CompositionTarget.TransformToDevice;
            var dpiFactor = 1 / m.M11;
            return new Pen(brush, 1 * dpiFactor);
        }

        public static void DrawGuidelineRoundedRect(this DrawingContext dc, Brush brush, Pen pen, Rect rect, double radiusX, double radiusY)
        {
            dc.PushGuidelineSet(CreateGuidelineSet(pen, rect));
            dc.DrawRoundedRectangle(brush, pen, rect, radiusX, radiusY);
            dc.Pop();
        }

        public static void DrawGuidelineRect(this DrawingContext dc, Brush brush, Pen pen, Rect rect)
        {
            dc.PushGuidelineSet(CreateGuidelineSet(pen, rect));
            dc.DrawRectangle(brush, pen, rect);
            dc.Pop();
        }
        
        

        public static void DrawRowFrameRect(this DrawingContext dc, Brush brush, Pen pen, Rect rect)
        {
            dc.PushGuidelineSet(CreateGuidelineSet(pen, rect));
            dc.DrawRectangle(brush, pen, rect);
            dc.Pop();
        }

        public static void DrawGuideLineLine(this DrawingContext dc, Pen pen, Point point0, Point point1)
        {
            var halfPenWidth = pen.Thickness / 2;
            var guidelines = new GuidelineSet();
            guidelines.GuidelinesX.Add(point0.X + halfPenWidth);
            guidelines.GuidelinesX.Add(point1.X + halfPenWidth);
            guidelines.GuidelinesY.Add(point0.Y + halfPenWidth);
            guidelines.GuidelinesY.Add(point1.Y + halfPenWidth);

            dc.PushGuidelineSet(guidelines);
            dc.DrawLine(pen, point0, point1);
            dc.Pop();
        }

        private static GuidelineSet CreateGuidelineSet(Pen pen, Rect rect)
        {
            var halfPenWidth = pen.Thickness / 2;
            var guidelines = new GuidelineSet(new[] {rect.Left - halfPenWidth, rect.Right - halfPenWidth},
                                              new [] {rect.Top - halfPenWidth, rect.Bottom - halfPenWidth});
            return guidelines;
        }
    }
}
