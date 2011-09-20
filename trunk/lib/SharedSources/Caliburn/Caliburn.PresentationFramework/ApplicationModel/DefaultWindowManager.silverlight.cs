#if SILVERLIGHT_30 || SILVERLIGHT_40

namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System;
    using System.ComponentModel;
    using System.Windows.Controls;
    using System.Windows.Data;
    using PresentationFramework.ApplicationModel;

    public class DefaultWindowManager : IWindowManager
    {
        private readonly IViewStrategy _viewStrategy;
        private readonly IBinder _binder;
        private bool _actuallyClosing;

        public DefaultWindowManager(IViewStrategy viewStrategy, IBinder binder)
        {
            _viewStrategy = viewStrategy;
            _binder = binder;
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The context.</param>
        /// <param name="handleShutdownModel">The handle shutdown model.</param>
        /// <returns></returns>
        public virtual void ShowDialog(object rootModel, object context, Action<ISubordinate, Action> handleShutdownModel)
        {
            var window = CreateWindow(rootModel, context, handleShutdownModel);
            window.Show();
        }

        /// <summary>
        /// Creates the window.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The context.</param>
        /// <param name="handleShutdownModel">The handle shutdown model.</param>
        /// <returns></returns>
        protected virtual ChildWindow CreateWindow(object rootModel, object context, Action<ISubordinate, Action> handleShutdownModel)
        {
            var view = EnsureWindow(rootModel, _viewStrategy.GetView(rootModel, null, context));

            _binder.Bind(rootModel, view, context);

            var presenter = rootModel as IPresenter;
            if (presenter != null)
            {
                presenter.Initialize();
                presenter.Activate();

                view.Closing += (s, e) => OnShutdownAttempted(presenter, view, handleShutdownModel, e);

                view.Closed += delegate
                {
                    presenter.Deactivate();
                    presenter.Shutdown();
                };
            }

            return view;
        }

        /// <summary>
        /// Ensures the that the view is a window or provides one.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="view">The view.</param>
        /// <returns></returns>
        protected virtual ChildWindow EnsureWindow(object model, object view)
        {
            var window = view as ChildWindow;

            if (window == null)
            {
                window = new ChildWindow
                {
                    Content = view,
                };

                var presenter = model as IPresenter;
                if (presenter != null)
                {
                    var binding = new Binding("DisplayName") { Mode = BindingMode.TwoWay };
                    window.SetBinding(ChildWindow.TitleProperty, binding);
                }
            }

            return window;
        }

        /// <summary>
        /// Called when shutdown attempted.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="view">The view.</param>
        /// <param name="handleShutdownModel">The handler for the shutdown model.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        protected virtual void OnShutdownAttempted(IPresenter rootModel, ChildWindow view, Action<ISubordinate, Action> handleShutdownModel, CancelEventArgs e)
        {
            if (_actuallyClosing || rootModel.CanShutdown())
            {
                _actuallyClosing = false;
                return;
            }

            bool runningAsync = false;

            var custom = rootModel as ISupportCustomShutdown;
            if (custom != null && handleShutdownModel != null)
            {
                var shutdownModel = custom.CreateShutdownModel();
                var shouldEnd = false;

                handleShutdownModel(
                    shutdownModel,
                    () =>
                    {
                        var canShutdown = custom.CanShutdown(shutdownModel);
                        if (runningAsync && canShutdown)
                        {
                            _actuallyClosing = true;
                            view.Close();
                        }
                        else e.Cancel = !canShutdown;

                        shouldEnd = true;
                    });

                if (shouldEnd)
                    return;
            }

            runningAsync = e.Cancel = true;
        }
    }
}

#endif