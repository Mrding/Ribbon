using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Caliburn.Core.Metadata;
using Caliburn.PresentationFramework.Filters;
using Luna.Common;
using Luna.Core.Extensions;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Domain;
using Luna.Shifts.Domain.Model;
using Luna.Shifts.Presenters.Interfaces;
using Luna.WPF.ApplicationFramework;
using Luna.WPF.ApplicationFramework.Extensions;
using Luna.Common.Domain;
using Luna.WPF.ApplicationFramework.Actions;

namespace Luna.Shifts.Presenters
{
    [PerRequest(typeof(ISeatDispatcherPresenter))]
    public partial class SeatDispatcherPresenter : ISeatDispatcherPresenter, IBlockMatrixContainer, Common.Interfaces.ISubmitablePresenter
    {
        private Schedule _schedule;
        private readonly ISeatDispatcherModel _seatDispatcherModel;
        private IDictionary<string, IEnumerable> _seatBoxes;
        private IList<Area> _areas;
        //private Action _sortDelegate;
        private ICanSupportAgentFinder _parentModel;
        private int _selectTermClickCount;
        private Occupation[] _occupationClipboard;
        private IList<TimeBox> _otherAgents;
        private Guid[] _excludedEmployeeIds;

        public SeatDispatcherPresenter(ISeatDispatcherModel seatDispatcherModel)
        {
            CurrentIndex = -1;
            _seatDispatcherModel = seatDispatcherModel;
            BlockConverter = new OccupationBlockConverter();
            BlockConverter.BlockChanged += success =>
            {
                _seatDispatcherModel.SubmitChanges();
                BlockConverter.Refresh();
            };
        }

        public void Load(ICanSupportAgentFinder model, Schedule schedule)
        {
            if (IsInitialized)
                return;

            _parentModel = model;
            _schedule = schedule;
            _excludedEmployeeIds = _parentModel.Agents.Cast<IAgent>().Select(o => o.Schedule.Id).ToArray();

            ExecuteManager.BackgroundAction(() =>
            {
                _seatBoxes = _seatDispatcherModel.GetSeats(_schedule, _excludedEmployeeIds,
                    out _otherAgents, out _areas)
                                            .ToDictionary(o => o.Seat.Id.ToString(), o => o as IEnumerable);
                IsDirty = false;
                BuildSeatArrangement<TimeBox>(_otherAgents, o => o); // the agents which have seat but, not in the schedule
                BuildSeatArrangement<IAgent>(_parentModel.Agents, o => o.Schedule);

                this.NotifyOfPropertyChange("Areas");
                SelectedArea = _areas.FirstOrDefault();

            }, BeginLoading, EndLoading);

            base.OnInitialize();
            IsInitialized = true;
        }

        public OccupationBlockConverter BlockConverter { get; private set; }

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

        private IList<IEnumerable> _bindableSeats;

        public IList<IEnumerable> BindableSeats
        {
            get { return _bindableSeats; }
            set { _bindableSeats = value; }
        }

        public IList<Area> Areas { get { return _areas; } }

        private SeatBox _selectedSeat;
        public SeatBox SelectedSeat
        {
            get { return _selectedSeat; }
            set
            {
                _selectedSeat = value;
                this.NotifyOfPropertyChange("SelectedSeat");
            }
        }

        public Func<bool, ISimpleEmployee, bool> TrySubmitChanged { get; set; }

        public Action<int> OccupationsChanged { get; set; }

        public Action<IList<IEnumerable>> OccupationsReloaded { get; set; }

        private int _currentIndex;
        public int CurrentIndex
        {
            get { return _currentIndex; }
            set { _currentIndex = value; NotifyOfPropertyChange(() => CurrentIndex); }
        }

        private Entity _selectedArea;
        public Entity SelectedArea
        {
            get { return _selectedArea; }
            set
            {
                _selectedArea = value;
                this.NotifyOfPropertyChange("SelectedArea");
            }
        }

        private object _selectedBlock;
        public object SelectedBlock
        {
            get { return _selectedBlock; }
            set
            {
                _selectedBlock = value;
                this.NotifyOfPropertyChange("SelectedBlock");
            }
        }

        private Occupation _selectedOccupation;
        public Occupation SelectedOccupation
        {
            get { return _selectedOccupation; }
            set
            {
                _selectedOccupation = value;
                NotifyOfPropertyChange(() => SelectedSeat);
                NotifyOfPropertyChange(() => SelectedBlock);
            }
        }

        private DateTime _clickTime;

        public DateTime ClickTime
        {
            get { return _clickTime; }
            set
            {
                _clickTime = value;
                this.NotifyOfPropertyChange("ClickTime");
            }
        }

        private void BuildSeatArrangement<T>(IEnumerable agents, Func<T, TimeBox> cast)
        {
           agents.ForEach<T>((item, i) =>
                               {
                                   var timeBox = cast(item);

                                   var occupations = timeBox.TermSet.GenSeatArrangements(timeBox.Agent, seat => _seatBoxes.ContainsKey(seat) ? _seatBoxes[seat].SaftyGetProperty<Seat, SeatBox>(o => o.Seat) : default(Seat),
                                       (seat, seatArrangement) => _seatBoxes[seat].If<SeatBox>(o => o.AddOccupation(seatArrangement)));

                                   //show seat block layer 
                                   item.SaftyInvoke<IWorkingAgent>(o =>
                                           {
                                               o.Occupations = new List<Occupation>(occupations.OfType<Occupation>());
                                           });
                               });
            if (typeof(T) == typeof(IAgent))
                OccupationsReloaded(null);
        }

        public override void ViewUnloaded(object view, object context)
        {
            //OccupationRefreshed(new List<IEnumerable>(0));
            base.ViewUnloaded(view, context);
        }

        protected override void OnShutdown()
        {
            if (_seatDispatcherModel != null)
                ((IDisposable)_seatDispatcherModel).Dispose();
            base.OnShutdown();
            this._schedule = null;
            if (this._areas != null)
            {
                this._areas.Clear();
                this._areas = null;
            }
            if (this._bindableSeats != null)
            {
                this._bindableSeats.Clear();
                this._bindableSeats = null;
            }
            if (this._seatBoxes != null)
            {
                this._seatBoxes.Clear();
                this._seatBoxes = null;
            }
            if (this._otherAgents != null)
            {
                this._otherAgents.Clear();
                this._otherAgents = null;
            }
            this._parentModel = null;
            this._selectedArea = null;
            this._selectedSeat = null;
            this.TrySubmitChanged = null;
            this.BlockConverter.Dispose();
            this.BlockConverter = null;
        }
    }
}
