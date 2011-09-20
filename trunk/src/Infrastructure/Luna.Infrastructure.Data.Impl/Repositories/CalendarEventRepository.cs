using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luna.Infrastructure.Data.Impl.Repositories
{
    using Luna.Data;
    using Luna.Infrastructure.Data.Repositories;
    using Luna.Infrastructure.Domain;

    using NHibernate.Criterion;

    public class CalendarEventRepository : Repository<CalendarEvent>, ICalendarEventRepository
    {
        public IList<T> GetAll<T>() where T : CalendarEvent
        {
            return Session.CreateCriteria<T>().List<T>();
        }

        public IList<T> GetByRange<T>(DateTime dateStart, DateTime dateEnd) where T : CalendarEvent
        {
            return Session.CreateCriteria<T>().Add(
                                  Restrictions.Or(Restrictions.Between("Start",dateStart,dateEnd),
                                  Restrictions.Between("End",dateStart,dateEnd)))
                          .List<T>();
        }

        public IList<CalendarEvent> GetByCountry(string country, IEnumerable<string> timeZonesOfCountry)
        {
            return Session.CreateSQLQuery(
                "select * from CalendarEvent c where c.Country=:country or c.TimeZoneId in(:timeZone)")
                .AddEntity(typeof(CalendarEvent)).SetString("country", country).SetParameterList("timeZone", timeZonesOfCountry).List<CalendarEvent>();
        }

        public IList<CalendarEvent> GetCalendarsBySubClassConditions(string country, TimeZoneInfo timeZone, DateTime fromDate = default(DateTime), DateTime toDate = default(DateTime))
        {
            return
                Session.CreateSQLQuery(
                    "select * from CalendarEvent c where (c.Country=:country or c.TimeZoneId=:timeZone) and c.StartDate>=:start and c.EndDate<=:end")
                    .AddEntity(typeof(CalendarEvent))
                    .SetString("country", country).SetString("timeZone", timeZone.Id).SetDateTime(
                        "start", fromDate).
                    SetDateTime("end", toDate).List<CalendarEvent>();
        }

        public Dictionary<DateTime, string> GetHolidays(string country, DateTime fromDate, DateTime toDate)
        {
            var holidays = Session.CreateSQLQuery(
                    "select * from CalendarEvent c where c.Country=:country and c.StartDate>=:start and c.EndDate<=:end")
                    .AddEntity(typeof(CalendarEvent))
                    .SetString("country", country)
                    .SetDateTime("start", fromDate).
                    SetDateTime("end", toDate).List<Holiday>();

            var results = new Dictionary<DateTime, string>(holidays.Count);

            foreach (var holiday in holidays)
            {
                var start = holiday.Start.Date;
                while (start < holiday.End.Date)
                {
                    results[start] = holiday.EventName;
                    start = start.AddDays(1);
                }
            }

            return results;
        }

        public IList<CalendarEvent> GetSubClassByEqCondition(CalendarEvent entity)
        {
            var queryCriteria = DetachedCriteria.For<CalendarEvent>();
            if (entity.CalendarEventType == typeof(DaylightSavingTime))
            {
                var daylightSavingTime = entity as DaylightSavingTime;
                if (daylightSavingTime != null)
                    queryCriteria.Add(Restrictions.Eq("TimeZone", daylightSavingTime.TimeZone.Id));
            }
            else
            {
                var holiday = entity as Holiday;
                if (holiday != null)
                    queryCriteria.Add(Restrictions.Eq("Country", holiday.Country));
            }

            queryCriteria.Add(
                Restrictions.Not(
                Restrictions.Or(
                    Restrictions.And(Restrictions.Gt("Start", entity.Start), Restrictions.Ge("Start", entity.End)),
                    Restrictions.And(Restrictions.Le("End", entity.Start), Restrictions.Lt("End", entity.End)))));

            queryCriteria.Add(Restrictions.Eq("EventName", entity.EventName));
            return queryCriteria.GetExecutableCriteria(Session).List<CalendarEvent>();
        }
    }
}
