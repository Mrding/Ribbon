using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using ActiproSoftware.Windows.Controls.Ribbon.Input;
using Caliburn.Core.Metadata;
using Caliburn.PresentationFramework.Actions;
using Luna.Common;
using Luna.Common.Constants;
using Luna.Common.Domain;
using Luna.Common.Extensions;
using Luna.Common.Interfaces;
using Luna.Core;
using Luna.Core.Extensions;
using Luna.Globalization;
using Luna.Infrastructure.Domain;
using Luna.Infrastructure.Domain.Model;
using Luna.Shifts.Domain;
using Luna.Shifts.Domain.Model;
using Luna.Shifts.Presenters.Interfaces;
using Luna.WPF.ApplicationFramework;
using Luna.WPF.ApplicationFramework.Attributes;
using Luna.WPF.ApplicationFramework.Commands;
using Luna.WPF.ApplicationFramework.Controls;
using Luna.WPF.ApplicationFramework.Interfaces;
using ApplicationCommands = System.Windows.Input.ApplicationCommands;

namespace Luna.Shifts.Presenters
{
    [PerRequest(typeof(IShiftComposerPresenter))]
    public partial class ShiftComposerPresenter : DockingPresenter, ICanSupportAgentFinder, IShiftComposerPresenter, ISubmitablePresenter
    {
        private readonly IShiftDispatcherModel _shiftDispatcherModel;
        private readonly ILaborHoursCountingModel _laborHoursCountingModel;
        private readonly ICalendarEventModel _calendarEventModel;
        private IList<IAgent> _attendances;
        private IList _bindableAgents;
        private bool _watchPointIsInitialized;
        private Dictionary<DateTime, int> _dateIndexer;
        //private Thread _onInitializeThread;
        private bool _isDirty;
        private bool _cellChanged;
        private DateTime[] _alterDateRange;
        private ITerm _enquiryRange;
        private readonly AutoResetEvent _autoReset = new AutoResetEvent(false);//异步调用，等待RegisterRunStaffingChart完成后再执行OnInitialize中Thread内容

        public ShiftComposerPresenter(IShiftDispatcherModel shiftDispatcherModel, ILaborHoursCountingModel laborHoursCountingModel, ICalendarEventModel calendarEventModel)
        {
            _shiftDispatcherModel = shiftDispatcherModel;
            _laborHoursCountingModel = laborHoursCountingModel;
            _calendarEventModel = calendarEventModel;

            //x_changedTerms = new List<Tuple<int, DateTerm>>();
            //x_changedCells = new List<Tuple<int, DateTerm>>();

            //x_assignmentInsertRuleIntersectionPoints = new Dictionary<AssignmentType, IList<Luna.Core.Tuple<int, int>>>();
            //x_assignmentContributionPositions = new Dictionary<AssignmentType, bool[]>();

            //InitializeCommandBindings
            var shiftPainterCommandBinding = new CommandBinding(Luna.WPF.ApplicationFramework.ApplicationCommands.ShiftPainter, CopyShiftForPainting, (sender, e) =>
            {
                e.CanExecute = InitalDrawBlock != null && CellMode && SelectionTimeRange != null;
                e.Parameter.SaftyInvoke<ICheckableCommandParameter>(p =>
                {
                    p.Handled = true;
                    p.IsChecked = PaintShiftIsEnabled;
                });
            });
            var saveCommandBinding = new AsyncCommandBinding(ApplicationCommands.Save,
                                                             (sender, e) => SubmitChanges(false),
                                                             (sender, e) => e.CanExecute = CellChanged,
                                                             (sender, e) =>
                                                             {
                                                                 if (e.Error == null) return;
                                                                 CellChanged = true;
                                                                 throw e.Error;
                                                             });

            var undoCommandBinding = new CommandBinding(ApplicationCommands.Undo, (sender, e) => SubmitChanges(true), (sender, e) => e.CanExecute = CellChanged);
            CommandBindings.Add(new CommandBinding(WPF.ApplicationFramework.ApplicationCommands.SetDayOff,
                                                   (sender, e) =>
                                                       {
                                                           SetAsDayOff();
                                                       }));
            CommandBindings.Add(shiftPainterCommandBinding);
            CommandBindings.Add(saveCommandBinding);
            CommandBindings.Add(undoCommandBinding);
            CommandBindings.Add(new CommandBinding(WPF.ApplicationFramework.ApplicationCommands.EstimateShift, EstimateShift, AnalyisComplete));
            CommandBindings.Add(new CommandBinding(WPF.ApplicationFramework.ApplicationCommands.ShowEstimateShift, SendParameter, AnalyisComplete));
            CommandBindings.Add(new CommandBinding(WPF.ApplicationFramework.ApplicationCommands.OpenStaffingStatistic, SendParameter));
            CommandBindings.Add(new CommandBinding(WPF.ApplicationFramework.ApplicationCommands.OpenCompositiveServiceQueue, SendParameter, AnalyisComplete));
            CommandBindings.Add(new CommandBinding(WPF.ApplicationFramework.ApplicationCommands.SortByTermStart, delegate { Sort(); }));
            CommandBindings.Add(new CommandBinding(WPF.ApplicationFramework.ApplicationCommands.SwitchView, SwitchView));

            ResetAlterDateRange();
        }

