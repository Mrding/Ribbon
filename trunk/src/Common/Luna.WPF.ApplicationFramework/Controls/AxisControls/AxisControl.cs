using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Threading;
using Caliburn.PresentationFramework;
using Luna.WPF.ApplicationFramework.Behaviors;
using Luna.WPF.ApplicationFramework.Collections;
using Luna.WPF.ApplicationFramework.Extensions;
using System.Diagnostics;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public static class AxisControlExt
    {
        public static bool AxisOutOfRange(int dataRowIndex, int y0, int yMax)
        {
            var yPostion = dataRowIndex - y0;  // item.Index just like the row index
            return yPostion < 0 || dataRowIndex > yMax;
        }

        public static bool AxisOutOfRange(int rowIndex, int viewMin, int viewMax, out int rowScreenIndex)
        {
            rowScreenIndex = rowIndex - viewMin;  // item.Index just like the row index
            return rowScreenIndex < 0 || rowIndex > viewMax;
        }
    }

    public abstract class AxisControl : FrameworkElement, IInitialize, IDisposable
    {
        static AxisControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AxisControl), new FrameworkPropertyMetadata(typeof(AxisControl)));
            //FocusableProperty.OverrideMetadata(typeof(AxisControl), new UIPropertyMetadata(false));
            FocusVisualStyleProperty.OverrideMetadata(typeof(AxisControl), new FrameworkPropertyMetadata(null));
        }

        private bool _disposed;

        protected AxisControl()
        {
            Interaction.GetBehaviors(this).Add(new InitializeBehavior());
        }

        public virtual void Initialize()// 只会執行一次
        {
            while (AxisPanel == null)
            {
                var axisPanel = GetParentPanelType().FindAncestor(this) as AxisPanel;

                if (axisPanel == null) continue;

                AxisPanel = axisPanel;
            }

            AddToPanel(AxisPanel);
            InvalidateVisual();
            OnLoaded();
        }

        protected virtual void OnLoaded() // 只会執行一次, 设计时可呈现使用
        {
            if (PresentationFrameworkModule.IsInDesignMode)
                ElementDraws.Add(new AxisControlDesignDraw());
        }

        public AxisPanel AxisPanel { get; set; }

        protected virtual void AddToPanel(IAxisPanel axisPanel) { }

        protected virtual Type GetParentPanelType()
        {
            return typeof(AxisPanel);
        }


        protected DispatcherTimer _mouseTrackingTimer;

        protected void StartMouseMovementTracking()
        {
            if (_mouseTrackingTimer == null)
                _mouseTrackingTimer = new DispatcherTimer { Interval = new TimeSpan(500) };

            _mouseTrackingTimer.Tick += TrackingMouseMovement;
            _mouseTrackingTimer.Start();
        }

        protected void EndMouseMovementTracking()
        {
            _mouseTrackingTimer.Stop();
            _mouseTrackingTimer.Tick -= TrackingMouseMovement;
        }

        private void TrackingMouseMovement(object sender, EventArgs e)
        {
            var point = Mouse.GetPosition(this);
            if (point.X < 0 || point.Y < 0 || point.X > ActualWidth || point.Y > ActualHeight)
                OnMouseLeavContainer();
        }

        protected virtual void OnMouseLeavContainer() { }

        //protected override Geometry GetLayoutClip(Size layoutSlotSize)
        //{
        //    return new RectangleGeometry(new Rect(RenderSize));
        //}

        //protected override Geometry GetLayoutClip(Size layoutSlotSize)
        //{
        //    return base.GetLayoutClip(layoutSlotSize);
        //}

        internal IHorizontalControl AxisXConverter { get { return (IHorizontalControl)AxisPanel; } }

        internal IVerticalControl AxisYConverter { get { return (IVerticalControl)AxisPanel; } }

        //protected override Size MeasureOverride(Size availableSize)
        //{
        //    if (double.IsInfinity(availableSize.Height) || double.IsInfinity(availableSize.Width))
        //        return new Size(200, 200);
        //    return availableSize;
        //}

        protected ElementDrawCollection ElementDraws
        {
            get { return ElementDrawBehavior.GetElementDraws(this); }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (AxisPanel != null)
            {
                //Debug.Print(string.Format("{0} {1}", GetHashCode(), GetType().FullName));
                ElementDraws.Draw(drawingContext);
            }
        }

        protected virtual void OnDispose() { }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Release nmanaged resources
                OnDispose();
                //this.ClearAllLocalValue();
                AxisPanel = null;
            }

            // Release unmanaged resources
            _disposed = true;
        }

        ~AxisControl()
        {
            Dispose(false);
        }
    }
}