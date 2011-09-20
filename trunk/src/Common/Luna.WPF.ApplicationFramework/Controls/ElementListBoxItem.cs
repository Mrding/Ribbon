using System.Windows.Controls;
using System.Windows;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public class ElementListBoxItem : ContentPresenter
    {
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            ListBoxItem.IsSelectedProperty.AddOwner(typeof(ElementListBoxItem),
            new FrameworkPropertyMetadata(false));
    }
}
