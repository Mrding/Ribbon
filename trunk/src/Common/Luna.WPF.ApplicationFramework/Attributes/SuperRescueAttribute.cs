using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using Caliburn.Core.Invocation;
using Caliburn.Core.Metadata;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.Actions;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Filters;
using Castle.DynamicProxy;
using Luna.Common.Attributes;
using Luna.Common.Extensions;
using Luna.Core.Extensions;
using Luna.Globalization;
using Luna.WPF.ApplicationFramework.Interfaces;
using Microsoft.Practices.ServiceLocation;

namespace Luna.WPF.ApplicationFramework.Attributes
{
    /// <summary>
    /// A super rescue filter.
    /// </summary>
    public class SuperRescueAttribute : HandleExceptionAttribute, IInitializable, IRescue
    {
        //for caliburn use
        private IMethod _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="RescueAttribute"/> class.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        public SuperRescueAttribute(string methodName)
            : this(methodName, null, null) { }

        public SuperRescueAttribute(string methodName, string messageKey, string title)
            : this(methodName, messageKey, title, false)
        { }

        public SuperRescueAttribute(string methodName, string message, string title, bool runMethodBeforeDialog)
            : base(methodName)
        {
            Message = message;
            Title = title;
            RunMethodBeforeDialog = runMethodBeforeDialog;
        }

        /// <summary>
        /// Initializes the filter.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="metadataContainer">The metadata container.</param>
        /// <param name="serviceLocator">The serviceLocator.</param>
        public void Initialize(Type targetType, IMetadataContainer metadataContainer, IServiceLocator serviceLocator)
        {
            if (_method == null)
                _method = serviceLocator.GetInstance<IMethodFactory>().CreateFrom(_methodName.AsMethodInfo(targetType));
        }
      
        public string Message { get; set; }
     
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [run method before dialog].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [run method before dialog]; otherwise, <c>false</c>.
        /// </value>
        public bool RunMethodBeforeDialog { get; set; }

        /// <summary>
        /// Gets the order the filter will be evaluated in.
        /// </summary>
        /// <value>The order.</value>
        public int Priority { get; set; }

        /// <summary>
        /// Handles an <see cref="Exception"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="handlingNode">The handling node.</param>
        /// <param name="exception">The exception.</param>
        /// <returns>
        /// true if the exception was handled, false otherwise
        /// </returns>
        public bool Handle(IRoutedMessage message, IInteractionNode handlingNode, Exception exception)
        {
            object result;
            if (RunMethodBeforeDialog)
            {
                result = RunMethod(message, handlingNode, exception);
                ShowDialog(exception);
            }
            else
            {
                ShowDialog(exception);
                result = RunMethod(message, handlingNode, exception);
            }

            if (_method.Info.ReturnType == typeof(bool))
                return (bool)result;
            return true;
        }

        private object RunMethod(IRoutedMessage message, IInteractionNode handlingNode, Exception exception)
        {
            return _method.Invoke(handlingNode.MessageHandler.Unwrap(), GetParameters(message, handlingNode, exception));
        }

        private void ShowDialog(Exception exception)
        {
            if (Message.IsNotNullOrEmpty())
            {
                ServiceLocator.Current.GetInstance<IMessagePresenter>()
                    .Self(q =>
                    {
                        //q.DisplayName = LanguageReader.GetValue(Title);
                        q.Text = LanguageReader.GetValue(Message);
                        q.Details = string.Format("{0}:{1}\r\n{2}", exception.GetType() ,exception.Message, exception.StackTrace);
                        ServiceLocator.Current.GetInstance<IWindowManager>().ShowDialog(q);
                    });
            }
        }

        /// <summary>
        /// For Aop behavior
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public override bool Predicate(IInvocation invocation, Exception ex)
        {
            if(RunMethodBeforeDialog)
                ShowDialog(ex);

            var shouldThrow = base.Predicate(invocation, ex);

            if(!RunMethodBeforeDialog)
                ShowDialog(ex);

            return shouldThrow;
        }

        private object[] GetParameters(IRoutedMessage message, IInteractionNode handlingNode, Exception exception)
        {
            var target = handlingNode.MessageHandler.Unwrap();
            var callParameters = target.GetType().GetMethod(message.As<ActionMessage>().MethodName, BindingFlags.Instance | BindingFlags.Public).GetParameters();
            var rescueParameters = _method.Info.GetParameters();
            return InvocationExtension.GetArguments(PrepareParameters(message.Parameters, exception), callParameters, rescueParameters);
        }

        private object[] PrepareParameters(FreezableCollection<Parameter> parameters, Exception exception)
        {
            var param = new List<object>(parameters.Count + 1);
            parameters.ForEach(p => param.Add(p.Value));
            param.Add(exception);
            return param.ToArray();
        }
    }
}
