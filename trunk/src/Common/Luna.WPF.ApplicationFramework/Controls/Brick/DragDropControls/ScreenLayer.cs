using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Interop;
using System.Security;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Windows.Input;

namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    [Localizability(LocalizationCategory.None), DefaultProperty("Child"), ContentProperty("Child")]
    public class ScreenLayer : Control
    {
        const int WS_EX_TRANSPARENT = 0x00000020;
        const int GWL_EXSTYLE = -20;

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);
        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool ShowWindow(HandleRef hWnd, int nCmdShow);
        private HwndSource _hwndSource;

        private Window _transparentWindow;
        DispatcherTimer _asyncDestroy;


        static ScreenLayer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScreenLayer), new FrameworkPropertyMetadata(typeof(ScreenLayer)));
        }

        public ScreenLayer()
        {
            //CreateWindow();
        }

        private void CreateWindow()
        {
            _transparentWindow = new Window();
            _transparentWindow.WindowState = WindowState.Maximized;
            _transparentWindow.Topmost = true;
            _transparentWindow.WindowStyle = WindowStyle.None;
            _transparentWindow.AllowsTransparency = true;
            _transparentWindow.AllowDrop = false;
            _transparentWindow.IsHitTestVisible = false;
            _transparentWindow.Focusable = false;
            _transparentWindow.ShowInTaskbar = false;
            _transparentWindow.Background = null;
            _transparentWindow.ResizeMode = ResizeMode.NoResize;

            //_transparentWindow.Dispatcher.BeginInvoke((Action)delegate
            //{
            //    _transparentWindow.Owner = Application.Current.MainWindow;
            //}, System.Windows.Threading.DispatcherPriority.Loaded);

            _transparentWindow.SourceInitialized += delegate
            {
                //为了能够穿透窗体
                //int WS_EX_NOACTIVATE = 0x08000000;
                var hwnd = new WindowInteropHelper(_transparentWindow).Handle;
                int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
                SetWindowLong(hwnd, GWL_EXSTYLE, 0x80000 | 0x20 | 0x00000008 | 0x08000000);

                int style = GetWindowLong(hwnd, -16);
                SetWindowLong(hwnd, -16, 0x4000000);
            };
        }

        public void Show()
        {
            this.UpdateLayout();
            if (_asyncDestroy != null)
            {
                _asyncDestroy.Stop();
                _asyncDestroy = null;
            }
            if (_hwndSource == null || _hwndSource.IsDisposed)
            {
                _hwndSource = CreateBackgroundWindow();
            }
            ShowWindow(new HandleRef(null, _hwndSource.Handle), 8);
        }

        public void Hide()
        {
            Mouse.Capture(null);
            IntervalLength = 0;
            if (_asyncDestroy == null)
            {
                this._asyncDestroy = new DispatcherTimer(DispatcherPriority.Input);
                this._asyncDestroy.Tick += delegate(object sender, EventArgs args)
                {
                    this._asyncDestroy.Stop();
                    this._asyncDestroy = null;
                    if (_hwndSource != null && !_hwndSource.IsDisposed)
                    {
                        _hwndSource.RootVisual = null;
                        _hwndSource.Dispose();
                        _hwndSource = null;
                    }
                };
                this._asyncDestroy.Interval = TimeSpan.FromMilliseconds(300);
                this._asyncDestroy.Start();
            }
            ShowWindow(new HandleRef(null, _hwndSource.Handle), 0);
        }

        private HwndSource CreateBackgroundWindow()
        {
            HwndSource presentationSource = PresentationSource.FromVisual(Application.Current.MainWindow) as HwndSource;
            IntPtr zero = IntPtr.Zero;
            if (presentationSource != null)
            {
                zero = presentationSource.Handle;
            }
            //WS_EX_TOPMOST   = 0x00000008,WS_CHILDWINDOW   = 0x40000000, WS_EX_NOACTIVATE  = 0x08000000,
            //WS_EX_LAYERED   = 0x00080000,WS_EX_TRANSPARENT  = 0x00000020,
            int num = 0;
            int num2 = 0x4000000;
            int num3 = 0x80000 | 0x20 | 0x00000008 | 0x08000000; //0x8000080;WS_EX_TRANSPARENT | WS_EX_LAYERED

            HwndSourceParameters parameters = new HwndSourceParameters()
            {
                WindowClassStyle = num,
                WindowStyle = num2,
                ExtendedWindowStyle = num3
            };
            parameters.SetPosition(0, 0);
            parameters.UsesPerPixelOpacity = true; //transparent;

            if ((zero != IntPtr.Zero))
            {
                //parameters.ParentWindow = zero;
            }

            HwndSource source2 = new HwndSource(parameters);
            source2.RootVisual = this;
            return source2;
        }

        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }


        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(ScreenLayer), new UIPropertyMetadata(0.0));



        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(ScreenLayer), new UIPropertyMetadata(0.0));


        public UIElement Target
        {
            get { return (UIElement)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(UIElement), typeof(ScreenLayer),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure,
            (d, e) =>
            {
                var screenLayer = (ScreenLayer)d;

                var newElement = e.NewValue as UIElement;
                if (newElement != null)
                    screenLayer.AddVisualChild(newElement);

                var oldElement = e.OldValue as UIElement;
                if (oldElement != null)
                    screenLayer.RemoveVisualChild(oldElement);
            }));

        public Point OriginalPoint
        {
            get { return (Point)GetValue(OriginalPointProperty); }
            set { SetValue(OriginalPointProperty, value); }
        }

        public static readonly DependencyProperty OriginalPointProperty =
            DependencyProperty.Register("OriginalPoint", typeof(Point), typeof(ScreenLayer),
            new FrameworkPropertyMetadata(new Point(), FrameworkPropertyMetadataOptions.AffectsArrange));


        private static double GetAngleBetween(double x, double y)
        {
            double temp = Math.Atan2(y, x) * 180 / Math.PI;
            if (temp == 0 && x < 0)
                return 180;
            return temp;
        }

        public Point CurrentPoint
        {
            get { return (Point)GetValue(CurrentPointProperty); }
            set { SetValue(CurrentPointProperty, value); }
        }

        public static readonly DependencyProperty CurrentPointProperty =
        DependencyProperty.Register("CurrentPoint", typeof(Point), typeof(ScreenLayer),
        new FrameworkPropertyMetadata(new Point(),
        FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) =>
        {
            var screenLayer = (ScreenLayer)d;
            var currentPoint = (Point)e.NewValue;
            double x = currentPoint.X;
            double y = currentPoint.Y;
            x += screenLayer.HorizontalOffset;
            y += screenLayer.VerticalOffset;

            double intervalLength = Math.Sqrt((x - screenLayer.OriginalPoint.X) * (x - screenLayer.OriginalPoint.X) + (y - screenLayer.OriginalPoint.Y) * (y - screenLayer.OriginalPoint.Y));

            ////Debug.WriteLine(string.Format("org:({0},{1}) current:({2},{3}),Length:{4}",OriginalPoint.X,OriginalPoint.Y, x, y,intervalLength));

            double angle = GetAngleBetween(x - screenLayer.OriginalPoint.X, y - screenLayer.OriginalPoint.Y);

            screenLayer.IntervalLength = intervalLength;
            screenLayer.Angle = angle;
        }));

        public double IntervalLength
        {
            get { return (double)GetValue(IntervalLengthProperty); }
            private set { SetValue(IntervalLengthPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey IntervalLengthPropertyKey =
            DependencyProperty.RegisterReadOnly("IntervalLength", typeof(double), typeof(ScreenLayer),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsArrange, (d, e) =>
            {
                var screenLayer = (ScreenLayer)d;
                var intervalLength = (double)e.NewValue;

                if (intervalLength < Math.Min(screenLayer.Target.DesiredSize.Width, screenLayer.Target.DesiredSize.Height))
                {
                    screenLayer.Visibility = Visibility.Hidden;
                }
                else if (intervalLength > Math.Min(screenLayer.Target.DesiredSize.Width, screenLayer.Target.DesiredSize.Height))
                {
                    screenLayer.Visibility = Visibility.Visible;
                }
            }));
        public static readonly DependencyProperty IntervalLengthProperty = IntervalLengthPropertyKey.DependencyProperty;


        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            private set { SetValue(AnglePropertyKey, value); }
        }

        public static readonly DependencyPropertyKey AnglePropertyKey =
            DependencyProperty.RegisterReadOnly("Angle", typeof(double), typeof(ScreenLayer), new FrameworkPropertyMetadata(0.0));
        public static readonly DependencyProperty AngleProperty = AnglePropertyKey.DependencyProperty;

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            if (Target != null)
            {
                if (IntervalLength == 0)
                    Target.Arrange(new Rect(CurrentPoint, new Size()));
                else
                    Target.Arrange(new Rect(CurrentPoint, Target.DesiredSize));

                //Console.WriteLine(CurrentPoint.X.ToString() + "," + CurrentPoint.Y.ToString());
            }


            if (this.VisualChildrenCount > 1)
            {
                UIElement visualChild = (UIElement)this.GetVisualChild(1);
                if (visualChild != null)
                {
                    //把子元素的开始坐标放到鼠标放下的中点
                    Point centerPoint = OriginalPoint;
                    centerPoint.Y -= visualChild.DesiredSize.Height / 2;

                    visualChild.Arrange(new Rect(centerPoint, visualChild.DesiredSize));
                }
            }
            return arrangeBounds;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (Target != null)
                Target.Measure(constraint);
            if (this.VisualChildrenCount > 1)
            {
                UIElement visualChild = (UIElement)this.GetVisualChild(1);
                if (visualChild != null)
                {
                    visualChild.Measure(constraint);
                }
            }

            //和底部窗口一样的大小，其实就是屏幕大小
            //return new Size(_transparentWindow.ActualWidth, _transparentWindow.ActualHeight);         
            return new Size(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);
        }

        protected override int VisualChildrenCount
        {
            get
            {
                if (Target != null)
                    return base.VisualChildrenCount + 1;
                return base.VisualChildrenCount;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index == 0)
                return Target;
            return base.GetVisualChild(index - 1);
        }

    }
}
