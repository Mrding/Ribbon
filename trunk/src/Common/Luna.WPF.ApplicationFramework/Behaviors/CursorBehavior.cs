using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Luna.WPF.ApplicationFramework.Extensions;
using Microsoft.Win32.SafeHandles;

namespace Luna.WPF.ApplicationFramework.Behaviors
{
    public class CursorBehavior
    {
        private static ContentPresenter _contentPresenter;
        private static DataTemplate _dataTemplate;

        private static void triggerElement_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            e.Action = DragAction.Drop;
            if ((e.KeyStates & DragDropKeyStates.LeftMouseButton) != DragDropKeyStates.None)
                e.Action = DragAction.Continue;
            if (e.Action == DragAction.Drop)
                _contentPresenter = null;
        }

        private static bool _isMouseOver;
        private static void triggerElement_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (e.Effects == DragDropEffects.None)
            {
                e.UseDefaultCursors = true;
                _isMouseOver = false;
            }
            else if (_isMouseOver)
            {
                Mouse.SetCursor(CreateCursor(_contentPresenter, 5, 5));
                e.UseDefaultCursors = false;
            }
            else
            {
                Mouse.SetCursor(Cursors.Arrow);
                e.UseDefaultCursors = false;
            }

            e.Handled = true;
        }

        private static void triggerElement_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ListBoxItem listBoxItem;
                var run = e.OriginalSource as System.Windows.Documents.Run;
                listBoxItem = run != null
                                  ? run.Parent.FindAncestor<ListBoxItem>()
                                  : (e.OriginalSource as DependencyObject).FindAncestor<ListBoxItem>();
                var parent = sender as ListBox;
                if (listBoxItem != null && parent != null && parent.SelectedItem != null && parent.SelectedValue != null)
                {
                    _contentPresenter = new ContentPresenter
                                            {
                                                Content = parent.SelectedItem,
                                                ContentTemplate = _dataTemplate
                                            };

                    System.Diagnostics.Debug.Print(parent.SelectedValue.ToString());
                    DragDrop.DoDragDrop(parent, parent.SelectedValue, DragDropEffects.Move);
                }
            }
        }

        private static void triggerElement_DragOver(object sender, DragEventArgs e)
        {
            _isMouseOver = true;
        }

        public static DataTemplate GetCursorTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(CursorTemplateProperty);
        }

        public static void SetCursorTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(CursorTemplateProperty, value);
        }

        // Using a DependencyProperty as the backing store for CursorTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CursorTemplateProperty =
            DependencyProperty.RegisterAttached("CursorTemplate", typeof(DataTemplate), typeof(CursorBehavior), new UIPropertyMetadata(null, null, OnCursorTemplateChanged));

        private static object OnCursorTemplateChanged(DependencyObject d, object basevalue)
        {
            var triggerElement = d as ListBox;
            if (triggerElement != null)
            {
                triggerElement.AllowDrop = true;
                _dataTemplate = basevalue as DataTemplate;

                triggerElement.QueryContinueDrag += triggerElement_QueryContinueDrag;
                triggerElement.GiveFeedback += triggerElement_GiveFeedback;
                triggerElement.MouseMove += triggerElement_MouseMove;
                triggerElement.DragOver += triggerElement_DragOver;
                triggerElement.Unloaded += delegate
                {
                    //triggerElement.QueryContinueDrag -= triggerElement_QueryContinueDrag;
                    //triggerElement.GiveFeedback -= triggerElement_GiveFeedback;
                    //triggerElement.MouseMove -= triggerElement_MouseMove;
                    //triggerElement.DragOver -= triggerElement_DragOver;
                    _dataTemplate = null;
                };
            }
            return basevalue;
        }



        #region CreateCursor
        private struct IconInfo
        {
            public bool FIcon;
            public int XHotspot;
            public int YHotspot;
            public IntPtr HbmMask;
            public IntPtr HbmColor;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr CreateIconIndirect(
            ref IconInfo icon);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetIconInfo(IntPtr hIcon,
                                               ref IconInfo pIconInfo);


        private static Cursor InternalCreateCursor(System.Drawing.Bitmap bmp,
                                                   int xHotSpot, int yHotSpot)
        {
            var tmp = new IconInfo();
            GetIconInfo(bmp.GetHicon(), ref tmp);
            tmp.XHotspot = xHotSpot;
            tmp.YHotspot = yHotSpot;
            tmp.FIcon = false;

            IntPtr ptr = CreateIconIndirect(ref tmp);
            var handle = new SafeFileHandle(ptr, true);
            return CursorInteropHelper.Create(handle);
        }

        public static Cursor CreateCursor(UIElement element, int xHotSpot,
                                          int yHotSpot)
        {
            if (element == null) return Cursors.Arrow;
            element.Measure(new Size(double.PositiveInfinity,
                                     double.PositiveInfinity));
            element.Arrange(new Rect(0, 0, element.DesiredSize.Width,
                                     element.DesiredSize.Height));

            var rtb =
                new RenderTargetBitmap((int)element.DesiredSize.Width,
                                       (int)element.DesiredSize.Height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(element);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            var ms = new MemoryStream();
            encoder.Save(ms);

            var bmp = new System.Drawing.Bitmap(ms);

            ms.Close();
            ms.Dispose();

            Cursor cur = InternalCreateCursor(bmp, xHotSpot, yHotSpot);

            bmp.Dispose();

            return cur;
        }
        #endregion
    }
}