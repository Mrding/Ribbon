using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Luna.WPF.ApplicationFramework.Extensions;
using System.Windows;
using Luna.Common;

namespace Luna.WPF.ApplicationFramework.Behaviors
{
    public static class CheckBoxSelectedAllBehavior
    {

        public static bool GetCanSelectedAll(CheckBox obj)
        {
            return (bool)obj.GetValue(CanSelectedAllProperty);
        }

        public static void SetCanSelectedAll(CheckBox obj, bool value)
        {
            obj.SetValue(CanSelectedAllProperty, value);
        }

        public static readonly DependencyProperty CanSelectedAllProperty =
            DependencyProperty.RegisterAttached("CanSelectedAll", typeof(bool),
            typeof(CheckBoxSelectedAllBehavior), new PropertyMetadata(new PropertyChangedCallback((o, e) =>
                {
                    var checkBox = o as CheckBox;
                    if (checkBox != null)
                    { 
                        //var listView = o.FindAncestor<ListView>();
                        var listView = o.FindAncestor<ItemsControl>();

                        checkBox.Checked += delegate
                        {
                            foreach (ISelectable item in listView.ItemsSource)
                            {
                                item.IsSelected = true;
                            }                           
                        };

                        checkBox.Unchecked += delegate
                        {
                            foreach (ISelectable item in listView.ItemsSource)
                            {
                                item.IsSelected = false;
                            }                           
                        };
                    }
                }
         )));
    }
}
