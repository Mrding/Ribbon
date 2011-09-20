using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Caliburn.PresentationFramework.Filters;
using Luna.Common;
using Luna.Common.Constants;
using Luna.Core.Extensions;
using Luna.Shifts.Domain;
using Luna.Shifts.Presenters.Interfaces;
using Luna.WPF.ApplicationFramework;
using Luna.WPF.ApplicationFramework.Actions;
using Luna.WPF.ApplicationFramework.Extensions;
using Luna.Common.Extensions;
using Luna.WPF.ApplicationFramework.Attributes;
using TermExt = Luna.Shifts.Domain.TermExt;

namespace Luna.Shifts.Presenters
{
    public partial class SeatDispatcherPresenter : DockingPresenter
    {
        private Action RetainCurrentSelectedSeat()
        {
            var selectedIndex = BindableSeats.IndexOf(SelectedSeat);
            return () => { SelectedSeat = selectedIndex == -1 ? null : _bindableSeats[selectedIndex] as SeatBox; };
        }

        public void Reload(IEnumerable affectedRows)
        {
            var rows = affectedRows ?? BindableSeats.ToList();

            foreach (SeatBox t in rows)
            {
                if (t.IsNot<SeatBox>()) break;
                var index = _bindableSeats.IndexOf(t);
                if (index == -1) continue;

                _bindableSeats[index] = t;
            }
            var reselectSeat = RetainCurrentSelectedSeat();
            this.QuietlyReload(ref _bindableSeats, "BindableSeats");
            reselectSeat.Invoke();
        }

        public void FastReloadData()
        {
            ExecuteManager.BackgroundAction(() =>
            {
                foreach (SeatBox seatBox in _seatBoxes.Values)
                    seatBox.Initial();
                BuildSeatArrangement<TimeBox>(_otherAgents, o => o);
                BuildSeatArrangement<IAgent>(_parentModel.Agents, o => o.Schedule);
                var reselectSeat = RetainCurrentSelectedSeat();
                this.QuietlyReload(ref _bindableSeats, "BindableSeats");
                reselectSeat.Invoke();
            });
        }

        public void ReloadData()
        {
            ExecuteManager.BackgroundAction(() =>
            {
                foreach (var item in _seatDispatcherModel.GetSeats(_schedule, _excludedEmployeeIds, out _otherAgents, out _areas))
                    _seatBoxes[item.Id.ToString()] = (IEnumerable)item;

                IsDirty = false;
                BuildSeatArrangement<TimeBox>(_otherAgents, o => o);
                BuildSeatArrangement<IAgent>(_parentModel.Agents, o => o.Schedule);
                FilterSeats(SelectedArea);
            }, BeginLoading, EndLoading);
        }

        [Function(FunctionKeys.AddSeatEvent)]
        public void LaunchAddSeatEvent()
        {
            var list = _bindableSeats.Where(o => o.If<ISelectable>(x => x.IsSelected == true)).ToArray();
            Action<IEnumerable> applyAction = (items) =>
            {
                //items.ForEach<SeatBox>(o => o.RetainSeatEventsThenAddBackOthers());
                _seatDispatcherModel.SubmitChanges();
                //IsDirty = true;
                Reload(items);
            };

            OpenDialog<IAddSeatEventPresenter>(new Dictionary<string, object> { 
                { "CanChooseRange", _schedule },
                { "SelectedDate", GetWatchPoint() },
                { "SelectedSeats", list.Length == 0 ? new[]{ SelectedSeat} : list },
                { "ReloadParent", applyAction}
            });
        }

        public bool CanDelete()
        {
            return SelectedBlock.Is<SeatEvent>();
        }

        [Dependencies("SelectedBlock")]
        [Preview("CanDelete")]
        public void Delete()
        {
            SelectedSeat.RemoveOccupation((Occupation)SelectedBlock);
            _seatDispatcherModel.SubmitChanges();
            //IsDirty = true;
            Reload(new[] { SelectedSeat });
        }

        public bool CanPaste()
        {
            if (_occupationClipboard == null || SelectedSeat == null) return false;

            if (_occupationClipboard.Any(o => o.Seat == SelectedSeat.Seat))
                return false;

            if (TrySubmitChanged != null && _occupationClipboard.Any(o => o is SeatArrangement))
                return TrySubmitChanged(true, null);

            return true;
        }

