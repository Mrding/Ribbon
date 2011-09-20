using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using ActiproSoftware.Windows.Controls.Editors;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Caliburn.Core.Metadata;
using Caliburn.PresentationFramework.ApplicationModel;
using Luna.Common;
using Luna.Common.Extensions;
using Luna.Common.Interfaces;
using Luna.Core.Extensions;
using Luna.Infrastructure.Domain;
using Luna.Infrastructure.Domain.Model;
using Luna.Shifts.Domain;
using Luna.Shifts.Domain.Model;
using Luna.Shifts.Presenters.Interfaces;
using Luna.WPF.ApplicationFramework;
using Luna.WPF.ApplicationFramework.Extensions;
using Luna.WPF.ApplicationFramework.Threads;
using Microsoft.Practices.ServiceLocation;
using Luna.Shifts.Presenters.Extentions;


namespace Luna.Shifts.Presenters
{
    [PerRequest(typeof(IShiftDispatcherPresenter))]
    public partial class ShiftDispatcherPresenter : DockingPresenter, IShiftDispatcherPresenter, ICanSupportAgentFinder, IShiftViewerPresenter, IBlockMatrixContainer, ISubmitablePresenter
    {
        #region Fields

        private IDefalutBlockConverter _blockConverter;
        private ILaborHoursCountingModel _laborHoursCountingModel;
        private IShiftDispatcherModel _shiftDispatcherModel;

        private IAgent _selectedAgent;
        private IList<IEnumerable> _agentAdherences;
        private IList<IAgent> _attendances;
        private IList _bindableAgents;
        private bool _enableRtaa;
        private DateTime _monitoringPoint;
        private DispatcherTimer _timer;

        /// <summary>
        /// External registed delegate
        /// </summary>
        private Action<IEnumerable> _refresh;

        private Action _uiRefresh;
        private bool _refreshLaborHourOnly;
        private int _selectTermClickCount;
        private Action _sortDelegate;

        private bool _watchPointIsInitialized;
        private IList _changedAgents = new List<IAgent>(10);
        private Func<IVisibleTerm, AdherenceEvent> _addingAdhEventDelegate;
        private Action<AdherenceEvent> _removingAdhEventDelegate;
        private Action<AdherenceEvent> _editingAdhEventDelegate;

        #endregion Fields

        #region Constructors

        public ShiftDispatcherPresenter(IShiftDispatcherModel shiftDispatcherModel, ILaborHoursCountingModel laborHoursCountingModel)
        {
            CurrentIndex = -1;

            _shiftDispatcherModel = shiftDispatcherModel;
            _laborHoursCountingModel = laborHoursCountingModel;
            _blockConverter = new DefalutBlockConverter();
       

            AdherenceBlockConverter = new AdherenceBlockConverter();
            AdherenceBlockConverter.BlockChanged += OnAdhEventBlockChanged;

            _sortDelegate = new Action(() => Sort(GetWatchPoint(), GetWatchPoint().AddDays(1)));


        }


        #endregion Constructors

        private void OnAdhEventBlockChanged(object block, bool success)
        {
            if (success)
            {
                _editingAdhEventDelegate(block as AdherenceEvent);
            }
        }

        protected override void OnInitialize()
        {
            BeginLoading();
            new Thread(() =>
                                {
                                    Application.Current.Resources["AbsentEventTypes"] = _shiftDispatcherModel.GetAllAbsenetTypes();
                                    _attendances = _shiftDispatcherModel.GetAgents(_schedule);
                                    _refreshLaborHourOnly = true;
                                    UIThread.Invoke(() =>
                                    {
                                        Agents = _attendances.Cast<IEnumerable>().ToList();
                                        EndLoading();
                                    });
                                    _refreshLaborHourOnly = false;
                                }).Start();
            base.OnInitialize();
        }


        #region Properties

        private bool _showAdditionalDate;
        public bool ShowAdditionalDate
        {
            get { return _showAdditionalDate; }
            set { _showAdditionalDate = value; NotifyOfPropertyChange(() => ShowAdditionalDate); }
        }

        private double _zoomValue = 40;
        public double ZoomValue
        {
            get { return _zoomValue; }
            set
            {
                _zoomValue = value;
            }
        }


        private IDictionary<Guid, int> _agentAdherencesMap;

