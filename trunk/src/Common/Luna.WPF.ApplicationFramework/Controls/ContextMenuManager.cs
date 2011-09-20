using System.Windows;
using System.Windows.Controls;

namespace Luna.WPF.ApplicationFramework
{
    public static class ContextMenuManager
    {
        public static ContextMenu GetContextMenu(DependencyObject obj)
        {
            return (ContextMenu)obj.GetValue(ContextMenuProperty);
        }

        public static void SetContextMenu(DependencyObject obj, ContextMenu value)
        {
            obj.SetValue(ContextMenuProperty, value);
        }

        // Using a DependencyProperty as the backing store for ContextMenu.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContextMenuProperty =
            DependencyProperty.RegisterAttached("ContextMenu", typeof(ContextMenu), typeof(ContextMenuManager), new UIPropertyMetadata(null, (s, e) =>
            {
                var source = s as FrameworkElement;
                var contextMenu = e.NewValue as ContextMenu;
                if (source != null && contextMenu != null)
                {
                    source.MouseRightButtonUp += (sender, arg) =>
                    {
                        var target = sender as UIElement;
                        contextMenu.PlacementTarget = target;
                        contextMenu.IsOpen = true;
                        arg.Handled = GetHandled(target);
                    };
                    source.Unloaded += (sender, arg) => contextMenu.PlacementTarget = null;
                }
            }));


        public static bool GetHandled(DependencyObject obj)
        {
            return (bool)obj.GetValue(HandledProperty);
        }

        public static void SetHandled(DependencyObject obj, bool value)
        {
            obj.SetValue(HandledProperty, value);
        }

        // Using a DependencyProperty as the backing store for Handled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HandledProperty =
            DependencyProperty.RegisterAttached("Handled", typeof(bool), typeof(ContextMenuManager), new UIPropertyMetadata(false));
    }
}