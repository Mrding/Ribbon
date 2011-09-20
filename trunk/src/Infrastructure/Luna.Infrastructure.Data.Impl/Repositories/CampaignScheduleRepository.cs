using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Data;
using Luna.Infrastructure.Data.Repositories;
using Luna.Infrastructure.Domain;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;

namespace Luna.Infrastructure.Data.Impl.Repositories
{
    public class CampaignScheduleRepository : Repository<Schedule>, ICampaignScheduleRepository
    {
        public override IList<Schedule> GetAll()
        {
            //var results=
            return Session.CreateCriteria<Schedule>()
                .AddOrder(new Order("Start", false))
                .List<Schedule>();
        }

        public void List(Action<Schedule> action)
        {
            Session.CreateCriteria<Schedule>()
                .AddOrder(new Order("Campaign", true)).AddOrder(new Order("End", true))
                
                .List(new ActionableList<Schedule>(action));
        }

        public IList<Schedule> GetAll(int month)
        {
            IQuery query;
            if (month == 0)
            {
                query = Session.CreateQuery("from Schedule s inner join fetch s.Campaign left join fetch s.ServiceQueues order by s.Campaign , s.End");
            }
            else
            {
                query = Session.CreateQuery("from Schedule s inner join fetch s.Campaign left join fetch s.ServiceQueues where s.Start>=:date order by s.Campaign , s.End")
                        .SetDateTime("date", DateTime.Today.AddMonths(-month));
            }
            return query.SetResultTransformer(new DistinctRootEntityResultTransformer())
                 .List<Schedule>();
        }

        /// <summary>
        /// 验证时间是否接触
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <returns></returns>
        public bool ValidateCross(ISchedule schedule)
        {
            return this.Count(o => o.Id != schedule.Id && o.Start <= schedule.End && o.End > schedule.Start && o.Start != schedule.End && o.Campaign == schedule.Campaign) != 0;
        }

        public Schedule FindLast(Campaign campaign)
        {
           // var last = this.Where(o => o.Campaign == campaign).OrderBy(o=>o.End).Fetch(o => o.ServiceQueues).Fetch(c => c.Organizations).FirstOrDefault();
            var result = Session.CreateQuery(@"from Schedule s inner join fetch s.Campaign left join fetch s.ServiceQueues left join fetch s.Organizations
                                               where s.Campaign = :campaign order by s.End")
                                .SetGuid("campaign", campaign.Id)
                                .SetMaxResults(1)
                                .UniqueResult<Schedule>();
            return result;
        }

        public Schedule GetScheduleDetail(Guid scheduleId)
        {
            return Session.Get<Schedule>(scheduleId);
        }

        public Schedule GetScheduleWithServiceQueues(Schedule schedule)
        {
            //using (var session = Factory.OpenStatelessSession())
            //{
            return Session.CreateCriteria<Schedule>()
                .SetFetchMode("ServiceQueues", FetchMode.Eager)
                .Add(Restrictions.IdEq(schedule.Id))
                .SetResultTransformer(new DistinctRootEntityResultTransformer())
                .UniqueResult<Schedule>();
            //}
        }
    }
}
