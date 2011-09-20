using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luna.Infrastructure.Domain.Model
{
    using Luna.Common.Model;

    public interface ICalendarEventModel
    {
        IList<CalendarEvent> GetAll();

        IList<CalendarEvent> GetByCountry(Country country);

        Exception Save(CalendarEvent entity);

        IEnumerable<CalendarEvent> SaveBatchOfWeekends(DateTime activeTime, string country);

        void Delete(CalendarEvent entity);

        IList<CalendarEvent> GetCalendarEvents(DateTime dateStart, DateTime dateEnd, string country, TimeZoneInfo timeZone);

        void LoadGlobalCalendar(DateTime start, DateTime end);
    }
}