        public IList<IEnumerable> AgentAdherences
        {
            get { return _agentAdherences; }
            set
            {

                if (value != null && value.Count != 0 && _agentAdherences != null && _agentAdherences.Count > 0 && value.Count < _agentAdherences.Count)
                {
                    _agentAdherencesMap = new Dictionary<Guid, int>(value.Count);
                    foreach (IEnumerable t in value)
                    {
                        var index = _agentAdherences.IndexOf(t);
                        if (index == -1) continue;
                        _agentAdherences[index] = t;
                        _agentAdherencesMap[t.SaftyGetProperty<Guid, AgentAdherence>(a => a.Profile.Id)] = index;
                    }
                }
                else
                {
                    _agentAdherences = value;

                }
                EnableRtaa = _agentAdherences != null && _agentAdherences.Count > 0;

                QuietlyReloadAgentAdherences();
            }
        }

        private IList<IEnumerable> _agentAdherenceEvents;
        public IList<IEnumerable> AgentAdherenceEvents
        {
            get { return _agentAdherenceEvents; }
            set
            {
                _agentAdherenceEvents = value;
                this.QuietlyReload(ref _agentAdherenceEvents, "AgentAdherenceEvents");
            }
        }

        private IList<IEnumerable> _agentOccupations;
        public IList<IEnumerable> AgentOccupations
        {
            get { return _agentOccupations; }
            set
            {
                _agentOccupations = value;
                this.QuietlyReload(ref _agentOccupations, "AgentOccupations");
            }
        }

        public IEnumerable Agents
        {
            get { return _attendances; }
            set
            {
                BindableAgents = value.Cast<IEnumerable>().ToList();
                FullyRefresh = () => true;
                this.NotifyOfPropertyChange("AttendancesCount");
            }
        }

        public int AttendancesCount
        {
            get { return _attendances == null ? 0 : _attendances.Count; }
        }

        public IList BindableAgents
        {
            get { return _bindableAgents; }
            set
            {
                if (value == null) return;
                _bindableAgents = value;

                for (var i = 0; i < _bindableAgents.Count; i++)
                {
                   
                    var agent = _bindableAgents[i] as IAgent;
                    if (agent == null) continue;
                    if (!_refreshLaborHourOnly)
                        _shiftDispatcherModel.Reload(ref agent);
                    _laborHoursCountingModel.SummarizeLaborRule(agent);
                    if (_refreshLaborHourOnly) continue;
                    _bindableAgents[i] = agent;
                    var index = _attendances.IndexOf(agent as Agent);
                    if (index != -1)
                        _attendances[index] = agent;
                }
                _agentAdherences = null;

                this.NotifyOfPropertyChange("BindableAgents");
                //this.NotifyOfPropertyChange("BindableAgentsCount");
            }
        }

        public IDefalutBlockConverter BlockConverter
        {
            get { return _blockConverter; }
        }

        public IDefalutBlockConverter AdherenceBlockConverter { get; private set; }

        public bool EnableRtaa
        {
            get { return _enableRtaa; }
            set
            {
                if (_enableRtaa != value)
                {
                    _enableRtaa = value;
                    if (_enableRtaa == false && _timer != null)
                        _timer.IsEnabled = false;
                    this.NotifyOfPropertyChange("EnableRtaa");
                }
            }
        }

        /// <summary>
        /// For rtaa use only
        /// </summary>
        public DateTime MonitoringPoint
        {
            get { return _monitoringPoint; }
            set
            {
                if (value.IsInTheRange(Schedule))
                {
                    if (_monitoringPoint == value) return;
                    _monitoringPoint = value;
                    if (_refresh != null)
                        _refresh.Invoke(BindableAgents);
                }
                else
                {
                    if (_timer != null)
                        _timer.IsEnabled = false;
                    RtaaIsAutoRunning = false;
                }
                this.NotifyOfPropertyChange("MonitoringPoint");
            }
        }

        private bool _rtaaIsAutoRunning;
        public bool RtaaIsAutoRunning
        {
            get { return _rtaaIsAutoRunning; }
            set { _rtaaIsAutoRunning = value; this.NotifyOfPropertyChange("RtaaIsAutoRunning"); }
        }

