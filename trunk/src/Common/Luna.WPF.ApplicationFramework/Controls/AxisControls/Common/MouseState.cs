using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luna.WPF.ApplicationFramework.Controls
{
    [Flags]
    public enum MouseState
    {
        MouseOver = 0x01,
        MouseDown = 0x02,
        MouseUp = 0x04,
        RightDirection = 0x08,
        LeftDirection = 0x10
    }
}
