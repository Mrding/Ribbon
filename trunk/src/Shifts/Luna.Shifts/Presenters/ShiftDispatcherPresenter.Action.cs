using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Caliburn.PresentationFramework.Filters;
using Luna.Common;
using Luna.Common.Extensions;
using Luna.Core.Extensions;
using Luna.Globalization;
using Luna.Shifts.Domain;
using Luna.Shifts.Presenters.Interfaces;
using Luna.WPF.ApplicationFramework;
using Luna.WPF.ApplicationFramework.Actions;
using Luna.WPF.ApplicationFramework.Attributes;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Metadata;
using Luna.WPF.ApplicationFramework.Extensions;
using Luna.Common.Domain;
using Luna.WPF.ApplicationFramework.Interfaces;

namespace Luna.Shifts.Presenters
{
    public partial class ShiftDispatcherPresenter
    {
        #region ContextMenu.Sort

        public void Sort()
        {
            if (_sortDelegate != null)
            {
                _sortDelegate.Invoke();
                AgentOccupations = (from IWorkingAgent a in BindableAgents where a.Occupations != null select (IEnumerable)a.Occupations).ToList();
            }
        }

        public void Sort(DateTime screenStart, DateTime screenEnd)
        {
            screenEnd = screenStart.Date.AddDays(1);
            _sortDelegate = new Action(() => Sort(screenStart, screenEnd));

            ((List<IEnumerable>)BindableAgents).Sort(delegate(IEnumerable x, IEnumerable y)
               {
                   var xAgent = x as IAgent;
                   var yAgent = y as IAgent;
                   var otherShift =
                       xAgent.Schedule.TermSet.FirstOrDefault(
                           o =>
                           o.Level == 0 &&
                           (o.End > screenStart && o.Start < screenEnd));
                   var selfShift =
                       yAgent.Schedule.TermSet.FirstOrDefault(
                           o =>
                           o.Level == 0 &&
                            (o.End > screenStart && o.Start < screenEnd));

                   if (selfShift == default(Term) &&
                       otherShift == default(Term))
                       return 0;
                   if (otherShift == default(Term))
                       return 1;
                   if (selfShift == default(Term))
                       return -1;

                   int temp = otherShift.Start.CompareTo(selfShift.Start);

                   if (temp == 0)
                   {
                       temp = otherShift.Text.CompareTo(selfShift.Text);
                       if (temp == 0)
                           return xAgent.Profile.Name.CompareTo(yAgent.Profile.Name);
                       return temp;
                   }
                   return temp;
               });
            QuietlyReloadAgents(_bindableAgents);
        }

        public void SortByAgentName(DateTime screenStart, DateTime screenEnd)
        {
            screenEnd = screenStart.Date.AddDays(1);
            _sortDelegate = new Action(() => SortByAgentName(screenStart, screenEnd));

            ((List<IEnumerable>)BindableAgents).Sort((IEnumerable x, IEnumerable y) =>
               {
                   var xAgent = x as IAgent;
                   var yAgent = y as IAgent;
                   return xAgent.Profile.Name.CompareTo(yAgent.Profile.Name);
               });
            QuietlyReloadAgents(_bindableAgents);
        }

        public void SortByLaborRuleHasError(DateTime screenStart, DateTime screenEnd)
        {
            screenEnd = screenStart.Date.AddDays(1);
            _sortDelegate = new Action(() => SortByLaborRuleHasError(screenStart, screenEnd));
            ((List<IEnumerable>)BindableAgents).Sort(delegate(IEnumerable x, IEnumerable y)
            {
                var xAgent = x as IAgent;
                var yAgent = y as IAgent;

                int temp = yAgent.LaborRule.HasError.CompareTo(xAgent.LaborRule.HasError);
                if (temp == 0)
                    return xAgent.Profile.Name.CompareTo(yAgent.Profile.Name);
                return temp;

            });
            QuietlyReloadAgents(_bindableAgents);
        }

