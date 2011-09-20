using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Luna.Common;

namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    public delegate void DraggingRoutedEventHandler(object sender, DragingRoutedEventArgs e);

    public class MoveSelectedItemsLayer : BlockGridLayerBase
    {
        protected bool _canMove;
        protected bool _isMoving;
        protected Point? _startPoint;
        protected Point? _lastPoint;
        protected double _stepMinDistance;
      
        protected override void OnParentMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            _startPoint = e.GetPosition(this);
            _lastPoint = _startPoint;

            if (!BlockConverter.CanConvert(PointOutBlock)) return;

            _canMove = PointOutBlock != null && !BlockConverter.GetLocked(PointOutBlock);

            if (_canMove)
            {
                StartMouseMovementTracking();
                var date = AxisXConverter.ScreenToData(0);
                var newDate = date.Add(StepMin);
                _stepMinDistance = AxisXConverter.DataToScreen(newDate);
            }
        }

        protected override void OnMouseLeavContainer()
        {
            if (Mouse.LeftButton != MouseButtonState.Released) return;

            EndOperation();
            EndMouseMovementTracking();
        }

        protected override void OnParentMouseMove(object sender, MouseEventArgs e)
        {
            if (_canMove && e.LeftButton == MouseButtonState.Pressed && PointOutBlock != null) //Henry modified: e.LeftButton == MouseButtonState.Pressed
            {
                MouseMoving(e.GetPosition(this));
                InvalidateVisual();
                e.Handled = true;

                LayerContainer.Focus();
            }
        }

        protected void MouseMoving(Point point)
        {
            //if(_startPoint == null || _lastPoint == null)
            //{
            //    return;
            //}


            var distance = point.X - (_startPoint == null ? 0 : _startPoint.Value.X);
            _lastPoint = new Point(point.X - distance % _stepMinDistance, point.Y); // do not modify 每格移动，减去不到标准距离的值

            //Henry modified
            MoveSpan = AxisXConverter.ScreenToData(GetMoveOffSet()).Subtract(AxisXConverter.ScreenToData(0)); // was => NotifyBlocknewPositionInfoList();
            DropedPlacement = new TimeRange(PointOutBlock.Start.Add(MoveSpan).AddSeconds(0.1), PointOutBlock.End.Add(MoveSpan).AddSeconds(0.1));
            _isMoving = true;

        }

        protected override void OnParentMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_canMove)
            {
                if (PointOutBlock != null && !Equals(GetMoveOffSet(), 0.0d))
                {
                    if (!BlockConverter.CanConvert(PointOutBlock)) return;

                    //Henry modified
                    LayerContainer.OnRaiseAfterMouseUpEvent(this); // presenter is called
                    //this.FindAncestor<BlockGrid>().InvalidateVisual(); // redraw blockgird only
                }
            }
            EndOperation();
        }

        protected override void OnParentKeyEscPress(object sender, KeyEventArgs e)
        {
            if (_isMoving)
            {
                EndOperation();
                e.Handled = true;
            }
        }

        protected virtual void EndOperation()
        {
            _canMove = false;
            _isMoving = false;
            InvalidateVisual();
        }

        public TimeSpan StepMin
        {
            get { return (TimeSpan)GetValue(StepMinProperty); }
            set { SetValue(StepMinProperty, value); }
        }

        public static readonly DependencyProperty StepMinProperty =
            DependencyProperty.Register("StepMin", typeof(TimeSpan), typeof(MoveSelectedItemsLayer),
            new UIPropertyMetadata(TimeSpan.FromMinutes(5)));

        protected double GetMoveOffSet()
        {
            if (_lastPoint == null || _startPoint == null) return 0.0d;

            return _lastPoint.Value.X - _startPoint.Value.X;
        }

        public override bool IsMouseDragging()
        {
            return _canMove && _isMoving;
        }

        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            return new RectangleGeometry(new Rect(0, LayoutClipY, layoutSlotSize.Width, layoutSlotSize.Height - LayoutClipY));
        }

        protected TimeSpan MoveSpan { get; private set; }
    }
}
