using System;
using System.Windows;
using System.Windows.Controls;
using Luna.WPF.ApplicationFramework.Extensions;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public static class ScrollViewerUtilities
    {
        private static ScrollViewer GeScrollViewer(DependencyObject obj)
        {
            if (obj == null) return null;
            var viewer = obj as ScrollViewer ?? obj.FindVisualChild<ScrollViewer>();
            return viewer;
        }

        public static readonly DependencyProperty ScrollSourceProperty =
           DependencyProperty.RegisterAttached("ScrollSource"
                                               , typeof(UIElement), typeof(ScrollViewerUtilities),
                                               new PropertyMetadata((d, e) =>
                                                                        {
                                                                            var currentViewer = GeScrollViewer(d);
                                                                            if (currentViewer == null)
                                                                                return;

                                                                            var sourceViewer = e.NewValue as ScheduleGrid;
                                                                            
                                                                            if (sourceViewer == null) 
                                                                                return;

                                                                            sourceViewer.ScrollChanged += (sender, arg) =>
                                                                                {
                                                                                    var scheduleGrid = sender as ScheduleGrid;

                                                                                    //if(arg.OriginalSource == currentViewer || arg.Source == currentViewer) 
                                                                                    //    return;

                                                                                    if (arg.OriginalSource == sender && arg.HorizontalChange != 0)
                                                                                    {
                                                                                        currentViewer.ScrollToHorizontalOffset(arg.HorizontalOffset);
                                                                                    }
                                                                                    if (arg.OriginalSource == sender && arg.VerticalChange != 0)
                                                                                    {
                                                                                        var offset = Math.Min(scheduleGrid.RowCount * scheduleGrid.RowHeight, scheduleGrid.ViewportRangeY.ViewMin);
                                                                                        currentViewer.ScrollToVerticalOffset(offset);
                                                                                    }
                                                                                };
                                                                        }));

        public static UIElement GetScrollSource(DependencyObject obj)
        {
            return (UIElement)obj.GetValue(ScrollSourceProperty);
        }

        public static void SetScrollSource(DependencyObject obj, UIElement value)
        {
            obj.SetValue(ScrollSourceProperty, value);
        }

        #region HorizontalOffset

        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.RegisterAttached("HorizontalOffset", typeof(double), typeof(ScrollViewerUtilities),
              new FrameworkPropertyMetadata(16d, new PropertyChangedCallback(OnHorizontalOffsetChanged)));

        public static double GetHorizontalOffset(DependencyObject d)
        {
            return (double)d.GetValue(HorizontalOffsetProperty);
        }

        public static void SetHorizontalOffset(DependencyObject d, double value)
        {
            d.SetValue(HorizontalOffsetProperty, value);
        }

        private static void OnHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //var viewer = (ScrollViewer)d;
            //viewer.ScrollToHorizontalOffset((double)e.NewValue);
        }

        public static double GetVerticalOffset(DependencyObject obj)
        {
            return (double)obj.GetValue(VerticalOffsetProperty);
        }

        public static void SetVerticalOffset(DependencyObject obj, double value)
        {
            obj.SetValue(VerticalOffsetProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.RegisterAttached("VerticalOffset", typeof(double), typeof(ScrollViewerUtilities),
            new UIPropertyMetadata(0d, OnVerticalOffsetChanged));

        private static void OnVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //var viewer = (ScrollViewer)d;
            //viewer.ScrollToVerticalOffset((double)e.NewValue);
        }

        #endregion
    }
}
