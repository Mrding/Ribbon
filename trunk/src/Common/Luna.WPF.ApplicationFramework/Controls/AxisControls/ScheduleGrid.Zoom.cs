using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public partial class ScheduleGrid
    {
        private int _zoomArrayIndex = 0;
        private double _wheelZoomSpeed = 1.2;
        private DateTime _zoomPointTime;

        public void IncreaseZoomCenter()
        {
            if (ZoomValue < 60)
                MouseWheelZoom(new Point(CenterPoint, 0), 120);
        }

        public void DecreaseZoomCenter()
        {
            if (ZoomValue > 1)
                MouseWheelZoom(new Point(CenterPoint, 0), -120);
        }

        public double CenterPoint
        {
            get { return this.DataToScreenX(this.ScreenEnd) / 2; }
        }

        public void MouseWheelZoom(Point mousePos, double wheelRotationDelta)
        {
            double oldZoomValue = ZoomValue;

            double zoomSpeed = 0;
            if (ZoomArray == null)
            {
                zoomSpeed = Math.Abs(wheelRotationDelta / Mouse.MouseWheelDeltaForOneLine);
                zoomSpeed *= _wheelZoomSpeed;
                if (wheelRotationDelta < 0)
                {
                    zoomSpeed = 1 / zoomSpeed;
                }
                ZoomX(mousePos.X, zoomSpeed);

            }
            else
            {
                if (wheelRotationDelta > 0)
                {
                    ++_zoomArrayIndex;
                    if (_zoomArrayIndex >= ZoomArray.Count)
                    {
                        _zoomArrayIndex = ZoomArray.Count - 1;
                    }
                }
                else
                {
                    --_zoomArrayIndex;
                    if (_zoomArrayIndex < 0)
                    {
                        _zoomArrayIndex = 0;
                    }
                }

                zoomSpeed = ZoomArray[_zoomArrayIndex];
                ZoomX(mousePos.X, 1 / ZoomValue, zoomSpeed);
            }

            ZoomValue = zoomSpeed;

            if (!ZoomWithMouseWheel)
            {
                if (ZoomChanged != null)
                {
                    int delta = -120;
                    if (ZoomValue > oldZoomValue)
                    {
                        delta = 120;
                    }
                    ZoomChanged(this, new ZoomChangedEventArgs() { Pos = mousePos, Delta = delta, ZoomValue = ZoomValue });
                }
            }

            //SetHorizontalOffset(ViewportRangeX.ViewMin);
            RefreshX();
            Refresh();
        }

        public bool ZoomWithMouseWheel { get; set; }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                ZoomWithMouseWheel = true;

                var zoomPoint = e.GetPosition(HorizontalMainElement);

                var x = zoomPoint.X;

                _zoomPointTime = ScreenToDataX(x, false);

                MouseWheelZoom(zoomPoint, e.Delta);
                if (ZoomChanged != null)
                    ZoomChanged(this, new ZoomChangedEventArgs() { Pos = zoomPoint, Delta = e.Delta, ZoomValue = ZoomValue });
                ZoomWithMouseWheel = false;
            }
        }

        public event EventHandler<ZoomChangedEventArgs> ZoomChanged;

        public DoubleCollection ZoomArray
        {
            get { return (DoubleCollection)GetValue(ZoomArrayProperty); }
            set { SetValue(ZoomArrayProperty, value); }
        }

        public static readonly DependencyProperty ZoomArrayProperty =
            DependencyProperty.Register("ZoomArray", typeof(DoubleCollection),
            typeof(ScheduleGrid), new UIPropertyMetadata(null));

        public double ZoomValue
        {
            get { return (double)GetValue(ZoomValueProperty); }
            set { SetValue(ZoomValueProperty, value); }
        }

        public event EventHandler<ZoomChangedEventArgs> ZoomValueChanged;

        private void OnZoomValueChanged()
        {
            if (ZoomValueChanged != null)
                ZoomValueChanged(this, new ZoomChangedEventArgs() { ZoomValue = ZoomValue });
        }

        public static readonly DependencyProperty ZoomValueProperty =
            DependencyProperty.Register("ZoomValue", typeof(double), typeof(ScheduleGrid),
            new UIPropertyMetadata(1.0, (o, a) =>
            {
                var scheduleGrid = (ScheduleGrid)o;
                scheduleGrid.OnZoomValueChanged();
            }));
    }
}