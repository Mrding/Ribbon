using System.Windows;
using System.Windows.Input;
using Luna.Core.Extensions;
using System.Windows.Controls;
using System.Windows.Data;
using Luna.WPF.ApplicationFramework.Threads;

namespace Luna.WPF.ApplicationFramework.Behaviors
{
    public class ComboBoxBehavior
    {
        public static BindingBase GetDeferredSelectedValue(ComboBox obj)
        {
            return (BindingBase)obj.GetValue(DeferredSelectedValueProperty);
        }

        public static void SetDeferredSelectedValue(ComboBox obj, BindingBase value)
        {
            obj.SetValue(DeferredSelectedValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for DeferredSelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeferredSelectedValueProperty =
            DependencyProperty.RegisterAttached("DeferredSelectedValue", typeof(BindingBase), typeof(ComboBoxBehavior), new PropertyMetadata(null,
                (d, e) =>
                {

                    UIThread.BeginInvoke(() =>
                    {
                        BindingOperations.SetBinding(d, ComboBox.SelectedValueProperty, (BindingBase)e.NewValue);
                    }, System.Windows.Threading.DispatcherPriority.DataBind);
                }));


    }

    public class CommandBidingBehavior
    {
        public static DependencyProperty RegisterCommandBindingsProperty =
            DependencyProperty.RegisterAttached("RegisterCommandBindings", typeof(CommandBindingCollection), typeof(CommandBidingBehavior),
                                                new PropertyMetadata(null, OnRegisterCommandBindingChanged, (sender, baseValue) =>
            {
                var bindings = baseValue as CommandBindingCollection;
                if (bindings == null)
                    return baseValue;

                sender.SaftyInvoke<UIElement>(el =>
                                                  {
                                                      var isBound = GetRegisterCommandBindings(sender) != null;
                                                      if (isBound && bindings.Count != el.CommandBindings.Count)
                                                      {
                                                         baseValue = new CommandBindingCollection(bindings);
                                                      }
                                                  });

                return baseValue;
            }));

        public static void SetRegisterCommandBindings(DependencyObject element, CommandBindingCollection value)
        {
            if (element != null)
                element.SetValue(RegisterCommandBindingsProperty, value);
        }
        public static CommandBindingCollection GetRegisterCommandBindings(DependencyObject element)
        {
            return (element != null ? (CommandBindingCollection)element.GetValue(RegisterCommandBindingsProperty) : null);
        }
        private static void OnRegisterCommandBindingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            //sender.SaftyInvoke<UIElement>(el =>
            //                                  {
            //                                      var bindings = e.NewValue as CommandBindingCollection;
            //                                      if (bindings != null)
            //                                          el.CommandBindings.AddRange(bindings);
            //                                  });

            var bindings = e.NewValue as CommandBindingCollection;
            if (bindings == null)
                return;


            sender.SaftyInvoke<UIElement>(el =>
            {
                if (bindings.Count != el.CommandBindings.Count)
                {
                    foreach (CommandBinding binding in bindings)
                    {
                        if (el.CommandBindings.Contains(binding))
                            continue;
                        el.CommandBindings.Add(binding);
                    }
                }
            });

        }
    }
}