        protected override void OnInitialize()
        {
            BeginLoading();

            AssignmentCellConverter = new BlockToCellConverter().ShowDayOffText(false);
            EvaluationStatisticConverter = new BlockToCellConverter();

            DateTerm[] dates;
            DailyCounter[] planningStaffings;
            DailyCounter[] planningOtStaffings;
            DailyCounter<DateTime>[] planningShifts;
            DailyCounter[] planningServiceLevel;

            _dateIndexer = Schedule.CreateDateIndexer();

            _selectedColumnDate = Schedule.Start;

            _calendarEventModel.LoadGlobalCalendar(Schedule.Start, Schedule.End);

            Schedule.CreateDateIndexer(_dateIndexer, date =>
            {
                var isHoliday = date.IsHoliday(Country.Local);

                if (isHoliday)
                    _holidays++;
                else
                    _workingDays++;

                return new DateTerm(date, _dateIndexer[date] == 0 ? "{0:M/d}" : "{0:d }") { IsHoliday = isHoliday, IsDaylightSaving = date.IsDaylightSaving(TimeZoneInfo.Local) };
            }, out dates);
            _dates = new List<IEnumerable> { dates };

            new Thread(() =>
            {

                // 获取Agent数据, 班次统计数据
                _attendances = _shiftDispatcherModel.GetPlanningAgents(_schedule.Campaign, _enquiryRange);
                _assignmentTypes = _shiftDispatcherModel.GetAssignmentTypeDailyCounter(null, _schedule);
                //x_prioritiedAssignmentTypeGroup = new Dictionary<int, IList<string>>(10);



                Schedule.CreateDateIndexer(_dateIndexer, date => new DailyCounter(date, "0.#"), out planningStaffings);
                Schedule.CreateDateIndexer(_dateIndexer, date => new DailyCounter(date, "0.#"), out planningOtStaffings);
                Schedule.CreateDateIndexer(_dateIndexer, date => new DailyCounter(date, "0.#%"), out planningServiceLevel);
                Schedule.CreateDateIndexer(_dateIndexer, date => new DailyCounter<DateTime>(date, date, 0), out planningShifts);

                //初始统计数据
                _statisticItems = new List<Tuple<string, IList>>(new[] { new Tuple<string, IList>("Staffings", planningStaffings),
                                                                    new Tuple<string, IList>("OtStaffings", planningOtStaffings),
                                                                    new Tuple<string, IList>("AssignedServiceLevel", planningServiceLevel),
                                                                    new Tuple<string, IList>("AgentCounts", planningShifts)});

                _currentDateAssignmentTypes = new Dictionary<DateTime, IList>(dates.Length);

                foreach (var i in dates)
                {
                    var dateKey = i.Date;
                    var indexOfDate = i.Index;
                    _dateIndexer[dateKey] = indexOfDate;

                    RecalculateDailyStatistic(indexOfDate, false);
                    DailySetAssignmentTypeEval(i); //caution: without assignment type frequency estimate
                }

                Agents = _attendances;

                SummarizeLaborHours();
                EndLoading();

                NotifyOfPropertyChange(() => AssignmentTypes);
                NotifyOfPropertyChange(() => StatisticItems);
                _autoReset.WaitOne();
                _staffingCalculatorService.Run(_staffingCalculatorArgs); //do not change order



                Thread.CurrentThread.Abort();
            }).Self(t => { t.IsBackground = true; t.Start(); });

            base.OnInitialize();
        }



