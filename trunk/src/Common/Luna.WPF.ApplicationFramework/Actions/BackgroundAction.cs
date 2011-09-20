using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Caliburn.Core.Invocation;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.Actions;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Filters;
using Luna.Core.Extensions;

namespace Luna.WPF.ApplicationFramework.Actions
{
    /// <summary>
    /// A  Background <see cref="IAction"/>.
    /// </summary>
    public class BackgroundAction : ActionBase
    {
        private readonly bool _blockInteraction;
        private int _runningCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundAction"/> class.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="messageBinder">The message binder.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="blockInteraction">if set to <c>true</c> [block interaction].</param>
        public BackgroundAction(IMethod method, IMessageBinder messageBinder, IFilterManager filters, bool blockInteraction)
            : base(method, messageBinder, filters)
        {
            _blockInteraction = blockInteraction;
        }

        /// <summary>
        /// Gets a value indicating whether to block intaction with the trigger during async execution.
        /// </summary>
        /// <value><c>true</c> if should block; otherwise, <c>false</c>.</value>
        public bool BlockInteraction
        {
            get { return _blockInteraction; }
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public IBackgroundThreadTask CurrentBackgroundTask
        { get; set; }

        /// <summary>
        /// Gets or sets the current background action info.
        /// </summary>
        /// <value>The current background action info.</value>
        public IBackgroundActionInfo CurrentBackgroundActionInfo
        { get; set; }

        /// <summary>
        /// Executes the specified this action on the specified target.
        /// </summary>
        /// <param name="actionMessage">The action message.</param>
        /// <param name="handlingNode">The node.</param>
        /// <param name="context">The context.</param>
        public override void Execute(ActionMessage actionMessage, IInteractionNode handlingNode, object context)
        {
            try
            {
                TryUpdateTrigger(actionMessage, handlingNode, true);

                var parameters = _messageBinder.DetermineParameters(
                    actionMessage,
                    _requirements,
                    handlingNode,
                    context
                    );

                var beforeProcessors = _filters.PreProcessors.OfType<IBeforeProcessor>();

                foreach (var filter in _filters.PreProcessors.Except(beforeProcessors.Cast<IPreProcessor>()))
                {
                    if (!filter.Execute(actionMessage, handlingNode, parameters)) return;
                }

                //Before
                foreach (var before in beforeProcessors.OrderByDescending(p => p.Priority))
                {
                    if (!before.BeforeExecute(actionMessage, handlingNode, parameters)) return;
                }

                CurrentBackgroundActionInfo = _filters.PostProcessors.OfType<IBackgroundActionInfo>().Single();

                var container = handlingNode.MessageHandler.Unwrap() as IExtendedPresenter;
                CurrentBackgroundTask = container.GetMetadata<IBackgroundThreadTask>();
                if (CurrentBackgroundTask == null)
                {
                    CurrentBackgroundTask = TaskImplFactory.CreateTask(CurrentBackgroundActionInfo.TaskMode);

                    container.WasShutdown += (s, e) => CurrentBackgroundTask.Dispose();
                    container.AddMetadata(CurrentBackgroundTask);
                    AttachTaskEvents(actionMessage, handlingNode, parameters);
                }

                //Exectue Before method
                if (CurrentBackgroundActionInfo.BeforeMethod != null)
                {
                    var result = CurrentBackgroundActionInfo.BeforeMethod.Invoke(container, parameters);
                    bool canProceed = true;
                    if (CurrentBackgroundActionInfo.BeforeMethod.Info.ReturnType == typeof(bool))
                        canProceed = result.As<bool>();
                    if (!canProceed)
                        return;
                }

                DoExecute(actionMessage, handlingNode, parameters);
            }
            catch (Exception ex)
            {
                TryUpdateTrigger(actionMessage, handlingNode, false);
                if (!TryApplyRescue(actionMessage, handlingNode, ex))
                    throw;
                OnCompleted();
            }
        }

        /// <summary>
        /// Attaches the task complete.
        /// </summary>
        /// <param name="actionMessage">The action message.</param>
        /// <param name="handlingNode">The handling node.</param>
        /// <param name="parameters">The parameters.</param>
        protected void AttachTaskEvents(ActionMessage actionMessage, IInteractionNode handlingNode, object[] parameters)
        {
            if (CurrentBackgroundActionInfo.ShowBusyCursor)
            {
                CurrentBackgroundTask.Starting +=
                    (s, e) => Caliburn.Core.Invocation.Execute.OnUIThread(
                        () =>
                        {
                            FrameworkElement element = handlingNode.UIElement as FrameworkElement;
                            element.Cursor = Cursors.Wait;
                        });

                CurrentBackgroundTask.Completed +=
                    (s, e) => Caliburn.Core.Invocation.Execute.OnUIThread(
                        () =>
                        {
                            FrameworkElement element = handlingNode.UIElement as FrameworkElement;
                            element.Cursor = Cursors.Arrow;
                        });
            }
            CurrentBackgroundTask.Completed +=
                (s, e) => Caliburn.Core.Invocation.Execute.OnUIThread(
                              () =>
                              {
                                  Interlocked.Decrement(ref _runningCount);
                                  if (e.Error != null)
                                  {
                                      TryUpdateTrigger(actionMessage, handlingNode, false);
                                      if (!TryApplyRescue(actionMessage, handlingNode, e.Error))
                                          throw e.Error;
                                      OnCompleted();
                                  }
                                  else
                                  {
                                      try
                                      {
                                          var outcome = new MessageProcessingOutcome(
                                              e.Result,
                                              _method.Info.ReturnType,
                                              e.Cancelled
                                              );

                                          foreach (var filter in _filters.PostProcessors)
                                          {
                                              filter.Execute(actionMessage, handlingNode, outcome);
                                          }

                                          var result = _messageBinder.CreateResult(outcome);

                                          result.Completed += (r, ex) =>
                                          {
                                              TryUpdateTrigger(actionMessage, handlingNode, false);

                                              if (ex != null)
                                              {
                                                  if (!TryApplyRescue(actionMessage, handlingNode, ex))
                                                      throw ex;
                                              }

                                              OnCompleted();
                                          };

                                          result.Execute(actionMessage, handlingNode);
                                      }
                                      catch (Exception ex)
                                      {
                                          TryUpdateTrigger(actionMessage, handlingNode, false);
                                          if (!TryApplyRescue(actionMessage, handlingNode, ex))
                                              throw;
                                          OnCompleted();
                                      }
                                  }
                              });

            Interlocked.Increment(ref _runningCount);
        }

        protected void DoExecute(ActionMessage actionMessage, IInteractionNode handlingNode, object[] parameters)
        {
            if (CurrentBackgroundActionInfo.BlockIfBusy && CurrentBackgroundTask.IsBusy)
                return;
            var context = new BackgroundActionContext(CurrentBackgroundActionInfo.BackgroundEndMode, () => _method.Invoke(handlingNode.MessageHandler.Unwrap(), parameters));
            CurrentBackgroundTask.Enqueue(context);
        }

        /// <summary>
        /// Determines how this instance affects trigger availability.
        /// </summary>
        /// <param name="actionMessage">The action message.</param>
        /// <param name="handlingNode">The node.</param>
        /// <returns>
        /// 	<c>true</c> if this instance enables triggers; otherwise, <c>false</c>.
        /// </returns>
        public override bool ShouldTriggerBeAvailable(ActionMessage actionMessage, IInteractionNode handlingNode)
        {
            if (BlockInteraction && _runningCount > 0) return false;
            return base.ShouldTriggerBeAvailable(actionMessage, handlingNode);
        }

        /// <summary>
        /// Tries to update trigger.
        /// </summary>
        /// <param name="actionMessage">The action message.</param>
        /// <param name="handlingNode">The handling node.</param>
        /// <param name="forceDisabled">if set to <c>true</c> [force disabled].</param>
        protected virtual void TryUpdateTrigger(ActionMessage actionMessage, IInteractionNode handlingNode, bool forceDisabled)
        {
            if (!BlockInteraction)
                return;

            foreach (var messageTrigger in actionMessage.Source.Triggers)
            {
                if (!messageTrigger.Message.Equals(actionMessage))
                    continue;

                if (forceDisabled)
                {
                    messageTrigger.UpdateAvailabilty(false);
                    return;
                }

                if (this.HasTriggerEffects())
                {
                    bool isAvailable = ShouldTriggerBeAvailable(actionMessage, handlingNode);
                    messageTrigger.UpdateAvailabilty(isAvailable);
                }
                else messageTrigger.UpdateAvailabilty(true);

                return;
            }
        }
    }
}
