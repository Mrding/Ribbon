using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Luna.Common;
using Luna.Common.Constants;
using Luna.Core.Extensions;
using Luna.Infrastructure.Data.Repositories;
using Luna.Infrastructure.Domain.Model;
using Luna.Shifts.Domain;
using uNhAddIns.Adapters;

namespace Luna.Infrastructure.Domain.Impl
{

    [PersistenceConversational(MethodsIncludeMode = MethodsIncludeMode.Implicit)]
    public class CalendarEventModel : ICalendarEventModel
    {
        private readonly ICalendarEventRepository _calendarEventRepository;

        public CalendarEventModel(ICalendarEventRepository calendarEventRepository)
        {
            _calendarEventRepository = calendarEventRepository;
        }
     
        public IList<CalendarEvent> GetAll()
        {
            return _calendarEventRepository.GetAll();
        }

        public IList<CalendarEvent> GetByCountry(Country country)
        {
            return _calendarEventRepository.GetByCountry(country.ToString(), country.TimeZoneIds);
        }


        [PersistenceConversation(ConversationEndMode = EndMode.CommitAndContinue)]
        public Exception Save(CalendarEvent entity)
        {
            return AddSession(entity);
        }

        [PersistenceConversation(ConversationEndMode = EndMode.Continue)]
        private Exception AddSession(CalendarEvent entity)
        {
            if (!CanSave(entity))
                return new ConstraintException();
            _calendarEventRepository.MakePersistent(entity);
            return null;
        }

        [PersistenceConversation(ConversationEndMode = EndMode.CommitAndContinue)]
        public IEnumerable<CalendarEvent> SaveBatchOfWeekends(DateTime activeTime, string country)
        {
            var days = DateTime.DaysInMonth(activeTime.Year, activeTime.Month);
            var calendarEvents = new List<CalendarEvent>();
            for (var i = 1; i <= days; i++)
            {
                var currentDay = new DateTime(activeTime.Year, activeTime.Month, i);
                if (currentDay.DayOfWeek == DayOfWeek.Saturday || currentDay.DayOfWeek == DayOfWeek.Sunday)
                {
                    var entity = new Holiday
                                         {
                                             Country = country,
                                             Start = currentDay,
                                             End = currentDay.AddDays(1),
                                             EventName = currentDay.DayOfWeek.ToString()
                                         };
                    if (CanSave(entity))
                    {
                        _calendarEventRepository.MakePersistent(entity);
                        calendarEvents.Add(entity);
                    }
                }
            }
            return calendarEvents;
        }

        [PersistenceConversation(ConversationEndMode = EndMode.CommitAndContinue)]
        public void Delete(CalendarEvent entity)
        {
            _calendarEventRepository.MakeTransient(entity);
        }

        public IList<CalendarEvent> GetCalendarEvents(DateTime dateStart, DateTime dateEnd, string country, TimeZoneInfo timeZone)
        {
            return _calendarEventRepository.GetCalendarsBySubClassConditions(country, timeZone, dateStart, dateEnd);
        }

       

        private bool CanSave(CalendarEvent entity)
        {
            IList<CalendarEvent> foundList = _calendarEventRepository.GetSubClassByEqCondition(entity);
            return foundList.Count <= 0;
        }

        public void LoadGlobalCalendar(DateTime start, DateTime end)
        {
           var  globalCalendar = ApplicationCache.Get<Dictionary<DateTime, Dictionary<string, bool>>>(Global.GlobalCalendar);

            foreach (var calendarEvent in _calendarEventRepository.GetByRange<CalendarEvent>(start, end))
            {
                var dateKey = calendarEvent.Start.Date;
                while (dateKey < calendarEvent.End.Date)
                {
                    if (!globalCalendar.ContainsKey(dateKey))
                        globalCalendar[dateKey] = new Dictionary<string, bool>();

                    calendarEvent.SaftyInvoke<Holiday>(h =>
                    {
                        globalCalendar[dateKey][h.Country] = true;
                    });

                    calendarEvent.SaftyInvoke<DaylightSavingTime>(d =>
                    {
                        globalCalendar[dateKey][d.TimeZone.Id] = true;
                    });

                    dateKey = dateKey.AddDays(1);
                }
            }
        }
    }
}
