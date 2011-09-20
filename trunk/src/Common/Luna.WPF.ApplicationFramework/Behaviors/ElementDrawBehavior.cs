using System.Windows;
using Luna.WPF.ApplicationFramework.Collections;

namespace Luna.WPF.ApplicationFramework.Behaviors
{
    public static class ElementDrawBehavior
    {
        public static ElementDrawCollection GetElementDraws(DependencyObject obj)
        {
            if (Caliburn.PresentationFramework.PresentationFrameworkModule.IsInDesignMode) return new ElementDrawCollection();
            var draws = (ElementDrawCollection)obj.GetValue(ElementDrawsProperty) ?? new ElementDrawCollection(obj as FrameworkElement);
            obj.SetValue(ElementDrawsProperty, draws);
            return draws;
        }

        public static void SetElementDraws(DependencyObject obj, ElementDrawCollection collections)
        {
            obj.SetValue(ElementDrawsProperty, collections);
        }

        public static readonly DependencyProperty ElementDrawsProperty =
            DependencyProperty.RegisterAttached("ShadowElementDraws", typeof(ElementDrawCollection), typeof(ElementDrawBehavior),
            new FrameworkPropertyMetadata(null));
    }
}
