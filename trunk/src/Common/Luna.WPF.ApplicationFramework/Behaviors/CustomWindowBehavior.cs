using System.Windows;
using System.Windows.Controls.Primitives;
using Caliburn.PresentationFramework;
using Luna.Core.Extensions;
using Luna.WPF.ApplicationFramework.Extensions;


namespace Luna.WPF.ApplicationFramework.Behaviors
{
    public static class CustomWindowBehavior
    {
        public static WindowState GetWindowState(ButtonBase obj)
        {
            return (WindowState)obj.GetValue(WindowStateProperty);
        }

        public static void SetWindowState(ButtonBase obj, WindowState value)
        {
            obj.SetValue(WindowStateProperty, value);
        }

        public static readonly DependencyProperty WindowStateProperty =
            DependencyProperty.RegisterAttached("WindowState", typeof(WindowState),
                                                typeof(CustomWindowBehavior), new PropertyMetadata(new PropertyChangedCallback((d, args) => d.SaftyInvoke<ButtonBase>(btn =>
        {
            if (btn.IsLoaded)
            {
                var window = btn.FindAncestor<Window>();
                window.WindowState = (WindowState)args.NewValue;
                return;
            }
            btn.Click += delegate
                             {
                                 var window = btn.FindAncestor<Window>();
                                 window.WindowState = GetWindowState(btn);
                             };
        }))));

        public static bool GetDragCaption(FrameworkElement obj)
        {
            return (bool)obj.GetValue(DragCaptionProperty);
        }

        public static void SetDragCaption(FrameworkElement obj, bool value)
        {
            obj.SetValue(DragCaptionProperty, value);
        }

        public static readonly DependencyProperty DragCaptionProperty =
            DependencyProperty.RegisterAttached("DragCaption", typeof(bool),
                                                typeof(CustomWindowBehavior), new PropertyMetadata(new PropertyChangedCallback((d, args) => d.SaftyInvoke<FrameworkElement>(btn =>
            {
                btn.MouseLeftButtonDown += (sender, e) =>
                                                {
                                                    var window = btn.FindAncestor<Window>();
                                                    window.DragMove();
                                                };
            }))));



        public static bool GetOpenOnOwnerCenterTop(Window obj)
        {
            return (bool)obj.GetValue(OpenOnOwnerCenterTopProperty);
        }

        public static void SetOpenOnOwnerCenterTop(Window obj, bool value)
        {
            obj.SetValue(OpenOnOwnerCenterTopProperty, value);
        }

        // Using a DependencyProperty as the backing store for OpenOnOwnerCenterTop.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OpenOnOwnerCenterTopProperty =
            DependencyProperty.RegisterAttached("OpenOnOwnerCenterTop", typeof(bool), typeof(CustomWindowBehavior), new UIPropertyMetadata(false,
                (d, e) =>
                        d.SaftyInvoke<Window>(w =>
                        {
                            w.Activated += delegate
                                               {
                                                   var parent = w.Owner;
                                                   if (parent == w || parent == null) return;

                                                   w.WindowStartupLocation = WindowStartupLocation.Manual;
                                                   w.Left = ((parent.ActualWidth - w.ActualWidth) / 2) + parent.Left;
                                                   w.Top = parent.Top;
                                               };
                        })
                    ));



        public static bool GetMakeCoverToOwner(DependencyObject obj)
        {
            return (bool)obj.GetValue(MakeCoverToOwnerProperty);
        }

        public static void SetMakeCoverToOwner(DependencyObject obj, bool value)
        {
            obj.SetValue(MakeCoverToOwnerProperty, value);
        }

        // Using a DependencyProperty as the backing store for MakeCoverToOwner.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MakeCoverToOwnerProperty =
            DependencyProperty.RegisterAttached("MakeCoverToOwner", typeof(bool), typeof(CustomWindowBehavior), new UIPropertyMetadata(false,
                (d, e) => d.SaftyInvoke<FrameworkElement>(el =>
                                                              {
                                                                  if (PresentationFrameworkModule.IsInDesignMode) return;

                                                                  var w = (el as Window) ?? el.FindAncestor<Window>();

                                                                  if (w == null || w.Owner == null) return;

                                                                  w.Width = w.Owner.ActualWidth - 20;
                                                                  w.Height = w.Owner.ActualHeight - 20;
                                                                  w.Top = w.Owner.Top + 10;
                                                                  w.Left = w.Owner.Left + 10;

                                                                  //el.Loaded += delegate
                                                                  //                 {


                                                                  //                 };
                                                              })));



        public static bool GetIsCloseButton(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsCloseButtonProperty);
        }

        public static void SetIsCloseButton(DependencyObject obj, bool value)
        {
            obj.SetValue(IsCloseButtonProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsCloseButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCloseButtonProperty =
            DependencyProperty.RegisterAttached("IsCloseButton", typeof(bool), typeof(CustomWindowBehavior), new UIPropertyMetadata(false, (d, args) => d.SaftyInvoke<ButtonBase>(btn =>
                                                                                                                                                                                     {
                                                                                                                                                                                         btn.Click += delegate
                                                                                                                                                                                                        {
                                                                                                                                                                                                            var window = btn.FindAncestor<Window>();
                                                                                                                                                                                                            window.Close();
                                                                                                                                                                                                        };
                                                                                                                                                                                     })));
    }
}