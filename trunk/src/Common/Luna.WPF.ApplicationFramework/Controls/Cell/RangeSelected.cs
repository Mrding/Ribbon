using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Luna.Common;
using Luna.Core.Extensions;
using Luna.WPF.ApplicationFramework.Controls.Cell;
using Luna.WPF.ApplicationFramework.Threads;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public class RangeSelected : BlockGridLayerBase
    {
        // 2点取对角范围
        private readonly Point _unsetPoint = new Point(-1, -1);
        protected Point _startPoint, _currentPoint;
        private bool _isMouseMove;
        protected bool IsDraging;
        private int _screenDataRowIndex;

        public static readonly RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent("Selected", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(RangeSelected));

        public static void AddSelectedHandler(DependencyObject d, RoutedEventHandler handler)
        {
            d.SaftyInvoke<UIElement>(uie => uie.AddHandler(SelectedEvent, handler));
        }

        public static void RemoveSelectedHandler(DependencyObject d, RoutedEventHandler handler)
        {
            d.SaftyInvoke<UIElement>(uie => uie.RemoveHandler(SelectedEvent, handler));
        }

        public override void Initialize()
        {
            _startPoint = _unsetPoint;
            _currentPoint = _unsetPoint;
            base.Initialize();
            //ElementDraws.Clear();
            //ElementDraws.Add(new RangeSelectedDraw());
        }

        protected override void AddToPanel(IAxisPanel axisPanel)
        {
            axisPanel.Add(this);
        }

        /// <summary>
        /// Gets or sets the Brush that specifies how the drag background interior is painted. 
        /// </summary>
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(RangeSelected),
            new PropertyMetadata(new SolidColorBrush(Colors.SkyBlue) { Opacity = 0.6 }));

        public DateTime ClickDate
        {
            get { return (DateTime)GetValue(ClickDateProperty); }
            set { SetValue(ClickDateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ClickDate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClickDateProperty =
            DependencyProperty.Register("ClickDate", typeof(DateTime), typeof(RangeSelected),
            new FrameworkPropertyMetadata(DateTime.MinValue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object InitalDrawBlock
        {
            get { return GetValue(InitalDrawBlockProperty); }
            set { SetValue(InitalDrawBlockProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InitalDrawBlock.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InitalDrawBlockProperty =
            DependencyProperty.Register("InitalDrawBlock", typeof(object), typeof(RangeSelected),
            new FrameworkPropertyMetadata(null));

        public object SelectionDataRowRange
        {
            get { return GetValue(SelectionDataRowRangeProperty); }
            set { SetValue(SelectionDataRowRangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectionDataRowRange.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionDataRowRangeProperty =
            DependencyProperty.Register("SelectionDataRowRange", typeof(object), typeof(RangeSelected), new FrameworkPropertyMetadata(null));

        public TimeRange SelectionTimeRange
        {
            get { return (TimeRange)GetValue(SelectionTimeRangeProperty); }
            set { SetValue(SelectionTimeRangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectionTimeRange.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionTimeRangeProperty =
            DependencyProperty.Register("SelectionTimeRange", typeof(TimeRange), typeof(RangeSelected), new UIPropertyMetadata(null));


        private int[] _dataRowRange = new[] { 0, -1 };
        public int[] DataRowRange
        {
            get { return _dataRowRange; }
        }

        private TimeRange _timeRange;
        public TimeRange TimeRange { get { return _timeRange; } }

        //public object SelectedBlocks
        //{
        //    get { return (object)GetValue(SelectedBlocksProperty); }
        //    set { SetValue(SelectedBlocksProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for SelectedBlocks.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty SelectedBlocksProperty =
        //    DependencyProperty.Register("SelectedBlocks", typeof(object), typeof(RangeSelected), new UIPropertyMetadata(null));


        protected int ClickCount;

        //public Rect? InitalDrawLocation { get; protected set; }

        public bool CellMode
        {
            get { return (bool)GetValue(CellModeProperty); }
            set { SetValue(CellModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellModeProperty =
            DependencyProperty.Register("CellMode", typeof(bool), typeof(RangeSelected), new UIPropertyMetadata(true));


        /// <summary>
        /// 当前选中格绝对位置
        /// </summary>
        public Rect? InitalDrawLocation
        {
            get { return (Rect?)GetValue(InitalDrawLocationProperty); }
            set { SetValue(InitalDrawLocationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InitalDrawLocation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InitalDrawLocationProperty =
            DependencyProperty.Register("InitalDrawLocation", typeof(Rect?), typeof(RangeSelected), new UIPropertyMetadata(null));

        private void SetInitalDrawLocation()
        {
            if (CellMode)
            {
                var rect = _startPoint.CellDesiredSzie(Interval, AxisXConverter, RenderSize, VerticalOffset, HorizontalOffset,
                                            AxisPanel.ViewportRangeY.ViewMax);
                if (rect != null)
                    InitalDrawLocation = rect.Value;
            }
            else
            {
                if (PointOutBlock == null)
                    InitalDrawLocation = default(Rect);
                else
                {
                    var top = LayerContainer.PointOutDataRowTop + BlockConverter.GetHeight(PointOutBlock);
                    var left = _startPoint.X;
                    InitalDrawLocation = new Rect(left, top, 0, 0);
                }
            }
        }

        protected override void OnParentMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source.IsNot<Rectangle>() && e.Source.IsNot<BlockGrid>()) return;
            _screenDataRowIndex = AxisPanel.GetScreenTopRowIndex();


            if (Keyboard.Modifiers == ModifierKeys.Shift && _startPoint != _unsetPoint && _currentPoint != _unsetPoint)
            {
                //只有一个选中
                //if (_startPoint == _currentPoint)
                //{
                _currentPoint = LayerContainer.GetMousePosition(true);

                //}
                //else //已经选中多个
                //{
                //_startPoint = _currentPoint;
                //}
            }
            else
            {
                _startPoint = LayerContainer.GetMousePosition(true);
                _currentPoint = _startPoint;
            }

            //ClickCount = e.ClickCount;

            SetInitalDrawLocation();
        }



        protected override void OnParentMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Source.IsNot<Rectangle>() && e.Source.IsNot<BlockGrid>()) return;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                TraceCurrentMousePoint(e);
                MeasureDragingArea(_screenDataRowIndex);
                _isMouseMove = true;
                IsDraging = true;
                InvalidateVisual();
            }
            if (IsDraging && e.LeftButton != MouseButtonState.Pressed)
                UIThread.BeginInvoke(ResetMouseStatus);
        }

        protected override void OnParentMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isMouseMove || IsDraging)
            {
                //do nothing
            }
            //else if (ClickCount < 2) // don't remove 这会影响EditControlLayer double click 编辑弹出框
            //{
                SetCurrentMousePoint(e);

                if (LayerContainer.HeaderIsHitted())// Column 全選 IsHeader(e.Source)
                {
                    _screenDataRowIndex = 0;
                    _startPoint = new Point(LayerContainer.GetMousePosition(true).X, 0);
                    _currentPoint = new Point(_startPoint.X, GetItemsSourceCount() * Interval);
                    SetInitalDrawLocation();
                }
                MeasureDragingArea(_screenDataRowIndex);
                e.Handled = true;
                InvalidateVisual();
            //}

            IsDraging = false;
            _isMouseMove = false;

            //xe.Handled = !AxisYConverter.IsInViewRagne(_currentPoint.Y);
            OnRangeSelected();
        }



        private Point ConvertToRelativePosition(Point point)
        {
            return new Point(point.X - HorizontalOffset, point.Y - VerticalOffset);
        }

        protected override void OnParentKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift) return;

            if (InitalDrawLocation == null) return;

            _screenDataRowIndex = AxisPanel.GetScreenTopRowIndex();
            var prevRowIndex = AxisYConverter.ToData(InitalDrawLocation.Value.Y);
            switch (e.Key)
            {
                case Key.Left:
                    MoveLeft(InitalDrawLocation.Value);
                    MeasureDragingArea(_screenDataRowIndex);
                    e.Handled = 0 <= ConvertToRelativePosition(_startPoint).X;
                    break;
                case Key.Right:
                    if (TryMoveRight(InitalDrawLocation.Value))
                        MeasureDragingArea(_screenDataRowIndex);
                    e.Handled = ConvertToRelativePosition(_currentPoint).X <= RenderSize.Width;
                    break;
                case Key.Up:
                    TryMoveUp(InitalDrawLocation.Value);
                    var moveUpRowIndex = AxisYConverter.ToData(_startPoint.Y);
                    e.Handled = moveUpRowIndex != prevRowIndex;
                    if (!e.Handled)
                        _screenDataRowIndex -= _screenDataRowIndex > 0 ? 1 : 0; // scroll up occurred
                    MeasureDragingArea(_screenDataRowIndex);
                    break;
                case Key.Down:
                    TryMoveDown(InitalDrawLocation.Value);
                    var moveDownRowIndex = AxisYConverter.ToData(_startPoint.Y);

                    if ((RenderSize.Height - AxisYConverter.DataToScreen(moveDownRowIndex)) < Interval) // 屏幕最后一格未满
                        e.Handled = false;
                    else
                        e.Handled = moveDownRowIndex != prevRowIndex;

                    MeasureDragingArea(_screenDataRowIndex);

                    if (!e.Handled)
                        _screenDataRowIndex += GetItemsSourceCount() - 1 <= moveDownRowIndex ? 0 : 1; // scroll down occurred
                    break;
                default:
                    ClickCount = 0;
                    break;
            }

            SetInitalDrawLocation();

            if (e.Handled)
                InvalidateVisual(); //未发生scroll bar 卷动, 手动刷新 cell 新的位置
            else
            {
                LayerContainer.Focus();
            }
            // e.Handled 如果为 false 代表由 scroll bar 滚动来引发重绘

        }

        private void OnRangeSelected()
        {
            this.RaiseEvent(new RoutedEventArgs(SelectedEvent, this));
        }

        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            return new RectangleGeometry(new Rect(RenderSize));
        }

        private bool IsYOutOfRange(Rect rect)
        {
            //var rowIndex = (int)(rect.Y / Interval) + _screenDataRowIndex;

            var rowIndex = (int)(rect.Y / Interval);
            return GetItemsSourceCount() - 1 <= rowIndex || rowIndex < _screenDataRowIndex;
        }

        public void MoveLeft(Rect rect)
        {
            var x = rect.Left - rect.Width;
            if (!AxisXConverter.IsInDataRagne(x, true)) //AxisXConverter.ScreenToData(x) < AxisPanel.StartTime
            {
                x = 0;
            }
            _currentPoint = new Point(x + rect.Width, rect.Y);
            _startPoint = new Point(x, _currentPoint.Y);
        }

        public bool TryMoveRight(Rect rect)
        {
            var x = rect.Left + rect.Width;
            if (!AxisXConverter.IsInDataRagne(x, true))//AxisPanel.EndTime <= AxisXConverter.ScreenToData(x)
                return false;
            _startPoint = new Point(x, rect.Y);
            _currentPoint = new Point(x + rect.Width, rect.Y);
            return true;
        }

        public bool TryMoveUp(Rect rect)
        {
            if (_screenDataRowIndex < AxisYConverter.ToData(_startPoint.Y)) // 不在位置 0 (第一笔)
            {
                _startPoint = new Point(rect.X, rect.Y <= 0 ? 0 : rect.Y - rect.Height);
                _currentPoint = new Point(rect.X + rect.Width, _startPoint.Y);
                return true;
            }
            return false;
        }

        public bool TryMoveDown(Rect rect)
        {
            if (IsYOutOfRange(rect)) return false;
            _currentPoint = new Point(rect.X + rect.Width, rect.Y + rect.Height);
            _startPoint = new Point(rect.X, _currentPoint.Y);
            return true;
        }

        private DispatcherTimer _autoScrollTimer;

        private void SetCurrentMousePoint(MouseEventArgs e)
        {
            var currentPoint = LayerContainer.GetMousePosition(true);
            // 防止 currentPoint 不让其超出控件 renderSize 范围
            var x = Math.Max(currentPoint.X, 0);
            var y = Math.Max(currentPoint.Y, 0);

            // 控制当mouse在边界外禁止响应x,y轴座标
            if (currentPoint.X <= 0 || AxisPanel.ViewportRangeX.ViewMax < currentPoint.X)
                y = _currentPoint.Y;

            if (currentPoint.Y <= 0 || AxisPanel.ViewportRangeY.ViewMax < currentPoint.Y)
                x = _currentPoint.X;



            _currentPoint = new Point(x, y);
        }


        private bool AutoScroll()
        {
            var position = Mouse.GetPosition(this);

            var scrolled = true;
            var hOffsetValue = AxisPanel.HorizontalOffSetValue;
            var hOffset = HorizontalOffset;
            var vOffsetValue = AxisPanel.VerticalOffSetValue;
            var vOffset = VerticalOffset;

            const double threshold = 3;

            if (position.X < threshold)
            {
                hOffset = hOffset - (hOffset / hOffsetValue);
                AxisPanel.SetHorizontalOffset(hOffset);
            }
            else if ((RenderSize.Width - threshold) < position.X)
            {
                hOffset += hOffsetValue;
                AxisPanel.SetHorizontalOffset(hOffset);
            }
            else if (position.Y < threshold)
            {
                vOffset = vOffset - (vOffset / vOffsetValue);
                AxisPanel.SetVerticalOffset(vOffset);
            }
            else if ((RenderSize.Height - threshold) < position.Y)
            {
                vOffset += vOffsetValue;
                AxisPanel.SetVerticalOffset(vOffset);
            }
            else
                scrolled = false;

            if (scrolled)
            {
                LayerContainer.Focus();
                Mouse.Capture(LayerContainer);
            }


            return scrolled;
        }

        private void TraceCurrentMousePoint(MouseEventArgs e)
        {

            var currentPoint = LayerContainer.GetMousePosition(true);
            // 防止 currentPoint 不让其超出控件 renderSize 范围
            var x = Math.Max(currentPoint.X, 0);
            var y = Math.Max(currentPoint.Y, 0);

            var startAutoScrollTimer = false;

            if (_autoScrollTimer == null || !_autoScrollTimer.IsEnabled)
                startAutoScrollTimer = AutoScroll();

            if (startAutoScrollTimer)
            {
                _autoScrollTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.3) };
                _autoScrollTimer.Tick += delegate
                {
                    if (e.LeftButton == MouseButtonState.Pressed && AutoScroll())
                    {
                        SetCurrentMousePoint(e);
                        MeasureDragingArea(_screenDataRowIndex);
                        return;
                    }
                    LayerContainer.ReleaseMouseCapture();
                    _autoScrollTimer.Stop();
                };

                _autoScrollTimer.Start();
            }
            _currentPoint = new Point(x, y);
        }

        private Rect _measureofArea;
        internal Rect MeasureofArea { get { return _measureofArea; } }

        public Rect MeasureDragingArea(int screenDataRowIndex)
        {
            // 取两点 画出一个范围方块
            var xMin = Math.Min(_currentPoint.X, _startPoint.X);
            var yMin = Math.Min(_currentPoint.Y, _startPoint.Y);

            var xMax = Math.Max(_currentPoint.X, _startPoint.X);
            var yMax = Math.Max(_currentPoint.Y, _startPoint.Y);

            var end = AxisXConverter.ToData(xMax);
            _timeRange = new TimeRange(AxisXConverter.ToData(xMin), end.TimeOfDay.TotalHours > 0 ? end.Date.AddDays(1) : end);

            ClickDate = _timeRange.Start.Date;

            screenDataRowIndex = 0;//_shiftKeyPressed ? 0 : screenDataRowIndex;

            var maxDataRowIndex = Math.Min(GetItemsSourceCount() - 1, (int)(yMax / Interval) + screenDataRowIndex);
            _dataRowRange = new[] { Math.Min((int)(yMin / Interval) + screenDataRowIndex, maxDataRowIndex), maxDataRowIndex };

            //Debug.Print(string.Format("y:{0}, {1}", yMax, _currentPoint.Y));
            //Debug.Print(string.Format("row:{0}", _dataRowRange[1]));

            _measureofArea = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);

            SelectionTimeRange = TimeRange;
            SelectionDataRowRange = DataRowRange;

            return _measureofArea;
        }

        public void ResetMouseStatus()
        {
            IsDraging = false;
            _isMouseMove = false;
            _measureofArea = new Rect();
        }

        internal bool IsInViewRange(out Point point)
        {
            if (InitalDrawLocation == null)
            {
                point = new Point(0, 0);
                return true;
            }

            point = new Point(InitalDrawLocation.Value.X - HorizontalOffset,
                              InitalDrawLocation.Value.Y - VerticalOffset); // 转换成相对位置
            return AxisXConverter.IsInViewRagne(point.X) && AxisYConverter.IsInViewRagne(point.Y);
        }

        internal virtual void DrawComplete(Rect[] range, object block)
        {
            //RowsCount = (int)((range[1].Y - range[0].Y) / range[0].Height) + 1;
            //ColumnsCount = (int)((range[1].X - range[0].X) / range[0].Width) + 1;

            InitalDrawBlock = block;
        }

        internal virtual void NotInViewRange()
        {
            
        }

        protected override void OnDispose()
        {
            _autoScrollTimer.SaftyInvoke(o => o.Stop());
            _autoScrollTimer = null;

        }
    }
}
