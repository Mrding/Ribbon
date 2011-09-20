using System;
using System.Collections.Generic;
using Luna.Common;
using Luna.Data;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Data.Repositories;
using NHibernate;
using NHibernate.Criterion;
using Luna.Shifts.Domain;
using Luna.Common.Extensions;

namespace Luna.Shifts.Data.Impl.Repositories
{
    public class SeatBoxRepository : Repository<SeatBox>, ISeatBoxRepository
    {

        public override IList<SeatBox> GetAll()
        {
            Session.EnableFilter("OccupationLargeRange").SetParameter("start", DateTime.Parse("2000/1/1"))
                                                        .SetParameter("end", DateTime.Parse("2000/1/2"));
            var results = Session.CreateQuery("from SeatBox s")
                              .List<SeatBox>();
            Session.DisableFilter("OccupationLargeRange");
            return results;
        }

        public IList<SeatBox> Search(Entity[] areas, DateTime start, DateTime end, bool includedNotInUse)
        {
            if (areas.Length == 0) return new List<SeatBox>();



            var hql = string.Empty;

            hql = includedNotInUse ? "from SeatBox s inner join fetch s.Seat where s.Seat.Area in (:areas) order by s.Seat.Number" :
                                    "from SeatBox s inner join fetch s.Seat where s.Seat.InUse = true and s.Seat.Area in (:areas) order by s.Seat.Number";

            Session.EnableFilter("OccupationLargeRange").SetParameter("start", start)
                                        .SetParameter("end", end);
            var results = Session.CreateQuery(hql)
                                 .SetParameterList("areas", areas)
                                 .List<SeatBox>();

            Session.DisableFilter("OccupationLargeRange");

            return results;
        }

        //public void 

        public IList<SeatBox> Search(Entity area, DateTime watchPoint)
        {
            Session.EnableFilter("OccupationCurrentRange").SetParameter("watchPoint", watchPoint);

            var results = Session.CreateQuery("from SeatBox s where s.Seat.Area=:area")
                                 .SetGuid("area", area.Id)
                                 .List<SeatBox>();

            Session.DisableFilter("OccupationCurrentRange");

            return results;
        }

        public void Search(Entity area, DateTime watchPoint, Action<string, Occupation> loopingDelegate)
        {
            var seats = Session.Get<Area>(area.Id).Seats;

            foreach (var seat in seats)
            {
                var seatEvent = GetSeatEvent(seat, watchPoint);
                var result = seatEvent.Value ?? GetPlannedSeatArrangement(seat, watchPoint);
                loopingDelegate(seat.ExtNo, result);
            }
        }

        private IFutureValue<Occupation> GetSeatEvent(ISeat seat, DateTime watchPoint)
        {
            return Session.CreateQuery("from SeatEvent s where s.Seat =:seat and s.Start <=:watchPoint and s.End >:watchPoint")
                .SetGuid("seat", seat.Id)
                 .SetDateTime("watchPoint", watchPoint)
                 .SetMaxResults(1).FutureValue<Occupation>();
        }



