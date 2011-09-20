using Caliburn.Core.Threading;
using System;
using System.Collections;
using System.Threading;
using System.Windows;
using Caliburn.Core.Invocation;
using Caliburn.PresentationFramework.ApplicationModel;
using Castle.Windsor;
using Luna.Common;
using Luna.Core.Extensions;
using Luna.Shifts.Domain;
using Luna.Statistic.Domain;
using Luna.WPF.ApplicationFramework.Behaviors;
using Luna.WPF.ApplicationFramework.Threads;
using Microsoft.Practices.ServiceLocation;
using Luna.Statistic.Domain.Service;
using Luna.Statistic.Presenters.Interfaces;
using Luna.WPF.ApplicationFramework;
using System.Collections.Generic;

namespace Luna.Statistic.Services
{
    public class StaffingCalculatorService : IStaffingCalculatorService
    {
        private Func<IEnumerable<IAgent>> _getAgents;
        private Func<DateRange> _getViewRange;
        private IStaffingChartPresenter _staffingCalculatorPresenter;
        private IWindsorContainer _container;
        private FrameworkElement _contentView;

        public StaffingCalculatorService(IWindsorContainer container, FrameworkElement contentView, Func<IEnumerable<IAgent>> getAgents,
            Func<DateRange> getViewRange)
        {
            _container = container;

            _contentView = contentView;
            _getViewRange = getViewRange;
            _getAgents = getAgents;
        }

        public void Run(IDictionary args)
        {
            _staffingCalculatorPresenter.SaftyInvoke(p => p.Deactivate());

            if (_staffingCalculatorPresenter == null)
            {
                args["service"] = this;

                _staffingCalculatorPresenter = _container.Resolve<IStaffingChartPresenter>(args);

                UIThread.BeginInvoke(() => View.SetModel(_contentView, _staffingCalculatorPresenter));
            }
            else
            {
                _staffingCalculatorPresenter.Activate();
            }
        }

        public IEnumerable<IAgent> GetAgents()
        {
            return _getAgents();
        }

        public DateRange GetViewRange()
        {
            return _getViewRange();
        }

        public void Callback()
        {
            //Application.Current.Dispatcher.BeginInvoke(() => StaffingCalculatorUI.MaxHeight = ((FrameworkElement)ScheduleGrid).ActualHeight - 100);
        }

        public void Show(double height)
        {

        }

        public void Hide()
        {

        }

        public bool Ready
        {
            get
            {
                return _staffingCalculatorPresenter != null && _staffingCalculatorPresenter.AnalyisComplete;
            }
        }

        public IStaffingStatistic BeginEstimation()
        {
            return _staffingCalculatorPresenter.StaffingStatistics.CurrentItem.Self<IStaffingStatistic>(o => o.Reset());
        }

        public void Shutdown()
        {
            View.SetModel(_contentView, null);
            if (_staffingCalculatorPresenter != null)
                _staffingCalculatorPresenter.Shutdown();
            _staffingCalculatorPresenter = null;

            _getAgents = null;
            _getViewRange = null;
            _container = null;
            _contentView = null;
        }
    }
}