using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Core.Metadata;
using Caliburn.PresentationFramework.Filters;
using Luna.Common.Extensions;
using Luna.Globalization;
using Luna.Shifts.Domain;
using Luna.Shifts.Presenters.Interfaces;
using Luna.WPF.ApplicationFramework;
using Luna.WPF.ApplicationFramework.Attributes;
using Caliburn.PresentationFramework.ApplicationModel;
using System.Collections;
using Luna.Core.Extensions;

namespace Luna.Shifts.Presenters
{
    [PerRequest(typeof(IAddingEventPresenter))]
    public class AddingEventPresenter : DockingPresenter, IAddingEventPresenter
    {
        private bool _applied;
        private Exception _applyFailException;

        protected override void OnInitialize()
        {
            if (AvailableTypes != null)
                SelectedEventType = AvailableTypes.FirstOrDefault();

            CloseWithInvoker(true);
            base.OnInitialize();
        }

        private TermStyle _selectedEventType;
        public TermStyle SelectedEventType
        {
            get { return _selectedEventType; }
            set
            {
                if (value != null && value.IsNew()) return;
                _selectedEventType = value;
                if (_selectedEventType != null)
                {
                    EventLength = _selectedEventType.TimeRange.Length;
                }
                  
                this.NotifyOfPropertyChange("SelectedEventType");
            }
        }

        public IEnumerable<TermStyle> AvailableTypes { get; set; }

        public Func<bool?, IEnumerable> GetSelectedAgents { get; set; }

        public Func<int, double> MeasureEventWidth { get; set; }

        private DateTime _eventStart;
        public DateTime EventStart
        {
            get { return _eventStart; }
            set
            {
                _eventStart = value;
                if (_selectedEventType != null)
                    EventEnd = _eventStart.AddMinutes(EventLength);
                this.NotifyOfPropertyChange("EventStart");
            }
        }

        public Action<IAddingEventPresenter> OnActivateDelegate { get; set; }

        private DateTime _eventEnd;
        public DateTime EventEnd
        {
            get { return _eventEnd; }
            set
            {
                _eventEnd = value;
                this.NotifyOfPropertyChange("EventEnd");
            }
        }

        private int _eventLength;
        public int EventLength
        {
            get { return _eventLength; }
            set
            {
                _eventLength = value.TurnToMultiplesOf5();
                if (_selectedEventType != null)
                {
                    EventEnd = _eventStart.AddMinutes(_eventLength);
                    _selectedEventType.TimespanWidth = MeasureEventWidth(_eventLength);
                    _selectedEventType.CustomLength = _eventLength;
                }
                NotifyOfPropertyChange(() => EventLength);
            }
        }

        public string Comments { get; set; }

        public bool CanAddEvent()
        {
            return SelectedEventType != null && GetSelectedAgents(EmployeeSelectionWay == 1).Count() > 0;
        }

        public bool SupportTwoWayAdding { get; set; }


        private int _employeeSelectionWay;
        public int EmployeeSelectionWay
        {
            get { return _employeeSelectionWay; }
            set
            {
                _employeeSelectionWay = value;
                this.NotifyOfPropertyChange("EmployeeSelectionWay");
            }
        }

        private string _results;
        public string Results
        {
            get { return _results; }
            set
            {
                _results = value;
                this.NotifyOfPropertyChange("Results");
            }
        }

        public Action<IEnumerable, bool> RefreshDelegate { get; set; }

        public Action<IAddingTermPresenter, Exception> WhenClosed { get; set; }



        [Preview("CanAddEvent")]
        [Dependencies("EmployeeSelectionWay", "SelectedEventType")]
        public void Apply()
        {
            Results = string.Empty;
            _applied = false;

          IEnumerable selectedAgents = GetSelectedAgents(EmployeeSelectionWay == 1).ForEach<IAgent>(o =>
            {
                var timeBox = o.Schedule;

                timeBox.Create(Term.New(EventStart, SelectedEventType, EventLength), (t, success) =>
                {
                    if (success)
                    {
                        ((Term)t).Tag = Comments;
                        _applied = true;
                    }

                    o.OperationFail = !success;
                }, false);
            });

            if (RefreshDelegate != null)
                RefreshDelegate.Invoke(selectedAgents, _applied);
            //if (ReloadAgentDelegate != null)
            //    SelectedAgents = ReloadAgentDelegate();
        }

        public override void Activate()
        {
            if (OnActivateDelegate != null)
                OnActivateDelegate(this);

            var list = GetSelectedAgents(null).Cast<IAgent>().ToArray();

            EmployeeSelectionWay = list.Length > 1 ? 1 : 0;

            CurrentAgent = list.Length > 0 ? list[0] : default(IAgent);
            SelectedCount = CurrentAgent.If<Luna.Common.ISelectable>(o => o.IsSelected == true) ? list.Length : list.Length > 0 ? list.Length - 1 : 0;
           
            if(SelectedEventType!=null)
                SelectedEventType.TimespanWidth = MeasureEventWidth(EventLength);
        }

        private IAgent _currentAgent;
        public IAgent CurrentAgent { get { return _currentAgent; } set { _currentAgent = value; this.NotifyOfPropertyChange(o => o.CurrentAgent); } }

        private int _selectedCount;
        public int SelectedCount { get { return _selectedCount; } set { _selectedCount = value; this.NotifyOfPropertyChange(o => o.SelectedCount); } }


        protected override void OnShutdown()
        {
            if (WhenClosed != null)
                WhenClosed(this, _applyFailException);
            Invoker = null;
            MeasureEventWidth = null;
            GetSelectedAgents = null;
            AvailableTypes = null;
            _selectedEventType = null;
            RefreshDelegate = null;
            CurrentAgent = null;
            WhenClosed = null;
            OnActivateDelegate = null;
            base.OnShutdown();
        }
    }
}
