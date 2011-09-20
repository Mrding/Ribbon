using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Luna.Common;

namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    public class ScaleSelectedItemLayer : BlockGridLayerBase
    {
        private const int ApprochDistance = 3;

        private Point _startPoint, _lastPoint;
        private double _stepMinDistance;
        private MouseState _mouseFlag;

        public override void Initialize()
        {
            base.Initialize();
            ElementDraws.Add(new ScaleSelectedItemLayerDraw());
        }

        internal bool InvalidPlacement { get; set; }

        public TimeSpan StepMin
        {
            get { return (TimeSpan)GetValue(StepMinProperty); }
            set { SetValue(StepMinProperty, value); }
        }

        public static readonly DependencyProperty StepMinProperty =
            DependencyProperty.Register("StepMin", typeof(TimeSpan), typeof(ScaleSelectedItemLayer),
            new UIPropertyMetadata(TimeSpan.FromMinutes(5)));

        protected override void OnParentMouseDown(object sender, MouseButtonEventArgs e)
        {
            //xif (LayerContainer.TryPrevent()) return;
            if (e.LeftButton != MouseButtonState.Pressed) return;

            _startPoint = e.GetPosition(this);
            _lastPoint = _startPoint;
            FlagAdd(MouseState.MouseDown);
            var date = AxisXConverter.ScreenToData(0);
            var newDate = date.Add(StepMin);
            _stepMinDistance = AxisXConverter.DataToScreen(newDate);

            if (FlagExist(MouseState.LeftDirection) || FlagExist(MouseState.RightDirection))
            {
                e.Handled = true;
                InvalidPlacement = true; // 注意 首次按下鼠标时先设定没任何拖动之前都不是有效的位置改变

                StartMouseMovementTracking();
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
            //xif (LayerContainer.TryPrevent()) return;

            var point = e.GetPosition(this);
            if (!FlagExist(MouseState.MouseDown))
                MoveStateCheck(point); // 尝试变更鼠标样式
            else
            {
                var moveDirection = MouseState.MouseOver;
                if (FlagExist(MouseState.LeftDirection))
                    moveDirection = MouseState.LeftDirection;
                if (FlagExist(MouseState.RightDirection))
                    moveDirection = MouseState.RightDirection;

                //未发现任何左右拉动现象
                if (moveDirection == MouseState.MouseOver) return;

                //var step = AxisXConverter.ScreenToData(distance) - AxisPanel.ScreenStart;
                //if (!(PointOutBlock.End >= PointOutBlock.Start.AddMinutes(step.TotalMinutes))
                //    || !(PointOutBlock.End.AddMinutes(step.TotalMinutes) >= PointOutBlock.Start))
                //    return;

                var distance = point.X - _startPoint.X;
                _lastPoint = new Point(point.X - distance % _stepMinDistance, point.Y); // 刻度式移动,根据 stepMinDistance(通常为5)步进

                var moveSpan = AxisXConverter.ScreenToData(GetMoveOffSet()).Subtract(AxisXConverter.ScreenToData(0));

                var draggingPlacement = default(TimeRange);

              
                switch (moveDirection)
                {
                    case MouseState.LeftDirection:
                        draggingPlacement = PointOutBlock == null ? default(TimeRange) : new TimeRange(PointOutBlock.Start.Add(moveSpan).AddSeconds(0.1), PointOutBlock.End);
                        break;
                    case MouseState.RightDirection:
                        draggingPlacement = PointOutBlock == null ? default(TimeRange) : new TimeRange(PointOutBlock.Start, PointOutBlock.End.Add(moveSpan).AddSeconds(0.1));
                        break;
                }

                InvalidPlacement = draggingPlacement == default(TimeRange) || draggingPlacement.Invalid; // 检查Start, End数据正确性(通常Start不能大于End)

                if (InvalidPlacement)
                {
                    DropedPlacement = null;
                    return;
                }

                DropedPlacement = draggingPlacement;
                e.Handled = true;
                InvalidateVisual();
            }
        }

        protected override void OnParentMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (PointOutBlock != null && IsMouseDragging())
            {
                if (!BlockConverter.CanConvert(PointOutBlock)) return;

                //Henry modified
                //InvalidateVisual(); // do not change the order, 通过重绘改变 DropedPlacement

                if (!InvalidPlacement && DropedPlacement != null)
                    LayerContainer.OnRaiseAfterMouseUpEvent(this);
            }
            EndOperation();
        }

        protected override void OnParentKeyEscPress(object sender, KeyEventArgs e)
        {
            if (IsMouseDragging())
            {
                EndOperation();
                e.Handled = true;
            }
        }

        private void EndOperation()
        {
            Mouse.OverrideCursor = null;
            FlagDelete(MouseState.LeftDirection);
            FlagDelete(MouseState.RightDirection);
            FlagDelete(MouseState.MouseDown);

            _lastPoint = _startPoint; // 让起点点和结束点一致, 可认视没发生任何拖动
            InvalidPlacement = true;
            DropedPlacement = null;
            InvalidateVisual();
        }

        private void MoveStateCheck(Point point)
        {
            FlagDelete(MouseState.LeftDirection);
            FlagDelete(MouseState.RightDirection);

            if (CanScale())
                ChangeBlockEdgeCursor(point);
        }

        private bool CanScale()
        {
            if (PointOutBlock == null) return false;

            return BlockConverter.CanConvert(PointOutBlock) && !BlockConverter.GetLocked(PointOutBlock);
        }

        private void ChangeBlockEdgeCursor(Point point)
        {
            var left = AxisXConverter.DataToScreen(PointOutBlock.Start);
            var right = AxisXConverter.DataToScreen(PointOutBlock.End);

            //如果在边上,鼠标形状就发生变化
            if ((point.X - left) < ApprochDistance)
            {
                LayerContainer.ChangeCursor(Cursors.SizeWE);
                FlagAdd(MouseState.LeftDirection);
            }
            else if ((right - point.X) < ApprochDistance)
            {
                LayerContainer.ChangeCursor(Cursors.SizeWE);
                FlagAdd(MouseState.RightDirection);
            }
        }

        internal void FlagDelete(MouseState flag)
        {
            _mouseFlag = _mouseFlag & ~flag;
        }

        private void FlagAdd(MouseState flag)
        {
            _mouseFlag = _mouseFlag | flag;
        }

        internal bool FlagExist(MouseState flag)
        {
            return (_mouseFlag & flag) == flag;
        }

        private double GetMoveOffSet()
        {
            return _lastPoint.X - _startPoint.X;
        }

        public override bool IsMouseDragging()
        {
            return _startPoint != _lastPoint;
        }

        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            return new RectangleGeometry(new Rect(0, LayoutClipY, layoutSlotSize.Width, layoutSlotSize.Height - LayoutClipY));
        }
    }
}
