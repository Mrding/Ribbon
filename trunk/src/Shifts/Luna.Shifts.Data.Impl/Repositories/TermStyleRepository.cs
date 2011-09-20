using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Common.Domain;
using Luna.Common.Extensions;
using Luna.Data;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using uNhAddIns.Transform;

namespace Luna.Shifts.Data.Impl.Repositories
{
    public class TermStyleRepository : Repository<TermStyle>, ITermStyleRepository
    {
        public Int64 Count(TermStyle example)
        {
            // example 几乎都是 IWellKnownProxy 类派生
            var queryCriteria = DetachedCriteria.For(example.GetType().BaseType);

            if (example.InUse)
                queryCriteria.Add(Restrictions.Eq("InUse", true));

            queryCriteria.Add(Restrictions.Eq("Name", example.Name));

            return DoCount(queryCriteria);
        }

        public IEnumerable<BasicAssignmentType> GetWholeAssignmentTypes()
        {
            var list = new List<BasicAssignmentType>();
            Session.CreateQuery(@"from BasicAssignmentType a where a.Type in (:Types) order by a.Name")
                                  .SetParameterList("Types",
                                   new[] {
                                     typeof (Assignment),
                                     typeof (OvertimeAssignment)})
                                     .List(new ActionableList<BasicAssignmentType>(o =>
            {
                o.GetEntityPrint();
                list.Add(o);
            }));
            return list;
        }

        public IEnumerable<TermStyle> GetByCategory(TermStyleCategory category)
        {
            var where = category == TermStyleCategory.Assignment ?
                new[] { typeof(Assignment), typeof(OvertimeAssignment), typeof(DayOff), typeof(TimeOff) } :
                new[] { typeof(UnlaboredSubEvent), typeof(RegularSubEvent), typeof(OvertimeSubEvent), typeof(AbsentEvent) };

            var q = Session.CreateCriteria<TermStyle>()
                    .Add(Restrictions.Eq("InUse", true))
                    .Add(Restrictions.In("Type", where))
                    .AddOrder(new Order("Name", true));

            if (category == TermStyleCategory.Assignment)
                q.SetFetchMode("SubEventInsertRules", FetchMode.Eager)
                 .SetResultTransformer(new DistinctRootEntityResultTransformer());



            return q.List<TermStyle>();
        }

