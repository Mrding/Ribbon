using System;
using System.Windows;
using System.Windows.Markup;

namespace Luna.WPF.ApplicationFramework.Markup
{
    /// <summary>
    /// Provides a base class for xaml markup extensions
    /// </summary>
    public abstract class BaseMarkupExtension : MarkupExtension
    {
        protected IProvideValueTarget ProvideValueTarget { get; set; }

        protected FrameworkElement TargetObject { get; set; }

        protected DependencyProperty TargetProperty { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new FrameworkElement())) return null;
            ProvideValueTarget = serviceProvider as IProvideValueTarget;
            TargetObject = ProvideValueTarget.TargetObject as FrameworkElement;
            TargetProperty = ProvideValueTarget.TargetProperty as DependencyProperty;
            return ProvideValue();
        }
        public abstract object ProvideValue();
    }
}
