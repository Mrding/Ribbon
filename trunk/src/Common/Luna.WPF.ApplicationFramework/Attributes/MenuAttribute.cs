using System;
using System.Windows;
using System.Windows.Controls;
using Caliburn.PresentationFramework.ApplicationModel;

namespace Luna.WPF.ApplicationFramework.Attributes
{
    /// <summary>
    /// An implementation of <see cref="IViewStrategy"/> that provides a basic lookup strategy for an attributed model.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct, AllowMultiple = true)]
    public class MenuAttribute : ViewStrategyAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MenuAttribute"/> class.
        /// </summary>
        /// <param name="resourceKey">The resource key.</param>
        public MenuAttribute(string resourceKey)
        {
            ResourceKey = resourceKey;
        }

        /// <summary>
        /// Gets or sets the resource key.
        /// </summary>
        /// <value>The resource key.</value>
        public string ResourceKey
        { get; set; }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public object Context { get; set; }

        /// <summary>
        /// Determines whether this strategy applies in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// true if it matches the context; false otherwise
        /// </returns>
        public override bool Matches(object context)
        {
            if (Context == null)
                return context == null;

            return Context.Equals(context);
        }

        /// <summary>
        /// Gets the view for displaying the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="displayLocation">The control into which the view will be injected.</param>
        /// <param name="context">Some additional context used to select the proper view.</param>
        /// <returns>The view.</returns>
        public override object GetView(object model, DependencyObject displayLocation, object context)
        {
            var menuItem = Application.Current.TryFindResource(ResourceKey);
            if (menuItem == null)
                return Activator.CreateInstance<MenuItem>();
            return menuItem;
        }
    }
}
