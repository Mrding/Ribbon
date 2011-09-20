using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Interop;
namespace Luna.NativeToolkit
{
    /// <summary>
    /// Windows messages operation
    /// </summary>
    public class MessageMethod
    {
        /// <summary>
        /// Sends the specified message to a window
        /// </summary>
        /// <param name="hWnd">Handle to the window </param>
        /// <param name="Msg">Specifies the message to be sent</param>
        /// <param name="wParam">Specifies additional message-specific information</param>
        /// <param name="lParam">Specifies additional message-specific information</param>
        /// <returns></returns>
        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool TranslateMessage([In, Out] ref MSG msg);

        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr DispatchMessage([In] ref MSG msg);


        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin,
           uint wMsgFilterMax);
    }
}
