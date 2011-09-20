using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace Luna.WPF.ApplicationFramework.Extensions
{
    public static class VisualExtension
    {
        /// <summary>
        /// Privide a way to find the specified ancestor(parent) object of the visual object in visual tree
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dependencyObject"></param>
        /// <returns></returns>
        public static T FindAncestor<T>(this DependencyObject dependencyObject) where T : DependencyObject
        {
            while (dependencyObject != null && !(dependencyObject is T))
            {
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            return dependencyObject as T;
        }

        public static T FindAncestor<T>(this DependencyObject dependencyObject, Predicate<DependencyObject> predicate) where T : DependencyObject
        {
            while (dependencyObject != null && !predicate(dependencyObject))
            {
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }
            return (T)dependencyObject;
        }

        public static DependencyObject FindAncestor(this DependencyObject dependencyObject, Type type)
        {
            while (dependencyObject != null && !dependencyObject.GetType().Equals(type))
            {
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }
            return dependencyObject;
        }

        //查找根节点
        public static DependencyObject FindAncestor(this Type findType, DependencyObject dependencyObject)
        {
            while (dependencyObject != null && !(findType.IsInstanceOfType(dependencyObject)))
            {
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            return dependencyObject;
        }

        /// <summary>
        /// Privide a way to find the specified ancestor(parent) object of the logical object in logical tree
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dependencyObject"></param>
        /// <returns></returns>
        public static T FindAncestorByLogicalTree<T>(this DependencyObject dependencyObject) where T : DependencyObject
        {
            while (dependencyObject != null && !(dependencyObject is T))
            {
                dependencyObject = LogicalTreeHelper.GetParent(dependencyObject);
            }
            return (T)dependencyObject;
        }

        public static T FindAncestorByLogicalTree<T>(this DependencyObject dependencyObject, Predicate<DependencyObject> predicate) where T : DependencyObject
        {
            while (dependencyObject != null && !(dependencyObject is T) && !predicate(dependencyObject))
            {
                dependencyObject = LogicalTreeHelper.GetParent(dependencyObject);
            }
            return (T)dependencyObject;
        }

        public static T FindVisualChild<T>(this DependencyObject obj) where T : DependencyObject
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    return (T)child;

                var childOfChild = child.FindVisualChild<T>();
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }

        public static FrameworkElement FindVisualChild(this DependencyObject obj, int index)
        {
            var currentIndex = -1;
            DependencyObject child = null;
            while (VisualTreeHelper.GetChildrenCount(obj) > 0 && currentIndex != index)
            {
                currentIndex++;
                child = VisualTreeHelper.GetChild(obj, 0);
                obj = child;

            }
            return (child as FrameworkElement);
        }

        public static T FindVisualChild<T>(this DependencyObject obj, string uid) where T : DependencyObject
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                {
                    var uiElement = child as UIElement;
                    if (uiElement != null && uiElement.Uid == uid)
                        return (T)child;
                }
                else
                {
                    var childOfChild = child.FindVisualChild<T>(uid);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        public static void LoopVisualChild(this DependencyObject obj, Action<DependencyObject> action)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child == null) continue;

                action(child);
                child.LoopVisualChild(action);
            }
        }

        public static void LoopVisualChild(this DependencyObject obj, Func<DependencyObject,bool> matched)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child == null) continue;

                if(matched(child)) 
                    break;
                child.LoopVisualChild(matched);
            }
        }

        public static FrameworkElement FindVisualChild(this DependencyObject obj, Type type)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i) as FrameworkElement;

                if (child != null && child.GetType().Equals(type))
                    return child;

                var childOfChild = FindVisualChild(child, type);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }

        public static FrameworkElement FindVisualChild(this FrameworkElement obj, string name)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i) as FrameworkElement;

                if (child != null && child.Name == name)
                    return child;

                var childOfChild = FindVisualChild(child, name);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }

        public static void ClearAllLocalValue(this DependencyObject obj)
        {
            if (obj == null) return;
            var localValueEnumerator = obj.GetLocalValueEnumerator();
            while (localValueEnumerator.MoveNext())
            {
                var property = localValueEnumerator.Current.Property;
                if (!property.ReadOnly)
                    obj.ClearValue(property);
            }
        }

        public static void ClearEffectiveValueEntry(this DependencyObject obj)
        {
            if (obj == null) return;
            var fieldInfo = typeof(DependencyObject).GetField("_effectiveValues", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo == null) return;
            var valueEntries = fieldInfo.GetValue(obj) as Array;
            if (valueEntries == null) return;
            foreach (var entry in valueEntries)
            {
                var valueFieldInfo = entry.GetType().GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance);
                if (valueFieldInfo != null)
                    valueFieldInfo.SetValue(entry, null);
            }
        }
    }
}