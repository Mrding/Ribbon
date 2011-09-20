using System;
using System.Windows;
using System.Linq;
using ActiproSoftware.Windows.Controls.Docking;

namespace Luna.WPF.ApplicationFramework
{
    public static class DockingWindowExtensions
    {
        public static IDockingWindowMetadata GetDockingWindowMetadata(this object view)
        {
            if (view == null)
                return null;

            if (view is IDockingWindowMetadata)
                return (IDockingWindowMetadata)view; 

            var dependencyObject = view as DependencyObject;
            if (dependencyObject != null)
                return DockingWindowMetadata.GetInstance(dependencyObject);

            return null;
        }

        public static DockingWindow GetDockingWindowForView(this object view, DockSite dockSite)
        {
            if (view == null)
                return null;

            if (view is DockingWindow)
                return (DockingWindow)view;

            if (dockSite == null)
                return null;

            foreach (var documentWindow in dockSite.DocumentWindows)
            {
                if ((documentWindow == view) || (documentWindow.Content == view))
                    return documentWindow;
            }

            foreach (var toolWindow in dockSite.ToolWindows)
            {
                if ((toolWindow == view) || (toolWindow.Content == view))
                    return toolWindow;
            }

            return null;
        }
    }
}