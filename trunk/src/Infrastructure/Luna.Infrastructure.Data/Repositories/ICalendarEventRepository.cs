using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luna.Infrastructure.Data.Repositories
{
    using Luna.Data;
    using Luna.Infrastructure.Domain;

    public interface ICalendarEventRepository : IRepository<CalendarEvent>
    {
        IList<T> GetAll<T>() where T : CalendarEvent;

        IList<CalendarEvent> GetByCountry(string country, IEnumerable<string> timeZonesOfCountry);

        IList<T> GetByRange<T>(DateTime dateStart, DateTime dateEnd) where T : CalendarEvent;

        IList<CalendarEvent> GetCalendarsBySubClassConditions(
            string country,
            TimeZoneInfo timeZone,
            DateTime fromDate = default(DateTime),
            DateTime toDate = default(DateTime));

        IList<CalendarEvent> GetSubClassByEqCondition(CalendarEvent entity);

        Dictionary<DateTime, string> GetHolidays(string country, DateTime fromDate, DateTime toDate);
    }
}
