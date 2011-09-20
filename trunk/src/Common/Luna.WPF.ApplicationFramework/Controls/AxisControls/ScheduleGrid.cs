using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using Luna.Common;
using Luna.WPF.ApplicationFramework.Extensions;
using Luna.Core.Extensions;
using Luna.WPF.ApplicationFramework.Threads;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public partial class ScheduleGrid : AxisPanel
    {
        static ScheduleGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScheduleGrid), new FrameworkPropertyMetadata(typeof(ScheduleGrid)));
        }

        public override double VerticalOffSetValue { get { return RowHeight; } }


        public int SelectedRowIndex
        {
            get { return (int)GetValue(SelectedRowIndexProperty); }
            set { SetValue(SelectedRowIndexProperty, value); }
        }

        public static readonly DependencyProperty SelectedRowIndexProperty = DependencyProperty.Register("SelectedRowIndex", typeof(int),
            typeof(ScheduleGrid),
            new FrameworkPropertyMetadata(-1));

        public double RowHeight
        {
            get { return (double)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }

        public static readonly DependencyProperty RowHeightProperty =
             DependencyProperty.Register("RowHeight", typeof(double), typeof(ScheduleGrid),
             new UIPropertyMetadata(25d));

        public int ExtendedRowCount
        {
            get { return (int)GetValue(ExtendedRowCountProperty); }
            set { SetValue(ExtendedRowCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExtendedRowCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExtendedRowCountProperty =
            DependencyProperty.Register("ExtendedRowCount", typeof(int), typeof(ScheduleGrid), new UIPropertyMetadata(0));


        public int RowCount
        {
            get { return (int)GetValue(RowCountProperty); }
            set { SetValue(RowCountProperty, value); }
        }

        public static readonly DependencyProperty RowCountProperty = DependencyProperty.Register("RowCount", typeof(int), typeof(ScheduleGrid), new UIPropertyMetadata(0, (d, e) =>
                       {
                           var oldRowCount = (int)e.OldValue;
                           var newRowCount = (int)e.NewValue;
                           if (newRowCount == 0 || newRowCount == oldRowCount) return;

                           var scheduleGrid = (ScheduleGrid)d;

                           if (scheduleGrid.DataRangeY.Max == newRowCount) return;

                           scheduleGrid.DataRangeY.Max = newRowCount + scheduleGrid.ExtendedRowCount;
                           scheduleGrid.ViewportRangeY.Max = scheduleGrid.DataRangeY.Max * scheduleGrid.RowHeight;
                           scheduleGrid.SetVerticalOffset(0);
                       }));

        #region HourWidth

        public double HourWidth
        {
            get { return (double)GetValue(HourWidthProperty); }
            set { SetValue(HourWidthProperty, value); }
        }

        public static readonly DependencyProperty HourWidthProperty = DependencyProperty.Register("HourWidth", typeof(double), typeof(ScheduleGrid),
            new FrameworkPropertyMetadata(40d, FrameworkPropertyMetadataOptions.Inherits, (d, e) =>
            {
                var oldHourWidth = (double)e.OldValue;
                var newHourWidth = (double)e.NewValue;
                if (Math.Abs(newHourWidth - oldHourWidth) < 0d) return;

                var scheduleGrid = (ScheduleGrid)d;
                scheduleGrid.CalculateExtentWidth();

                scheduleGrid.SaftyInvoke(o => o.InvalidateScrollInfo());

                if(scheduleGrid.ScreenStart == scheduleGrid.StartTime && oldHourWidth!=newHourWidth && (oldHourWidth!=0 && newHourWidth!=0))
                    scheduleGrid.RefreshX();
            }));

        #endregion


        private void CalculateExtentWidth()
        {
            if (DataRangeX.Max == DateTime.MinValue || DataRangeX.Min == DateTime.MinValue)
                return;

            var totalHours = DataRangeX.Max.Subtract(DataRangeX.Min).TotalHours;
            ViewportRangeX.Max = totalHours * HourWidth;

            //SetHorizontalOffset(HorizontalOffset);

            DataRangeX.ViewMin = ViewportToDataX(ViewportRangeX.ViewMin);
            ScreenStart = DataRangeX.ViewMin;

            DataRangeX.ViewMax = ViewportToDataX(ViewportRangeX.ViewMax);
            ScreenEnd = DataRangeX.ViewMax;
        }

        public DateTime StartTime
        {
            get { return (DateTime)GetValue(StartTimeProperty); }
            set { SetValue(StartTimeProperty, value); }
        }

        public static readonly DependencyProperty StartTimeProperty =
            DependencyProperty.Register("StartTime", typeof(DateTime), typeof(ScheduleGrid),
            new FrameworkPropertyMetadata(DateTime.MinValue, FrameworkPropertyMetadataOptions.Inherits, (d, e) =>
            {
                var scheduleGrid = (ScheduleGrid)d;
                scheduleGrid.DataRangeX.Min = (DateTime)e.NewValue;
                scheduleGrid.DataRangeX.ViewMin = scheduleGrid.DataRangeX.Min;
                scheduleGrid.CalculateExtentWidth();
            }));

        public DateTime EndTime
        {
            get { return (DateTime)GetValue(EndTimeProperty); }
            set { SetValue(EndTimeProperty, value); }
        }

        public static readonly DependencyProperty EndTimeProperty =
            DependencyProperty.Register("EndTime", typeof(DateTime), typeof(ScheduleGrid),
            new FrameworkPropertyMetadata(DateTime.MaxValue, FrameworkPropertyMetadataOptions.Inherits, (d, e) =>
            {
                var scheduleGridBase = (ScheduleGrid)d;
                scheduleGridBase.DataRangeX.Max = (DateTime)e.NewValue;
                scheduleGridBase.CalculateExtentWidth();
            }));

        public DateTime ScreenStart
        {
            get { return (DateTime)GetValue(ScreenStartProperty); }
            set { SetValue(ScreenStartProperty, value); }
        }

        public static readonly DependencyProperty ScreenStartProperty = DependencyProperty.Register("ScreenStart", typeof(DateTime), typeof(ScheduleGrid),
                                new FrameworkPropertyMetadata(DateTime.MinValue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                            OnScreenStartChanged, (d, baseValue) =>
                                                      {
                                                          var value = Convert.ToDateTime(baseValue);
                                                          var el = ((ScheduleGrid)d);

                                                          if (value == DateTime.MinValue)
                                                              value = el.StartTime;

                                                          return value;
                                                      }));

        // BMK OnScreenStartChanged
        private static void OnScreenStartChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var scheduleGrid = (ScheduleGrid)sender;
            var value = (DateTime)e.NewValue;

            if (scheduleGrid.DataRangeX.ViewMin == value)
                return;

            // 当一切就绪才进行Scrolling, 请勿调整顺序
            //UIThread.BeginInvoke(() =>
            //                         {
                                         var horizontalOffset = scheduleGrid.DataToViewportX(value);
                                         if (horizontalOffset < 0 || double.IsNaN(horizontalOffset)) return;

                                         scheduleGrid.SetHorizontalOffset(horizontalOffset);

                                         //scheduleGrid.DataRangeX.ViewMax = scheduleGrid.ViewportToDataX(scheduleGrid.ViewportRangeX.ViewMax);
                                         //scheduleGrid.ScreenEnd = scheduleGrid.DataRangeX.ViewMax;
                                     //});
        }

        public DateTime ScreenEnd
        {
            get { return (DateTime)GetValue(ScreenEndProperty); }
            set { SetValue(ScreenEndProperty, value); }
        }

        public static readonly DependencyProperty ScreenEndProperty =
            DependencyProperty.Register("ScreenEnd", typeof(DateTime), typeof(ScheduleGrid),
            new FrameworkPropertyMetadata(DateTime.MaxValue, FrameworkPropertyMetadataOptions.AffectsRender));

        #region LeftWidth

        public GridLength LeftWidth
        {
            get { return (GridLength)GetValue(LeftWidthProperty); }
            set { SetValue(LeftWidthProperty, value); }
        }

        public static readonly DependencyProperty LeftWidthProperty =
            DependencyProperty.Register("LeftWidth", typeof(GridLength), typeof(ScheduleGrid),
             new UIPropertyMetadata(new GridLength(0, GridUnitType.Pixel)));

        #endregion

        #region ClickTime

        public DateTime ClickTime
        {
            get { return (DateTime)GetValue(ClickTimeProperty); }
            set { SetValue(ClickTimeProperty, value); }
        }

        public static readonly DependencyProperty ClickTimeProperty =
            DependencyProperty.Register("ClickTime", typeof(DateTime), typeof(ScheduleGrid),
            new UIPropertyMetadata());

        #endregion

        #region Main Attach Property

        public static bool GetHorizontalMain(DependencyObject obj)
        {
            return (bool)obj.GetValue(HorizontalMainProperty);
        }

        public static void SetHorizontalMain(DependencyObject obj, bool value)
        {
            obj.SetValue(HorizontalMainProperty, value);
        }

        public static readonly DependencyProperty HorizontalMainProperty =
            DependencyProperty.RegisterAttached("HorizontalMain", typeof(bool), typeof(ScheduleGrid),
            new UIPropertyMetadata(OnHorizontalMainChanged));

        public static bool GetVerticalMain(DependencyObject obj)
        {
            return (bool)obj.GetValue(VerticalMainProperty);
        }

        public static void SetVerticalMain(DependencyObject obj, bool value)
        {
            obj.SetValue(VerticalMainProperty, value);
        }

        public static readonly DependencyProperty VerticalMainProperty = DependencyProperty.RegisterAttached("VerticalMain", typeof(bool),
            typeof(ScheduleGrid), new UIPropertyMetadata(OnVerticalMainChanged));

        #endregion

        #region MainAction

        private static void OnVerticalMainChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var reference = d as UIElement;
            if (reference != null)
            {
                reference.Dispatcher.BeginInvoke((Action)delegate
                {
                    var parent = reference.FindAncestor<ScheduleGrid>();
                    if (parent != null)
                    {
                        if (Equals(e.NewValue, true))
                            parent.SetVerticalMain(reference);
                        else
                            parent.RemoveVerticalMain(reference);

                        parent.SetVerticalOffset(0);
                        parent.SetOnSizeChangedEvent();
                    }
                }, System.Windows.Threading.DispatcherPriority.Input);
            }
        }

        private static void OnHorizontalMainChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var hElement = d as UIElement;
            if (hElement != null)
            {
                hElement.Dispatcher.BeginInvoke((Action)delegate
                {
                    var parent = hElement.FindAncestor<ScheduleGrid>();
                    if (parent != null)
                    {
                        if (Equals(e.NewValue, true))
                            parent.HorizontalMainElement = hElement;

                        parent.SetHorizontalOffset(parent.ViewportRangeX.ViewMin);
                        parent.SetOnSizeChangedEvent();
                    }
                }, System.Windows.Threading.DispatcherPriority.Input);
            };
        }

        #endregion

        protected override double DataToViewportX(DateTime data)
        {
            // return data.Subtract(DataRangeX.Min).TotalDays*72d;   // 返回一个数，这个数是72的倍数，
            double totalticks = DataRangeX.Max.Subtract(DataRangeX.Min).Ticks;
            double dataticks = data.Subtract(DataRangeX.Min).Ticks;
            return dataticks / totalticks * ViewportRangeX.Max;
        }

        //BMK Viewport X to DateTime 
        protected override DateTime ViewportToDataX(double viewportX)
        {
            double totalticks = DataRangeX.Max.Subtract(DataRangeX.Min).TotalMilliseconds;
            double viewticks = viewportX * totalticks / ViewportRangeX.Max;
            return DataRangeX.Min.AddMilliseconds(double.IsNaN(viewticks) ? viewportX : viewticks);
        }

        protected override double DataToAbsoluteY(int data)
        {
            return data * RowHeight;

            double totalticks = DataRangeY.Max - DataRangeY.Min;
            double dataticks = data - DataRangeY.Min;
            return dataticks / totalticks * ViewportRangeY.Max;
        }

        protected override int AbsoluteYToDataRowIndex(double viewportY)
        {
            var y = (int)(viewportY / RowHeight);
            return y;
        }

        public override int GetScreenTopRowIndex()
        {
            var index = DataRangeY.ViewMin;
            if (RowCount <= index)
                index = RowCount - index - 1;
            return index;
        }

        protected override double ComputeStep()
        {
            var datetimeSteps = (DataRangeX.Max - DataRangeX.Min).TotalMinutes / 5;
            var points = ViewportRangeX.Max - ViewportRangeX.Min;
            return points / datetimeSteps;
        }

        // BMK SetHorizontalOffset
        public override void SetHorizontalOffset(double offset)
        {
            if ((ViewportRangeX.Max - GetViewportWidth()) <= offset)
                offset = Math.Max(0, ViewportRangeX.Max - GetViewportWidth());
            else
            {
                offset = Math.Max(0, offset);

                if (0 < HorizontalOffSetValue)
                    offset -= (offset % (HorizontalOffSetValue));
            }

            var scrolled = false;

            if (0 < Math.Abs(ViewportRangeX.ViewMin - offset))
            {
                ViewportRangeX.ViewMin = offset;
                DataRangeX.ViewMin = ViewportToDataX(ViewportRangeX.ViewMin);
                ScreenStart = DataRangeX.ViewMin;
                scrolled = true;
            }

            var viewMaxValue = Math.Min(ViewportRangeX.ViewMin + GetViewportWidth(), ViewportRangeX.Max);

            if (0 < Math.Abs(ViewportRangeX.ViewMax - viewMaxValue))
            {
                ViewportRangeX.ViewMax = viewMaxValue;
                DataRangeX.ViewMax = ViewportToDataX(ViewportRangeX.ViewMax);
                ScreenEnd = DataRangeX.ViewMax;
                scrolled = true;
            }

            if (!scrolled) return;

            ScrollOwner.SaftyInvoke(o => o.InvalidateScrollInfo());

            RefreshX();
            Refresh(); // content Elements
        }




        // BMK SetVerticalOffset
        public override void SetVerticalOffset(double offset)
        {
            //Debug.Print(string.Format("{0} SetVerticalOffset called", GetType().FullName));
            // 当 offset < ExtentHeight - ViewportHeight 代表已经拉到 scrollbar 底
            // 当 offset 为负数代表已经拉到 scrollbar 顶


            if ((ExtentHeight - GetViewportHeight()) <= offset)
            {
                _remainsOfVerticalOffset = Math.Max(0, ExtentHeight - GetViewportHeight()) % VerticalOffSetValue;
            }
            else
            {
                _remainsOfVerticalOffset = 0;
            }

            //else
            //{
            //    offset = Math.Max(0, offset);

            //    if (0 < VerticalOffSetValue)
            //        offset -= (offset % (VerticalOffSetValue));
            //}


            offset = Math.Min(Math.Max(0, offset), ExtentHeight - GetViewportHeight());



            offset = Math.Max(0, offset - (offset % VerticalOffSetValue)); // 需要处理末端多馀的高度 VerticalOffSetValue == RowHeight, ex: 以25为单位移动

            var scrolled = false;

            if (0 < Math.Abs(ViewportRangeY.ViewMin - offset)) // changed
            {
                ViewportRangeY.ViewMin = offset;

                var index = AbsoluteYToDataRowIndex(ViewportRangeY.ViewMin);
                if (RowCount <= index)
                    index = RowCount - index - 1;

                DataRangeY.ViewMin = index;
                scrolled = true;
            }

            //计算 ViewportRangeY.ViewMax
            var desiredDisplayCount = (int)(GetViewportHeight() / RowHeight + 0.9);
            var maxRowIndex = DataRangeY.ViewMin + desiredDisplayCount;
            var viewMax = (RowCount <= maxRowIndex ? RowCount : maxRowIndex) * RowHeight;

            if (0 < Math.Abs(ViewportRangeY.ViewMax - viewMax) || RowCount <= maxRowIndex)
            {
                ViewportRangeY.ViewMax = viewMax;
                DataRangeY.ViewMax = Math.Min(RowCount - 1, (int)((viewMax / RowHeight) + 0.9));
                scrolled = true;
            }
          

            if (!scrolled) return;

            if (!_isSizeChanged)
                Refresh();
            ScrollOwner.SaftyInvoke(o => o.InvalidateScrollInfo());
        }

        protected override double GetViewportWidth()
        {
            return HorizontalMainElement.RenderSize.Width;
        }

        // BMK Fix Get ViewportHeight
        protected override double GetViewportHeight()
        {
            return VerticalMainElement.RenderSize.Height;
            //return VerticalMainElement.RenderSize.Height - VerticalMainElement.RenderSize.Height % RowHeight;
        }

        public void NavigateToRow(int rowIndex)
        {
            var offset = rowIndex * RowHeight;
            SetVerticalOffset(offset);
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            var possibleRowIndex = ScreenToDataY(e.GetPosition(HorizontalMainElement).Y, false);
            if (0 <= possibleRowIndex && possibleRowIndex < RowCount)
                SelectedRowIndex = possibleRowIndex;

            base.OnMouseDown(e);
        }
    }
}