using System;
using System.Reflection;
using Caliburn.Core.Invocation;
using Caliburn.Core.Metadata;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.Filters;
using Luna.Core.Extensions;
using Microsoft.Practices.ServiceLocation;

namespace Luna.WPF.ApplicationFramework.Actions
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class BackgroundActionAttribute : Attribute, IInitializable, IPostProcessor, IBackgroundActionInfo
    {
        /// <summary>
        /// the Callback method
        /// </summary>
        private IMethod _callback;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundActionAttribute"/> class.
        /// </summary>
        public BackgroundActionAttribute()
        {
            BackgroundEndMode = EndMode.End;
            Priority = -2;
        }

        /// <summary>
        /// Gets or sets the background end mode.
        /// </summary>
        /// <value>The background end mode.</value>
        public EndMode BackgroundEndMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to block interaction with the trigger during asynchronous execution.
        /// </summary>
        /// <value><c>true</c> if should block; otherwise, <c>false</c>.</value>
        public bool BlockInteraction { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [block if busy].
        /// </summary>
        /// <value><c>true</c> if [block if busy]; otherwise, <c>false</c>.</value>
        public bool BlockIfBusy { get; set; }

        /// <summary>
        /// Gets or sets the task mode.
        /// </summary>
        /// <value>The task mode.</value>
        public TaskImplMode TaskMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show busy cursor].
        /// </summary>
        /// <value><c>true</c> if [show busy cursor]; otherwise, <c>false</c>.</value>
        public bool ShowBusyCursor
        { get; set; }

        /// <summary>
        /// Gets or sets the callback method.
        /// </summary>
        /// <value>The callback.</value>
        public string Callback { get; set; }

        /// <summary>
        /// Gets or sets the before.
        /// </summary>
        /// <value>The before.</value>
        public string Before { get; set; }

        /// <summary>
        /// Gets the order the filter will be evaluated in.
        /// </summary>
        /// <value>The order.</value>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the before method.
        /// </summary>
        /// <value>The before method.</value>
        public IMethod BeforeMethod { get; set; }

        /// <summary>
        /// Initializes the filter.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="metadataContainer">The metadata container.</param>
        /// <param name="serviceLocator">The service locator.</param>
        public virtual void Initialize(Type targetType, IMetadataContainer metadataContainer, IServiceLocator serviceLocator)
        {
            if (!Callback.IsNullOrEmpty())
            {
                var methodInfo = targetType.GetMethod(Callback,
                                                      BindingFlags.Public | BindingFlags.Instance |
                                                      BindingFlags.Static);

                if (methodInfo == null)
                    throw new Exception(string.Format("Method '{0}' could not be found on '{1}'.", Callback,
                                                      targetType.FullName));

                _callback = serviceLocator.GetInstance<IMethodFactory>().CreateFrom(methodInfo);
            }
            if (!Before.IsNullOrEmpty())
            {
                var methodInfo = targetType.GetMethod(Before,
                                                      BindingFlags.Public | BindingFlags.Instance |
                                                      BindingFlags.Static);

                if (methodInfo == null)
                    throw new Exception(string.Format("Method '{0}' could not be found on '{1}'.", Before,
                                                      targetType.FullName));

                BeforeMethod = serviceLocator.GetInstance<IMethodFactory>().CreateFrom(methodInfo);
            }
        }


        /// <summary>
        /// Executes the filter.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="handlingNode">The handling node.</param>
        /// <param name="outcome">The outcome of processing the message</param>
        public virtual void Execute(IRoutedMessage message, IInteractionNode handlingNode, MessageProcessingOutcome outcome)
        {
            if (_callback == null || outcome.WasCancelled)
                return;

            outcome.Result = _callback.Invoke(handlingNode.MessageHandler.Unwrap(), outcome.Result);
            outcome.ResultType = _callback.Info.ReturnType;
        }
    }
}