        public void SubmitChangesFail(Exception ex)
        {
            //ExecuteManager.BackgroundAction(this, () =>
            //{
            //    ReloadAgents(null, ex);//also might include rtaa reload
            //    _changedAgents.Clear();
            //}, () => { }, () => { });
        }


        [Dependencies("SelectedSeat")]
        [Preview("CanPaste")]
        public void Paste()
        {
            var effectedRows = new SeatBox[_occupationClipboard.Length + 1];
            effectedRows[effectedRows.Length - 1] = SelectedSeat;
            Action applyChanges = () => { };
            for (var i = 0; i < _occupationClipboard.Length; i++)
            {
                var index = i;
                var occupation = _occupationClipboard[i];
                _seatBoxes[occupation.Seat.Id.ToString()].SaftyInvoke<SeatBox>(o =>
                {
                    if (o.RemoveOccupation(occupation) && SelectedSeat.AddOccupation(occupation))
                    {
                        occupation.Seat = SelectedSeat.Seat;
                        applyChanges =
                            occupation.Is<SeatArrangement>() ?
                                (() => TrySubmitChanged(false, ((SeatArrangement)occupation).Agent)) : new Action(() =>
                                {
                                    _seatDispatcherModel.SubmitChanges();
                                    _occupationClipboard = null;
                                });
                    }
                    effectedRows[index] = o;
                });
            }
            applyChanges.Invoke();
            Reload(effectedRows);
            OccupationsReloaded(null);
        }

        public void ClearClipboard()
        {
            _occupationClipboard = null;
        }

        public bool CanCut()
        {
            if (TrySubmitChanged != null && SelectedBlock.Is<SeatArrangement>())
                return TrySubmitChanged(true, null);

            return true;
        }

        [Dependencies("SelectedBlock")]
        [Preview("CanCut")]
        public void Cut()
        {
            _occupationClipboard = new[] { (Occupation)SelectedBlock };
        }

        public void SortBySeatNo()
        {
            _bindableSeats = _bindableSeats.OrderBy(o => o.SaftyGetProperty<string, SeatBox>(x => x.Seat.Number)).ToList();
            var reselectSeat = RetainCurrentSelectedSeat();
            this.QuietlyReload(ref _bindableSeats, "BindableSeats");
            reselectSeat.Invoke();

            //_sortDelegate = () => { SortBySeatNo(); };
        }

        public void FilterSeats(Entity area)
        {
            _bindableSeats = (from item in _seatBoxes
                              where item.Value.If<SeatBox>(o => o.Seat.Area.Equals(area))
                              select item.Value).ToList();

            var reselectSeat = RetainCurrentSelectedSeat();
            this.QuietlyReload(ref _bindableSeats, "BindableSeats");
            reselectSeat.Invoke();
            BlockConverter.Refresh();
        }

        public void FindEmptySeat(DateTime targetPoint)
        {
            _bindableSeats = _bindableSeats.OrderBy(s =>
                                              {
                                                  var seatbox = (SeatBox)s;
                                                  var result = default(Occupation);
                                                  foreach (var o in seatbox.Occupations.Retrieve<Occupation>(targetPoint.Date, targetPoint.Date).OrderBy(o => o.Start))
                                                  {
                                                      if (o.Start <= ClickTime && ClickTime <= o.End)
                                                      {
                                                          return TimeSpan.FromDays(2);//sorting score
                                                      }
                                                      if (o.Start > targetPoint)
                                                      {
                                                          result = o;
                                                          break;
                                                      }
                                                  }
                                                  if (result == null)
                                                      return TimeSpan.FromDays(1);//sorting score
                                                  return result.Start.Subtract(targetPoint); //sorting score
                                              }).ToList();
            //var reselectSeat = RetainCurrentSelectedSeat();
            this.QuietlyReload(ref _bindableSeats, "BindableSeats");
            //reselectSeat.Invoke();
            CurrentIndex = 0;
            SelectedSeat = BindableSeats[0] as SeatBox;
        }

