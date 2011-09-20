using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Luna.Core.Extensions;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public abstract partial class AxisPanel
    {
        public bool CanHorizontallyScroll { get; set; }

        public bool CanVerticallyScroll { get; set; }

        double IScrollInfo.ExtentHeight
        {
            get
            {
                return _viewportRangeY == null ? RenderSize.Height : _viewportRangeY.Max;
            }
        }

        double IScrollInfo.ExtentWidth
        {
            get { return ViewportRangeX.Max; }
        }

        double IScrollInfo.HorizontalOffset
        {
            get { return ViewportRangeX.ViewMin; }
        }

        void IScrollInfo.LineDown()
        {
            SetVerticalOffset(VerticalOffset + VerticalOffSetValue);
        }

        void IScrollInfo.LineLeft()
        {
            SetHorizontalOffset(HorizontalOffset - HorizontalOffSetValue);
        }

        void IScrollInfo.LineRight()
        {
            SetHorizontalOffset(HorizontalOffset + HorizontalOffSetValue);
        }

        void IScrollInfo.LineUp()
        {
            SetVerticalOffset(VerticalOffset - VerticalOffSetValue);
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            return rectangle;
        }

        public void MouseWheelDown()
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
                LineDown();
        }

        public void MouseWheelLeft()
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
                LineLeft();
        }

        public void MouseWheelRight()
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
                LineRight();
        }

        public void MouseWheelUp()
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
                LineUp();
        }

        void IScrollInfo.PageDown()
        {
            SetVerticalOffset(VerticalOffset + ViewportHeight);
        }

        void IScrollInfo.PageLeft()
        {
            SetHorizontalOffset(HorizontalOffset - ViewportWidth);
        }

        void IScrollInfo.PageRight()
        {
            SetHorizontalOffset(HorizontalOffset + ViewportWidth);
        }

        void IScrollInfo.PageUp()
        {
            SetVerticalOffset(VerticalOffset - ViewportHeight);
        }

        public abstract void SetHorizontalOffset(double offset);

        public abstract void SetVerticalOffset(double offset);

        public ScrollViewer ScrollOwner { get; set; }

        protected double _remainsOfVerticalOffset;

        double IScrollInfo.VerticalOffset
        {
            get
            {
                //if (GetViewportHeight() % VerticalOffSetValue != 0  )
                //    return Math.Max(0, ExtentHeight - GetViewportHeight());
                return ViewportRangeY.ViewMin + _remainsOfVerticalOffset;
            }
        }

        double IScrollInfo.ViewportHeight
        {
            get { return GetViewportHeight(); }
        }

        double IScrollInfo.ViewportWidth
        {
            get { return GetViewportWidth(); }
        }

      
      

        protected abstract double GetViewportWidth();

        protected abstract double GetViewportHeight();


    }
}