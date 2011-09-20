using System.Collections.Generic;
using System.Linq;
using Luna.Data;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain;
using NHibernate.Criterion;
using Luna.Common;

namespace Luna.Shifts.Data.Impl.Repositories
{
    public class SeatRepository : Repository<Seat>, ISeatRepository
    {
        private Dictionary<string, Area> _areas;

        public IList<Seat> GetAllWithFullDetail()
        {
           return Session.CreateQuery("select s from Seat s inner join fetch s.Area").List<Seat>();
        }

        public override void LoadRelatedEntities()
        {
            _areas = DetachedCriteria.For<Area>()
                                        .GetExecutableCriteria(Session).List<Area>()
                                        .ToDictionary(o => o.GetUniqueKey());
        }

        public int GetMaxPriority(Entity area)
        {
            var result = Session.CreateQuery("select max(s.Index) from Seat s where s.Area = :area")
                .SetGuid("area", area.Id)
                .UniqueResult<int>();

            return result;
        }

        /// <summary>
        /// Also delete with SeatBox entity
        /// </summary>
        /// <param name="entity"></param>
        public override void MakeTransient(Seat entity)
        {
            var result = Session.Get<SeatBox>(entity.Id);

            if (result != null)
                Session.Delete(result);

            base.MakeTransient(entity);

        }


        public void MakePersistentWithSync(Seat entity)
        {
            if (_areas.ContainsKey(entity.Area.Name))
            {
                entity.Area = _areas[entity.Area.Name];

                entity.SetLocation(_areas[entity.Area.Name].Dimension);
                MakePersistent(entity);
            }
        }


        public override bool HaveAnyRelationships(Seat seat)
        {
            using (var session=Factory.OpenStatelessSession())
            {
                const string organizationSeatingArea = "select count(*) from OrganizationSeatingArea osa where osa.TargetSeat =:seat ";
                const string occupation = "select count(*) from Occupation o where o.SeatBoxId =:seat ";
                const string term = "select count(*) from Term t where t.SeatId =:seat ";
                const string seatConsolidationRule = "select count(*) from SeatConsolidationRule s where s.TargetSeat =:seat ";

                var sql = string.Format("select ({0})+({1})+({2})+({3})", organizationSeatingArea, occupation, term, seatConsolidationRule);

                var results = session.CreateSQLQuery(sql).SetGuid("seat", seat.Id).UniqueResult<int>();

                return results > 0;
            }
        }
    }
}
