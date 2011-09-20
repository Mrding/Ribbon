namespace Caliburn.PresentationFramework
{
    using System.Reflection;
    using System.Windows;
    using System.Windows.Media;
    using Core;
    using Microsoft.Practices.ServiceLocation;
    using System;

    /// <summary>
    /// Extension methods related to the PresentationFramework classes.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets the resource by searching the hierarchy of of elements.
        /// </summary>
        /// <typeparam name="T">The type of resource.</typeparam>
        /// <param name="element">The element.</param>
        /// <param name="key">The key.</param>
        /// <returns>The resource.</returns>
        public static T GetResource<T>(this DependencyObject element, object key)
        {
            var currentElement = element;

            while (currentElement != null)
            {
                var fe = currentElement as FrameworkElement;

                if (fe != null)
                {
                    if (fe.Resources.Contains(key))
                        return (T)fe.Resources[key];
                }
#if !SILVERLIGHT
                else
                {
                    var fce = currentElement as FrameworkContentElement;
                    if (fce != null)
                    {
                        if (fce.Resources.Contains(key))
                            return (T)fce.Resources[key];
                    }
                }

                currentElement = (LogicalTreeHelper.GetParent(currentElement) ??
                    VisualTreeHelper.GetParent(currentElement));
#else
                currentElement = VisualTreeHelper.GetParent(currentElement);
#endif
            }

            if (Application.Current != null && Application.Current.Resources.Contains(key))
                return (T)Application.Current.Resources[key];

            return default(T);
        }

        /// <summary>
        /// Adds the routed UI messaging module's configuration to the system.
        /// </summary>
        /// <param name="hook">The hook.</param>
        /// <returns>The configuration.</returns>
        public static PresentationFrameworkModule WithPresentationFramework(this IConfigurationHook hook)
        {
            return new PresentationFrameworkModule(hook);
        }

        /// <summary>
        /// Finds the interaction defaults or fail.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static InteractionDefaults FindDefaultsOrFail(this IRoutedMessageController controller, object element)
        {
            var type = element.GetType();
            var defaults = controller.GetInteractionDefaults(type);

            if (defaults == null)
                throw new CaliburnException(
                    string.Format("Could not locate InteractionDefaults for {0}.  Please register with the IRoutedMessageController.", type.Name)
                    );

            return defaults;
        }

        /// <summary>
        /// Gets the data context of the depdendency object.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>The data context value.</returns>
        public static object GetDataContext(this DependencyObject dependencyObject)
        {
            var fe = dependencyObject as FrameworkElement;
            if (fe != null)
                return fe.DataContext;
#if !SILVERLIGHT
            var fce = dependencyObject as FrameworkContentElement;
            if (fce != null)
                return fce.DataContext;
#endif
            throw new CaliburnException(
                string.Format(
                    "Instance {0} must be a FrameworkElement or FrameworkContentElement in order to get its DataContext property.",
                    dependencyObject.GetType().Name
                    )
                );
        }

        /// <summary>
        /// Finds a child element by name.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="name">The name.</param>
        /// <returns>The found element.</returns>
        public static DependencyObject FindName(this DependencyObject parent, string name)
        {
            var fe = parent as FrameworkElement;
            if (fe != null)
            {
                if (fe.Name == name)
                    return fe;
                return fe.FindName(name) as DependencyObject;
            }
#if !SILVERLIGHT
            else
            {
                var fce = parent as FrameworkContentElement;
                if (fce != null)
                {
                    if (fce.Name == name)
                        return fce;
                    return fce.FindName(name) as DependencyObject;
                }
            }
#endif

            return null;
        }

        /// <summary>
        /// Finds an element by name or fails.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element">The element.</param>
        /// <param name="name">The name.</param>
        /// <param name="shouldFail">Indicates whether an exception should be throw if the named item is not found.</param>
        /// <returns>The found element.</returns>
        public static T FindNameExhaustive<T>(this DependencyObject element, string name, bool shouldFail)
            where T : class
        {
            T found = null;

            if (!string.IsNullOrEmpty(name))
            {
                found = (name == "$this" ? element as T : element.FindName(name) as T) ?? element.GetResource<T>(name);
            }

            if (found == null && shouldFail) throw new CaliburnException(
                    string.Format("Could not locate {0} with name {1}.", typeof(T).Name, name)
                    );

            return found;
        }

        /// <summary>
        /// Wires the delegate to the Loaded event of the element.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="callback">The callback.</param>
        public static void OnLoad(this DependencyObject dependencyObject, RoutedEventHandler callback)
        {
            var fe = dependencyObject as FrameworkElement;
            if (fe != null)
                fe.Loaded += callback;
#if !SILVERLIGHT
            else
            {
                var fce = dependencyObject as FrameworkContentElement;
                if (fce != null)
                    fce.Loaded += callback;
            }
#endif
        }

        /// <summary>
        /// Binds the specified parameter to an element's property without using databinding.
        /// Rather, event name conventions are used to wire to property changes and push updates to the parameter value.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="element">The element.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="path">The path.</param>
        public static void Bind(this Parameter parameter, DependencyObject element, string elementName, string path)
        {
            bool isLoaded = false;

            element.OnLoad(
                (s, e) =>
                {
                    if (isLoaded)
                        return;

                    isLoaded = true;

                    var source = element.FindNameExhaustive<object>(elementName, false);
                    if (source == null)
                        return;

                    if (!string.IsNullOrEmpty(path))
                    {
                        var sourceType = source.GetType();
                        var property = sourceType.GetProperty(path);

                        EventInfo changeEvent = null;

                        if (path == "SelectedItem")
                            changeEvent = sourceType.GetEvent("SelectionChanged");
                        if (changeEvent == null)
                            changeEvent = sourceType.GetEvent(path + "Changed");
                        if (changeEvent == null)
                            WireToDefaultEvent(parameter, sourceType, source, property);
                        else parameter.Wire(source, changeEvent, () => property.GetValue(source, null));
                    }
                    else WireToDefaultEvent(parameter, source.GetType(), source, null);
                });
        }

        private static void WireToDefaultEvent(Parameter parameter, Type type, object source, PropertyInfo property)
        {
            var defaults = ServiceLocator.Current.GetInstance<IRoutedMessageController>()
                .GetInteractionDefaults(type);

            if (defaults == null)
                throw new CaliburnException(
                    "Insuficient information provided for wiring action parameters.  Please set interaction defaults for " + type.FullName
                    );

            var eventInfo = type.GetEvent(defaults.DefaultEventName);

            if (property == null)
                parameter.Wire(source, eventInfo, () => defaults.GetDefaultValue(source));
            else parameter.Wire(source, eventInfo, () => property.GetValue(source, null));
        }
    }
}