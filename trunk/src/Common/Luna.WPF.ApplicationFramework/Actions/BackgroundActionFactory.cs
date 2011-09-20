using System.Reflection;
using Caliburn.Core.Invocation;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.Actions;

namespace Luna.WPF.ApplicationFramework.Actions
{
    public class CustomActionFactory : ActionFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionFactory"/> class.
        /// </summary>
        /// <param name="methodFactory">The method factory.</param>
        /// <param name="messageBinder">The parameter binder used by actions.</param>
        public CustomActionFactory(IMethodFactory methodFactory, IMessageBinder messageBinder)
            : base(methodFactory, messageBinder)
        {
        }

        protected override IAction CreateAction(IActionHost host, MethodInfo methodInfo)
        {
            var method = _methodFactory.CreateFrom(methodInfo);
            var asyncAtt = method.GetMetadata<AsyncActionAttribute>();
            var backAtt = method.GetMetadata<BackgroundActionAttribute>();

            var filters = host.GetFilterManager(method);

            TryAddCanExecute(filters, method);

            if (asyncAtt != null)
                return new AsynchronousAction(method, _messageBinder, filters, asyncAtt.BlockInteraction);
            if (backAtt != null && backAtt.BackgroundEndMode != EndMode.Unspecified)
                return new BackgroundAction(method, _messageBinder, filters, backAtt.BlockInteraction);
            return new CustomSynchronousAction(method, _messageBinder, filters);
        }
    }
}