        public void SortByShiftName(DateTime screenStart, DateTime screenEnd)
        {
            screenEnd = screenStart.Date.AddDays(1);
            _sortDelegate = new Action(() => Sort(screenStart, screenEnd));

            ((List<IEnumerable>)BindableAgents).Sort(delegate(IEnumerable x, IEnumerable y)
            {
                var xAgent = x as IAgent;
                var yAgent = y as IAgent;
                var otherShift =
                    xAgent.Schedule.TermSet.FirstOrDefault(
                        o =>
                        o.Level == 0 &&
                         (o.End > screenStart && o.Start < screenEnd));
                var selfShift =
                    yAgent.Schedule.TermSet.FirstOrDefault(
                        o =>
                        o.Level == 0 &&
                         (o.End > screenStart && o.Start < screenEnd));

                if (selfShift == default(Term) &&
                    otherShift == default(Term))
                    return 0;
                if (otherShift == default(Term))
                    return 1;
                if (selfShift == default(Term))
                    return -1;

                int temp = otherShift.Text.CompareTo(selfShift.Text);

                if (temp == 0)
                {
                    temp = otherShift.Start.CompareTo(selfShift.Start);
                    if (temp == 0)
                        return xAgent.Profile.Name.CompareTo(yAgent.Profile.Name);
                    return temp;
                }
                return temp;
            });
            QuietlyReloadAgents(_bindableAgents);
        }

        public void SortByStartTime(DateTime screenStart, DateTime screenEnd)
        {
            Sort(screenStart, screenEnd);
        }

        #endregion

        public void ShowShiftText(bool isChecked)
        {
            _blockConverter.ShowText = isChecked;
            //BlockConverter.Refresh();
        }

        public void SelectAll(bool isChecked)
        {
            foreach (ISelectable item in _bindableAgents)
            {
                item.IsSelected = isChecked;
            }
        }

        public void HideOccupationMask()
        {
            ShowOccupationMask = !ShowOccupationMask;
        }

        public bool IsBelongToSchedule()
        {
            if (SelectedTerm == null || SelectedAgent == null || SelectedTerm.Is<IImmutableTerm>())
                return false;
            //return !SelectedTerm.GetLowestTerm().IsOutOfBoundary(SelectedTerm.Start, SelectedAgent.Schedule);
            return true;
        }

        public bool CanLaunchMaintenanceSchedule()
        {
            return !IsDirty && GetSelectedAgent(true).Count() > 0 && !_maintenanceScheduleOpened;
        }

        private bool _maintenanceScheduleOpened;

        [Preview("CanLaunchMaintenanceSchedule")]
        public void LaunchMaintenanceSchedule()
        {
            //_maintenanceScheduleOpened = true;

            //base.OpenModelessDialog<IMaintenanceSchedulePresenter>(new Dictionary<string, object> { 
            //    { "GetSelectedAgents", new Func<bool?, IEnumerable>(GetSelectedAgent)},
            //    {"Invoker", this},
            //    { "CanChooseRange", Schedule },
            //    { "SelectedDate", GetWatchPoint() },
            //    { "InvokerCallBack", new Action<IList>(list=> ReloadAgents(list, null)) },
            //    { "WhenClosed", new Action<IPresenter, Exception>((p,ex)=>
            //                                                          {
            //                                                              _maintenanceScheduleOpened = false;
            //                                                              Application.Current.MainWindow.Activate();
            //                                                          })}
            //});
        }

        public bool CanLoad()
        {
            return !IsDirty;
        }

        [BackgroundAction(Before = "BeginLoading", Callback = "EndLoading", BlockInteraction = true)]
        [Preview("CanLoad")]
        [Dependencies("IsDirty")]
        public void ReloadAgents(IList affectedAgents, Exception ex)
        {
            var agents = (affectedAgents == null || affectedAgents.Count == 0) ? BindableAgents.ToList() : affectedAgents;
            var list = ReAssignAgents(agents);
            QuietlyReloadAgents(list);

        }

        //public bool CanNavigateToSeatLocation()
        //{
        //    return SelectedTerm != null;
        //}