        public void NavigateTo(object item)
        {
            var index = -1;
            if (item != null && item.IsNot<SeatBox>())
            {
                var seatId = item.ToString();
                if (_seatBoxes.ContainsKey(seatId))
                {

                    var target = (SeatBox)_seatBoxes[seatId];
                    if (target.Seat.Area != SelectedArea)
                    {
                        FilterSeats(target.Seat.Area);
                        SelectedArea = target.Seat.Area;
                    }
                    index = BindableSeats.IndexOf(target);
                }
            }

            if (index == -1)
                return;

            CurrentIndex = index;
            SelectedSeat = BindableSeats[index] as SeatBox;
        }

        public void SetSelectTermClickCount(MouseButtonEventArgs args)
        {
            _selectTermClickCount = args.ClickCount;
        }

        public void NavigateToAgent()
        {
            if (_selectTermClickCount == 2) //double click
            {
                SelectedOccupation.SaftyInvoke<SeatArrangement>(
                    o => _parentModel.SaftyInvoke<IBlockMatrixContainer>(x => x.NavigateTo(o.Agent)));

                _selectTermClickCount = 0;
            }
        }

        public void AssignSeat(Term shift, IWorkingAgent selectedAgent)
        {
            var affectedRows = new[] { SelectedSeat };
            if (!shift.SeatIsEmpty())
            {
                var removeDelegates = new Action(() => { });
                affectedRows = selectedAgent.Schedule.GetCoveredTermsWithAbsent(shift).Add(shift)
                    .Where(o => !o.SeatIsEmpty() && _seatBoxes.ContainsKey(o.Seat))
                    .Select(o =>
                                {
                                    var seatBox = (SeatBox)_seatBoxes[o.Seat];
                                    foreach (var c in seatBox.Occupations.Where(c => c.If<SeatArrangement>(a => a.Source.Equals(o))))
                                    {
                                        var occupation = c;
                                        removeDelegates += () =>
                                        {
                                            seatBox.RemoveOccupation(occupation);
                                            selectedAgent.Occupations.Remove(occupation);
                                        };
                                    }
                                    return seatBox;
                                })
                    .ToArray();
                removeDelegates();

                //var originalSeat = (SeatBox)_seatBoxes[shift.Seat];
                //var seatArrangement = originalSeat.Occupations.OfType<SeatArrangement>().FirstOrDefault(o => o.Source == shift && o.TimeEquals(shift));
                //if (seatArrangement != null)
                //    originalSeat.RemoveOccupation(seatArrangement);
                //affectedRows = new[] { SelectedSeat, originalSeat };
            }

            var prv = default(Term);
            var seatId = SelectedSeat.Id.ToString();
            shift.Seat = seatId;
            var coveredTerms = selectedAgent.Schedule.GetCoveredTermsWithAbsent(shift);
            coveredTerms.ForEach(o => o.Seat = null).Add(shift)
                                  .SliceOccupied((dateRange, term) =>
                                                     {
                                                         if (!term.IsNeedSeat)
                                                         {
                                                             term.Seat = seatId;
                                                             prv = term;
                                                             return default(SeatArrangement);
                                                         }

                                                         if (prv != null && !ReferenceEquals(prv, term) && prv.Level == 0 && term.Level == 0)
                                                         {
                                                             prv = null;
                                                         }

                                                         var source = default(Term);
                                                         TermExt.X(prv, term, ref source);
                                                         var instance = new SeatArrangement(selectedAgent.Profile, dateRange.Start, dateRange.End, SelectedSeat.Seat) { Source = source };
                                                         source.Seat = seatId;
                                                         SelectedSeat.AddOccupation(instance);
                                                         selectedAgent.Occupations.Add(instance);
                                                         prv = term;
                                                         return instance;
                                                     }, t => t.IsNeedSeat);
            //shift.SaftyInvoke<AssignmentBase>(o => { o.OccupyStatus = "C"; });
            Reload(affectedRows);
            OccupationsChanged(-1);
        }

        public void SubmitChanges(bool abort)
        {

        }

        public void SelectAll(bool isChecked)
        {
            foreach (ISelectable item in _bindableSeats)
            {
                item.IsSelected = isChecked;
            }
        }
    }
}