        /// <summary>
        /// Block hit time
        /// </summary>
        private DateTime _clickTime;
        public DateTime ClickTime
        {
            get { return _clickTime; }
            set
            {
                _clickTime = value;//value.TrunToMultiplesOf5();
                this.NotifyOfPropertyChange("ClickTime");
            }
        }

        public DateTime ScreenEnd { get; set; }

        //private ITerm _viewRange;
        //public ITerm ViewRange
        //{
        //    get { return _viewRange; }
        //    set { _viewRange = value; 
        //        NotifyOfPropertyChange(()=> ViewRange);

        //    }
        //}

        private Schedule _schedule;
        public ITerm Schedule
        {
            get { return _schedule; }
            set
            {
                if (base.IsInitialized) return;
                _schedule = (Schedule)value;
                ScheduleRange = new DateRange(_schedule.Start, _schedule.End);

                var today = DateTime.Today;

                if (!_watchPointIsInitialized && today.IsInTheRange(_schedule))
                {
                    this.SetWatchPoint(today);
                }
                else
                {
                    SetWatchPoint(_schedule.Start);
                }
                _watchPointIsInitialized = true;

                //ViewRange = new DateRange(GetWatchPoint().Date.AddHours(-8), GetWatchPoint().Date.AddHours(8));
            }
        }

        public DateRange ScheduleRange { get; private set; }

        public IAgent SelectedAgent
        {
            get { return _selectedAgent; }
            set
            {
                _selectedAgent = value;
                this.NotifyOfPropertyChange("SelectedAgent");
            }
        }

        private int _currentIndex;
        public int CurrentIndex
        {
            get { return _currentIndex; }
            set { _currentIndex = value; NotifyOfPropertyChange(() => CurrentIndex); }
        }

        private Term _selectedTerm;
        public Term SelectedTerm
        {
            get { return _selectedTerm; }
            set { _selectedTerm = value; NotifyOfPropertyChange(() => SelectedTerm); }
        }

        private ITerm _selectedAdhEvent;
        public ITerm SelectedAdhEvent
        {
            get { return _selectedAdhEvent; }
            set { _selectedAdhEvent = value; }
        }

