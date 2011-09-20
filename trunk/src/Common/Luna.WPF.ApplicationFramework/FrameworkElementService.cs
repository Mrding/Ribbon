using System;
using System.Windows;
using Luna.Core.Extensions;

namespace Luna.WPF.ApplicationFramework
{
    /// <summary>
    /// Privides some extensible attach property for FrameworkElement
    /// </summary>
    public class FrameworkElementService
    {

        #region TagProperty

        public static object GetTag(DependencyObject obj)
        {
            return (object)obj.GetValue(TagProperty);
        }

        public static void SetTag(DependencyObject obj, object value)
        {
            obj.SetValue(TagProperty, value);
        }

        /// <summary>
        /// Privide another Tag replace with the Tag of FrameworkElement
        /// </summary>
        public static readonly DependencyProperty TagProperty =
            DependencyProperty.RegisterAttached("Tag", typeof(object), typeof(FrameworkElementService),
            new UIPropertyMetadata(null));

        #endregion

        public static ActionCondition GetUnLoadedCondition(DependencyObject obj)
        {
            return (ActionCondition)obj.GetValue(UnLoadedConditionProperty);
        }

        public static void SetUnLoadedCondition(DependencyObject obj, ActionCondition value)
        {
            obj.SetValue(UnLoadedConditionProperty, value);
        }

        public static readonly DependencyProperty UnLoadedConditionProperty =
            DependencyProperty.RegisterAttached("UnLoadedCondition", typeof(ActionCondition), typeof(FrameworkElementService),
            new UIPropertyMetadata(null));


        /// <summary>
        /// Gets the value of the IsFirstVisit attached property for the specified DependencyObject
        ///When you first change The propery's value,then set IsFirstVisit as mark
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetIsFirstVisit(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFirstVisitProperty);
        }

        public static void SetIsFirstVisit(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFirstVisitProperty, value);
        }

        public static readonly DependencyProperty IsFirstVisitProperty =
            DependencyProperty.RegisterAttached("IsFirstVisit", typeof(bool), typeof(FrameworkElementService),
            new UIPropertyMetadata(true));


        public static bool GetMarkEdit(DependencyObject obj)
        {
            return (bool)obj.GetValue(MarkEditProperty);
        }

        public static void SetMarkEdit(DependencyObject obj, bool value)
        {
            obj.SetValue(MarkEditProperty, value);
        }

        public static readonly DependencyProperty MarkEditProperty =
            DependencyProperty.RegisterAttached("MarkEdit", typeof(bool), typeof(FrameworkElementService), 
            new PropertyMetadata(true));


        public static bool GetHandleRequestBringIntoView(FrameworkElement obj)
        {
            return (bool)obj.GetValue(HandleRequestBringIntoViewProperty);
        }

        public static void SetHandleRequestBringIntoView(FrameworkElement obj, bool value)
        {
            obj.SetValue(HandleRequestBringIntoViewProperty, value);
        }

        // Using a DependencyProperty as the backing store for HandelRequestBringIntoView.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HandleRequestBringIntoViewProperty =
            DependencyProperty.RegisterAttached("HandleRequestBringIntoView", typeof(bool), typeof(FrameworkElementService), new FrameworkPropertyMetadata(false,
                (sender, e) =>
                {
                    sender.SaftyInvoke<FrameworkElement>(fe =>
                    {
                        if (Convert.ToBoolean(e.NewValue))
                        {
                            fe.RequestBringIntoView +=
                                (invoker, arg) => { arg.Handled = true; };
                        }
                    });
                }));


        
    }
}
