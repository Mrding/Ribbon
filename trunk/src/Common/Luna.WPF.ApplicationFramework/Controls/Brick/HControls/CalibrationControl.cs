using Luna.Core.Extensions;

namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    using System;
    using System.Windows;
    using System.Windows.Media;

    //此控件須接收scheudle grid 放大縮小事件
    public class Ruler<T> : AxisControl
    {
        static Ruler()
        {
            SnapsToDevicePixelsProperty.OverrideMetadata(typeof(Ruler<T>), new FrameworkPropertyMetadata(true));
        }

        private double _internalInterval;

        public override void Initialize()
        {
            base.Initialize();
            ElementDraws.Add(new DateTimeLineDraw());
            GetAxisPanel().ZoomValueChanged += AxisPanelZoomValueChanged;
            base.OnLoaded();
            ChangeGraduations();
        }

        protected override void AddToPanel(IAxisPanel axisPanel)
        {
            axisPanel.AddHorizontalControl(this);
        }

        void AxisPanelZoomValueChanged(object sender, ZoomChangedEventArgs e)
        {
            ChangeGraduations();
        }

        internal Action Zooming;

        internal void ChangeGraduations()
        {
            if (_internalInterval <= 0)
                _internalInterval = Interval;

            if (Zooming != null)
                Zooming();



            switch (AxisPanel.SaftyGetProperty<int, ScheduleGrid>(o => (int)o.ZoomValue, () => 1))
            {
                case 1:
                    Interval = _internalInterval;
                    Graduations = new DoubleCollection { 14d, 3d, 7d, 3d };
                    break;
                case 2:
                    Interval = _internalInterval * 4 / 6;
                    Graduations = new DoubleCollection { 14d, 0d, 0d, 7d, 0d, 0d };
                    break;
                case 4:
                    Interval = _internalInterval * 4 / 3;
                    Graduations = new DoubleCollection { 14d, 3d, 3d };
                    break;
                case 12:
                    Interval = _internalInterval;
                    Graduations = new DoubleCollection { 14d, 0d, 0d, 0d };
                    break;
            }
        }

        private ScheduleGrid GetAxisPanel()
        {
            return (ScheduleGrid)AxisPanel;
        }

        #region Properties



        public bool ShowBaseLine
        {
            get { return (bool)GetValue(ShowBaseLineProperty); }
            set { SetValue(ShowBaseLineProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowBaseLine.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowBaseLineProperty =
            DependencyProperty.Register("ShowBaseLine", typeof(bool), typeof(Ruler<T>), new UIPropertyMetadata(true));



        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(Ruler<T>),
            new UIPropertyMetadata(Brushes.Gray));

        public double Interval
        {
            get { return (double)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        public static readonly DependencyProperty IntervalProperty =
               DependencyProperty.Register("Interval", typeof(double), typeof(Ruler<T>),new FrameworkPropertyMetadata(10.0, (d,e)=>
                {
                    d.SaftyInvoke<Ruler<T>>(o => { 
                        o.ChangeGraduations(); 
                        o.InvalidateVisual();});
                }));

        public DoubleCollection Graduations
        {
            get { return (DoubleCollection)GetValue(GraduationsProperty); }
            set { SetValue(GraduationsProperty, value); }
        }

        public static readonly DependencyProperty GraduationsProperty =
            DependencyProperty.Register("Graduations", typeof(DoubleCollection), typeof(Ruler<T>),
                 new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public double XTranslateTransform
        {
            get { return (double)GetValue(XTranslateTransformProperty); }
            set { SetValue(XTranslateTransformProperty, value); }
        }

        public static readonly DependencyProperty XTranslateTransformProperty = DependencyProperty.Register("XTranslateTransform", typeof(double),
           typeof(Ruler<T>), new FrameworkPropertyMetadata(-5.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool ShowSelectedLine
        {
            get { return (bool)GetValue(ShowSelectedLineProperty); }
            set { SetValue(ShowSelectedLineProperty, value); }
        }

        public static readonly DependencyProperty ShowSelectedLineProperty =
            DependencyProperty.Register("ShowSelectedLine", typeof(bool), typeof(Ruler<T>),
            new UIPropertyMetadata(true));

        public IBlockConverter TimeDrawConverter
        {
            get { return (IBlockConverter)GetValue(TimeDrawConverterProperty); }
            set { SetValue(TimeDrawConverterProperty, value); }
        }

        // 呈現選中班表顏色作用
        public static readonly DependencyProperty TimeDrawConverterProperty =
            DependencyProperty.Register("TimeDrawConverter", typeof(IBlockConverter), typeof(Ruler<T>),
            new UIPropertyMetadata());

        private bool _isNewBlockLine;
        public bool IsNewBlockLine
        {
            get { return _isNewBlockLine; }
            set { _isNewBlockLine = value; }
        }

        public object BlockLine
        {
            get { return GetValue(BlockLineProperty); }
            set { SetValue(BlockLineProperty, value); }
        }

        public static readonly DependencyProperty BlockLineProperty =
            DependencyProperty.Register("BlockLine", typeof(object), typeof(Ruler<T>),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender,
                (sender, e) =>
                {
                    var timeLine = sender as Ruler<T>;
                    if (timeLine != null) timeLine._isNewBlockLine = true;
                }));

        internal double GetSelectedLineHeight()
        {
            return ShowSelectedLine ? 5 : 0;
        }

        #endregion Properties

        #region Methods

        internal void InitializeGuidelines()
        {
            VisualXSnappingGuidelines = new DoubleCollection();
            VisualYSnappingGuidelines = new DoubleCollection();
        }

        internal void AddXGuidelines(double value)
        {
            VisualXSnappingGuidelines.Add(value);
        }

        internal void AddYGuidelines(double value)
        {
            VisualYSnappingGuidelines.Add(value);
        }

        #endregion Methods

        protected override Size MeasureOverride(Size availableSize)
        {
            var width = double.IsInfinity(availableSize.Width) ? 0 : availableSize.Width;
            
                
            return new Size(width, 14); // 14 is from LineHeight Max Value
        }

        protected override void OnDispose()
        {
            GetAxisPanel().ZoomValueChanged -= AxisPanelZoomValueChanged;
            base.OnDispose();
        }
    }
}