        public string Title
        {
            get { return string.Format("{0} - {1}", LanguageReader.GetValue("Shifts_ShiftComposer_Title"), _schedule.Name); }
        }

        private IList _dates;
        public IList Dates
        {
            get { return _dates; }
            set { _dates = value; }
        }

        private void SwitchView(object sender, ExecutedRoutedEventArgs e)
        {
            CellMode = System.Convert.ToBoolean(e.Parameter);
        }

        private DateTime _hourViewScreenStart;
        private DateTime _monthViewScreenStart;

        private bool _cellMode = true;
        public bool CellMode
        {
            get { return _cellMode; }
            set
            {
                if (_cellMode)
                    _monthViewScreenStart = ScreenStart;
                else
                    _hourViewScreenStart = ScreenStart;


                _cellMode = value;

                if (_cellMode)
                    AssignmentCellConverter = new BlockToCellConverter().ShowDayOffText(false);
                else
                    AssignmentCellConverter = new DefalutBlockConverter();

                NotifyOfPropertyChange(() => CellMode);

                ScreenStart = _cellMode ? _monthViewScreenStart : _hourViewScreenStart;

                NotifyOfPropertyChange(() => BindableAgents);
            }
        }

        private IDictionary<string, HeaderContainer<AssignmentType, DailyCounter<AssignmentType>, DateTime>> _assignmentTypes;
        private IList _bindableAssignmentTypes;
        public IList AssignmentTypes
        {
            get
            {
                if (_assignmentTypes == null)
                    return new List<IEnumerable>();
                if (_bindableAssignmentTypes == null && 0 < _assignmentTypes.Count)
                {
                    _bindableAssignmentTypes = new List<IEnumerable>(_assignmentTypes.Count);

                    //初始化AssignmentTypes
                    foreach (var item in _assignmentTypes)
                    {
                        //UI绑定对象转为IEnumerable
                        _bindableAssignmentTypes.Add(item.Value.As<IEnumerable>());
                    }
                }
                return _bindableAssignmentTypes;
            }
        }

        private Schedule _schedule;
        public ITerm Schedule
        {
            get { return _schedule; }
            set
            {
                if (base.IsInitialized) return;
                _schedule = (Schedule)value;

                var today = DateTime.Today;

                if (!_watchPointIsInitialized && today.IsInTheRange(_schedule))
                    SetWatchPoint(today);
                else
                    SetWatchPoint(_schedule.Start);

                _watchPointIsInitialized = true;
                _selectedColumnDate = _schedule.Start; //set default value
                _monthViewScreenStart = _schedule.Start;
                _hourViewScreenStart = _schedule.Start;
                _screenStart = _schedule.Start;
                _selectedServiceQueue = _schedule.ServiceQueues.First().Key.Name;

                _enquiryRange = new DateRange(_schedule.Start.AddDays(Global.HeadDayAmount), _schedule.End.AddDays(Global.TailDayAmount));
            }
        }

        private DateTime _selectedColumnDate;
        public DateTime SelectedColumnDate
        {
            get
            {
                return _selectedColumnIndex < 0 ? _schedule.Start : (Dates[0] as DateTerm[])[_selectedColumnIndex].Date;
            }
            set
            {
                if (_selectedColumnDate == value) return;

                if (value != DateTime.MinValue)
                    _selectedColumnDate = value;
                NotifyOfPropertyChange(() => SelectedColumnDate);
                NotifyOfPropertyChange(() => SelectedColumnIndex);
            }
        }

        private int _selectedColumnIndex;
        public int SelectedColumnIndex
        {
            get { return _dateIndexer == null ? -1 : _dateIndexer[_selectedColumnDate.Date]; }
            set
            {
                if (_selectedColumnIndex == value) return;
                _selectedColumnIndex = value;
                NotifyOfPropertyChange(() => SelectedColumnIndex);

                //_selectedColumnDate = _selectedColumnIndex < 0 ? _schedule.Start : (Dates[0] as DateTerm[])[SelectedColumnIndex].Date;
                NotifyOfPropertyChange(() => SelectedColumnDate);
            }
        }

