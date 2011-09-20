using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Controls;

namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    public class DragDisplayControl:DependencyObject
    {
        protected delegate int HookProc(int nCode, int wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern bool GetCursorPos(out POINT pt);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        protected static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        protected static extern int UnhookWindowsHookEx(int idHook);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        protected static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);

        protected const int WM_MOUSEMOVE = 0x200;
        protected const int WH_MOUSE_LL = 14;
        protected const int WM_LBUTTONUP = 0x202;
        private static int _handleToHook;
        static ScreenLayer _mouseTip = new ScreenLayer();
        protected static readonly HookProc _hookProc = new HookProc(HookCallbackProcedure);

        private static Point Win32PointToWPFPoint(POINT point)
        {
            return new Point(point.X, point.Y);
        }

        private static void UnHookCallbackProcedure()
        {
            if (_handleToHook != 0)
            {
                _mouseTip.Hide();
                UnhookWindowsHookEx(_handleToHook);
                _handleToHook = 0;
            }
        }

        private static int HookCallbackProcedure(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode > -1)
            {
                //鼠标移动窗口跟随
                if (wParam == WM_MOUSEMOVE)
                {
                    POINT point;
                    GetCursorPos(out point);
                    _mouseTip.CurrentPoint = Win32PointToWPFPoint(point);
                }
                //鼠标左键弹起起关闭跟随窗口
                else if (wParam == WM_LBUTTONUP)
                {
                    UnHookCallbackProcedure();
                }
            }

            return CallNextHookEx(_handleToHook, nCode, wParam, lParam);
        }


        public static bool GetIsDisplay(UIElement obj)
        {
            return (bool)obj.GetValue(IsDisplayProperty);
        }

        public static void SetIsDisplay(UIElement obj, bool value)
        {
            obj.SetValue(IsDisplayProperty, value);
        }

        public static DataTemplate GetTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(TemplateProperty);
        }

        public static void SetTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(TemplateProperty, value);
        }
       
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.RegisterAttached("Template", typeof(DataTemplate), typeof(DragDisplayControl), 
            new UIPropertyMetadata());

        public static object GetContent(DependencyObject obj)
        {
            return (object)obj.GetValue(ContentProperty);
        }

        public static void SetContent(DependencyObject obj, object value)
        {
            obj.SetValue(ContentProperty, value);
        }
        
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.RegisterAttached("Content", typeof(object), typeof(DragDisplayControl), 
            new UIPropertyMetadata());


        public static readonly DependencyProperty IsDisplayProperty =
            DependencyProperty.RegisterAttached("IsDisplay", typeof(bool),
            typeof(DragDisplayControl), new UIPropertyMetadata((d, e) =>
                {
                    var element = d as UIElement;
                    bool value = (bool)e.NewValue;
                    if (value)
                        element.PreviewMouseLeftButtonDown += element_MouseLeftButtonDown;
                    else
                        element.PreviewMouseLeftButtonDown -= element_MouseLeftButtonDown;
                }));

        static void element_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var dpObject = sender as DependencyObject;
            if (dpObject == null)
                return;
            var content = GetContent(dpObject);
            if (content == null)
                return;
    
            var template = GetTemplate(dpObject);
            var contentPresenter = new ContentPresenter();
            contentPresenter.Content = content;
            contentPresenter.ContentTemplate = template;

            POINT point;
            GetCursorPos(out point);
            _mouseTip.Target = contentPresenter;           
            _mouseTip.OriginalPoint = Win32PointToWPFPoint(point);
            _mouseTip.Show();

            HookMouse();
        }

        private static void HookMouse()
        {
            IntPtr modulePoint = IntPtr.Zero;
            if (System.Environment.OSVersion.Version.Major < 6)//xp Major is 5 ,vista & win7 >=6
                modulePoint = GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);

            _handleToHook = SetWindowsHookEx(WH_MOUSE_LL, _hookProc, modulePoint, 0);
        }
    }
}
