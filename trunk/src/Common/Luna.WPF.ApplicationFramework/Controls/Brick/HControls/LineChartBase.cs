using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Luna.WPF.ApplicationFramework.Controls
{
    using Luna.WPF.ApplicationFramework.Behaviors;

    public abstract class LineChartBase<T> : AxisControl
    {
        static LineChartBase()
        {
            TimeLineChartBehavior.DataProperty.OverrideMetadata(typeof(LineChartBase<T>),
                new FrameworkPropertyMetadata(TimeLineChartBehavior.DataProperty.DefaultMetadata.DefaultValue,
            FrameworkPropertyMetadataOptions.AffectsRender));
        }

        protected abstract void WhenMouseOverValuePoint(int valueIndex);
        protected abstract double GetCellInterval();
        protected abstract double GetXTransform(double cellWidth);
        protected abstract int GetStartIndex(double xTransform);
        protected abstract int GetCurrentIndex(Point point);
        protected abstract double CalibrateValue(double value); // 当有负数的值, 需要用此方法校正

        private double _actualRadius;
        private int _startIndex;

        public override void Initialize()
        {
            base.Initialize();

            //实际圆的半径(线宽/2+半径),注意控件的Pen的Thickness的线的中点落在给定圆的半径上，而不是在半径外有Thickness
            _actualRadius = Radius == 0 ? 0 : Radius + LineThickness / 4;
            InvalidateVisual(); // ? 有可能是因为使用模版, 所以必须强制刷新
        }

        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            return new RectangleGeometry(new Rect(RenderSize));
        }

        protected override void AddToPanel(IAxisPanel axisPanel)
        {
            axisPanel.AddHorizontalControl(this);
        }

        private double GetY(double value)
        {
            if (double.IsNaN(value)) return 0;

            if (MaxValue == 0.0)
                return RenderSize.Height/2;

            //计算高度时还需要把圆的半径和线的宽度考虑在内
            return value / MaxValue * (RenderSize.Height - 2 * _actualRadius) + _actualRadius;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            //base.OnRender(drawingContext);
            if (AxisPanel == null)
                return;

            //计算每个点之间的间距是多少
            var cellWidth = GetCellInterval();
            var xTransform = GetXTransform(cellWidth);
            _startIndex = GetStartIndex(xTransform);
            //没有点，或是只有一个都不能画线，在平面中过一点可以画无数条线
            if (Datas == null || Datas.Count <= 1 || _startIndex >= Datas.Count)
                return;
            //实际圆的半径(线宽+半径)
            //var realRadius = GetRealRadius();

            var displayCount = (int)Math.Min(ActualWidth / cellWidth + 2, Datas.Count);
            //每次动态产生对效能有影响，可以放到Arrange或是赋值之后计算一遍，此为偷懒写法
            //或可以说时间换空间?
            var pointList = new List<Point>(displayCount);
            for (int i = 0, j = _startIndex; j < Datas.Count && i < displayCount; i++, j++)
            {
                var y = GetY(Datas[j]);
                //if (realRadius == y)
                //    y = 0;
                //注意横坐标有加realRadius
                //var x = i * cellWidth + realRadius - Radius;
                var x = i * cellWidth;
                var point = new Point(x, y); // orginal : var point = new Point(i * cellWidth + realRadius, y);
                pointList.Add(point);
            }

            drawingContext.PushTransform(new TranslateTransform(xTransform, 0));

            //因为没有动画效果，并为了追求更好的效能，所以使用StreamGeometry
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                context.BeginFigure(pointList[0], true, false);
                context.PolyLineTo(pointList, true, true);
                context.Close();
            }
            geometry.Freeze();

            //这些画笔也可以理解为时间换空间
            var dashStyle = new DashStyle(LineDashArray, LineDashOffset);
            dashStyle.Freeze();

            #region shadow drawing
            //Pen shadowPen = new Pen(ShadowColor, LineThickness);
            //shadowPen.StartLineCap = PenLineCap.Round;
            //shadowPen.EndLineCap = PenLineCap.Round;
            //shadowPen.DashStyle = dashStyle;
            //shadowPen.DashCap = LineDashCap;
            //shadowPen.Freeze();

            //if (IsShowShadow)
            //{
            //    //draw offset line shadow
            //    drawingContext.PushTransform(new TranslateTransform(LineThickness * 0.312, -LineThickness * 0.681));
            //    drawingContext.DrawGeometry(null, shadowPen, geometry);
            //    drawingContext.Pop();
            //}
            #endregion

            //draw line
            var linePen = new Pen(LineColor, LineThickness) { DashCap = LineDashCap, DashStyle = dashStyle };
            linePen.Freeze();
            drawingContext.DrawGeometry(null, linePen, geometry);

            if (0 < Radius)
            {
                var ellipseLinePen = new Pen(LineColor, LineThickness / 2);
                ellipseLinePen.Freeze();
                //draw ellipse
                foreach (var point in pointList)
                    drawingContext.DrawEllipse(PointColor, ellipseLinePen, point, Radius, Radius);
            }
            drawingContext.Pop();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            var point = Mouse.GetPosition(this);

            //以下是判断鼠标的点是否移动到了点上，如果移动到了就让ToolTip展开，否则设置 e.Handled = true
            //不让ToolTip打开
            //计算每个点之间的间距是多少
            //var cellWidth = GetCellInterval();

             
         
              var value = 0d;

                var currentIndex = GetCurrentIndex(point);
                if (currentIndex == -1)
                {
                    IsMouseOverValuePoint = Visibility.Collapsed;
                    CurrentData = 0;
                }
                else
                {
                    if (currentIndex <= Datas.Count - 1)
                        value = Datas[currentIndex];

                    CurrentData = CalibrateValue(value);
                    WhenMouseOverValuePoint(currentIndex);
                    IsMouseOverValuePoint = Visibility.Visible;
                }




            //var leftLength = AxisPanel.HorizontalOffset;//AxisXConverter.ScreenToViewport(0);
                //var mod = (point.X + leftLength) % cellWidth;
                //var realRadius = _actualRadius;
              
                //orginal: if (mod <= 2 * realRadius) //判断错误
                //Old: if(mod<=realRadius|| (cellWidth-mod)<=realRadius) //判别是否在圆心周围，以半径作为判断条件
                //var resultRight = mod <= realRadius;
                //var resultLeft = (cellWidth - mod) <= realRadius;

                //if (resultLeft)
                //    currentIndex++;
                //else if (!resultRight)
                //{
                //    IsMouseOverValuePoint = Visibility.Collapsed;
                //    // Debug.Print(string.Format("X: {0} Mod: {1},false", point.X, mod));
                //    return;
                //}
                //计算Y轴是否也是符合点的位置，点包括他的半径(Radius)+线宽
                



               // var cellY = (value / MaxValue) * (this.RenderSize.Height); //GetY(value);// -_actualRadius;

               // System.Diagnostics.Debug.Print(string.Format("Y: {0} CellY: {1}", point.Y, cellY));

                //if ((cellY - realRadius) <= point.Y && point.Y <= (cellY + realRadius))
                //{

                   

                 
                   
                    //Debug.Print("x:{0},y:{1}", point.X, point.Y);
                //}
                //else
                //{

                //    // IsMouseOverValuePoint = Visibility.Collapsed;
                //}
            



        }

        //protected override Size MeasureOverride(Size availableSize)
        //{
        //    return base.MeasureOverride(availableSize);
        //}

        #region Control Property

        public double LineThickness
        {
            get { return (double)GetValue(LineThicknessProperty); }
            set { SetValue(LineThicknessProperty, value); }
        }

        public static readonly DependencyProperty LineThicknessProperty =
            DependencyProperty.Register("LineThickness", typeof(double),
            typeof(LineChartBase<T>),
            new FrameworkPropertyMetadata(1.6, FrameworkPropertyMetadataOptions.AffectsRender));

        public PenLineCap LineDashCap
        {
            get { return (PenLineCap)GetValue(LineDashCapProperty); }
            set { SetValue(LineDashCapProperty, value); }
        }

        public static readonly DependencyProperty LineDashCapProperty =
           DependencyProperty.Register("LineDashCap", typeof(PenLineCap),
           typeof(LineChartBase<T>),
           new FrameworkPropertyMetadata(PenLineCap.Round, FrameworkPropertyMetadataOptions.AffectsRender));


        public Brush PointColor
        {
            get { return (Brush)GetValue(PointColorProperty); }
            set { SetValue(PointColorProperty, value); }
        }

        public static readonly DependencyProperty PointColorProperty =
            DependencyProperty.Register("PointColor", typeof(Brush),
            typeof(LineChartBase<T>),
            new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush LineColor
        {
            get { return (Brush)GetValue(LineColorProperty); }
            set { SetValue(LineColorProperty, value); }
        }

        public static readonly DependencyProperty LineColorProperty =
            DependencyProperty.Register("LineColor", typeof(Brush),
            typeof(LineChartBase<T>),
            new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        public double LineDashOffset
        {
            get { return (double)GetValue(LineDashOffsetProperty); }
            set { SetValue(LineDashOffsetProperty, value); }
        }

        public static readonly DependencyProperty LineDashOffsetProperty =
            DependencyProperty.Register("LineDashOffset", typeof(double),
            typeof(LineChartBase<T>),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public DoubleCollection LineDashArray
        {
            get { return (DoubleCollection)GetValue(LineDashArrayProperty); }
            set { SetValue(LineDashArrayProperty, value); }
        }

        public static readonly DependencyProperty LineDashArrayProperty =
            DependencyProperty.Register("LineDashArray", typeof(DoubleCollection),
            typeof(LineChartBase<T>),
            new FrameworkPropertyMetadata(default(DoubleCollection), FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush ShadowColor
        {
            get { return (Brush)GetValue(ShadowColorProperty); }
            set { SetValue(ShadowColorProperty, value); }
        }

        public static readonly DependencyProperty ShadowColorProperty =
            DependencyProperty.Register("ShadowColor", typeof(Brush),
            typeof(LineChartBase<T>),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black) { Opacity = 0.4 }, FrameworkPropertyMetadataOptions.AffectsRender));

        private static readonly DependencyPropertyKey CurrentDataPropertyKey =
           DependencyProperty.RegisterReadOnly("CurrentData", typeof(double), typeof(LineChartBase<T>),
           new FrameworkPropertyMetadata(0d));

        public static readonly DependencyProperty CurrentDataProperty = CurrentDataPropertyKey.DependencyProperty;

        public double CurrentData
        {
            get { return (double)GetValue(CurrentDataProperty); }
            private set { SetValue(CurrentDataPropertyKey, value); }
        }

        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(LineChartBase<T>),
            new FrameworkPropertyMetadata(3.5, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double MaxValue
        {
            get { return TimeLineChartBehavior.GetMaxValue(this); }
            set { TimeLineChartBehavior.SetMaxValue(this, value); }
        }

        protected IList<double> Datas
        {
            get { return TimeLineChartBehavior.GetData(this); }
        }

        public Visibility IsMouseOverValuePoint
        {
            get { return (Visibility)GetValue(IsMouseOverValuePointProperty); }
            set { SetValue(IsMouseOverValuePointProperty, value); }
        }
        public static readonly DependencyProperty IsMouseOverValuePointProperty = DependencyProperty.Register("IsMouseOverValuePoint",
            typeof(Visibility), typeof(LineChartBase<T>), new UIPropertyMetadata(Visibility.Collapsed));

        #endregion
    }
}
