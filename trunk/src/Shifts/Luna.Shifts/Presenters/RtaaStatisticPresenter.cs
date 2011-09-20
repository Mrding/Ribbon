using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Caliburn.PresentationFramework.ApplicationModel;
using Luna.Core.Extensions;
using Caliburn.Core.Metadata;
using Luna.Common;
using Luna.Shifts.Domain;
using Luna.Shifts.Presenters.Interfaces;
using Luna.WPF.ApplicationFramework;
using Luna.Common.Extensions;
using Luna.WPF.ApplicationFramework.Extensions;
using System.Windows;
using Luna.Infrastructure.Domain;
namespace Luna.Shifts.Presenters
{
    [Singleton(typeof(IRtaaStatisticPresenter))]
    public class RtaaStatisticPresenter : DockingPresenter, IRtaaStatisticPresenter
    {
        private ICanSupportAgentFinder _shiftViewerPresenter;
        private DateTime _monitoringTime;
        private IList<IEnumerable> _data;

        protected override void OnInitialize()
        {
            _shiftViewerPresenter.As<ILifecycleNotifier>().AttemptingShutdown += RtaaStatisticPresenterAttemptingShutdown;
            Calculate();
            base.OnInitialize();
        }

        protected override void OnShutdown()
        {
            if (IsInitialized)
            {
                IsInitialized = false;
                _shiftViewerPresenter.As<ILifecycleNotifier>().AttemptingShutdown -= RtaaStatisticPresenterAttemptingShutdown; 
                base.OnShutdown();
                Close();
            }
        }

        private void RtaaStatisticPresenterAttemptingShutdown(object sender, EventArgs e)
        {
            Close();
            _shiftViewerPresenter = null;
            _data = null;
        }

        public ICanSupportAgentFinder ShiftViewerPresenter
        {
            get { return _shiftViewerPresenter; }
            set { _shiftViewerPresenter = value; }
        }

        public void UpdateData(IList<IEnumerable> values)
        {
            _data = values;

            if (base.IsInitialized)
                Calculate();

        }

        private void Calculate()
        {
            if (_data == null) return;
            var agentAdherences = _data.Cast<AgentAdherence>().ToList();

            Totals = _shiftViewerPresenter.BindableAgents.Count;

            OnServiceRegual = 0;
            CurrentOnService = 0;
            CurrentOnServiceAgents = new List<ISimpleEmployee>();
            CurrentUnService = 0;
            CurrentUnServiceAgents = new List<ISimpleEmployee>();
            Absents = 0;
            AbsentAgents = new List<ISimpleEmployee>();
            LeaveRegual = 0;
            LeaveOnTimeAgents = new List<ISimpleEmployee>();
            LeaveOnTime = 0;
            LateToWork = 0;
            LateToWorkAgents = new List<ISimpleEmployee>();
            WorkWithoutShift = 0;
            WorkWithoutShiftAgents = new List<ISimpleEmployee>();
            NoActivities = 0;
            NoActivitiesAgents = new List<ISimpleEmployee>();
            NoShifts = 0;

            foreach (var adherence in agentAdherences)
            {
                var employee = adherence.Profile;
                var slicedTerm = adherence.SlicedTerms.FirstOrDefault(o => o.Start <= _monitoringTime && o.End > _monitoringTime);

                if (slicedTerm != null && slicedTerm.Text == "Absent")//请假
                {
                    Absents++;
                    AbsentAgents.Add(employee);
                    continue;
                }

                var agentAdherence = adherence.AdherenceTerms.FirstOrDefault(o => o.End == _monitoringTime);


                if (slicedTerm != null && slicedTerm.OnService)//表定值机
                {
                    OnServiceRegual++;
                    if (agentAdherence == null)
                    {
                        CurrentOnService++;
                        CurrentOnServiceAgents.Add(employee);
                    }
                    else
                    {
                        CurrentUnService++;
                        CurrentUnServiceAgents.Add(employee);
                    }
                }
                else if (slicedTerm != null && slicedTerm.Text != "Gap")//表定离开
                {
                    LeaveRegual++;
                    if (agentAdherence == null)
                    {
                        LeaveOnTimeAgents.Add(employee);
                        LeaveOnTime++;
                    }
                    else
                    {
                        LateToWorkAgents.Add(employee);
                        LateToWork++;
                    }
                }
                else//无班人数
                {
                    NoShifts++;
                    if (agentAdherence != null)
                    {
                        WorkWithoutShiftAgents.Add(employee);
                        WorkWithoutShift++;
                    }
                    else
                    {
                        NoActivitiesAgents.Add(employee);
                        NoActivities++;
                    }

                }
            }

            Totals = agentAdherences.Count;

            this.NotifyOfPropertyChange(o => o.OnServiceRegual);
            this.NotifyOfPropertyChange(o => o.CurrentOnService);
            this.NotifyOfPropertyChange(() => CurrentOnServiceAgents);
            this.NotifyOfPropertyChange(o => o.CurrentUnService);
            this.NotifyOfPropertyChange(() => CurrentUnServiceAgents);
            this.NotifyOfPropertyChange(o => o.Absents);
            this.NotifyOfPropertyChange(() => AbsentAgents);
            this.NotifyOfPropertyChange(o => o.LeaveRegual);
            this.NotifyOfPropertyChange(o => o.LeaveOnTime);
            this.NotifyOfPropertyChange(() => LeaveOnTimeAgents);
            this.NotifyOfPropertyChange(o => o.LateToWork);
            this.NotifyOfPropertyChange(() => LateToWorkAgents);
            this.NotifyOfPropertyChange(o => o.WorkWithoutShift);
            this.NotifyOfPropertyChange(() => WorkWithoutShiftAgents);
            this.NotifyOfPropertyChange(o => o.NoActivities);
            this.NotifyOfPropertyChange(() => NoActivitiesAgents);
            this.NotifyOfPropertyChange(o => o.NoShifts);

            if (_reloadWatchList != null)
                _reloadWatchList.Invoke();

            //Refresh();
        }

