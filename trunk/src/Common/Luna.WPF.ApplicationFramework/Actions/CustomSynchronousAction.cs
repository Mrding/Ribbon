using System;
using System.Linq;
using Caliburn.Core.Invocation;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.Actions;
using Caliburn.PresentationFramework.Filters;
using Luna.Core.Extensions;

namespace Luna.WPF.ApplicationFramework.Actions
{
    public class CustomSynchronousAction : ActionBase
    {
        public CustomSynchronousAction(IMethod method, IMessageBinder messageBinder, IFilterManager filters)
            : base(method, messageBinder, filters) { }
       
        public override void Execute(ActionMessage actionMessage, IInteractionNode handlingNode, object context)
        {
            try
            {
                var parameters = _messageBinder.DetermineParameters(actionMessage,_requirements,handlingNode,context);

                // IBeforeProcessor 继承 IPreProcessor 相当于一样
                var processors = _filters.PreProcessors.OfType<IBeforeProcessor>();
                

                // caliburn preProcessor (before) (针对 IBeforeProcessor)
                foreach (var filter in _filters.PreProcessors.Except(processors.Cast<IPreProcessor>()))
                {
                    if (!filter.Execute(actionMessage, handlingNode, parameters)) return;
                }

                // before custom aop hehavior (针对IPreProcessor)
                foreach (var before in processors.OrderByDescending(p => p.Priority))
                {
                    if (!before.BeforeExecute(actionMessage, handlingNode, parameters)) return;
                }
             
                object result = null;

                // the method will excute
                result = _method.Invoke(handlingNode.MessageHandler.Unwrap(), parameters);

                var outcome = new MessageProcessingOutcome(result ,_method.Info.ReturnType,false);

                // post
                foreach (var filter in _filters.PostProcessors)
                {
                    filter.Execute(actionMessage, handlingNode, outcome);
                }

                HandleOutcome(actionMessage, handlingNode, outcome);
            }
            catch (Exception ex)
            {
                if (!TryApplyRescue(actionMessage, handlingNode, ex))
                    throw;
                OnCompleted();
            }
        }

        private void HandleOutcome(IRoutedMessageWithOutcome message, IInteractionNode handlingNode, MessageProcessingOutcome outcome)
        {
            var result = _messageBinder.CreateResult(outcome);

            result.Completed += (r, ex) =>
            {
                if (ex != null)
                {
                    if (!TryApplyRescue(message, handlingNode, ex))
                        throw ex;
                }

                OnCompleted();
            };

            result.Execute(message, handlingNode);
        }
    }
}
