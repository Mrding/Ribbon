using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Caliburn.Core.Metadata;
using Caliburn.PresentationFramework.ApplicationModel;
using Luna.Common;
using Luna.Common.Constants;
using Luna.Common.Interfaces;
using Luna.Core.Extensions;
using Luna.Shifts.Presenters.Interfaces;
using Luna.WPF.ApplicationFramework;
using Luna.WPF.ApplicationFramework.Interfaces;
using Microsoft.Practices.ServiceLocation;
using Luna.Infrastructure.Presenters.Interfaces;
using Commands = Luna.WPF.ApplicationFramework.ApplicationCommands;

namespace Luna.GUI.Presenters
{
    [Singleton(typeof(IShellPresenter))]
    public class ShellPresenter : DefaultPresenter, IShellPresenter
    {
        //private IDockSitePresenter _dockSitePresenter;

        public ShellPresenter()
        {
            EnableNhProfiler = AppConfig.EnableProfiler;

            //CommandBindings.Add(new CommandBinding(Luna.WPF.ApplicationFramework.ApplicationCommands.OpenAgentFinder, (sender, e) =>
            //                                                                                                                       {
            //                                                                                                                           OpenDialog<IAgentFinderPresenter>(new Dictionary<string, object> { { "Invoker", _dockSitePresenter.GetActiveModel<ICanSupportAgentFinder>() } });
            //                                                                                                                       }, (sender, e) =>
            //    {
            //        var activeCanSupportAgentFinderPresenter = _dockSitePresenter.GetActiveModel<ICanSupportAgentFinder>();
            //        var found = activeCanSupportAgentFinderPresenter != null && activeCanSupportAgentFinderPresenter.If<ISubmitablePresenter>(p => !p.IsDirty);


            //        e.CanExecute = found;
            //    }));
            
            CommandBindings.Add(CreateCommand<IPresenterManager>(Commands.OpenTermStyleManager, FunctionKeys.ManageAssignmentType));
            CommandBindings.Add(CreateCommand<ICalendarEventPresenter>(Commands.OpenCalendarEvent, FunctionKeys.CalendarEvent));
            //CommandBindings.Add(CreateCommand<IShiftImportPresenter>(Commands.OpenShiftImport, new Dictionary<string, object>()));

        }

        private CommandBinding CreateCommand<TPresenter>(RoutedCommand command, string key) where TPresenter : IPresenter
        {
            return new CommandBinding(command, delegate { OpenDialog<TPresenter>(key,true); });
        }

        private CommandBinding CreateCommand<TPresenter>(RoutedCommand command, IDictionary args) where TPresenter : IDockablePresenter
        {
            return new CommandBinding(command, delegate { Show<TPresenter>(args); });
        }

        private bool _cellMode;
        public bool CellMode
        {
            get { return _cellMode; }
            set { _cellMode = value; NotifyOfPropertyChange(()=> CellMode); }
        }

        private object _activePlan;
        public object ActivePlan
        {
            get { return _activePlan; }
            set { _activePlan = value; NotifyOfPropertyChange(()=> ActivePlan ); }
        }

        //public bool AnyCanSupportAgentFinder()
        //{
        //    var activeCanSupportAgentFinderPresenter = _dockSitePresenter.GetActiveModel<ICanSupportAgentFinder>();
        //    return activeCanSupportAgentFinderPresenter != null && activeCanSupportAgentFinderPresenter.If<ISubmitablePresenter>(p => !p.IsDirty);
        //}

        private bool _enableNhProfiler;

        public bool EnableNhProfiler
        {
            set
            {
                _enableNhProfiler = value;
                if (_enableNhProfiler)
                    HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
                else
                {
                    try
                    {
                        if (_enableNhProfiler)
                            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Stop();
                    }
                    catch
                    { }
                }
            }
            get { return _enableNhProfiler; }
        }

        protected override void OnShutdown()
        {
            EnableNhProfiler = false;
            base.OnShutdown();
        }

        public void OnClosing(CancelEventArgs e)
        {
            if (!ServiceLocator.Current.GetInstance<IDockSitePresenter>().TryCloseAllWindows())
                e.Cancel = true;
        }
    }
}