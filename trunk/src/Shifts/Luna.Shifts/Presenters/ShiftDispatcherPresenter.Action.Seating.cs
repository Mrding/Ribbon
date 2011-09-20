using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Filters;
using Luna.Common;
using Luna.Common.Extensions;
using Luna.Core;
using Luna.Core.Extensions;
using Luna.Globalization;
using Luna.Shifts.Domain;
using Luna.WPF.ApplicationFramework;
using System.Collections;
using System.Linq;
using Luna.Shifts.Presenters.Interfaces;
using Luna.WPF.ApplicationFramework.Actions;
using Luna.WPF.ApplicationFramework.Attributes;
using Luna.WPF.ApplicationFramework.Extensions;
using Luna.WPF.ApplicationFramework.Interfaces;


namespace Luna.Shifts.Presenters
{
    public partial class ShiftDispatcherPresenter
    {
        private Action<Reflector> _buildSeatDispatcher;
        private Action _destroySeatDispatcher;
        private Reflector _seatDispatcherPresenter;
        private double _seatDispatcherHeight = 250;

        public void RegisterRunSeatDispatcher(FrameworkElement el, Grid container, int rowPosition)
        {
            _buildSeatDispatcher = new Action<Reflector>(model =>
            {
                container.RowDefinitions[rowPosition].Height = new GridLength(_seatDispatcherHeight);
                View.SetModel(el, model.Target);

                model.SetProperty<System.Func<bool, Luna.Infrastructure.Domain.ISimpleEmployee, bool>>("TrySubmitChanged", (test, employee) =>
                {
                    if (test == false && IsDirty == false)
                    {
                        SubmitSeatChanges(new[] { _attendances.FirstOrDefault(o => o.Profile.Equals(employee)) }, false);
                    }
                    return !IsDirty;  // IsDirty means can't call UpdateSchedule
                });
                model.SetProperty<Action<int>>("OccupationsChanged", (i) =>
                                                                         {
                                                                             var index = i == -1 ? BindableAgents.IndexOf(SelectedAgent) : i;
                                                                             AgentOccupations[index] = BindableAgents[index].SaftyGetProperty<IEnumerable, IWorkingAgent>(o => o.Occupations);
                                                                             this.QuietlyReload(ref _agentOccupations, "AgentOccupations");
                                                                         });
                model.SetProperty<Action<IList<IEnumerable>>>("OccupationsReloaded", (list) =>
                                                                            {
                                                                                AgentOccupations = (from IWorkingAgent a in BindableAgents where a.Occupations != null select (IEnumerable)a.Occupations).ToList();
                                                                            });
                _seatDispatcherPresenter = model;

            });
            _destroySeatDispatcher = new Action(() =>
            {
                _seatDispatcherHeight = container.RowDefinitions[rowPosition].ActualHeight;

                View.SetModel(el, null);
                container.RowDefinitions[rowPosition].Height = new GridLength(0);

            });
        }

        /// <summary>
        /// RunSeatDispatcher(Message attach use)
        /// </summary>
        /// <param name="model">SeatDispatcher</param>
        public void RunSeatDispatcher(IPresenter model)
        {
            if (model != null)
            {
                SeatDispatcherOpened = _seatDispatcherPresenter == null;
                if (SeatDispatcherOpened)
                {
                    _buildSeatDispatcher(new Reflector(model));
                }
                else
                {
                    model.Shutdown();
                    _destroySeatDispatcher.Invoke();
                    _seatDispatcherPresenter = null;
                }
            }
        }

        public bool CanAssignSeat()
        {
            var shift = SelectedTerm;

            if (IsDirty || _seatDispatcherPresenter == null || shift == null) return false;

            var selectedSeat = _seatDispatcherPresenter.Property<SeatBox>("SelectedSeat");
            if (selectedSeat == null || !shift.IsNeedSeat)
                return false;

            return shift.Bottom == null || !shift.Bottom.IsNeedSeat;
        }

        public bool BeforeAssignSeat(IQuestionPresenter p)
        {
            var shift = SelectedTerm;
            if (shift == null) return false;
            var selectedSeat = _seatDispatcherPresenter.Property<SeatBox>("SelectedSeat");
            p.DisplayName = LanguageReader.GetValue("Shifts_ShiftDispatcher_AskAssignSeat");
            p.Editable = false;
            p.Text = string.Format(LanguageReader.GetValue("Shifts_ShiftDispatcher_AskAssignSeatText"),
              selectedSeat.Seat.Area.Name, selectedSeat.Seat.Number, SelectedAgent.Profile.Name, shift.Start, shift.Text);
            return true;
        }

