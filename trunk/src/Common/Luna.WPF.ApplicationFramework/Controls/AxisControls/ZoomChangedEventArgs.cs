using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public delegate void ZoomChangedHandler(Point pos, int delta);

    public enum ZoomOperation
    {
        Mouse,
        Hand
    }

    public class ZoomChangedEventArgs : EventArgs
    {
        public Point Pos { get; set; }

        public int Delta { get; set; }

        public double ZoomValue { get; set; }

        public ZoomOperation Operation { get; set; }
    }
}