        private bool _isDirty;
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                _isDirty = value;
                this.NotifyOfPropertyChange("IsDirty");
            }
        }

        private bool _seatDispatcherOpened;
        public bool SeatDispatcherOpened
        {
            get { return _seatDispatcherOpened; }
            set
            {
                _seatDispatcherOpened = value; NotifyOfPropertyChange(() => SeatDispatcherOpened);
                ShowOccupationMask = _seatDispatcherOpened;
            }
        }

        private bool _view3D;
        public bool View3D
        {
            get { return _view3D; }
            set
            {
                _view3D = value;
                NotifyOfPropertyChange("View3D");
            }
        }

        private bool _showOccupationMask;
        public bool ShowOccupationMask
        {
            get { return _showOccupationMask; }
            set { _showOccupationMask = value; NotifyOfPropertyChange(() => ShowOccupationMask); }
        }



        #endregion Properties

        #region Methods

        public void Refresh(IEnumerable list)
        {
            if (_refresh != null)
                _refresh.Invoke(list ?? _bindableAgents);

            //BlockConverter.Refresh();
        }

        public bool CanAddTo(object item)
        {
            return true;
        }

        public bool UnselectedAfterUpateQueryResult
        {
            get { return true; }
        }


        private Action RetainCurrentSelectedAgent()
        {
            if (BindableAgents != null && SelectedAgent != null)
            {
                var selectedIndex = BindableAgents.IndexOf(SelectedAgent);
                return () => { SelectedAgent = selectedIndex == -1 ? null : _bindableAgents[selectedIndex] as IAgent; };
            }
            return () => { };
        }

        private IEnumerable<IAgent> ReAssignAgents(IList agents)
        {
            IsDirty = false;
            return agents.ReAssignAgents(_bindableAgents, _attendances, (dirtyAgent =>
            {
                _shiftDispatcherModel.Reload(ref dirtyAgent);

                _laborHoursCountingModel.SummarizeLaborRule(dirtyAgent);
                return dirtyAgent;
            }));

           // var reassignedAgents = new IAgent[agents.Count];

            //agents.ForEach<IAgent>((t, i) =>
            //                           {
            //                               var index = _bindableAgents.IndexOf(t);
            //                               if (index != -1)
            //                               {
            //                                   var agent = (IAgent)_bindableAgents[index];
            //                                   _shiftDispatcherModel.Reload(ref agent);
            //                                   _laborHoursCountingModel.SummarizeLaborRule(agent);

            //                                   _bindableAgents[index] = agent;
            //                                   index = _attendances.IndexOf(agent as Agent);
            //                                   if (index != -1)
            //                                   {
            //                                       _attendances[index] = agent as Agent;
            //                                       reassignedAgents[i] = _attendances[index];
            //                                   }
            //                               }
            //                           });
        }

        private void QuietlyReloadAgents(IEnumerable list)
        {
            var reselectAgent = RetainCurrentSelectedAgent();
            this.QuietlyReload(ref _bindableAgents, "BindableAgents");
            if (_refresh != null)
                _refresh.Invoke(list);
            reselectAgent.Invoke();

            if (SeatDispatcherOpened)
            {
                _seatDispatcherPresenter.Proc("FastReloadData")();
                //AgentOccupations = (from IAgent a in BindableAgents where a.Occupations != null select (IEnumerable)a.Occupations).ToList();
            }
        }

        private void QuietlyReloadAgentAdherences()
        {
            this.QuietlyReload(ref _agentAdherences, "AgentAdherences");
        }

        public void NavigateTo(object item)
        {
            var result = default(IAgent);
            foreach (IAgent agent in BindableAgents)
            {
                if (agent.Profile.Equals(item))
                {
                    result = agent;
                    break;
                }
            }

            var index = BindableAgents.IndexOf(result);
            if (index == -1)
                return;
            CurrentIndex = index;
            SelectedAgent = BindableAgents[CurrentIndex] as IAgent;
            this.NotifyOfPropertyChange("CurrentIndex");
        }

        public void AlterTermFail(Exception ex)
        {
            ReloadAgents(new List<IAgent>(new[] { SelectedAgent }), ex);
        }

        public override bool Equals(object obj)
        {
            var otherSchedule = obj as Schedule;
            if (otherSchedule != null)
                return otherSchedule.Equals(_schedule);

            var other = obj as ShiftDispatcherPresenter;
            if (other != null)
                return other.Schedule.Equals(_schedule);
            return false;
        }

        private int _hashCode;

        public override int GetHashCode()
        {
            if (_schedule != null)
                _hashCode = _schedule.GetHashCode();

            return _hashCode;
        }

        public override bool CanSetWatchPoint(DateTime dateTime)
        {
            return dateTime.IsInTheRange(Schedule);
        }

        public override void SetWatchPoint(DateTime dateTime)
        {
            var value = dateTime;

            base.SetWatchPoint(value);
            if (CanSetWatchPoint(value) && !EnableRtaa)
            {
                _monitoringPoint = value;
            }
        }


        public Func<bool> FullyRefresh { get; set; }

        //public override DateTime GetWatchPoint()
        //{
        //    return EnableRtaa ? MonitoringPoint : base.GetWatchPoint();
        //}

        public DateRange GetMonitoringRange()
        {
            var end = MonitoringPoint;
            var start = end.AddHours(-12).RemoveSeconds();
            return new DateRange(start, end);
        }

        public DateTime GetCenterOfScreenTime()
        {
            var screenStart = base.GetWatchPoint();
            return screenStart.AddMinutes(ScreenEnd.Subtract(screenStart).TotalMinutes / 2).TurnToMultiplesOf5();
        }

        public bool RedirectToScreenStart(bool isAutoRunning)
        {
            if (!isAutoRunning && !EnableRtaa) return false;

            var serverTime = ServiceLocator.Current.GetInstance<IBackendModel>().GetUniversialTime();
            var canSetWatchPoint = CanSetWatchPoint(serverTime);

            if (canSetWatchPoint && (isAutoRunning || !EnableRtaa))
            {
                var screenStart = base.GetWatchPoint();
                SetWatchPoint(serverTime.AddMinutes(-ScreenEnd.Subtract(screenStart).TotalMinutes / 2).TurnToMultiplesOf5());
            }


            if (!EnableRtaa) return canSetWatchPoint;

            // 自动时，如果在时间范围内，开启Timer自动刷新
            if (isAutoRunning)
            {
                Action<DateTime> action = time =>
                {
                    MonitoringPoint = time;
                };

                if (_timer == null)
                {
                    var seconds = Convert.ToDouble(Application.Current.Resources["RtaaAutoRefreshInterval"]);
                    _timer = new DispatcherTimer(DispatcherPriority.ContextIdle) { Interval = TimeSpan.FromSeconds(seconds < 10 ? 10d : seconds) };
                    _timer.Tick += (s, e) => action(ServiceLocator.Current.GetInstance<IBackendModel>().GetUniversialTime());
                }

                if (canSetWatchPoint)
                {
                    RtaaIsAutoRunning = true;
                    action(serverTime);
                }
            }
            else//手动，如果开启Timer，那么停止自动刷新
            {
                RtaaIsAutoRunning = false;
            }

            if (_timer != null)
                _timer.IsEnabled = RtaaIsAutoRunning;
            return RtaaIsAutoRunning;
        }

        public void SetSelectTermClickCount(MouseButtonEventArgs args)
        {
            _selectTermClickCount = args.ClickCount;
        }

        public TResult Transform<TResult>(object item)
            where TResult : class
        {
            return ((IAgent)item).Profile as TResult;
        }

        public void UnregisterDelegate(object source)
        {
            if (source.Is<Action<IEnumerable>>())
                _refresh -= source as Action<IEnumerable>;
        }

        public void RegisterRefreshDelegate(object source)
        {
            if (source is UIElement && _uiRefresh == null)
            {
                var refreshLayerMethodInfo = source.GetType().GetMethod("RefreshLayer");
                if (refreshLayerMethodInfo != null)
                {
                }
            }
            else if (_refresh == null)
                _refresh += source as Action<IEnumerable>;
        }

        public override void ViewLoaded(object view, object context)
        {
            var lockTermCommandBinding = new CommandBinding(
Luna.WPF.ApplicationFramework.ApplicationCommands.LockTerm, (sender, e) => UnlockOrLockShift(), (sender, e) =>
                                                          {
                                                              e.CanExecute = VerifyNotAbsentEvent() && IsBelongToSchedule();
                                                          });

            var sortByTermNameCommandBinding = new CommandBinding(
                Luna.WPF.ApplicationFramework.ApplicationCommands.SortByTermName, (sender, e) => SortByShiftName(GetWatchPoint(), ScreenEnd));

            var sortByAgentNameCommandBinding = new CommandBinding(
                Luna.WPF.ApplicationFramework.ApplicationCommands.SortByAgentName, (sender, e) => SortByAgentName(GetWatchPoint(), ScreenEnd));

            var sortByTermStartCommandBinding = new CommandBinding(
                Luna.WPF.ApplicationFramework.ApplicationCommands.SortByTermStart, (sender, e) => SortByStartTime(GetWatchPoint(), ScreenEnd));


            var showOrHideTermNameCommandBinding = new CommandBinding(
                Luna.WPF.ApplicationFramework.ApplicationCommands.ShowTermName, (sender, e) => ShowShiftText(Convert.ToBoolean(e.Parameter)));

            var addingEventCommandBinding = new CommandBinding(
                Luna.WPF.ApplicationFramework.ApplicationCommands.AddingEvent, (sender, e) => OpenAddingEventDialog(), (sender, e) =>
                                                                                       {
                                                                                           e.CanExecute = CanOpenAddEventDialog() && IsBelongToSchedule();
                                                                                       });

            var adherenceMonitoringCommandBinding = new CommandBinding(
               Luna.WPF.ApplicationFramework.ApplicationCommands.AdherenceMonitoring, (sender, e) => StartMonitoringAdherence());

            var adherenceStatisticCommandBinding = new CommandBinding(
            Luna.WPF.ApplicationFramework.ApplicationCommands.AdherenceStatistic, (sender, e) => OpenRtaaStatistic(), (sender, e) =>
                                                                                              {
                                                                                                  e.CanExecute = EnableRtaa;
                                                                                              });

            var rescheduleSeatCommandBinding = new CommandBinding(
           Luna.WPF.ApplicationFramework.ApplicationCommands.RescheduleSeat, (sender, e) => RescheduleSeat(), (sender, e) =>
                                                                                             {
                                                                                                 e.CanExecute = CanRescheduleSeat() && IsBelongToSchedule();
                                                                                             });

            var cancelSeatCommandBinding = new CommandBinding(
          Luna.WPF.ApplicationFramework.ApplicationCommands.CancelSeat, (sender, e) => CancelSeat(), (sender, e) =>
                                                                                            {
                                                                                                e.CanExecute = CanCancelSeat() && IsBelongToSchedule();
                                                                                            });

           //x var seatDispatcherCommandBinding = new CommandBinding(Luna.WPF.ApplicationFramework.ApplicationCommands.SeatDispatcher, (sender, e) => RunSeatDispatcher(Container.Resolve<ISeatDispatcherPresenter>()));

            var openStaffingStatisticCommandBinding = new CommandBinding(
     Luna.WPF.ApplicationFramework.ApplicationCommands.OpenStaffingStatistic, (sender, e) => ShowStaffingCalculatorDialog());

       
            view.SaftyInvoke<FrameworkElement>(f =>
                {
                    f.CommandBindings.Add(lockTermCommandBinding);
                    f.CommandBindings.Add(sortByTermNameCommandBinding);
                    f.CommandBindings.Add(sortByAgentNameCommandBinding);
                    f.CommandBindings.Add(sortByTermStartCommandBinding);
                    f.CommandBindings.Add(showOrHideTermNameCommandBinding);
                    f.CommandBindings.Add(addingEventCommandBinding);
                    f.CommandBindings.Add(adherenceMonitoringCommandBinding);
                    f.CommandBindings.Add(adherenceStatisticCommandBinding);
                    f.CommandBindings.Add(rescheduleSeatCommandBinding);
                    f.CommandBindings.Add(cancelSeatCommandBinding);
                    //xf.CommandBindings.Add(seatDispatcherCommandBinding);
                    f.CommandBindings.Add(openStaffingStatisticCommandBinding);
                    //xf.CommandBindings.Add(openShiftComposerCommandBinding);
                });
        }

        protected override void OnShutdown()
        {
            if (_uiRefresh != null)
                _uiRefresh.GetInvocationList().ForEach(o =>
                {
                    _uiRefresh -= o as Action;
                });

            if (_refresh != null)
                _refresh.GetInvocationList().ForEach(o =>
                {
                    _refresh -= o as Action<IEnumerable>;
                });

            if (_seatDispatcherPresenter != null)
            {
                RunSeatDispatcher(_seatDispatcherPresenter.Target.As<IPresenter>());
            }


            _shiftDispatcherModel.Release();
            ((IDisposable)_shiftDispatcherModel).Dispose();
            _shiftDispatcherModel = null;

            _laborHoursCountingModel.SaftyInvoke<IDisposable>(o => o.Dispose());
            _laborHoursCountingModel = null;

            ApplicationCache.Remove(_schedule);

            if (_blockConverter != null)
                _blockConverter.Dispose();
            _blockConverter = null;

            AdherenceBlockConverter.Dispose();
            AdherenceBlockConverter = null;


            if (this._attendances != null)
            {
                this._attendances.Clear();
                this._attendances = null;
            }
            if (this._agentAdherences != null)
            {
                this._agentAdherences.Clear();
                this._agentAdherences = null;
            }
            if (this._bindableAgents != null)
            {
                this._bindableAgents.Clear();
                this._bindableAgents = null;
            }
            if (this._changedAgents != null)
            {
                this._changedAgents.Clear();
                this._changedAgents = null;
            }
            this._refresh = null;
            this._sortDelegate = null;
            this._uiRefresh = null;
            this._buildSeatDispatcher = null;
            this._destroySeatDispatcher = null;
            this._seatDispatcherPresenter = null;
            this._schedule = null;
            this._selectedAgent = null;

            _buildStaffingChart = null;

            if (_staffingCalculatorArgs != null)
                _staffingCalculatorArgs.Clear();
            _staffingCalculatorArgs = null;

            if (_staffingCalculatorService != null)
                _staffingCalculatorService.Shutdown();
            _staffingCalculatorService = null;

            FullyRefresh = null;

            if (this._timer != null)
            {
                _timer.Stop();
                _timer = null;
            }

            base.OnShutdown();
        }

        #endregion Methods
    }
}
