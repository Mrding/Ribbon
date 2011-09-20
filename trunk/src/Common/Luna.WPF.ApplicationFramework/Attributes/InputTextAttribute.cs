using System;
using System.Windows;
using Caliburn.Core.Invocation;
using Caliburn.Core.Metadata;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Filters;
using Luna.WPF.ApplicationFramework.Interfaces;
using Microsoft.Practices.ServiceLocation;

namespace Luna.WPF.ApplicationFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class InputTextAttribute : Attribute, IPostProcessor, IPreProcessor
    {
        public InputTextAttribute()
        {
            PartView = "BlankView";
        }

        public int Priority
        {
            get { return 0; }
        }

        public string PartView { get; set; }

        public string ConfirmDelegate { get; set; }

        public bool AffectsTriggers
        {
            get { return false; }
        }

        void IPostProcessor.Execute(IRoutedMessage message, IInteractionNode handlingNode, MessageProcessingOutcome outcome)
        {
            ServiceLocator.Current.GetInstance<IWindowManager>().ShowDialog(outcome.Result);
        }

        bool IPreProcessor.Execute(IRoutedMessage message, IInteractionNode handlingNode, object[] parameters)
        {  
            var target = handlingNode.MessageHandler.Unwrap();

            var inputPresenter = ServiceLocator.Current.GetInstance<IDialogBoxPresenter>();
            inputPresenter.Invoker = target;
            inputPresenter.ConfirmDelegate = ConfirmDelegate;
            inputPresenter.PartView = PartView;
            parameters[0] = inputPresenter;

            return true;
        }
    }
}