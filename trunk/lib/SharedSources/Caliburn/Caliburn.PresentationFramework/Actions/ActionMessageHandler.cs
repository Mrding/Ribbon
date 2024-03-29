﻿using Caliburn.Core.MemoryManagement;

namespace Caliburn.PresentationFramework.Actions
{
    using System.Linq;
    using Core;
    using Core.Metadata;

    /// <summary>
    /// An implementation of <see cref="IRoutedMessageController"/> for action messages.
    /// </summary>
    public class ActionMessageHandler : MetadataContainer, IRoutedMessageHandler
    {
        private readonly WeakReference<object> _weakTarget;
        private readonly IActionHost _host;
        private IInteractionNode _node;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionMessageHandler"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="target">The target.</param>
        public ActionMessageHandler(IActionHost host, object target)
        {
            _weakTarget = new WeakReference<object>(target);
            _host = host;

            _host.SelectMany(x => x.Filters.HandlerAware)
                .Union(_host.Filters.HandlerAware)
                .Apply(x => x.MakeAwareOf(this));
        }

        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <value>The host.</value>
        public IActionHost Host
        {
            get { return _host; }
        }

        /// <summary>
        /// Initializes this handler on the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public void Initialize(IInteractionNode node)
        {
            _node = node;
        }

        /// <summary>
        /// Gets the data context value.
        /// </summary>
        /// <returns></returns>
        public object Unwrap()
        {
            if (_weakTarget != null && _weakTarget.IsAlive)
                return _weakTarget.Target;
            return null;
        }

        /// <summary>
        /// Determines whethyer the target can handle the specified action.
        /// </summary>
        /// <param name="message">The action details.</param>
        /// <returns></returns>
        public bool Handles(IRoutedMessage message)
        {
            var actionMessage = message as ActionMessage;
            return actionMessage != null && _host.GetAction(actionMessage) != null;
        }

        /// <summary>
        /// Processes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">An object that provides additional context for message processing.</param>
        public void Process(IRoutedMessage message, object context)
        {
            var actionMessage = message as ActionMessage;

            if (actionMessage != null)
                _host.GetAction(actionMessage).Execute(actionMessage, _node, context);
            else throw new CaliburnException("The handler cannot process this message.");
        }

        /// <summary>
        /// Updates the availability of the trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        public void UpdateAvailability(IMessageTrigger trigger)
        {
            var actionMessage = trigger.Message as ActionMessage;

            if (actionMessage == null)
                throw new CaliburnException("The handler cannot update availability for this trigger.");

            var action = _host.GetAction(actionMessage);
            if (!action.HasTriggerEffects()) return;

            bool isAvailable = action.ShouldTriggerBeAvailable(actionMessage, _node);
            trigger.UpdateAvailabilty(isAvailable);
        }

        /// <summary>
        /// Makes the handler aware of a specific trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        public void MakeAwareOf(IMessageTrigger trigger)
        {
            var actionMessage = trigger.Message as ActionMessage;
            if (actionMessage == null) return;

            var action = _host.GetAction(actionMessage);

            if (action.HasTriggerEffects())
            {
                bool isAvailable = action.ShouldTriggerBeAvailable(actionMessage, _node);
                trigger.UpdateAvailabilty(isAvailable);
            }

            action.Filters.HandlerAware.Apply(x => x.MakeAwareOf(this, trigger));
        }
    }
}