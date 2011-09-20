using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using ActiproSoftware.Windows.Controls.Docking;
using ActiproSoftware.Windows.Media;
using Caliburn.Core.Metadata;
using Caliburn.PresentationFramework.ApplicationModel;
using Luna.Core.Extensions;
using Luna.GUI.Views;
using Luna.WPF.ApplicationFramework;
using Luna.WPF.ApplicationFramework.Extensions;
using Luna.WPF.ApplicationFramework.Interfaces;

namespace Luna.GUI.Presenters
{
    //[View(typeof(DockSiteView))]
    [Singleton(typeof(IDockSitePresenter))]
    public class DockSitePresenter : Presenter, IDockSitePresenter
    {
        private readonly Dictionary<IPresenter, DockingWindow> _cache = new Dictionary<IPresenter, DockingWindow>();

        private readonly IViewStrategy _viewStrategy;
        private readonly IBinder _binder;
        private readonly IShellPresenter _shell;
        private DockSite _dockSite;


        public DockSitePresenter(IViewStrategy viewStrategy, IBinder binder, IShellPresenter shell)
        {
            _viewStrategy = viewStrategy;
            _binder = binder;
            _shell = shell;
        }

        private DateTime _watchPoint;
        public DateTime WatchPoint
        {
            get { return _watchPoint; }
            set
            {
                if (value == default(DateTime) || value == _watchPoint)
                    return;
                _watchPoint = value;
                NotifyOfPropertyChange("WatchPoint");
                if (WatchPointChanged != null)
                    WatchPointChanged(_watchPoint);
            }
        }

        private IPresenter _activePresenter;
        public IPresenter ActivePresenter
        {
            get { return _activePresenter; }
            private set
            {
                if (value != null)
                    value.Activate();

                _activePresenter = value;
                _shell.ActivePlan = value;
                this.NotifyOfPropertyChange("ActivePresenter");
            }
        }

        public Action<DateTime> WatchPointChanged { get; set; }

        public override void ViewLoaded(object view, object context)
        {
            _dockSite = view as DockSite;
            if (_dockSite == null)
                throw new ArgumentNullException(Properties.Resources.NullDockSiteException);

            _dockSite.WindowActivated += (sender, e) =>
                                             {
                                                 ActivePresenter = GetPresenter(e.Window.Content) as Presenter;
                                             };
            _dockSite.WindowDeactivated += (sender, e) => GetPresenter(e.Window.Content).Deactivate();
            _dockSite.WindowClosing += (sender, e) =>
            {
                e.Cancel = GetPresenter(e.Window.Content).CanShutdown() == false;
            };
            _dockSite.WindowClosed += (sender, e) =>
                                          {
                                              var model = GetPresenter(e.Window.Content);
                                              
                                              if(_dockSite.ActiveWindow != null)
                                                ActivePresenter = GetPresenter(_dockSite.ActiveWindow.Content);
                                              else
                                                  ActivePresenter = null;
                                              
                                              _cache.Remove(model);
                                              model.Shutdown();
                                              WatchPointChanged = null;

                                              GC.Collect();
                                          };

            //var dpdDataContext = DependencyPropertyDescriptor.FromProperty(DockSite.ActiveWindowProperty, typeof(DockingWindow));
            //dpdDataContext.AddValueChanged(_dockSite, delegate
            //{
            //    ActivePresenter = _dockSite.ActiveWindow.DataContext as Presenter;
            //});
        }

        private static IPresenter GetPresenter(object view)
        {
            var fe = view as FrameworkElement;
            if (fe == null)
                return null;

            var presenter = fe.DataContext as IPresenter;
            if (presenter == null)
                throw new Exception(Properties.Resources.NullRootModelException);
            return presenter;
        }

