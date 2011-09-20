using System.Windows;
using System.Windows.Controls;

namespace Luna.GUI.Views.Shifts
{
    public class ShiftView : UserControl
    {
        public GridLength HeaderWidth
        {
            get { return (GridLength)GetValue(HeaderWidthProperty); }
            set { SetValue(HeaderWidthProperty, value); }
        }

        public static readonly DependencyProperty HeaderWidthProperty =
            DependencyProperty.Register("HeaderWidth", typeof(GridLength), typeof(ShiftView),
                                        new UIPropertyMetadata(new GridLength(200, GridUnitType.Pixel)));


        public GridLength TopRowHeight
        {
            get { return (GridLength)GetValue(TopRowHeightProperty); }
            set { SetValue(TopRowHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TopRowHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopRowHeightProperty =
            DependencyProperty.Register("TopRowHeight", typeof(GridLength), typeof(ShiftView), new UIPropertyMetadata(new GridLength(0, GridUnitType.Pixel)));



        public Visibility TimeLineVisibility
        {
            get { return (Visibility)GetValue(TimeLineVisibilityProperty); }
            set { SetValue(TimeLineVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TimeLineVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimeLineVisibilityProperty =
            DependencyProperty.Register("TimeLineVisibility", typeof(Visibility), typeof(ShiftView), new UIPropertyMetadata(Visibility.Collapsed));
    }
}