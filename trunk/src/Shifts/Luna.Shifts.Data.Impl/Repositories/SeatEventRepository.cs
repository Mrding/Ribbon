using System.Collections.Generic;
using Luna.Common;
using Luna.Data;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain;
using NHibernate.Criterion;

namespace Luna.Shifts.Data.Impl.Repositories
{
    public class SeatEventRepository : Repository<SeatEvent>, ISeatEventRepository
    {
      

        public IList<SeatEvent> Searh(Entity site, string category, DateRange range)
        {
            //Session.EnableFilter("OccupationRange").SetParameter("start", range.Start)
            //                .SetParameter("end", range.End);

            // var results = Session.CreateCriteria<SeatBox>()
            //    .CreateAlias("Seat", "seat")
            //    .CreateAlias("seat.Area", "area")
            //    .Add(Restrictions.Eq("area.Site", site))
            //    .CreateAlias("Occupations", "o")
            //    .Add(Restrictions.Eq("o.Category", category))
            //    .SetProjection(Projections.Property("Occupations"))
            //    .List<SeatEvent>();

            //Session.DisableFilter("OccupationRange");

            //return results;
            return Session.CreateCriteria<SeatEvent>()
               .CreateAlias("Seat", "seat")
               .CreateAlias("seat.Area", "area")
               .Add(Restrictions.Eq("Category", category))
               .Add(Restrictions.Ge("Start", range.Start))
               .Add(Restrictions.Le("End", range.End))
               .Add(Restrictions.Eq("area.Site", site))
               .List<SeatEvent>();
        }
    }
}