        private DockingWindow CreateDockingWindow(object rootModel, DockSite siteTarget)
        {
            var view = _viewStrategy.GetView(rootModel, null, null);
            var presenter = rootModel as IPresenter;
            if (presenter != null)
            {
                if (_cache.ContainsKey(presenter))
                    return _cache[presenter];
                var initialDockDirection = Direction.Left;
                var metadata = view.GetDockingWindowMetadata();
                DockingWindow dockingWindow = new DocumentWindow(siteTarget);

                if (metadata == null)
                {
                    dockingWindow.SetBinding(
                         DockingWindow.TitleProperty,
                         new Binding
                         {
                             BindsDirectlyToSource = true,
                             Source = presenter.DisplayName,
                             Mode = BindingMode.OneWay
                         });
                }
                else
                {
                    if (metadata.IsToolWindow)
                    {
                        var toolWindow = new ToolWindow(siteTarget);

                        #region ToolWindow Property Bindings

                        toolWindow.SetBinding(
                            ToolWindow.CanRaftProperty,
                            new Binding
                            {
                                Source = metadata,
                                Path = new PropertyPath(DockingWindowMetadata.CanRaftProperty),
                                Mode = BindingMode.OneWay
                            });

                        toolWindow.SetBinding(
                            ToolWindow.CanAutoHideProperty,
                            new Binding
                            {
                                Source = metadata,
                                Path = new PropertyPath(DockingWindowMetadata.CanAutoHideProperty),
                                Mode = BindingMode.OneWay
                            });

                        toolWindow.SetBinding(
                            ToolWindow.CanBecomeDocumentProperty,
                            new Binding
                            {
                                Source = metadata,
                                Path = new PropertyPath(DockingWindowMetadata.CanBecomeDocumentProperty),
                                Mode = BindingMode.OneWay
                            });

                        #endregion

                        dockingWindow = toolWindow;
                    }
                    else
                        dockingWindow.Content = view;

                    #region DockingWindow Property Bindings

                    dockingWindow.SetBinding(
                        DockingWindow.TitleProperty,
                        new Binding
                        {
                            Source = metadata,
                            Path = new PropertyPath(DockingWindowMetadata.TitleProperty),
                            Mode = BindingMode.OneWay
                        });

                    dockingWindow.SetBinding(
                        DockingWindow.CanCloseProperty,
                        new Binding
                        {
                            Source = metadata,
                            Path = new PropertyPath(DockingWindowMetadata.CanCloseProperty),
                            Mode = BindingMode.OneWay
                        });

                    dockingWindow.SetBinding(
                        DockingWindow.CanDragProperty,
                        new Binding
                        {
                            Source = metadata,
                            Path = new PropertyPath(DockingWindowMetadata.CanDragProperty),
                            Mode = BindingMode.OneWay
                        });

                    dockingWindow.SetBinding(
                        DockingWindow.CanAttachProperty,
                        new Binding
                        {
                            Source = metadata,
                            Path = new PropertyPath(DockingWindowMetadata.CanAttachProperty),
                            Mode = BindingMode.OneWay
                        });

                    dockingWindow.SetBinding(
                        DockingWindow.CanDockLeftProperty,
                        new Binding
                        {
                            Source = metadata,
                            Path = new PropertyPath(DockingWindowMetadata.CanDockLeftProperty),
                            Mode = BindingMode.OneWay
                        });

                    dockingWindow.SetBinding(
                        DockingWindow.CanDockRightProperty,
                        new Binding
                        {
                            Source = metadata,
                            Path = new PropertyPath(DockingWindowMetadata.CanDockRightProperty),
                            Mode = BindingMode.OneWay
                        });

                    dockingWindow.SetBinding(
                        DockingWindow.CanDockTopProperty,
                        new Binding
                        {
                            Source = metadata,
                            Path = new PropertyPath(DockingWindowMetadata.CanDockTopProperty),
                            Mode = BindingMode.OneWay
                        });

                    dockingWindow.SetBinding(
                        DockingWindow.CanDockBottomProperty,
                        new Binding
                        {
                            Source = metadata,
                            Path = new PropertyPath(DockingWindowMetadata.CanDockBottomProperty),
                            Mode = BindingMode.OneWay
                        });

                    #endregion

                    switch (metadata.DefaultDock)
                    {
                        case DockSiteDock.Left:
                            initialDockDirection = Direction.Left;
                            break;
                        case DockSiteDock.Right:
                            initialDockDirection = Direction.Right;
                            break;
                        case DockSiteDock.Top:
                            initialDockDirection = Direction.Top;
                            break;
                        case DockSiteDock.Bottom:
                            initialDockDirection = Direction.Bottom;
                            break;
                        case DockSiteDock.Content:
                            initialDockDirection = Direction.Content;
                            break;
                    }

                    if (dockingWindow is ToolWindow)
                    {
                        var dockTarget = GetDockTarget(siteTarget, initialDockDirection);

                        if (metadata.Undock)
                        {
                            dockingWindow.Float(metadata.DefaultSize);
                        }
                        else
                        {
                            ((ToolWindow)dockingWindow).Dock(dockTarget, (metadata.CreateNewDockingGroup) && !dockTarget.Equals(siteTarget) ? Direction.Content
                                          : initialDockDirection);
                        }
                    }
                }
                _binder.Bind(rootModel, view, null);
                dockingWindow.Content = view;
                presenter.Initialize();
                _cache[presenter] = dockingWindow;

                return _cache[presenter];
            }

            throw new Exception(Properties.Resources.NullRootModelException);
        }

