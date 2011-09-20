using System;
using System.Windows;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public partial class ScheduleGrid
    {
        private bool _isSizeChanged;

        //public ScheduleGrid()
        //{
           
        //    //ScrollChanged += ScheduleGridScrollChanged;
        //}

        //private void ScheduleGridScrollChanged(object sender, ScrollChangedEventArgs e)
        //{
        //    if (RowCount == 0 || ScreenStart == (DateTime)ScreenStartProperty.DefaultMetadata.DefaultValue)
        //        return;

        //    Refresh();
        //    //if (e.ViewportHeight != e.ViewportHeightChange || e.ViewportWidth != e.ViewportWidthChange)
        //    //    _isScrollChanged = true;
        //}

        internal void SetOnSizeChangedEvent()
        {
            if(HorizontalMainElement != null && VerticalMainElement != null)
                SizeChanged += ScheduleGridSizeChanged;
        }

        protected virtual void ScheduleGridSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ScreenStart == (DateTime) ScreenStartProperty.DefaultMetadata.DefaultValue)
                return;
            
            _isSizeChanged = true; // 阻止 SetHorizontalOffset, SetVerticalOffset 引发双次 Refresh();

            SetHorizontalOffset(ViewportRangeX.ViewMin);
            SetVerticalOffset(VerticalOffset);

            _isSizeChanged = false; // 释放

            Refresh();
            return;
        }

        //protected bool HitOnVisiable(MouseButtonEventArgs e, UIElement element, out Point point)
        //{
        //    point = e.GetPosition(element);
        //    return 0 <= point.Y && point.Y <= VerticalMainElement.RenderSize.Height;
        //}

        //protected bool IsPointXInEditArea(Point point)
        //{
        //    return 0 <= point.X && point.X <= HorizontalMainElement.RenderSize.Width;
        //}

        //protected void SetScreenColumnIndex()
        //{
        //    var startIndex = Convert.ToInt32((this.ScreenStart.Date - this.StartTime.Date).TotalDays);
        //    var endIndex = Convert.ToInt32((this.ScreenEnd.Date - this.StartTime.Date).TotalDays);
        //    if (!(SelectedColumnIndex >= startIndex && SelectedColumnIndex <= endIndex))
        //        SelectedColumnIndex = startIndex;
        //}

        //protected void SetColumnIndex(Point xOffset)
        //{
        //    var x = ScreenToViewportX(xOffset.X);
        //    SelectedColumnIndex = IsXPointInRange(x) ? (int)(x / RowWidth) : -1;
        //    Console.WriteLine("SelectedColumnIndex {0}", SelectedColumnIndex);
        //}

        //protected void SetRowIndex(Point yOffset)
        //{
        //    var y = ScreenToViewportY(yOffset.Y);
        //    SelectedRowIndex = IsYPointInRange(y) ? (int)(y / RowHeight) : -1;
        //}

        //private bool IsXPointInRange(double x)
        //{
        //    return x >= 0 && x <= ColumnCount * RowWidth;
        //}

        //private bool AxisY(double y)
        //{
        //    return 0 <= y && y <= RowCount * RowHeight;
        //}

        //public int SelectedColumnIndex
        //{
        //    get { return (int)GetValue(SelectedColumnIndexProperty); }
        //    set { SetValue(SelectedColumnIndexProperty, value); }
        //}

        //public static readonly DependencyProperty SelectedColumnIndexProperty =
        //    DependencyProperty.Register("SelectedColumnIndex", typeof(int), typeof(ScheduleGrid),
        //    new UIPropertyMetadata(-1, (d, e) =>
        //    {
        //        var scheduleGrid = d as ScheduleGrid;
        //        var oldIndex = (int)e.OldValue;
        //        var newIndex = (int)e.NewValue;
        //        if (newIndex < 0 || newIndex == oldIndex)
        //            return;

        //        if (newIndex < oldIndex)
        //        {
        //            var newMinX = newIndex * scheduleGrid.RowWidth;
        //            if (newMinX < scheduleGrid.ViewportRangeX.ViewMin)
        //                scheduleGrid.LineLeft();
        //        }
        //        else
        //        {
        //            var newMaxX = (newIndex + 1) * scheduleGrid.RowWidth;
        //            if (newMaxX > scheduleGrid.ViewportRangeX.ViewMin + scheduleGrid.ViewportWidth)
        //                scheduleGrid.LineRight();
        //        }
        //    }));
    }
}