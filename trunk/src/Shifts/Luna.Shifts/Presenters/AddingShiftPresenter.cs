using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Core.Metadata;
using Caliburn.PresentationFramework.Filters;
using Luna.Common.Extensions;
using Luna.Shifts.Domain;
using Luna.Shifts.Presenters.Interfaces;
using Luna.WPF.ApplicationFramework;
using Luna.WPF.ApplicationFramework.Attributes;
using Luna.Core.Extensions;
using System.Collections;
using Luna.Common;

namespace Luna.Shifts.Presenters
{
    [PerRequest(typeof(IAddingShiftPresenter))]
    public class AddingShiftPresenter : DefaultPresenter, IAddingShiftPresenter
    {
        private bool _applied;
        private Exception _applyFailException;

        protected override void OnInitialize()
        {
            if (AvailableTypes != null)
                SelectedEventType = AvailableTypes.FirstOrDefault();

            var list = GetSelectedAgents(null).Cast<IAgent>().ToArray();

            EmployeeSelectionWay = list.Length > 1 ? 1 : 0;

            CurrentAgent = list.Length > 0 ? list[0] : default(IAgent);
            SelectedCount = CurrentAgent.If<ISelectable>(o => o.IsSelected == true) ? list.Length : list.Length > 0 ? list.Length - 1 : 0;


            base.OnInitialize();
        }

        private IAgent _currentAgent;
        public IAgent CurrentAgent { get { return _currentAgent; } set { _currentAgent = value; this.NotifyOfPropertyChange(o => o.CurrentAgent); } }

        private int _selectedCount;
        public int SelectedCount { get { return _selectedCount; } set { _selectedCount = value; this.NotifyOfPropertyChange(o => o.SelectedCount); } }

        public IEnumerable<TermStyle> AvailableTypes { get; set; }

        public Func<bool?, IEnumerable> GetSelectedAgents { get; set; }

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
                    ShiftStart = ShiftStart.Date.AddMinutes(_selectedEventType.TimeRange.StartValue);
                    ShiftEnd = ShiftStart.AddMinutes(_selectedEventType.TimeRange.Length);
                }
                this.NotifyOfPropertyChange("SelectedEventType");
            }
        }

        private DateTime _shiftStart;
        public DateTime ShiftStart
        {
            get { return _shiftStart; }
            set
            {
                _shiftStart = value;
                if (_selectedEventType != null)
                    ShiftEnd = _shiftStart.AddMinutes(_selectedEventType.TimeRange.Length);
                //this.SetWatchPoint(_shiftStart.Date);
                this.NotifyOfPropertyChange("ShiftStart");
            }
        }

        private DateTime _shiftEnd;
        public DateTime ShiftEnd
        {
            get { return _shiftEnd; }
            set
            {
                _shiftEnd = value;
                this.NotifyOfPropertyChange("ShiftEnd");
            }
        }

        public string Comments { get; set; }

        public bool CanAddEvent()
        {
            return SelectedEventType != null && GetSelectedAgents(EmployeeSelectionWay == 1).Count() > 0;
        }

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

        public Action<IAddingTermPresenter, Exception> WhenClosed { get; set; }

        public Action<IEnumerable, bool> RefreshDelegate { get; set; }

        public void NavigateTo()
        {
            this.SetWatchPoint(_shiftStart.Date);
        }


        [Preview("CanAddEvent")]
        [Dependencies("EmployeeSelectionWay", "SelectedEventType")]
        public void Apply()
        {
            if (RefreshDelegate == null) return;

            _applied = false;

            var rules = SelectedEventType.As<AssignmentType>().GetSubEventInsertRules();

            var selectedAgents = GetSelectedAgents(EmployeeSelectionWay == 1).ForEach<IAgent>(o =>
            {
                var timeBox = o.Schedule;
                var newAssignment = Term.NewAssignment(ShiftStart, ShiftEnd, SelectedEventType);
                newAssignment.Tag = Comments;
                timeBox.Create(newAssignment, (terms, success) =>
                {
                    if (success)
                    {
                        timeBox.ArrangeSubEvent(newAssignment as IAssignment, rules, (t, result) =>
                        {
                            if (success)
                                ((Term)t).Tag = Comments;
                        });
                        _applied = true;
                    }
                    else
                    {
                        o.OperationFail = true;
                    }
                }, false);
            });

            RefreshDelegate.Invoke(selectedAgents, _applied);
        }

        protected override void OnShutdown()
        {
            if (WhenClosed != null)
                WhenClosed(this, _applyFailException);
            base.OnShutdown();
        }
    }
}

