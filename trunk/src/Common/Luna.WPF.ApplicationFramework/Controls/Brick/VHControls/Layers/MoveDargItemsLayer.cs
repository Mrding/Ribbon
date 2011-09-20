using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Luna.Common;
using Luna.Core.Extensions;

namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    public class MoveDargItemsLayer : MoveSelectedItemsLayer
    {
        static MoveDargItemsLayer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MoveDargItemsLayer), new FrameworkPropertyMetadata(typeof(MoveDargItemsLayer)));
        }

        #region Attached Event 'Dragging'

        public static readonly RoutedEvent DraggingEvent = EventManager.RegisterRoutedEvent("Dragging", RoutingStrategy.Tunnel, typeof(DraggingRoutedEventHandler), typeof(MoveDargItemsLayer));

        public static void AddDraggingHandler(DependencyObject d, DraggingRoutedEventHandler handler)
        {
            d.SaftyInvoke<UIElement>(uie => uie.AddHandler(DraggingEvent, handler));
        }

        public static void RemoveDraggingHandler(DependencyObject d, DraggingRoutedEventHandler handler)
        {
            d.SaftyInvoke<UIElement>(uie => uie.RemoveHandler(DraggingEvent, handler));
        }

        #endregion

        internal override double GetPointOutY()
        {
            if (!AxisYConverter.IsInViewRagne(PointOutDataRowIndex))
            {
                if (PointOutDataRowIndex < 0)
                    PointOutDataRowIndex = 0;
                else
                    PointOutDataRowIndex = ItemsSource.Count - 1;
            }
            var rowY = AxisYConverter.DataToScreen(PointOutDataRowIndex);
            var y = rowY + BlockConverter.GetTop(PointOutBlock);
            return y;
        }

        protected override void OnParentMouseDown(object sender, MouseButtonEventArgs e)
        {
            //base.OnParentMouseDown(sender, e);
        }

        protected override void OnParentDragOver(DragEventArgs e)
        {
            
            _isDragLeave = false;

            var point = e.GetPosition(this);

            if (_startPoint == null || _startPoint.Value.X < 0)
            {
                var date = AxisXConverter.ScreenToData(0);
                var newDate = date.Add(StepMin);
                _stepMinDistance = AxisXConverter.DataToScreen(newDate);

                var pointTime = AxisXConverter.ScreenToData(point.X - point.X % _stepMinDistance).ConvertToMultiplesOfFive();
                System.Diagnostics.Debug.Print(pointTime.ToString());
                var arg = default(DragingRoutedEventArgs);

                var data = e.Data.GetData("Castle.Proxies.TermStyleProxy");

                if (data != null)
                {
                    arg = new DragingRoutedEventArgs(pointTime, data, DraggingEvent, this);
                    RaiseEvent(arg);
                }

                if (arg != null && arg.Target != null)
                {
                    _startPoint = point;
                    _lastPoint = _startPoint;
                }
                else
                {
                    _startPoint = null;
                    _lastPoint = null;
                }

                PointOutBlock = arg == null ? null : arg.Target;
            }

            _canMove = PointOutBlock != null;

            if (_canMove)
            {
                MouseMoving(point);
                InvalidateVisual();
            }
        }

        //protected override void OnParentKeyEscPress(object sender, KeyEventArgs e)
        //{
            
        //}

        protected override void OnParentDrop(DragEventArgs e)
        {
            _isDragLeave = true;
            _canMove = false;
            _isMoving = false;
            _startPoint = null;
            _lastPoint = null;


            e.Data.SetData("PointOutBlock", PointOutBlock);
            e.Data.SetData("DropedPlacement", DropedPlacement);

            InvalidateVisual();
        }

        private bool _isDragLeave;

        protected override void OnParentDragLeave(DragEventArgs e)
        {
            Debug.Print("OnParentDragLeave");
            _startPoint = null;
            _isDragLeave = true;

            InvalidateVisual();
        }

        internal bool IsDragLeave()
        {
            return _isDragLeave;
        }

        public override bool IsMouseDragging()
        {
            return _canMove && _isMoving && !_isDragLeave;
        }
    }
}