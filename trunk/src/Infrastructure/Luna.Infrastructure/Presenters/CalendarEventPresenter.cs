using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using ActiproSoftware.Windows.Controls.Calendar;
using Caliburn.Core.Metadata;
using Caliburn.PresentationFramework.Filters;
using Luna.Common.Constants;
using Luna.Common.Interfaces;
using Luna.Core.Extensions;
using Luna.Infrastructure.Domain;
using Luna.Infrastructure.Domain.Model;
using Luna.Infrastructure.Presenters.Interfaces;
using Luna.WPF.ApplicationFramework;
using Luna.WPF.ApplicationFramework.Attributes;
using Luna.WPF.ApplicationFramework.Interfaces;

namespace Luna.Infrastructure.Presenters
{
    [PerRequest(FunctionKeys.CalendarEvent, typeof(ICalendarEventPresenter))]
    public class CalendarEventPresenter : DefaultPresenter, ICalendarEventPresenter, IIndexer
    {
        private readonly ICalendarEventModel _model;
        private DateTime _fromDate;
        private DateTime _toDate;
        private List<Country> _countries;
        private TimeZoneInfo _selectedTimeZone;
        private DateTime _activeTime;
        private Dictionary<DateTime, DayItemModel> _dayItemsSource;
        private IList<CalendarEvent> _allEvents;

        public CalendarEventPresenter(ICalendarEventModel model)
        {
            _model = model;
            _dayItemsSource = new Dictionary<DateTime, DayItemModel>(10 * 12);
            _activeTime = DateTime.Today;
        }

        protected override void OnInitialize()
        {
            var country = Countries.CurrentItem.As<Country>();

            //var timezoneIds = Application.Current.Resources[country] as string[];
            //if (timezoneIds != null)
            //SelectedTimeZone = (from string timezoneId in timezoneIds select TimeZoneInfo.FindSystemTimeZoneById(timezoneId)).FirstOrDefault();
            SelectedTimeZone = country.TimeZones.FirstOrDefault();

            base.OnInitialize();
        }

        public void GetAllEventsOfCurrentYear()
        {
            // 查询时，每次取一年数据，如果切换下一年再另取，否则不取
            var yearOfCalendar = DateTimeHelper.GetFirstDateTimeInMonth(_activeTime).Year;
            _fromDate = new DateTime(yearOfCalendar - 1, 12, 1);
            _toDate = new DateTime(yearOfCalendar + 1, 1, 31);

            SetDayItemsSource(_model.GetCalendarEvents(_fromDate, _toDate, Countries.CurrentItem.ToString(), SelectedTimeZone));

            AllEventsView = CollectionViewSource.GetDefaultView(_allEvents)
                .Self(v =>
                {
                    v.Filter = o => o.SaftyGetProperty<bool, CalendarEvent>(c => !c.IsWeekendHoliday);
                    v.CurrentChanged += delegate
                                            {
                                                var currentEvent = v.CurrentItem.As<CalendarEvent>();
                                                if (currentEvent == null || currentEvent.Start == _activeTime) return;

                                                // 移动MonthCalendar, don't use set property

                                                _activeTime = currentEvent.Start;
                                                NotifyOfPropertyChange(() => ActiveTime);
                                            };
                    v.SortDescriptions.Add(new SortDescription("Start", ListSortDirection.Ascending)); // add sorting
                    v.Refresh();
                });
        }

        private void SummarizeActiveMonth()
        {
            _holidays = 0;
            _workingDays = 0;

            var from = _activeTime.AddDays(1 - _activeTime.Day);
            var lastDayOfMonth = from.AddMonths(1).AddDays(-1);
            while (from <= lastDayOfMonth)
            {
                if (((DayItemModel)GetItem(from, typeof(DateTime))).IsHoliday)
                    _holidays += 1;
                else
                    _workingDays += 1;
                from = from.AddDays(1);
            }
            NotifyOfPropertyChange(() => Holidays);
            NotifyOfPropertyChange(() => WorkingDays);
        }

        private void SyncEventListCurrentItemWithActiveDate()
        {
            if (_allEvents == null || _allEvents.Count == 0) return;

            CollectionViewSource.GetDefaultView(_allEvents).MoveCurrentTo(_allEvents.FirstOrDefault(x => x.Start <= _activeTime && _activeTime < x.End));
        }

        public ICollectionView Countries
        {
            get
            {
                if (_countries == null)
                {
                    Application.Current.Resources["CountryList"].SaftyInvoke<string[]>(array =>
                    {
                        _countries = new List<Country>(array.Length);
                        foreach (var v in array)
                        {
                            var cultureInfo = CultureInfo.GetCultureInfo(v);
                            _countries.Add(new Country(cultureInfo, Application.Current.Resources.Contains(v) ? Application.Current.Resources[v] as string[] : new string[0]));

                        }
                    });
                }
                return CollectionViewSource.GetDefaultView(_countries).Self(v =>
                {
                    // 迫使选中Country的TimeZone默认选中第一项
                    v.CurrentChanged += (sender, e) => v.CurrentItem.SaftyInvoke<Country>(c => SelectedTimeZone = c.TimeZones.FirstOrDefault());

                    if (v.CurrentItem == null)
                        v.MoveCurrentToFirst();
                });
            }
        }