        private DateTime _screenStart;
        public DateTime ScreenStart
        {
            get { return _screenStart; }
            set
            {
                if (value != DateTime.MinValue)
                    _screenStart = value;

                NotifyOfPropertyChange(() => ScreenStart);
            }
        }

        private DateTime _screenEnd;
        public DateTime ScreenEnd
        {
            get { return _screenEnd; }
            set
            {
                _screenEnd = value;
                NotifyOfPropertyChange(() => ScreenEnd);
            }
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                _isDirty = value;
                NotifyOfPropertyChange(() => IsDirty);
            }
        }

        public bool CellChanged
        {
            get { return _cellChanged; }
            set { _cellChanged = value; NotifyOfPropertyChange(() => CellChanged); }
        }

        private int _workingDays;
        public int WorkingDays
        {
            get { return _workingDays; }
        }

        private int _holidays;
        public int Holidays
        {
            get { return _holidays; }
        }

        private IBlockConverter _assignmentCellConverter;

        public IBlockConverter AssignmentCellConverter
        {
            get { return _assignmentCellConverter; }
            private set { _assignmentCellConverter = value; NotifyOfPropertyChange(() => AssignmentCellConverter); }
        }

        private IBlockConverter _evaluationStatisticConverter;

        public IBlockConverter EvaluationStatisticConverter
        {
            get { return _evaluationStatisticConverter; }
            private set { _evaluationStatisticConverter = value; NotifyOfPropertyChange(() => EvaluationStatisticConverter); }
        }

        public void Refresh(IEnumerable list) { }

        protected override bool CanShutdownCore()
        {
            if (!CellChanged) return true;

            return ShutdownConfirm(q =>
            {
                q.ClosingConfirmModeOn = true;
                q.Text = string.Format("Do you want to save changes you made to {0}", _schedule);
                q.DisplayName = "Shift Composer";
                q.ConfirmDelegate = "[Event Closing] = [Action CompleteConfirm($dataContext)]";

            });
        }

        [AsyncAction(BlockInteraction = true)]
        [SuperRescue("SubmitChangesFail", "Shifts_ShiftDispatcher_SubmitChangesFail", "Save failed", false)]
        public void CompleteConfirm(IQuestionPresenter q)
        {
            if (q.Answer == Answer.Cancel) return;

            SubmitChanges(q.Answer != Answer.Yes);
        }

        public virtual void SubmitChanges(bool abort)
        {
            //x_changedTerms.Clear();
            //x_changedCells.Clear();
            CellChanged = false;
            if (abort) // reload
            {
                ReloadChangedAgents(); // 具有顺序性, 请勿变更
                ReloadChangedShiftEstimations(); // 具有顺序性, 请勿变更
                NotifyOfPropertyChange(() => AssignmentTypes);
                NotifyOfPropertyChange(() => StatisticItems); // don't  remove 不管班表或班次统计是否重取数据都需要刷新统计
            }
            else
            {
                System.Diagnostics.Debug.Print("SaveSchedule");
                _shiftDispatcherModel.UpdateSchedule(true);

                _isDirty = false; // 无需响应NotifyOfPropertyChange,亦无需让StaffingChartView更新
                NotifyOfPropertyChange(() => BindableAgents);
                NotifyOfPropertyChange(() => AssignmentTypes);
            }

        }

        public virtual void SubmitChangesFail(Exception ex)
        {
            CellChanged = true;
        }

        protected override void OnShutdown()
        {
            _staffingCalculatorService.SaftyInvoke(o => o.Shutdown());
            _staffingCalculatorService = null;

            //xforeach (IDisposable item in _assignmentTypes.Values)
            //x    item.Dispose();
            //x_assignmentTypes.Clear();
            //x_assignmentTypes = null;

            //x_schedule = null;
            //x_bindableAgents.Clear();
            //x_attendances.Clear();
            //x_shiftDispatcherModel.Release();

            //x_autoReset.Close();
            //x_staffingCalculatorArgs.Clear();


            //xforeach (var item in _statistics2)
            //x{
            //x    item.SaftyInvoke<DailyCounter<DateTime>[]>(list =>
            //x                                                   {
            //x                                                       foreach (IDisposable dailyCounter in list)
            //x                                                           dailyCounter.Dispose();
            //x                                                   });
            //x}
            //x_statistics2.Clear();
            //x_statistics2 = null;

            base.OnShutdown();
        }
    }
}

