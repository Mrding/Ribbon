using System.Windows;
using System.Windows.Controls;
using Luna.Common;

namespace Luna.WPF.ApplicationFramework.DataTemplateSelectors
{
    public class ActionItemDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var customAction = item as ICustomAction;
            if (customAction != null)
            {
                var window = Application.Current.Windows[Application.Current.Windows.Count - 1];
                if (window != null)
                    return window.TryFindResource(customAction.ResourceKey) as DataTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }
}