        //[Preview("CanNavigateToSeatLocation")]
        public void NavigateToSeatLocation()
        {
            if (_selectTermClickCount == 2 && SelectedTerm != null) //double click
            {
                var termHasSeat = SelectedTerm.SeatIsEmpty() ? null : SelectedTerm;
                if (termHasSeat == null)
                {
                    SelectedTerm.DiveInto(t =>
                        {
                            if (!t.SeatIsEmpty())
                            {
                                termHasSeat = t;
                                return true;
                            }
                            return false;
                        });
                }
                if (termHasSeat != null)
                    _seatDispatcherPresenter.SaftyInvoke<Core.Reflector>(o => o.Target.SaftyInvoke<IBlockMatrixContainer>(p => p.NavigateTo(termHasSeat.Seat)));
                _selectTermClickCount = 0;
            }
        }

        public bool CanBeAbsent()
        {
            if (this.IsDirty) return false;
            var assignment = SelectedTerm as AssignmentBase;

            return assignment != null && SelectedAgent != null;
        }

        [Preview("IsBelongToSchedule")]
        [Preview("CanBeAbsent")]
        [Dependencies("IsDirty")]
        public void SetAsAbsent(TermStyle type, string kind)
        {
            //SelectedAgent.Schedule.Operator = string.Format("{0}@{1:yyyy/MM/dd HH:mm:ss.fff}", Core.ApplicationCache.Get<string>(Keys.AgentId), DateTime.Now);
            _shiftDispatcherModel.ApplyAbsent(SelectedAgent, SelectedTerm as AssignmentBase, type,
                                              kind == "Fully" ? SelectedTerm.GetLength() : TimeSpan.FromHours(1));
            Refresh(new[] { SelectedAgent }); //also might include rtaa reload

            if (_seatDispatcherPresenter != null)
                _seatDispatcherPresenter.Proc("FastReloadData")();
        }

        public bool AskDeleteShift(IQuestionPresenter p)
        {
            if (SelectedTerm == null) return false;

            p.DisplayName = LanguageReader.GetValue("Shifts_ShiftDispatcher_AskDeleteShift");
            p.Editable = false;
            p.Text = string.Format(LanguageReader.GetValue("Shifts_ShiftDispatcher_AskDeleteShiftText"),
                SelectedAgent.Profile.Name, SelectedTerm.Start, SelectedTerm.Text);
            return true;
        }

        public bool VerifyNoAbsentConvered()
        {
            if (SelectedAgent != null && SelectedTerm != null)
            {
                var absentEvents = SelectedAgent.Schedule.GetCoveredTermsWithAbsent(SelectedTerm).OfType<AbsentEvent>();

                return absentEvents.Count() == 0;
            }
            return false;
        }

        [Preview("VerifyNoAbsentConvered")]
        [Preview("IsBelongToSchedule")]
        [Question("AskDeleteShift")]
        [Dependencies("SelectedTerm")]
        [SuperRescue("AlterTermFail", "Shifts_ShiftDispatcher_SubmitChangesFail", "Shifts_AlterFail_Title")]
        public void DeleteShift()
        {
            if (SelectedAgent.Schedule.Delete(SelectedTerm, false))
            {
                _changedAgents.AddNotContains(SelectedAgent);
                IsDirty = true;
            }
            Refresh(new[] { SelectedAgent });//also might include rtaa reload
        }

        public bool VerifyNotAbsentEvent()
        {
            return !(SelectedTerm is AbsentEvent);
        }

        [SuperRescue("AlterTermFail", "Shifts_ShiftDispatcher_SubmitChangesFail", "Shifts_AlterFail_Title")]
        [Preview("VerifyNotAbsentEvent")]
        [Preview("IsBelongToSchedule")]
        [Dependencies("SelectedTerm")]
        public void UnlockOrLockShift()
        {
            SelectedTerm.Snapshot();//for logging
            SelectedTerm.Locked = !SelectedTerm.Locked;
            NotifyOfPropertyChange(() => SelectedTerm);
            _changedAgents.AddNotContains(SelectedAgent);
            IsDirty = true;
            //BlockConverter.Refresh();
        }

        public bool CanOpenAddEventDialog()
        {
            //if (!NoDialogOpened) return false;
            if (SelectedAgent == null || SelectedTerm == null) return false;
            return true;
        }

