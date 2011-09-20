using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using Luna.WPF.ApplicationFramework.Extensions;
namespace Luna.WPF.ApplicationFramework.Markup
{
    public class VisualExtension : BaseMarkupExtension
    {

        public VisualExtension()
        { }

        [ConstructorArgument("Type")]
        public Type Type { get; set; }

        [ConstructorArgument("ElementName")]
        public string ElementName { get; set; }

        [ConstructorArgument("ChildName")]
        public string ChildName { get; set; }

        [ConstructorArgument("IsRoot")]
        public int VisualIndex { get; set; }

        public override object ProvideValue()
        {
            var binding = new Binding() { ElementName = ElementName };
            TargetObject.SetBinding(ElementProperty, binding);
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var element = TargetObject.GetBindingExpression(ElementProperty).DataItem as FrameworkElement;
                FrameworkElement obj;
                if (VisualIndex > 0)
                    obj =element.FindVisualChild(VisualIndex);
                else
                {
                    if (string.IsNullOrEmpty(this.ChildName))
                        obj = element.FindVisualChild(Type);
                    else
                        obj = element.FindVisualChild(ChildName);
                }
                if (obj != null)
                    TargetObject.SetBinding(TargetProperty, new Binding() { Source = obj });
            }), System.Windows.Threading.DispatcherPriority.Render);
            return null;
        }

        public static object GetElement(DependencyObject obj)
        {
            return (object)obj.GetValue(ElementProperty);
        }

        public static void SetElement(DependencyObject obj, object value)
        {
            obj.SetValue(ElementProperty, value);
        }

        public static readonly DependencyProperty ElementProperty =
            DependencyProperty.RegisterAttached("Element", typeof(object), typeof(VisualExtension),
            new UIPropertyMetadata(null));
    }
}