        public DateTime MonitoringTime
        {
            get { return _monitoringTime; }
            set { _monitoringTime = value; }
        }

        /// <summary>
        /// 没有班表
        /// </summary>
        public double NoShifts { get; set; }

        /// <summary>
        /// 既没班也没来上班(休假)
        /// </summary>
        public double NoActivities { get; set; }
        public IList NoActivitiesAgents { get; set; }

        /// <summary>
        /// 请假
        /// </summary>
        public double Absents { get; set; }
        public IList AbsentAgents { get; set; }

        /// <summary>
        /// 表定离开
        /// </summary>
        public double LeaveRegual { get; set; }


        /// <summary>
        /// 无班表值机
        /// </summary>
        public double WorkWithoutShift { get; set; }
        public IList WorkWithoutShiftAgents { get; set; }

        /// <summary>
        /// 未离开
        /// </summary>
        public double LateToWork { get; set; }
        public IList LateToWorkAgents { get; set; }

        /// <summary>
        /// 准时离席
        /// </summary>
        public double LeaveOnTime { get; set; }
        public IList LeaveOnTimeAgents { get; set; }
        /// <summary>
        /// 未值机
        /// </summary>
        public double CurrentUnService { get; set; }

        public IList CurrentUnServiceAgents { get; set; }

        /// <summary>
        /// 目前值机
        /// </summary>
        public double CurrentOnService { get; set; }

        public IList CurrentOnServiceAgents { get; set; }

        /// <summary>
        /// 表定值机
        /// </summary>
        public double OnServiceRegual { get; set; }

        private double _totals;

        public double Totals
        {
            get { return _totals; }
            set
            {
                _totals = value;
                this.NotifyOfPropertyChange(o => o.Totals);
            }
        }

        public double OnlineAgents
        {
            get { return CurrentOnService + LateToWork + WorkWithoutShift; }
        }

        public double OnlineAgentsPercentage
        {
            get { return OnlineAgents / Totals * 100; }
        }

        private IList _watchList;
        public IList WatchList
        {
            get { return _watchList; }
            set { _watchList = value; NotifyOfPropertyChange(() => WatchList); }
        }

        //private void Refresh()
        //{
        //    if (_chart == null) return;

        //    _chart.Series[0].DataPoints[0].YValue = CurrentOnService;
        //    _chart.Series[0].DataPoints[1].YValue = CurrentUnService;
        //    _chart.Series[0].DataPoints[2].YValue = LeaveOnTime;
        //    _chart.Series[0].DataPoints[3].YValue = LateToWork;
        //    _chart.Series[0].DataPoints[4].YValue = WorkWithoutShift;

        //    _chart.Titles[0].Text = string.Format("當前值機總人數 {0} ({1:0.00}%)", OnlineAgents, OnlineAgentsPercentage);
        //}

        public void Sort()
        {
        }

        #region Implementation of ICanSupportAgentFinder

        public IEnumerable Agents
        {
            get { return _shiftViewerPresenter.Agents; }
            set { _shiftViewerPresenter.Agents = value; }
        }

        public IList BindableAgents
        {
            get { return _shiftViewerPresenter.BindableAgents; }
            set { _shiftViewerPresenter.BindableAgents = value; }
        }

        public TResult Transform<TResult>(object item) where TResult : class
        {
            return _shiftViewerPresenter.Transform<TResult>(item);
        }

        public void Refresh(IEnumerable list)
        {
        }

        private Action _reloadWatchList;

        public void SetWatchList(IList agents, FrameworkElement el)
        {
            _reloadWatchList = () => { WatchList = el.Tag as IList; };
            WatchList = agents;
        }

        public void RedirectTo(object o)
        {
            _shiftViewerPresenter.SaftyInvoke<IBlockMatrixContainer>(p => p.NavigateTo(o));
        }

        #endregion
    }

}