        [Preview("CanAssignSeat")]
        [Preview("IsBelongToSchedule")]
        [Dependencies("SelectedTerm")]
        [Question("BeforeAssignSeat")]
        public void AssignSeat()
        {
            _seatDispatcherPresenter.Proc<Term, IWorkingAgent>("AssignSeat")(SelectedTerm, SelectedAgent as IWorkingAgent);
            SubmitSeatChanges(new[] { SelectedAgent }, false);
        }

        public bool CanCancelSeat()
        {
            var o = SelectedTerm;
            if (IsDirty || o == null) return false;
            return (o.IsNeedSeat || o.GetIsNeedSeatField());
        }


        [Preview("IsBelongToSchedule")]
        [Preview("CanCancelSeat")]
        [Dependencies("SelectedTerm")]
        public void CancelSeat()
        {
            var term = SelectedTerm;
            term.IsNeedSeat = false;
            term.Seat = null;

            if (term.Level == 1)
            {
                term.DiveInto(t =>
                             {
                                 if (t.SeatIsEmpty()) return false;
                                 term.Seat = t.Seat;
                                 return true;
                             });
            }

            term.SaftyInvoke<AssignmentBase>(o =>
            {
                foreach (var t in SelectedAgent.Schedule.GetCoveredTermsWithAbsent(term))
                {
                    if (t.IsNeedSeat) continue;
                    t.Seat = null;
                }
            });
            SubmitSeatChanges(new[] { SelectedAgent }, true);
        }

        public bool CanRescheduleSeatByDate()
        {
            return IsDirty == false && GetSelectedAgent(true).Count() > 0;
        }

        public void BeforeCancelSeat(IQuestionPresenter p)
        {
            p.DisplayName = LanguageReader.GetValue("Shifts_ShiftDispatcher_AskCancelSeat");
            p.Editable = false;
            p.Text = string.Format(LanguageReader.GetValue("Shifts_ShiftDispatcher_AskCancelSeatText"), GetWatchPoint());
        }

        [Preview("CanRescheduleSeatByDate")]
        [Question("BeforeCancelSeat")]
        public void RescheduleSeatByDate()
        {
            var start = GetWatchPoint().Date;
            var end = start.AddDays(1);

            var agents = GetSelectedAgent(true);
            foreach (IAgent agent in agents)
            {
                agent.Schedule.TermSet.ForEach(o => o.GetLowestTerm().SaftyInvoke<ITerm>(t =>
                {
                    if (t.StartIsCoverd(start, end))
                    {
                        o.ReleaseSeat();
                    }
                }));
            }
            SubmitSeatChanges(agents, true);
        }

        public bool CanRescheduleSeat()
        {
            var term = SelectedTerm;
            if (term == null || term.Is<IOffWork>()) return false;

            if (!term.SeatIsEmpty()) return true;

            if (!term.GetIsNeedSeatField()) return true;

            return false;
            //return term != null && (term is AssignmentBase || (term.Level == 1 && term.GetIsNeedSeatField() && term.SeatIsEmpty()) ||
            //    (term.Level == 2 && !term.SeatIsEmpty() && term.IsNeedSeat));
        }

        [Preview("IsBelongToSchedule")]
        [Preview("CanRescheduleSeat")]
        [Dependencies("SelectedTerm")]
        public void RescheduleSeat()
        {
            if (SelectedTerm is AssignmentBase)
                SelectedAgent.Schedule.EmptySeatArrangment<AssignmentBase>(SelectedTerm);
            else
            {
                SelectedTerm.ReleaseSeat();
                SelectedTerm.IsNeedSeat = true;
            }
            SubmitSeatChanges(new[] { SelectedAgent }, true);
        }

        private void SubmitSeatChanges(IEnumerable effectedAgents, bool reactToSeatDispatcher)
        {
            try
            {
                //effectedAgents.ForEach<IAgent>(o => o.Schedule.Operator = string.Format("{0}@{1:yyyy/MM/dd HH:mm:ss.fff}", Luna.Core.ApplicationCache.Get<string>(Keys.AgentId), DateTime.Now));
                _shiftDispatcherModel.UpdateSchedule(false);
                //BlockConverter.Refresh();
                if (reactToSeatDispatcher && _seatDispatcherPresenter != null)
                    _seatDispatcherPresenter.Proc("FastReloadData")();
            }
            catch (Exception ex)
            {
                //MsgBoxWindow.Show(LanguageReader.GetValue("Seating_SeatDispatcher_SubmitFailMessage"),
                //    LanguageReader.GetValue("Seating_SeatDispatcher_SubmitFailTitle"), ex.GetExceptionMessage(), MessageBoxButton.OK, MessageBoxImage.Warning);
                ReloadAgents(effectedAgents.ToList(), ex);
            }
        }
    }
}