        [SuperRescue("AlterTermFail", "Shifts_ShiftDispatcher_SubmitChangesFail", "Shifts_AlterFail_Title")]
        [Preview("CanOpenAddEventDialog")]
        [Preview("IsBelongToSchedule")]
        [Dependencies("SelectedTerm")]
        public void OpenAddingEventDialog()
        {
            var current = FirstOrDefault<IAddingEventPresenter>(o => o != null);
            if (current != null)
            {
                Activate(current);
                return;
            }

            var hourWidth = Convert.ToDouble(Application.Current.FindResource("BlockZoomingHourWidth"));

            Show<IAddingEventPresenter>(new Dictionary<string, object> {
                                    {"MeasureEventWidth", new Func<int,double>((len)=> (len / 60d) * hourWidth * ZoomValue)},
                                    {"GetSelectedAgents",  new Func<bool?, IEnumerable>(GetSelectedAgent)},
                                    {"AvailableTypes",  _shiftDispatcherModel.GetAllEventTypes()},
                                    {"OnActivateDelegate", new Action<IAddingEventPresenter>((p)=> {
                                        p.EventStart = ClickTime.TurnToMultiplesOf5();
                                    })},
                                    {"SupportTwoWayAdding" , true},
                                    {"Invoker" , this},
                                    {"EventStart", ClickTime},
                                    {"WhenClosed",  new Action<IAddingTermPresenter,Exception>((p, ex)=> {
                                        //NoDialogOpened = true;                                                         
                                    }) },
                                    {"RefreshDelegate", new Action<IEnumerable,bool>(AddingTermPresenterCallback)}});

        }

        public bool CanOpenAddShiftDialog()
        {
            return (SelectedAgent != null && NoDialogOpened);
        }

        [Preview("CanOpenAddShiftDialog")]
        [Dependencies("SelectedAgent")]
        [SuperRescue("AlterTermFail", "Shifts_ShiftDispatcher_SubmitChangesFail", "Shifts_AlterFail_Title")]
        public void OpenAddingShiftDialog()
        {
            NoDialogOpened = false;
            OpenDialog<IAddingShiftPresenter>(new Dictionary<string, object> {
                                    {"GetSelectedAgents", new Func<bool?, IEnumerable>(GetSelectedAgent)},
                                    {"AvailableTypes", _shiftDispatcherModel.GetAllShiftTypes().OfType<TermStyle>() },
                                    {"ShiftStart", ClickTime.Date},
                                    {"WhenClosed",  new Action<IAddingTermPresenter,Exception>((p, ex)=> {
                                        NoDialogOpened = true;                                                         
                                    }) },
                                    {"RefreshDelegate", new Action<IEnumerable, bool>(AddingTermPresenterCallback)}});

        }

        public Dictionary<string, object> GetOperationParams()
        {
            return new Dictionary<string, object>
                       {
                           {"Invoker" , this},
                           {"GetSelectedAgents", new Func<bool?, IEnumerable>(GetSelectedAgent)},
                           {"RefreshDelegate", new Action<IEnumerable, bool>(AddingTermPresenterCallback)}
                       };
        }

        public IEnumerable GetSelectedAgent(bool? filterWithIsSelected)
        {
            if (filterWithIsSelected == true)
            {
                return BindableAgents.Cast<ISelectable>().Where(x => x.IsSelected == true);
            }
            else
            {
                var list = SelectedAgent == null ? new IAgent[0] : new[] { SelectedAgent };
                if (filterWithIsSelected == false)
                    return list;
                else
                {
                    var selectedItems = list.Union(BindableAgents.Cast<IAgent>().Where(x => ((ISelectable)x).IsSelected == true && !ReferenceEquals(x, SelectedAgent))).ToList();
                    if (selectedItems.Count > 0)
                        return selectedItems;
                    return list;
                }
            }
        }

        private void AddingTermPresenterCallback(IEnumerable affectedAgents, bool anySuccess)
        {
            if (anySuccess)
            {
                QuietlyReloadAgents(affectedAgents.ForEach<IAgent>(o => _changedAgents.AddNotContains(o)));
                IsDirty = true;
            }
        }

        public void SubmitChangesFail(Exception ex)
        {
            ExecuteManager.BackgroundAction(this, () =>
            {
                ReloadAgents(null, ex);//also might include rtaa reload
                _changedAgents.Clear();
            }, () => { }, () => { });
        }

