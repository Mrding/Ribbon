using System;
using System.Windows;
using System.Windows.Controls;
using Luna.Core.Extensions;

namespace Luna.WPF.ApplicationFramework.Behaviors
{
    public static class ControlBehavior
    {


        public static string GetText(DependencyObject obj)
        {
            return (string)obj.GetValue(TextProperty);
        }

        public static void SetText(DependencyObject obj, string value)
        {
            obj.SetValue(TextProperty, value);
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached("Text", typeof(string), typeof(ControlBehavior), new FrameworkPropertyMetadata(string.Empty,  FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


    }


    public static class TabControlBehavior
    {
        public static int GetSelectedTabIndex(TabControl obj)
        {
            return (int)obj.GetValue(SelectedTabIndexProperty);
        }

        public static void SetSelectedTabIndex(TabControl obj, int value)
        {
            obj.SetValue(SelectedTabIndexProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectedTabIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedTabIndexProperty =
            DependencyProperty.RegisterAttached("SelectedTabIndex", typeof(int), typeof(TabControlBehavior), new PropertyMetadata(-1,(d,e)=>
        {
                    
        }, (d, baseValue) =>
        {
            d.SaftyInvoke<TabControl>(t =>
                                            {
                                                var newTabIndex = Convert.ToInt32(baseValue);
                                                if(newTabIndex==-1) return;
                                                                
                                                t.SelectedIndex = newTabIndex;
                                            });
            return baseValue;
        }));

        

        
    }
}