        public IEnumerable<AssignmentType> GetAssignmentTypes()
        {
            var list = Session.CreateQuery(@"from AssignmentType a
                                            where a.InUse = true and a.Type in (:Types)
                                            order by a.Name")
                                    .SetParameterList("Types",
                                     new[] {
                                     typeof (Assignment),
                                     typeof (OvertimeAssignment)})
                                     .SetCacheable(true)
                                     .List<AssignmentType>();
            return list;
        }

        public IList<TConverted> GetAssignmentTypes<TConverted>(IResultTransformer resultTransformer)
        {
            var list = Session.CreateQuery(@"select a from AssignmentType a where a.InUse = true and a.Type in (:Types) order by a.Name")
                                   .SetParameterList("Types",
                                    new[] {
                                     typeof (Assignment),
                                     typeof (OvertimeAssignment)})
                              .SetResultTransformer(resultTransformer)
                              .List<TConverted>();
            return list;
        }



        public IDictionary<string, HeaderContainer<AssignmentType, DailyCounter<AssignmentType>, DateTime>> GetAssignmentTypeDailyCounter(DateTime start, DateTime end, ICollection<ServiceQueue> svcQueues)
        {
            DateTime[] dates;

            //x var indexer = new DateRange(start, end).CreateDateIndexer(new Dictionary<DateTime, int>(), d => d, out dates);
            var viewDays = end.IndexOf(start);

            var list = Session.CreateQuery(@"select a,k from AssignmentTypeDailyCounter a right join a.Kind k with k.InUse = true and k.Type = :classType left join fetch k.ServiceQueue q
                                            where (a.Date >= :from and a.Date < :end) or q in (:svcQueues) order by k.EstimationPriority")
                                    .SetDateTime("from", start)
                                    .SetDateTime("end", end)
                                    .SetParameterList("svcQueues", svcQueues)
                                    .SetParameter("classType", typeof(Assignment))
                                     .SetResultTransformer(new DelegateTransformer<DailyCounter<AssignmentType>>(t =>
                                        {
                                            var counter = t[0] as DailyCounter<AssignmentType>;
                                            if (counter != null)
                                                return counter;

                                            var assignmentType = t[1] as AssignmentType;
                                            if (assignmentType != null)
                                            {
                                                counter = new DailyCounter<AssignmentType>(assignmentType, start, 0);
                                                Session.Save(counter);
                                            }
                                            //if (result != null && result.Kind.ServiceQueue != null && svcQueues.Contains(result.Kind.ServiceQueue))
                                            return counter;
                                        }))
                                     .List<DailyCounter<AssignmentType>>();

            var gp = from d in list
                     //where d != default(DailyCounter<AssignmentType>)
                     group d by d.Kind into g
                     where g.Key.ServiceQueue != null && svcQueues.Contains(g.Key.ServiceQueue)
                     select new HeaderContainer<AssignmentType, DailyCounter<AssignmentType>, DateTime>(g.Key, date => date.IndexOf(start))
                     .BuildDailyCounter(g.ToArray(), o =>
                     {
                         Session.Save(o);
                     }
                        , start, viewDays)
                ;
            return gp.ToDictionary(o => o.Header.Text, o => o);
        }

        public IEnumerable<AssignmentType> GetAssignmentTypesWithInsertRules()
        {
            var list = Session.CreateQuery(@"select distinct a from AssignmentType a join a.SubEventInsertRules where a.InUse = true and a.Type in (:Types) order by a.Name")
                                    .SetParameterList("Types",
                                     new[] {
                                     typeof (Assignment),
                                     typeof (OvertimeAssignment)})
                                     .SetCacheable(true)
                                     .List<AssignmentType>();
            return list;
        }



        public IEnumerable<TermStyle> GetEventTypes()
        {
            return Session.CreateCriteria<TermStyle>()
                .Add(Restrictions.Eq("InUse", true))
                .Add(Restrictions.In("Type", new[]
                                                                      {
                                                                          typeof (UnlaboredSubEvent),
                                                                          typeof (RegularSubEvent),
                                                                          typeof (OvertimeSubEvent),
                                                                          typeof (Shrink)
                                                                      }))

                .AddOrder(new Order("Name", true))
                .SetCacheable(true)
                .List<TermStyle>();
        }

        public IEnumerable<TermStyle> GetWholeEventTypes()
        {
            return Session.CreateCriteria<TermStyle>()
                .Add(Restrictions.In("Type", new[]
                                                                      {
                                                                          typeof (UnlaboredSubEvent),
                                                                          typeof (RegularSubEvent),
                                                                          typeof (OvertimeSubEvent),
                                                                          typeof (Shrink)
                                                                      }))

                .AddOrder(new Order("Name", true))
                .SetCacheable(true)
                .List<TermStyle>();
        }

        public IEnumerable<TermStyle> GetAllUnlaboredSubEvents()
        {
            return Session.CreateCriteria<TermStyle>()
                .Add(Restrictions.Eq("InUse", true))
                .Add(Restrictions.In("Type", new[]
                                                 {
                                                     typeof (UnlaboredSubEvent)
                                                 }))
                .AddOrder(new Order("Name", true))
                .SetCacheable(true)
                .List<TermStyle>();
        }

        public IEnumerable<TermStyle> GetAllRegularSubEvents()
        {
            return Session.CreateCriteria<TermStyle>()
                .Add(Restrictions.Eq("InUse", true))
                .Add(Restrictions.In("Type", new[]
                                                 {
                                                     typeof (RegularSubEvent)
                                                 }))
                .AddOrder(new Order("Name", true))
                .SetCacheable(true)
                .List<TermStyle>();
        }

        public IEnumerable<TermStyle> GetInUseAbsentTypes()
        {
            return Session.CreateCriteria<TermStyle>()
                .Add(Restrictions.Eq("InUse", true))
                  .Add(Restrictions.In("Type", new[]
                                                                      {
                                                                          typeof (AbsentEvent)
                                                                      }))
                .AddOrder(new Order("Name", true))
                .SetCacheable(true)
                .List<TermStyle>();
        }

        public IEnumerable<TermStyle> GetAbsentTypes()
        {
            return Session.CreateCriteria<TermStyle>()
                  .Add(Restrictions.In("Type", new[]
                                                                      {
                                                                          typeof (AbsentEvent)
                                                                      }))
                .AddOrder(new Order("Name", true))
                .SetCacheable(true)
                .List<TermStyle>();
        }

        public override void MakeTransient(TermStyle entity)
        {
            Session.CreateQuery("delete from AssignmentTypeDailyCounter a where a.Kind = :et")
                   .SetEntity("et", entity)
                   .ExecuteUpdate();

            base.MakeTransient(entity);
        }
    }
}
