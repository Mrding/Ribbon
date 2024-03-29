namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// An implementation of <see cref="IPresenterManager"/> that tracks the state of multiple presenters.
    /// </summary>
    public class MultiPresenterManager : PresenterBase, IPresenterManager, ISupportCustomShutdown
    {
        private readonly IObservableCollection<IPresenter> _presenters = new BindableCollection<IPresenter>();
        private IPresenter _currentPresenter;
        private bool _changingThroughProperty;

        /// <summary>
        /// Gets or sets the current presenter.
        /// </summary>
        /// <value>The current presenter.</value>
        public virtual IPresenter CurrentPresenter
        {
            get { return _currentPresenter; }
            set
            {
                if (Equals(_currentPresenter,value))
                    return;
                
                _changingThroughProperty = true;

                Open(
                    value,
                    success =>
                    {
                        _changingThroughProperty = false;
                        _currentPresenter = value;
                        NotifyOfPropertyChange("CurrentPresenter");
                    });
            }
        }

        /// <summary>
        /// Gets the presenters that are currently managed.
        /// </summary>
        /// <value>The presenters.</value>
        public virtual IObservableCollection<IPresenter> Presenters
        {
            get { return _presenters; }
        }

        /// <summary>
        /// Determines whether this instance can shutdown.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can shutdown; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanShutdown()
        {
            foreach (var presenter in _presenters)
            {
                if (!presenter.CanShutdown()) return false;
            }
            
            return true;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public override void Initialize()
        {
            if (!IsInitialized)
            {
                if (_currentPresenter != null)
                    _currentPresenter.Initialize();

                OnInitialize();
                IsInitialized = true;
            }
        }

        /// <summary>
        /// Shutdowns this instance.
        /// </summary>
        public override void Shutdown()
        {
            foreach (var presenter in _presenters)
            {
                presenter.Shutdown();
            }
            OnShutdown();
            
        }

        /// <summary>
        /// Activates this instance.
        /// </summary>
        public override void Activate()
        {
            if (!IsActive)
            {
                if (_currentPresenter != null)
                    _currentPresenter.Activate();

                OnActivate();
                IsActive = true;
            }
        }

        /// <summary>
        /// Deactivates this instance.
        /// </summary>
        public override void Deactivate()
        {
            if (IsActive)
            {
                if (_currentPresenter != null)
                    _currentPresenter.Deactivate();

                OnDeactivate();
                IsActive = false;
            }
        }

        /// <summary>
        /// Opens the specified presenter.
        /// </summary>
        /// <param name="presenter">The presenter.</param>
        /// <param name="completed">Called when the open action is finished.</param>
        public virtual void Open(IPresenter presenter, Action<bool> completed)
        {
            if (presenter == null)
            {
                completed(false);
                return;
            }

            if (presenter.Equals(_currentPresenter))
            {
                completed(true);
                return;
            }

            if (_currentPresenter != null)
                _currentPresenter.Deactivate();

            presenter = EnsurePresenter(presenter);

            if (IsActive)
                presenter.Activate();

            ChangeCurrentPresenterCore(presenter);

            completed(true);
        }

        private IPresenter EnsurePresenter(IPresenter presenter)
        {
            int index = _presenters.IndexOf(presenter);

            if (index == -1)
                _presenters.Add(presenter);
            else presenter = _presenters[index];

            var node = presenter as IPresenterNode;
            if (node != null) node.Parent = this;

            if (IsInitialized)
                presenter.Initialize();

            return presenter;
        }

        /// <summary>
        /// Shuts down the current presenter.
        /// </summary>
        /// <param name="completed">Called when the shutdown action is finished.</param>
        public virtual void ShutdownCurrent(Action<bool> completed)
        {
            if (_currentPresenter == null)
            {
                completed(true);
                return;
            }

            CanShutdownPresenter(
                _currentPresenter,
                isSuccess =>
                {
                    if (!isSuccess)
                    {
                        completed(false);
                        return;
                    }

                    var lastCurrent = _currentPresenter;
                    int index = _presenters.IndexOf(lastCurrent);
                    var next = DetermineNextPresenterToActivate(index);

                    if (next != null)
                    {
                        next = EnsurePresenter(next);

                        if (IsActive)
                            next.Activate();
                    }

                    ChangeCurrentPresenterCore(next);

                    _presenters.Remove(lastCurrent);
                    lastCurrent.Deactivate();
                    lastCurrent.Shutdown();

                    var node = lastCurrent as IPresenterNode;
                    if (node != null) node.Parent = null;

                    completed(true);
                });
        }

        /// <summary>
        /// Gets the next presenter to activate after another presenter is shutdown.
        /// </summary>
        /// <param name="lastIndex">The index of the previously removed presenter.</param>
        /// <returns>The presenter to activate or null if none.</returns>
        protected virtual IPresenter DetermineNextPresenterToActivate(int lastIndex)
        {
            var toRemoveAt = lastIndex - 1;

            if (toRemoveAt == -1 && _presenters.Count > 1)
                return _presenters[1];
            if (toRemoveAt > -1 && toRemoveAt < _presenters.Count - 1)
                return _presenters[toRemoveAt];
            return null;
        }

        /// <summary>
        /// Shuts down the specified presenter.
        /// </summary>
        /// <param name="presenter">The presenter.</param>
        /// <param name="completed">Called when the shutdown action is finished.</param>
        public virtual void Shutdown(IPresenter presenter, Action<bool> completed)
        {
            if (_currentPresenter == presenter)
            {
                ShutdownCurrent(completed);
                return;
            }

            if (presenter == null)
            {
                completed(true);
                return;
            }

            CanShutdownPresenter(
                presenter,
                isSuccess =>
                {
                    if (!isSuccess)
                    {
                        completed(false);
                        return;
                    }

                    _presenters.Remove(presenter);

                    presenter.Deactivate();
                    presenter.Shutdown();

                    var node = presenter as IPresenterNode;
                    if (node != null) node.Parent = null;

                    completed(true);
                });
        }

        /// <summary>
        /// Changes the current presenter.
        /// </summary>
        /// <param name="newCurrent">The new current presenter.</param>
        protected virtual void ChangeCurrentPresenterCore(IPresenter newCurrent)
        {
            _currentPresenter = newCurrent;

            if (!_changingThroughProperty)
                NotifyOfPropertyChange("CurrentPresenter");
        }

        /// <summary>
        /// Creates the shutdown model.
        /// </summary>
        /// <returns></returns>
        ISubordinate ISupportCustomShutdown.CreateShutdownModel()
        {
            var model = new SubordinateGroup(this);

            foreach (var presenter in _presenters)
            {
                if (presenter.CanShutdown()) continue;

                var custom = presenter as ISupportCustomShutdown;

                if (custom != null)
                {
                    var childModel = custom.CreateShutdownModel();

                    if (childModel != null)
                        model.Add(childModel);
                }
            }

            return model;
        }

        /// <summary>
        /// Determines whether this instance can shutdown based on the evaluated shutdown model.
        /// </summary>
        /// <param name="shutdownModel">The shutdown model.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can shutdown; otherwise, <c>false</c>.
        /// </returns>
        bool ISupportCustomShutdown.CanShutdown(ISubordinate shutdownModel)
        {
            var subordinateGroup = (SubordinateGroup)shutdownModel;
            var presentersToRemove = new List<IPresenter>();
            bool result = true;

            foreach (var presenter in _presenters)
            {
                var match = (from child in subordinateGroup
                             where child.Master == presenter
                             select child).FirstOrDefault();

                if (match == null)
                {
                    if (presenter.CanShutdown())
                        presentersToRemove.Add(presenter);
                    else result = false;
                }
                else
                {
                    var custom = (ISupportCustomShutdown)presenter;
                    var canShutdown = custom.CanShutdown(match);

                    if (canShutdown)
                        presentersToRemove.Add(presenter);
                    else result = false;
                }
            }

            FinalizeShutdown(result, presentersToRemove);

            if (_currentPresenter == null || !_presenters.Contains(_currentPresenter))
            {
                if (_presenters.Count > 0)
                {
                    _presenters[0].Activate();
                    ChangeCurrentPresenterCore(_presenters[0]);
                }
            }

            return result;
        }

        /// <summary>
        /// Finalizes the shutdown of some or all child presenters.
        /// </summary>
        /// <param name="canShutdown">if set to <c>true</c> all presenters in the Presenters collection can shutdown.</param>
        /// <param name="allowedToShutdown">Only the presenters which are allowed to shutdown.</param>
        protected virtual void FinalizeShutdown(bool canShutdown, IEnumerable<IPresenter> allowedToShutdown)
        {
            foreach (var presenter in allowedToShutdown)
            {
                _presenters.Remove(presenter);
                presenter.Deactivate();
                presenter.Shutdown();
            }
        }
    }
}