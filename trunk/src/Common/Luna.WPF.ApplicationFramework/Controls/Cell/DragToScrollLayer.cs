using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.PresentationFramework;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public class DragToScrollLayer : BlockGridLayerBase
    {
        private Cursor _grabCursor, _grabbingCursor;

        private Point _mouseDragStartPoint;
        private Point _scrollStartOffset;
        private bool _spaceKeyPressed;


        public DragToScrollLayer()
        {
            if (PresentationFrameworkModule.IsInDesignMode) return;   
            _grabCursor = ((TextBlock)Application.Current.Resources["CursorGrab"]).Cursor;
            _grabbingCursor = ((TextBlock)Application.Current.Resources["CursorGrabbing"]).Cursor;
        }


        protected override void AddToPanel(IAxisPanel axisPanel)
        {
            axisPanel.Add(this);
        }

        protected override void OnParentKeyDown(object sender, KeyEventArgs e)
        {
            _spaceKeyPressed = e.Key == Key.Space;
            e.Handled = _spaceKeyPressed;

            if (_spaceKeyPressed && !LayerContainer.IsMouseCaptured)
            {
                LayerContainer.PreventMouseEvent(this);
                Cursor = _grabCursor;
                Mouse.Capture(LayerContainer, CaptureMode.SubTree);
            }
        }

        protected override void OnParentKeyUp(object sender, KeyEventArgs e)
        {
            if (_spaceKeyPressed)
            {
                LayerContainer.PreventMouseEvent(null);
                _spaceKeyPressed = false;
                Cursor = Cursors.Arrow;
                Mouse.OverrideCursor = Cursors.Arrow;
                ReleaseMouseCapture();
            }
        }

        protected override void OnParentMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_spaceKeyPressed)
                return;

            _mouseDragStartPoint = e.GetPosition(this);
            _scrollStartOffset.X = AxisPanel.HorizontalOffset;
            _scrollStartOffset.Y = AxisPanel.VerticalOffset;

            // Update the cursor if scrolling is possible 
            //this.Cursor = (AxisPanel.ExtentWidth > AxisPanel.ViewportWidth) || (AxisPanel.ExtentHeight > AxisPanel.ViewportHeight) ?
            //                Cursors.ScrollAll : Cursors.Arrow;

            Mouse.OverrideCursor = _grabbingCursor;

            e.Handled = true;
        }

        protected override void OnParentMouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = _spaceKeyPressed;

            if (LayerContainer.IsMouseCaptured && _spaceKeyPressed && e.LeftButton == MouseButtonState.Pressed)
            {
                // Get the new mouse position. 
                var mouseDragCurrentPoint = e.GetPosition(this);

                // Determine the new amount to scroll. 
                var delta = new Point(
                    (mouseDragCurrentPoint.X > _mouseDragStartPoint.X) ? -(mouseDragCurrentPoint.X - _mouseDragStartPoint.X) : (_mouseDragStartPoint.X - mouseDragCurrentPoint.X),
                    (mouseDragCurrentPoint.Y > _mouseDragStartPoint.Y) ?
                                                                           -(mouseDragCurrentPoint.Y - _mouseDragStartPoint.Y) :
                                                                                                                                   (_mouseDragStartPoint.Y - mouseDragCurrentPoint.Y));

                // Scroll to the new position. 
                AxisPanel.ScrollToHorizontalOffset(_scrollStartOffset.X + delta.X);
            }
        }

        protected override void OnParentMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_spaceKeyPressed)
                Mouse.OverrideCursor = _grabCursor;
        }

        protected override void OnDispose()
        {
            _grabCursor = null;
            _grabbingCursor = null;

            base.OnDispose();
        }
    }
}