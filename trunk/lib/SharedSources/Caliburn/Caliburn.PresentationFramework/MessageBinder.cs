﻿namespace Caliburn.PresentationFramework
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;
    using System.Windows;
    using Core;
    using System.Linq;

#if SILVERLIGHT
    using Caliburn.Core.Metadata;
#endif

    /// <summary>
    /// The default implementation of <see cref="IMessageBinder"/>.
    /// </summary>
    public class MessageBinder : IMessageBinder
    {
        private readonly IRoutedMessageController _routedMessageController;
        private readonly IDictionary<string, Func<IInteractionNode, object, object>> _valueHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBinder"/> class.
        /// </summary>
        /// <param name="routedMessageController">The routed message controller.</param>
        public MessageBinder(IRoutedMessageController routedMessageController)
        {
            _routedMessageController = routedMessageController;
            _valueHandlers = new Dictionary<string, Func<IInteractionNode, object, object>>();
            
            InitializeDefaultValueHandlers();
        }

        /// <summary>
        /// Determines whether the supplied value is recognized as a specialy treated value.
        /// </summary>
        /// <param name="potential">The potential value.</param>
        /// <returns>
        /// 	<c>true</c> if a special value; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsSpecialValue(string potential)
        {
            if(string.IsNullOrEmpty(potential))
                return false;

            if (!potential.StartsWith("$"))
                return false;

            var splits = potential.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            var normalized = splits[0].TrimStart('$').ToLower();

            return _valueHandlers.ContainsKey(normalized);
        }

        /// <summary>
        /// Identifies a special value along with its handler.
        /// </summary>
        /// <param name="specialValue">The special value.</param>
        /// <param name="handler">The handler.</param>
        public void AddValueHandler(string specialValue, Func<IInteractionNode, object, object> handler)
        {
            _valueHandlers[specialValue.TrimStart('$').ToLower()] = handler;
        }

        /// <summary>
        /// Determines the parameters that a method should be invoked with.
        /// </summary>
        /// <param name="message">The message to determine the parameters for.</param>
        /// <param name="requiredParameters">The requirements for method binding.</param>
        /// <param name="handlingNode">The handling node.</param>
        /// <param name="context">The context.</param>
        /// <returns>The actual parameters</returns>
        public virtual object[] DetermineParameters(IRoutedMessage message, IList<RequiredParameter> requiredParameters, IInteractionNode handlingNode, object context)
        {
            if (requiredParameters == null || requiredParameters.Count == 0)
                return new object[] {};

            var providedValues = message.Parameters.Select(x => x.Value).ToArray();

            if (requiredParameters.Count == providedValues.Length)
                return CoerceValues(requiredParameters, providedValues, message.Source, context);

            if (providedValues.Length == 0)
                return LocateAndCoerceValues(requiredParameters, message.Source, handlingNode, context);

            throw new CaliburnException(
                string.Format(
                    "Parameter count mismatch.  {0} parameters were provided but {1} are required for {2}.",
                    providedValues.Length,
                    requiredParameters.Count,
                    message
                    )
                );
        }

        /// <summary>
        /// Binds the return value to the UI.
        /// </summary>
        /// <param name="outcome">The outcome or processing the message.</param>
        public virtual IResult CreateResult(MessageProcessingOutcome outcome)
        {
            if (outcome.ResultType == typeof(void))
                return new EmptyResult();

            var enumerable = outcome.Result as IEnumerable<IResult>;

            if (enumerable != null) 
                return new SequentialResult(enumerable);

            return outcome.Result as IResult ?? new DefaultResult(_routedMessageController, outcome);
        }

        /// <summary>
        /// Locates and perofrms type coercion for the required parameters.
        /// </summary>
        /// <param name="requiredParameters">The required parameters.</param>
        /// <param name="sourceNode">The source node.</param>
        /// <param name="handlingNode">The handling node.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        protected virtual object[] LocateAndCoerceValues(IList<RequiredParameter> requiredParameters, IInteractionNode sourceNode, IInteractionNode handlingNode, object context)
        {
            var values = new object[requiredParameters.Count];

            for (int i = 0; i < requiredParameters.Count; i++)
            {
                var parameter = requiredParameters[i];
                object value;

                if (!DetermineSpecialValue(parameter.Name.ToLower(), sourceNode, context, out value))
                {
                    var element = handlingNode.UIElement as FrameworkElement;

                    if (element != null)
                    {
                        var control = element.FindNameExhaustive<object>(parameter.Name, true);
                        var defaults = _routedMessageController.FindDefaultsOrFail(control);
                        value = defaults.GetDefaultValue(control);
                    }
#if !SILVERLIGHT
                    else
                    {
                        var fce = handlingNode.UIElement as FrameworkContentElement;

                        if (fce == null)
                            throw new CaliburnException(
                                "Cannot determine parameters unless handler node is a FrameworkElement or FrameworkContentElement.");

                        var control = fce.FindNameExhaustive<object>(parameter.Name, true);
                        var defaults = _routedMessageController.FindDefaultsOrFail(control);
                        value = defaults.GetDefaultValue(control);
                    }
#else
                    else
                        throw new CaliburnException(
                            "Cannot determine parameters unless handler node is a FrameworkElement.");
#endif
                }

                values[i] = CoerceParameter(parameter, value);
            }

            return values;
        }

        /// <summary>
        /// Coerces the values.
        /// </summary>
        /// <param name="requiredParameters">The required parameters.</param>
        /// <param name="providedValues">The provided values.</param>
        /// <param name="sourceNode">The source node.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        protected virtual object[] CoerceValues(IList<RequiredParameter> requiredParameters, object[] providedValues, IInteractionNode sourceNode, object context)
        {
            var values = new object[requiredParameters.Count];

            for (int i = 0; i < requiredParameters.Count; i++)
            {
                var possibleSpecialValue = providedValues[i] as string;

                if (possibleSpecialValue != null)
                    providedValues[i] = ResolveSpecialValue(possibleSpecialValue, sourceNode, context);

                values[i] = CoerceParameter(requiredParameters[i], providedValues[i]);
            }

            return values;
        }

        /// <summary>
        /// Determines if the key is a special value.
        /// </summary>
        /// <param name="possibleKey">The possible key.</param>
        /// <param name="sourceNode">The source node.</param>
        /// <param name="context">The context.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected virtual bool DetermineSpecialValue(string possibleKey, IInteractionNode sourceNode, object context, out object value)
        {
            Func<IInteractionNode, object, object> handler;
 
            if(_valueHandlers.TryGetValue(possibleKey, out handler))
            {
                value = handler(sourceNode, context);
                return true;
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Resolves the special value.
        /// </summary>
        /// <param name="potential">The possible special value.</param>
        /// <param name="sourceNode">The source node.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        protected virtual object ResolveSpecialValue(string potential, IInteractionNode sourceNode, object context)
        {
            var splits = potential.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            if (splits.Length == 0)
                return potential;

            var normalized = splits[0].TrimStart('$').ToLower();
            
            object value;
            if(DetermineSpecialValue(normalized, sourceNode, context, out value))
            {
                if(splits.Length > 1)
                {
                    var getter = CreateGetter(value, splits.Skip(1).ToArray());
                    return getter();
                }

                return value;
            }

            return potential;
        }

        /// <summary>
        /// Coerces the parameter.
        /// </summary>
        /// <param name="parameter">The required parameter.</param>
        /// <param name="providedValue">The provided value.</param>
        /// <returns></returns>
        protected virtual object CoerceParameter(RequiredParameter parameter, object providedValue)
        {
            return CoerceValueCore(
                parameter.Type,
                providedValue
                );
        }

        /// <summary>
        /// Coerces the provided value to the destination type.
        /// </summary>
        /// <param name="destinationType">The destination type.</param>
        /// <param name="providedValue">The provided value.</param>
        /// <returns></returns>
        public static object CoerceValueCore(Type destinationType, object providedValue)
        {
            if (providedValue == null) return GetDefaultValue(destinationType);

            var providedType = providedValue.GetType();

            if (destinationType.IsAssignableFrom(providedType))
                return providedValue;

            try
            {
                var converter = TypeDescriptor.GetConverter(destinationType);

                if (converter.CanConvertFrom(providedType))
                    return converter.ConvertFrom(providedValue);

                converter = TypeDescriptor.GetConverter(providedType);

                if (converter.CanConvertTo(destinationType))
                    return converter.ConvertTo(providedValue, destinationType);
            }
            catch
            {
                return GetDefaultValue(destinationType);
            }

            try
            {
                return Convert.ChangeType(providedValue, destinationType, CultureInfo.CurrentUICulture);
            }
            catch
            {
                return GetDefaultValue(destinationType);
            }
        }

        /// <summary>
        /// Gets the default value for a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static object GetDefaultValue(Type type)
        {
            return type.IsClass || type.IsInterface ? null : Activator.CreateInstance(type);
        }

        /// <summary>
        /// Creates the default value handlers.
        /// </summary>
        /// <returns></returns>
        private void InitializeDefaultValueHandlers()
        {
            AddValueHandler("eventargs", HandleContext);
            AddValueHandler("parameter", HandleContext);
            AddValueHandler("source", HandleSource);
            AddValueHandler("datacontext", HandleDataContext);
            AddValueHandler("value", HandleValue);
        }

        private object HandleContext(IInteractionNode sourceNode, object context)
        {
            return context;
        }

        private object HandleSource(IInteractionNode sourceNode, object context)
        {
            return sourceNode.UIElement;
        }

        private object HandleDataContext(IInteractionNode sourceNode, object context)
        {
            var fe = sourceNode.UIElement as FrameworkElement;
            if (fe != null)
                return fe.DataContext;

#if !SILVERLIGHT
            var fce = sourceNode.UIElement as FrameworkContentElement;
            if (fce != null)
                return fce.DataContext;
#endif

            throw new CaliburnException(
                string.Format(
                    "Source {0} must be a FrameworkElement or FrameworkContentElement in order to bind to its DataContext property.",
                    sourceNode.UIElement.GetType().Name
                    )
                );
        }

        private object HandleValue(IInteractionNode sourceNode, object context)
        {
            var ele = sourceNode.UIElement;
            var defaults = _routedMessageController.FindDefaultsOrFail(ele);
            return defaults.GetDefaultValue(ele);
        }

        /// <summary>
        /// Finds the property setter.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="propertyPath">The property path.</param>
        /// <returns></returns>
        protected virtual Func<object> CreateGetter(object target, string[] propertyPath)
        {
            PropertyInfo propertyInfo;

            for(int i = 0; i < propertyPath.Length; i++)
            {
                string propertyName = propertyPath[i];
                propertyInfo = target.GetType().GetProperty(propertyName);

                if(propertyInfo == null)
                    throw new CaliburnException(
                        string.Format("{0} is not a valid property path.", propertyPath.Aggregate((a, c) => a + c))
                        );

                if(i < propertyPath.Length - 1)
                {
                    target = propertyInfo.GetValue(target, null);
                    if(target == null) return () => null;
                }
                else
                {
                    return () => propertyInfo.GetValue(
                                     target,
                                     null
                                     );
                }
            }

            return () => null;
        }

        private class EmptyResult : IResult
        {
            public void Execute(IRoutedMessageWithOutcome message, IInteractionNode handlingNode)
            {
                Completed(this, null);
            }

            public event Action<IResult, Exception> Completed = delegate { };
        }
    }
}