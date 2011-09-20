using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luna.NativeToolkit
{
    //ms-help://MS.MSDNQTR.v90.chs/winui/winui/windowsuserinterface/windowing/windows/windowreference/windowfunctions/showwindow.htm
    public class ShowWindowStyle
    {
        public static readonly int SW_HIDE = 0;
        public static readonly int SW_SHOWNORMAL = 1;
        public static readonly int SW_NORMAL = 1;
        public static readonly int SW_SHOWMINIMIZED = 2;
        public static readonly int SW_SHOWMAXIMIZED = 3;
        public static readonly int SW_MAXIMIZE = 3;
        public static readonly int SW_SHOWNOACTIVATE = 4;
        public static readonly int SW_SHOW = 5;
        public static readonly int SW_MINIMIZE = 6;
        public static readonly int SW_SHOWMINNOACTIVE = 7;
        public static readonly int SW_SHOWNA = 8;
        public static readonly int SW_RESTORE = 9;
        public static readonly int SW_SHOWDEFAULT = 10;
        public static readonly int SW_FORCEMINIMIZE = 11;
        public static readonly int SW_MAX = 11;
    }
}