        public SeatArrangement GetPlannedSeatArrangement(ISeat seat, DateTime watchPoint)
        {
            var result = default(SeatArrangement);
            var seatId = seat.Id.ToString();

            //const int maxLayerCount = 1;

            var terms = Session.CreateQuery("from Term t where t.Seat =:seatId and t.Start <=:watchPoint order by t.End desc, t.Level desc")
                .SetString("seatId", seatId)
                .SetMaxResults(1)
                .SetDateTime("watchPoint", watchPoint)
                .List<Term>();

            if (terms.Count > 0)
            {
                var term = terms[0];
                var employeeId = term.GetSnapshotValue<Guid>("EmployeeId");

                var source = default(Term);
                var startTime = default(DateTime);

                var watchPointIsInTheTerm = watchPoint.IsInTheRange(term);
               
                if(term is AssignmentBase)
                {
                    if (!watchPointIsInTheTerm) return null;
                    
                    //find upper term
                    var upperLevelTerm = Session.CreateQuery("from Term t where t.Bottom is not null and t.Start <=:watchPoint and t.Start >=:parentStart and t.End <=:parentEnd and t.Seat is not null order by t.End desc, t.Level desc")
                        .SetDateTime("parentStart", term.Start)
                        .SetDateTime("parentEnd", term.End)
                                        .SetDateTime("watchPoint", watchPoint)
                                        .SetMaxResults(1)
                                        .UniqueResult<Term>();

                    if (upperLevelTerm == null || (upperLevelTerm.SeatIsEmpty() && upperLevelTerm.IsNeedSeat))
                    {
                        source = term;
                        startTime = term.Start;
                    }
                    else if (!upperLevelTerm.SeatIsEmpty() && upperLevelTerm.Seat != seatId)
                        return null;
                    else if(upperLevelTerm.Seat == term.Seat && !upperLevelTerm.IsNeedSeat)
                    {
                        source = upperLevelTerm;
                        startTime = upperLevelTerm.End;
                    }
                }
                else
                {
                    //subevnet
                    if(term.IsNeedSeat)
                    {
                        if(watchPointIsInTheTerm)
                        {
                            source = term;
                            startTime = term.Start;
                        }
                        else
                            return null;
                    }
                    else
                    {
                        if (watchPointIsInTheTerm)
                            return null;
                        else
                        {
                            if (!watchPoint.IsInTheRange(term.Bottom))
                                return null;
                            else
                            {
                                source = term;
                                startTime = term.End;
                            }
                        }
                    }
                }
                if (source == null || startTime == default(DateTime))
                    return null;
                result = new SeatArrangement(Session.Get<Employee>(employeeId), startTime, watchPoint, source, seat);
            }
            return result;
        }


        public SeatBox GetByRagne(Guid seatId, DateTime start, DateTime end)
        {
            Session.EnableFilter("OccupationLargeRange")
                   .SetParameter("start", start)
                   .SetParameter("end", end);

            var result = Session.CreateQuery("from SeatBox s where s.Id = :seatId")
                .SetGuid("seatId", seatId).UniqueResult<SeatBox>();

            Session.DisableFilter("OccupationLargeRange");

            return result;
        }

        public IList<SeatBox> GetByRagne(Guid[] seatIds, DateTime start, DateTime end)
        {
            if (seatIds == null || seatIds.Length == 0) return new List<SeatBox>();

            Session.EnableFilter("OccupationLargeRange")
                   .SetParameter("start", start)
                   .SetParameter("end", end);

            var results = Session.CreateQuery("from SeatBox s where s.Id in (:seatIds)")
                .SetParameterList("seatIds", seatIds).List<SeatBox>();

            Session.DisableFilter("OccupationLargeRange");

            return results;
        }

        //public IList<SeatBox> GetSeatArrangement(Guid empId, DateTime start, DateTime end)
        //{
        //    Session.EnableFilter("OccupationLargeRange").SetParameter("start", start)
        //                               .SetParameter("end", end);
        //    var results = Session.CreateCriteria<SeatBox>()
        //        .CreateCriteria("Occupations", "occupation")
        //        .Add(Restrictions.Eq("occupation.Agent.Id", empId))
        //        .SetResultTransformer(new DistinctRootEntityResultTransformer())
        //        .List<SeatBox>();
        //    Session.DisableFilter("OccupationLargeRange");
        //    return results;
        //}

        public void MakePersistent(SeatBox seatBox, ITerm[] shifts)
        {
            var fail = false;
            MakePersistent(seatBox, (sb, ex) =>
                                        {
                                            Refresh(ref seatBox);
                                            fail = true;
                                        });
            if (fail) return;

            foreach (var term in shifts)
                UpdateShiftOccupyStatus(term);
        }

        public void MakeTransient(Seat seat)
        {
            var result = Session.CreateCriteria<SeatBox>()
                .Add(Restrictions.Eq("Seat.Id", seat.Id))
                .UniqueResult<SeatBox>();
            if (result != null)
                Session.Delete(result);
        }


        public void UpdateShiftOccupyStatus(ITerm shift)
        {
            Session.SaveOrUpdate(shift);
        }


    }
}
