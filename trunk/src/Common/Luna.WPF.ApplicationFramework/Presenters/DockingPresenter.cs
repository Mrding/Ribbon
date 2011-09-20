using System;
using System.Collections;
using System.Windows.Input;
using ActiproSoftware.Windows.Controls.Docking;
using Caliburn.PresentationFramework.ApplicationModel;
using System.Windows;
using Luna.Core.Extensions;
using Caliburn.PresentationFramework.Metadata;
using Luna.WPF.ApplicationFramework.Extensions;

namespace Luna.WPF.ApplicationFramework
{
    public interface IDockablePresenter : IPresenter
    {
        void Show();
    }

    public class DockingPresenter : DefaultPresenter
    {
        private readonly IDockSitePresenter _dockSitePresenter;
       
        public DockingPresenter()
        {
            _dockSitePresenter = Container.Resolve<IDockSitePresenter>();
        }

       

        public virtual void Show()
        {
            _dockSitePresenter.Show(this);
        }

        public virtual IPresenter Open<T>() where T : IPresenter
        {
            var presenter = Container.Resolve<T>();
            _dockSitePresenter.Show(presenter);
            return presenter;
        }

        public virtual IPresenter Open<T>(IDictionary values) where T : IPresenter
        {
            var presenter = Container.Resolve<T>(values);
            _dockSitePresenter.Show(presenter);
            return presenter;
        }

        public virtual void Activate(object model)
        {
            _dockSitePresenter.Activate(model);
        }

        public T GetActiveModel<T>() where T : class
        {
            return _dockSitePresenter.GetActiveModel<T>();
        }

        public bool Contains<T>() where T : IPresenter
        {
            return _dockSitePresenter.Contains<T>();
        }

        public T FirstOrDefault<T>(Func<T, bool> predicate) where T : IPresenter
        {
            return _dockSitePresenter.FirstOrDefault(predicate);
        }

        public override void Close()
        {
            if (Parent != null)
                Parent.Shutdown(this, delegate { });
            else
            {
                var view = this.GetView<FrameworkElement>(null);
                if (view != null)
                {
                    view.Parent.SaftyInvoke<DockingWindow>(v => v.Close());
                }
            }
        }

        protected void SetOnWatchPointChanged(Action<DateTime> @delegate)
        {
            if (_dockSitePresenter.WatchPointChanged == null)
                _dockSitePresenter.WatchPointChanged = @delegate;
            else
            {
                _dockSitePresenter.WatchPointChanged += @delegate;
            }
        }

        
    }
}