        private static IDockTarget GetDockTarget(DockSite dockSite, Direction direction)
        {
            if (dockSite == null)
                return null;
            return dockSite.ToolWindows.Where(o => o.GetDirectionRelativeToWorkspace() == direction).Cast<IDockTarget>().FirstOrDefault()
                   ?? dockSite;
        }

        public void Show(object rootModel)
        {
            var dockingWindow = CreateDockingWindow(rootModel, _dockSite);

            //dockingWindow.Open();
            dockingWindow.Activate();

            if (dockingWindow is ToolWindow)
            {
                var descendents = VisualTreeHelperExtended.GetAllDescendants(_dockSite, typeof(SplitContainer));
                foreach (SplitContainer splitContainer in descendents)
                    splitContainer.ResizeSlots(2.1, 7.9);
            }
        }

        public void Activate(object presenter)
        {
            var item = _cache.FirstOrDefault(o => o.Key.Equals(presenter));
            if (Equals(item, default(KeyValuePair<IPresenter, DockingWindow>))) return;

            var dockingWindow = item.Value;
            dockingWindow.Activate();
        }

        public T GetActiveModel<T>() where T : class
        {
            var item = _cache.FirstOrDefault(o => o.Key.GetType().GetInterfaces().Contains(typeof(T)) && _dockSite.ActiveWindow == o.Value);
            if (Equals(item, default(KeyValuePair<T, DockingWindow>)))
                return default(T);

            return item.Key as T;
        }

        public bool Contains<T>() where T : IPresenter
        {
            return _cache.Any(o => o.Key.GetType().GetInterfaces().Contains(typeof(T)));
        }

        public bool TryCloseAllWindows()
        {
            bool isClose = true;
            _dockSite.MdiHost.SaftyInvoke<StandardMdiHost>(x =>
            {
                if (x.Items.ToList().OfType<DocumentWindow>().Any(view => !view.Close()))
                {
                    isClose = false;
                }
            });
            return isClose;
        }

        public T FirstOrDefault<T>(Func<T, bool> predicate) where T : IPresenter
        {
            var item = _cache.FirstOrDefault(o => o.Key.GetType().GetInterfaces().Contains(typeof(T)) &&
                (_dockSite.Documents.Contains(o.Value) || (o.Value is ToolWindow && _dockSite.ToolWindows.Contains((ToolWindow)o.Value))) && predicate((T)o.Key));
            if (Equals(item, default(KeyValuePair<T, DockingWindow>)))
                return default(T);

            return (T)item.Key;
        }
    }
}