        [SuperRescue("SubmitChangesFail", "Shifts_ShiftDispatcher_SubmitChangesFail", "Shifts_AlterFail_Title", false)]
        public void SubmitChanges(bool abort)
        {
            if (abort)
            {
                //_shiftDispatcherModel.Abort(); 此处会造成旧session上的对象全部消失,请不要随意使用abort
            }
            else
            {
                //_changedAgents.ForEach<IAgent>(o => o.Schedule.Operator = string.Format("{0}@{1:yyyy/MM/dd HH:mm:ss.fff}", Luna.Core.ApplicationCache.Get<string>(Keys.AgentId), DateTime.Now));
                _shiftDispatcherModel.UpdateSchedule(true);
                _seatDispatcherPresenter.SaftyInvoke<Core.Reflector>(p =>
                {
                    p.Proc("ClearClipboard")();
                });
            }
            //above statements might throw exception

            ReloadAgents(_changedAgents, null);//also might include rtaa reload
            _changedAgents.Clear();
        }

        public void AddSubEvent(DateTime start, TermStyle style, int rowIndex)
        {
            BindableAgents[rowIndex].SaftyInvoke<IAgent>(o =>
                                                             {
                                                                 var applied = false;
                                                                 var timeBox = o.Schedule;
                                                                 timeBox.Create(Term.New(start, style, style.CustomLength), (t, success) =>
                                                                    {
                                                                        if (success)
                                                                        {
                                                                            //((Term)t).Tag = Comments;
                                                                            applied = true;
                                                                        }

                                                                        o.OperationFail = !success;

                                                                    }, false);
                                                                 AddingTermPresenterCallback(new[] { o }, applied);
                                                             });
        }

        public bool CanLaunchRtaaPrefix()
        {
            return EnableRtaa;
        }


        private bool _rtaaPrefixEnabled;
        public bool RtaaPrefixEnabled
        {
            get { return _rtaaPrefixEnabled; }
            set { _rtaaPrefixEnabled = value; NotifyOfPropertyChange(() => RtaaPrefixEnabled); }
        }

        [Preview("CanLaunchRtaaPrefix")]
        public void LaunchRtaaPrefix()
        {
            //var current = FirstOrDefault<IRTAAPrefixPresenter>(o => o != null);
            //if (current != null)
            //{
            //    Activate(current);
            //    return;
            //}
            //Show<IRTAAPrefixPresenter>(new Dictionary<string, object> {
            //                        {"Reload", new Action<Func<DateTime, DateTime, IDictionary<Guid, IEnumerable>>>(f=>
            //                        {
            //                            if(AgentAdherenceEvents == null)
            //                                _agentAdherenceEvents = AgentAdherences.Select(o => new List<AdherenceEvent>(10) as IEnumerable).ToList();

            //                            var counter = 0;
            //                            foreach ( var item in  f(MonitoringPoint.AddHours(-12), MonitoringPoint) )
            //                            {
            //                                _agentAdherenceEvents[_agentAdherencesMap != null ? _agentAdherencesMap[item.Key] : counter] = item.Value;
            //                                counter++;
            //                            }                                                  
                                                                                                                
            //                            this.QuietlyReload(ref _agentAdherenceEvents, "AgentAdherenceEvents");
            //                        }) },
            //                        {"Invoker" , this},
            //                        {"WhenClosed",  new Action<IRTAAPrefixPresenter,Exception>((p, ex)=>
            //                        {
            //                            _addingAdhEventDelegate = null;
            //                            _editingAdhEventDelegate = null;
            //                            _removingAdhEventDelegate = null;
            //                            AgentAdherenceEvents = AgentAdherences == null ? new List<IEnumerable>(0): AgentAdherences.Select(o => new List<AdherenceEvent>() as IEnumerable).ToList();
            //                            RtaaPrefixEnabled = false;
            //                        }) }
            //                        }, p =>
            //                        {
            //                            _addingAdhEventDelegate = p.WhenAdding;
            //                            _editingAdhEventDelegate = p.WhenChanged;
            //                            _removingAdhEventDelegate = p.WhenRemoving;
            //                            RtaaPrefixEnabled = true;
            //                        });
        }