        private CalendarEvent _newCalendarEvent;
        public CalendarEvent NewCalendarEvent
        {
            get { return _newCalendarEvent; }
            set { _newCalendarEvent = value; NotifyOfPropertyChange(() => NewCalendarEvent); }
        }

        private DayOfWeek _firstDayOfWeek;
        public DayOfWeek FirstDayOfWeek
        {
            get { return _firstDayOfWeek; }
            set
            {
                if (value.Equals(null))
                {
                    value = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
                }
                _firstDayOfWeek = value;
            }
        }

        private ICollectionView _allEventsView;
        public ICollectionView AllEventsView
        {
            get { return _allEventsView; }
            set { _allEventsView = value; NotifyOfPropertyChange(() => AllEventsView); }
        }

        private void SetDayItemsSource(IList<CalendarEvent> value)
        {
            _allEvents = value;
            // convert to dayItemsSource
            _dayItemsSource = new Dictionary<DateTime, DayItemModel>();
            foreach (var calendarEvent in _allEvents)
            {
                var start = calendarEvent.Start.Date;
                while (start < calendarEvent.End.Date)
                {
                    if (!_dayItemsSource.ContainsKey(start))
                        _dayItemsSource[start] = new DayItemModel(start);

                    _dayItemsSource[start].Add(calendarEvent);

                    start = start.AddDays(1);
                }
            }
            _workingDays -= Holidays;

            NotifyOfPropertyChange(() => DayItemsSource);
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

        public Dictionary<DateTime, DayItemModel> DayItemsSource
        {
            get { return _dayItemsSource; }
        }

        //可引发重新查询CalendarEvent的2个属性
        public TimeZoneInfo SelectedTimeZone
        {
            get { return _selectedTimeZone; }
            set
            {
                if (value == null || value.Equals(_selectedTimeZone)) return;

                _selectedTimeZone = value;
                GetAllEventsOfCurrentYear();//重置为Null,方可根据Country，TimeZone从数据库中捞出整年数据
                SyncEventListCurrentItemWithActiveDate();
                SummarizeActiveMonth();
                NotifyOfPropertyChange(() => SelectedTimeZone);
            }
        }

        public DateTime ActiveTime
        {
            get { return _activeTime; }
            set
            {
                var monthChanged = value.Month != _activeTime.Month;

                // 由selectedDate给值
                _activeTime = value;
                if (IsYearChanged())
                    GetAllEventsOfCurrentYear();
                SyncEventListCurrentItemWithActiveDate();
                if (monthChanged)
                    SummarizeActiveMonth();
                NotifyOfPropertyChange(() => ActiveTime);
            }
        }
        // end

        private DateTime _displayMaxTime;
        public DateTime DisplayMaxTime
        {
            get { return _displayMaxTime; }
        }

        private bool IsYearChanged()
        {
            var yearOfCalendar = DateTimeHelper.GetFirstDateTimeInMonth(_activeTime).Year;
            return !(_fromDate.Year < yearOfCalendar && yearOfCalendar < _toDate.Year && _allEvents != null) && _activeTime != default(DateTime);
        }

        public object GetItem(object index, Type indexType)
        {
            if (indexType == typeof(DateTime))
            {
                var dateKey = Convert.ToDateTime(index);
                if (DayItemsSource != null)
                {
                    if (!DayItemsSource.ContainsKey(dateKey))
                        DayItemsSource[dateKey] = new DayItemModel(dateKey);

                    return DayItemsSource[dateKey];
                }
            }
            return null;
        }

        public bool CanOpenAddDstDialog()
        {
            var list = SelectedDayItemEventList;

            var found = list != null && list.Any(item => item.Is<DaylightSavingTime>());
            return !found;
        }

        private void BeforeDialog(IDialogBoxPresenter dialog)
        {
            dialog.DisplayName = "";
            dialog.DialogPosition = VerticalAlignment.Center;

            NewCalendarEvent.Start = Convert.ToDateTime(ActiveTime);
            NewCalendarEvent.End = NewCalendarEvent.Start.AddDays(1);
            _displayMaxTime = NewCalendarEvent.End.AddDays(10);
        }

        [InputText(PartView = "Infrastructure.NewCalendarEventDialog", ConfirmDelegate = "[Event Unchecked] = [Action CreateNew($dataContext)]")]
        public IDialogBoxPresenter OpenAddHolidayDialog(IDialogBoxPresenter dialog)
        {
            NewCalendarEvent = new Holiday() { Country = Countries.CurrentItem.ToString() };
            BeforeDialog(dialog);
            return dialog;
        }

        public void ApplyWeekendAsHoliday()
        {
            var newHolidays = _model.SaveBatchOfWeekends(ActiveTime, Countries.CurrentItem.ToString());
            AddToGroup(newHolidays);
        }

        [Dependencies("ActiveTime")]
        [Preview("CanOpenAddDstDialog")]
        [InputText(PartView = "Infrastructure.NewCalendarEventDialog", ConfirmDelegate = "[Event Unchecked] = [Action CreateNew($dataContext)]")]
        public IDialogBoxPresenter OpenAddDstDialog(IDialogBoxPresenter dialog)
        {
            NewCalendarEvent = new DaylightSavingTime() { TimeZone = SelectedTimeZone };
            BeforeDialog(dialog);
            dialog.Text = "DST";
            return dialog;
        }

        public void CreateNew(IDialogBoxPresenter result)
        {
            if (result.IsCancel == true) return;
            NewCalendarEvent.EventName = result.Text;

            var exception = _model.Save(NewCalendarEvent);

            result.ConfrimCallback(exception);

            if (exception == null)
            {
                AddToGroup(new[] { NewCalendarEvent });
                NotifyOfPropertyChange(() => ActiveTime);
            }
        }

        public bool CanOpenDeleteDialog()
        {
            var list = SelectedDayItemEventList;
            return list != null && 0 < list.Count();
        }

        public IList<CalendarEvent> SelectedDayItemEventList
        {
            get { return !_dayItemsSource.ContainsKey(_activeTime) ? null : _dayItemsSource[_activeTime]; }
        }

        [Dependencies("ActiveTime")]
        [Preview("CanOpenDeleteDialog")]
        [InputText(PartView = "Infrastructure.DeleteCalendarEventDialog", ConfirmDelegate = "[Event Unchecked] = [Action Delete($dataContext)]")]
        public IDialogBoxPresenter OpenDeleteDialog(IDialogBoxPresenter dialog)
        {
            dialog.BackgroundTitle = "Delete";
            dialog.DialogPosition = VerticalAlignment.Center;
            dialog.Message = "Select a event to delete.";
            dialog.HasInputField = false;
            dialog.DisplayName = "";
            return dialog;
        }

        public void Delete(IDialogBoxPresenter result)
        {
            if (result.IsCancel == true) return;

            CollectionViewSource.GetDefaultView(SelectedDayItemEventList).CurrentItem.SaftyInvoke<CalendarEvent>(x =>
            {
                _model.Delete(x);
                RemoveFromGroup(x);
                result.Close();
            });
        }

        public void OpenImportView()
        {
            OpenDialog<ICalendarEventImportPresenter>(new Dictionary<string, object> { { "country", Countries.CurrentItem } });
        }

        private void RemoveFromGroup(CalendarEvent calendarEvent)
        {
            var start = calendarEvent.Start.Date;
            while (start < calendarEvent.End.Date)
            {
                if (!_dayItemsSource.ContainsKey(start))
                    break;

                _dayItemsSource[start].Remove(calendarEvent);
                _dayItemsSource[start].Refresh();
                start = start.AddDays(1);
            }
            _allEvents.Remove(calendarEvent);


            // 当Refresh时总是会默认选中第一项（执行CurrentChanged）
            var temp = _activeTime;
            AllEventsView.Refresh();

            if (temp.Month == _activeTime.Month && calendarEvent.Is<Holiday>())
                SummarizeActiveMonth();

            ActiveTime = temp;

        }

        private void AddToGroup(IEnumerable<CalendarEvent> calendarEvents)
        {
            var anyHolidayAdd = false;
            CalendarEvent lastAddedCalendarEvent = null;

            foreach (var calendarEvent in calendarEvents)
            {
                var start = calendarEvent.Start.Date;
                while (start < calendarEvent.End.Date)
                {
                    if (!_dayItemsSource.ContainsKey(start))
                        _dayItemsSource[start] = new DayItemModel(start);

                    if (calendarEvent.Is<DaylightSavingTime>())
                        _dayItemsSource[start].Insert(0, calendarEvent);
                    else
                        _dayItemsSource[start].Add(calendarEvent);

                    _dayItemsSource[start].Refresh();
                    //CollectionViewSource.GetDefaultView(_dayItemsSource[start])
                    //    .Self(v =>
                    //    {   //可防止删除calendarEvent的时候未选中项
                    //        v.MoveCurrentToFirst();
                    //    });
                    start = start.AddDays(1);
                }
                _allEvents.Add(calendarEvent);
                if (calendarEvent.Is<Holiday>())
                    anyHolidayAdd = true;

                lastAddedCalendarEvent = calendarEvent;
            }

            AllEventsView.Refresh();
            if (lastAddedCalendarEvent != null)
                AllEventsView.MoveCurrentTo(lastAddedCalendarEvent);

            if (anyHolidayAdd)
                SummarizeActiveMonth();
        }
    }
}
