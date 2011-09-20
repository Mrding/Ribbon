using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;
using Caliburn.Core.Metadata;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Metadata;
using Castle.Windsor;
using Luna.Common;
using Luna.Core.Extensions;
using Luna.Globalization;
using Luna.WPF.ApplicationFramework.Interfaces;
using Luna.WPF.ApplicationFramework.Threads;
using Microsoft.Practices.ServiceLocation;

namespace Luna.WPF.ApplicationFramework
{
    public class DefaultPresenter : Presenter
    {
        protected static readonly IWindsorContainer Container;
        protected static readonly IWindowManager WindowManager;
        protected Action ShowMessageCompleted;

        private System.Func<CommandBinding, int> _addingCommandToInvoker;
        private readonly CommandBindingCollection _commandBindings;

        protected string MessageKey { private get; set; }


        static DefaultPresenter()
        {
            Container = ServiceLocator.Current.GetInstance<IWindsorContainer>();
            WindowManager = Container.Resolve<IWindowManager>();
        }

        protected DefaultPresenter()
        {
            _commandBindings = new CommandBindingCollection();
        }

        public ILifecycleNotifier Invoker { get; set; }

        private bool _noDialogOpened = true;
        public bool NoDialogOpened
        {
            get { return _noDialogOpened; }
            set
            {
                _noDialogOpened = value;
                this.NotifyOfPropertyChange(() => NoDialogOpened);
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                this.NotifyOfPropertyChange(() => IsLoading);
            }
        }

        public CommandBindingCollection CommandBindings
        {
            get { return _commandBindings; }
        }

        public virtual void BeginLoading()
        {
            IsLoading = true;
            UIThread.Invoke(() => { Application.Current.MainWindow.Cursor = Cursors.Wait; });
        }

        public virtual void EndLoading()
        {
            IsLoading = false;
            UIThread.Invoke(() => { Application.Current.MainWindow.Cursor = Cursors.Arrow; });
        }


        protected void CloseWithInvoker(bool value)
        {
            if (Invoker != null && value)
            {
                Invoker.WasShutdown += delegate
                {
                    this.Shutdown();
                    this.Close();
                };
            }
        }

        public virtual void Show<T>(IDictionary values) where T : IDockablePresenter
        {
            var presenter = Container.Resolve<T>(values);
            presenter.Show();
        }

        public virtual void Show<T>(IDictionary values, Action<T> beforeShowup) where T : IDockablePresenter
        {
            var presenter = Container.Resolve<T>(values);
            if (beforeShowup != null)
                beforeShowup(presenter);
            presenter.Show();
        }

        public bool? OpenDialog<T>() where T : IPresenter
        {
            var presenter = Container.Resolve<T>();
            return WindowManager.ShowDialog(presenter);
        }

        public virtual void OpenDialog<T>(IDictionary values) where T : IPresenter
        {
            var presenter = Container.Resolve<T>(values);
            WindowManager.ShowDialog(presenter);
        }

        public void OpenModelessDialog<T>(IDictionary values) where T : IPresenter
        {
            var presenter = Container.Resolve<T>(values);

        }

        public virtual void OpenDialog<T>(string key, bool modeless) where T : IPresenter
        {
            var presenter = Container.Resolve<T>(key);
            if (modeless)
                WindowManager.Show(presenter);
            else
            {
                WindowManager.ShowDialog(presenter);
            }

        }

        public virtual bool? OpenDialog<T>(string key, IDictionary values) where T : IPresenter
        {
            var presenter = Container.Resolve<T>(key, values);
            return WindowManager.ShowDialog(presenter);
        }

        public virtual void OpenDialog(object model)
        {
            WindowManager.ShowDialog(model);
        }

        public InputTextResult OpendInputText(string text)
        {
            var inputTextBoxPresenter = Container.Resolve<IDialogBoxPresenter>(new Dictionary<string, object> { { "text", text } });
            WindowManager.ShowDialog(inputTextBoxPresenter);

            return new InputTextResult { Content = inputTextBoxPresenter.Text, IsCancel = inputTextBoxPresenter.IsCancel == true };
        }


        public virtual bool CanSetWatchPoint(DateTime dateTime)
        {
            return true;
        }

        public virtual void SetWatchPoint(DateTime dateTime)
        {
            var dockSitePresenter = Container.Resolve<IDockSitePresenter>();
            if (CanSetWatchPoint(dateTime))
                dockSitePresenter.WatchPoint = dateTime;
        }

        public virtual DateTime GetWatchPoint()
        {
            var dockSitePresenter = Container.Resolve<IDockSitePresenter>();
            return dockSitePresenter.WatchPoint;
        }

        protected bool ShutdownConfirm(Action<IQuestionPresenter> setQuestion)
        {
            var presenter = Container.Resolve<IQuestionPresenter>();
            presenter.Invoker = this;

            setQuestion(presenter);
            WindowManager.ShowDialog(presenter);

            if (presenter.Answer == Answer.Cancel)
                return false; // cancel
            return true;
        }

        protected override void OnShutdown()
        {
            Invoker = null;
            base.OnShutdown();
        }

        public override void ViewLoaded(object view, object context)
        {
            Invoker.SaftyInvoke<IMetadataContainer>(m => m.GetView<object>(null)
                   .SaftyInvoke<FrameworkElement>(f => _addingCommandToInvoker = f.CommandBindings.Add));
        }

        protected void AddCommandBindingToInvoker(CommandBinding commandBinding)
        {
            _addingCommandToInvoker(commandBinding);
        }
    }
}
