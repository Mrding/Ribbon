using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Luna.NativeToolkit
{
    /// <summary>
    /// Callback delegate which is used by the Windows API to
    /// submit window messages.
    /// </summary>
    public delegate long WindowProcedureHandler(IntPtr hwnd, uint uMsg, uint wparam, uint lparam);


    /// <summary>
    /// Win API WNDCLASS struct - represents a single window.
    /// Used to receive window messages.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowClass
    {
        public uint style;
        public WindowProcedureHandler lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszMenuName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszClassName;
    }
}