        public void Erase(IVisibleTerm target, int rowIndex)
        {
            if (_addingAdhEventDelegate == null || target == null) return;

            if (rowIndex >= AgentAdherenceEvents.Count)
                return;

            if (AgentAdherenceEvents[rowIndex] == null)
                AgentAdherenceEvents[rowIndex] = new List<AdherenceEvent>();

            AgentAdherenceEvents[rowIndex].SaftyInvoke<List<AdherenceEvent>>(list =>
            {

                if (target.End == MonitoringPoint || list.Any(o => o.IsCoverd(target)))
                    return;

                var e = _addingAdhEventDelegate(target);
                e.EmployeeId = BindableAgents[rowIndex].SaftyGetProperty<Guid, IAgent>(o => o.Profile.Id);
                list.Add(e);
                this.QuietlyReload(ref _agentAdherenceEvents, "AgentAdherenceEvents");
            });
        }

        public void DeleteAdhEvent(AdherenceEvent target)
        {
            if (AgentAdherenceEvents[CurrentIndex] == null) return;

            AgentAdherenceEvents[CurrentIndex].SaftyInvoke<List<AdherenceEvent>>(list =>
            {
                list.Remove(target);
                _removingAdhEventDelegate(target);
                this.QuietlyReload(ref _agentAdherenceEvents, "AgentAdherenceEvents");
            });
        }


        public void FilterWithOnDuty()
        {
            var watchPoint = GetWatchPoint().Date;
            var results = _attendances.Where(o => o.Schedule.RetriveTerms(watchPoint, watchPoint).Any(t => t.Start.Date == watchPoint.Date || t.End == watchPoint.Date));
            Agents = results; // castedAgents;
            Sort();
        }

        public void ViewWith3D()
        {
            View3D = !View3D;
        }


        public bool Ask(RoutedEventArgs e, IQuestionPresenter p)
        {
            var source = new Luna.Core.Reflector(e.OriginalSource);
            var newPlacement = source.Property<ITerm>("DropedPlacement");
            var agent = (IAgent)BindableAgents[source.Property<int>("PointOutDataRowIndex")];
            var pointOutBlock = source.Property<ITerm>("PointOutBlock");

            var newStart = newPlacement.Start;
            var newEnd = newPlacement.End;

            if (pointOutBlock.Start == newStart && pointOutBlock.End == newEnd)
                return false;

            p.DisplayName = LanguageReader.GetValue("Shifts_ShiftDispatcher_AskEditShift");
            p.Editable = true;

            pointOutBlock.SaftyInvoke<Term>(x =>
                                                {
                                                    p.Text = string.Format(LanguageReader.GetValue("Shifts_ShiftDispatcher_AskEditShiftText"),
                                                    agent.Profile.Name, x.Start, x.Text);
                                                    p.Comments = x.Tag;
                                                });
            return true;
        }
        private string _alterComments;

        public bool Callback(RoutedEventArgs e, IQuestionPresenter p)
        {
            _alterComments = p.Answer == Answer.Yes ? p.Comments : string.Empty;
            return p.Answer == Answer.Yes;
        }

        public void SetNewTimeFail(Exception exception)
        {
        }

        [SuperRescue("SetNewTimeFail", "Shifts_ShiftDispatcher_SubmitChangesFail", "Shifts_AlterFail_Title")]
        [Question("Ask", "Callback")]
        public void SetNewTime(RoutedEventArgs e)
        {
            var source = new Luna.Core.Reflector(e.OriginalSource);
            var newPlacement = source.Property<ITerm>("DropedPlacement");
            var agent = BindableAgents[source.Property<int>("PointOutDataRowIndex")];
            var pointOutBlock = source.Property<ITerm>("PointOutBlock");

            var newStart = newPlacement.Start;
            var newEnd = newPlacement.End;

            if ((pointOutBlock.Start == newStart && pointOutBlock.End == newEnd))
                return;

            pointOutBlock.SaftyInvoke<Term>(x =>
                                {
                                    if (!x.Locked)
                                        agent.SaftyInvoke<IAgent>(o=>
                                            o.Schedule.SetTime(x, newStart, newEnd,
                                            (t, success) =>
                                            {
                                                if (success)
                                                {
                                                    x.Tag = _alterComments;
                                                    IsDirty = true;
                                                }

                                                o.OperationFail = !success;
                                            }, false)
                                        );
                                });
        }
    }
}
