using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Luna.WPF.ApplicationFramework.Graphics;

namespace Luna.WPF.ApplicationFramework.Controls
{
    /// <summary>
    /// Provides LeftContent,RightContent,TopContent,FloatTopContent properties for the ScheduleGrid's layout
    /// when Content changed it will call ReplaceLogicalChild method 
    /// </summary>
    public class ScheduleGridLayout
    {

        public static object GetLeftContent(DependencyObject obj)
        {
            return (object)obj.GetValue(LeftContentProperty);
        }

        public static void SetLeftContent(DependencyObject obj, object value)
        {
            obj.SetValue(LeftContentProperty, value);
        }

        public static readonly DependencyProperty LeftContentProperty =
            DependencyProperty.RegisterAttached("LeftContent", typeof(object), typeof(ScheduleGridLayout),
                new FrameworkPropertyMetadata(null, ReplaceLogicialChild));


        public static object GetRightContent(DependencyObject obj)
        {
            return (object)obj.GetValue(RightContentProperty);
        }

        public static void SetRightContent(DependencyObject obj, object value)
        {
            obj.SetValue(RightContentProperty, value);
        }

        public static readonly DependencyProperty RightContentProperty =
            DependencyProperty.RegisterAttached("RightContent", typeof(object), typeof(ScheduleGridLayout),
                new FrameworkPropertyMetadata(null, ReplaceLogicialChild));

        public static object GeTopContent(DependencyObject obj)
        {
            return (object)obj.GetValue(TopContentProperty);
        }

        public static void SetTopContent(DependencyObject obj, object value)
        {
            obj.SetValue(TopContentProperty, value);
        }

        public static readonly DependencyProperty TopContentProperty =
            DependencyProperty.RegisterAttached("TopContent", typeof(object), typeof(ScheduleGridLayout),
                new FrameworkPropertyMetadata(null, ReplaceLogicialChild));

        public static object GetBottomContent(DependencyObject obj)
        {
            return (object)obj.GetValue(BottomContentProperty);
        }

        public static void SetBottomContent(DependencyObject obj, object value)
        {
            obj.SetValue(BottomContentProperty, value);
        }

        public static readonly DependencyProperty BottomContentProperty =
                DependencyProperty.RegisterAttached("BottomContent", typeof(object), typeof(ScheduleGridLayout),
                new FrameworkPropertyMetadata(null, ReplaceLogicialChild));

        private static void ReplaceLogicialChild(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            var element = (ILogicalParent)o;
            element.ReplaceLogicalChild(args.OldValue, args.NewValue);
        }
    }
}
