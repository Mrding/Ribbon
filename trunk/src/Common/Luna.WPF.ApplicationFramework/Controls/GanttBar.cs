namespace Luna.WPF.ApplicationFramework.Controls
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    public static class DrawingHelper
    {
        #region Methods

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

        public static void DrawGuidelineRoundedRect(this DrawingContext dc, Pen pen, Brush brush, Rect rect, double radiusX, double radiusY)
        {
            dc.PushGuidelineSet(CreateGuidelineSet(pen, rect));
            dc.DrawRoundedRectangle(brush, pen, rect, radiusX, radiusY);
            dc.Pop();
        }

        public static void DrawRowFrameRect(this DrawingContext dc, Pen pen, Brush brush, Rect rect)
        {
            dc.PushGuidelineSet(CreateGuidelineSet(pen, rect));
            dc.DrawRectangle(brush, pen, rect);
            dc.Pop();
        }

        public static Pen GetGuidePen(this FrameworkElement element, Brush brush)
        {
            var ps = PresentationSource.FromVisual(element);
            if (ps == null) throw new Exception("GetGuidePen fail");

            var m = ps.CompositionTarget.TransformToDevice;
            var dpiFactor = 1 / m.M11;
            return new Pen(brush, 1 * dpiFactor);
        }

        private static GuidelineSet CreateGuidelineSet(Pen pen, Rect rect)
        {
            var halfPenWidth = pen.Thickness / 2;
            var guidelines = new GuidelineSet();
            guidelines.GuidelinesX.Add(rect.Left + halfPenWidth);
            guidelines.GuidelinesX.Add(rect.Right + halfPenWidth);
            guidelines.GuidelinesY.Add(rect.Top + halfPenWidth);
            guidelines.GuidelinesY.Add(rect.Bottom + halfPenWidth);

            return guidelines;
        }

        #endregion Methods
    }

    public class GanttBar : Control
    {
        #region Fields

        public static readonly DependencyProperty ExceptionBrushProperty =
            DependencyProperty.Register("ExceptionBrush", typeof(Brush), typeof(GanttBar),
            new FrameworkPropertyMetadata(Brushes.Red, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty LineBrushProperty =
            DependencyProperty.Register("LineBrush", typeof(Brush), typeof(GanttBar),
            new FrameworkPropertyMetadata(Brushes.DarkGray, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty MaxBrushProperty =
            DependencyProperty.Register("MaxBrush", typeof(Brush), typeof(GanttBar),
            new FrameworkPropertyMetadata(Brushes.DarkGray, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(GanttBar),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty MinBrushProperty =
            DependencyProperty.Register("MinBrush", typeof(Brush), typeof(GanttBar),
            new FrameworkPropertyMetadata(Brushes.DarkGray, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(GanttBar),
           new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty ValueBrushProperty =
            DependencyProperty.Register("ValueBrush", typeof(Brush), typeof(GanttBar),
            new FrameworkPropertyMetadata(Brushes.Green, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(GanttBar),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        private const int TextWidth = 10;
        private const int GraduationHeight = 3;

        private Pen _maxPen;
        private Pen _minPen;

        #endregion Fields

        #region Properties

        public Brush ExceptionBrush
        {
            get { return (Brush)GetValue(ExceptionBrushProperty); }
            set { SetValue(ExceptionBrushProperty, value); }
        }

        public Brush LineBrush
        {
            get { return (Brush)GetValue(LineBrushProperty); }
            set { SetValue(LineBrushProperty, value); }
        }

        public Brush MaxBrush
        {
            get { return (Brush)GetValue(MaxBrushProperty); }
            set { SetValue(MaxBrushProperty, value); }
        }

        public Pen MaxPen
        {
            get
            {
                if (_maxPen == null)
                {
                    _maxPen = this.GetGuidePen(LineBrush);
                    _maxPen.Freeze();
                }
                return _maxPen;
            }
        }

        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public Brush MinBrush
        {
            get { return (Brush)GetValue(MinBrushProperty); }
            set { SetValue(MinBrushProperty, value); }
        }

        public Pen MinPen
        {
            get
            {
                if (_minPen == null)
                {
                    _minPen = this.GetGuidePen(LineBrush);
                    _minPen.Freeze();
                }
                return _minPen;
            }
        }

        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public Brush ValueBrush
        {
            get { return (Brush)GetValue(ValueBrushProperty); }
            set { SetValue(ValueBrushProperty, value); }
        }

        private bool IsException
        {
            get { return Value < MinValue || Value > MaxValue; }
        }

        #endregion Properties

        #region Methods

        protected override void OnRender(DrawingContext drawingContext)
        {
            var points = ComputePointXOfValue();
            //1: 画水平线
            if (!(Value == MinValue && Value == MaxValue))
                DrawHorizontalLine(drawingContext);
            //2: 画2条垂直线, Min and Max
            DrawMinMaxLine(drawingContext, points);
            //3: 画Value Dot
            DrawValueDot(drawingContext, points);
        }

        private PointXOfValue ComputePointXOfValue()
        {
            if (Value >= MinValue && Value <= MaxValue)
            {
                return new PointXOfValue
                           {
                               MinPointX = TextWidth,
                               MaxPointX = RenderSize.Width - TextWidth,
                               ValuePointX = GetMiddlePointX(MinValue, Value, MaxValue)
                           };
            }
            else if (Value < MinValue)
            {
                return new PointXOfValue
                           {
                               MinPointX = GetMiddlePointX(Value, MinValue, MaxValue),
                               MaxPointX = RenderSize.Width - TextWidth,
                               ValuePointX = TextWidth
                           };
            }
            else
            {
                return new PointXOfValue
                           {
                               MinPointX = TextWidth,
                               MaxPointX = GetMiddlePointX(MinValue, MaxValue, Value),
                               ValuePointX = RenderSize.Width - TextWidth
                           };
            }
        }

        private void DrawHorizontalLine(DrawingContext drawingContext)
        {
            var middleY = RenderSize.Height / 2;
            drawingContext.DrawGuideLineLine(MaxPen, new Point(TextWidth, middleY), new Point(RenderSize.Width - TextWidth, middleY));
        }

        private void DrawMinMaxLine(DrawingContext drawingContext, PointXOfValue points)
        {
            if (MinValue == MaxValue)
            {
                if (Value < MinValue)
                    DrawTextAndRoundedRectangle(drawingContext, MaxValue.ToString("0.##"), points.MaxPointX, MaxPen, false);
                else
                    DrawTextAndRoundedRectangle(drawingContext, MinValue.ToString("0.##"), points.MinPointX, MinPen, true);
            }
            else
            {
                DrawTextAndRoundedRectangle(drawingContext, MinValue.ToString("0.##"), points.MinPointX, MinPen, true);
                DrawTextAndRoundedRectangle(drawingContext, MaxValue.ToString("0.##"), points.MaxPointX, MaxPen, false);
            }
        }

        private void DrawTextAndDot(DrawingContext drawingContext, string text, double distance, Brush brush, bool up)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation(2, 0, new Duration(TimeSpan.FromMilliseconds(800)));
            doubleAnimation.AutoReverse = true;
            //doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;

            Pen ellipsePen = this.GetGuidePen(brush);
            ellipsePen.Freeze();
            //drawingContext.DrawEllipse(brush, ellipsePen, new Point(distance - 0.5, this.RenderSize.Height / 2 - 0.5), 3, 3);
            drawingContext.DrawEllipse(brush, ellipsePen, new Point(distance, this.RenderSize.Height / 2 - 0.5), null, 2,
                doubleAnimation.CreateClock(), 2, doubleAnimation.CreateClock());

            FormattedText formatted = new FormattedText(text,
                                            CultureInfo.CurrentCulture,
                                            FlowDirection.LeftToRight,
                                            new Typeface("Verdana"),
                                            9,
                                            brush);
            //formatted.SetFontWeight(FontWeights.Bold);
            formatted.TextAlignment = TextAlignment.Left;
            double position = up ? (this.RenderSize.Height / 2 - 3 - formatted.Height) : (this.RenderSize.Height / 2 + 2);
            var point = new Point(Math.Max(0, distance - formatted.Width / 2), position);
            var sub = RenderSize.Width - point.X - formatted.Width;
            if (sub < 0)
                point.Offset(sub, 0);
            drawingContext.DrawText(formatted, point);
        }

        private void DrawTextAndRoundedRectangle(DrawingContext drawingContext, string text, double distance, Pen pen, bool isLeft)
        {
            var rect = new Rect(new Point(distance, this.RenderSize.Height / 2 - GraduationHeight), new Size(0.3, GraduationHeight));
            drawingContext.DrawRectangle(pen.Brush, pen, rect);

            var formatted = new FormattedText(text,
                                              CultureInfo.CurrentCulture,
                                              FlowDirection.LeftToRight,
                                              new Typeface("Verdana"),
                                              9,
                                              pen.Brush) { TextAlignment = TextAlignment.Left };
            //formatted.SetFontWeight(FontWeights.Bold);
            double position = this.RenderSize.Height / 2;

            //if (isThreeLevel)
            //    position += formatted.Height;
            // - formatted.Width / 2
            drawingContext.DrawText(formatted,
                                    isLeft
                                        ? new Point(distance - formatted.Width, position)
                                        : new Point(distance, position));
        }

        private void DrawValueDot(DrawingContext drawingContext, PointXOfValue points)
        {
            DrawTextAndDot(drawingContext, Value.ToString("0.##"), points.ValuePointX, IsException ? ExceptionBrush : ValueBrush, true);
        }

        private double GetMiddlePointX(double min, double middle, double max)
        {
            var range = RenderSize.Width - 2 * TextWidth;
            return max != min ? (middle - min) / (max - min) * range + TextWidth : min + TextWidth;
        }

        #endregion Methods

        #region Nested Types

        private struct PointXOfValue
        {
            #region Properties

            internal double MaxPointX
            {
                get;
                set;
            }

            internal double MinPointX
            {
                get;
                set;
            }

            internal double ValuePointX
            {
                get;
                set;
            }

            #endregion Properties
        }

        #endregion Nested Types
    }
}