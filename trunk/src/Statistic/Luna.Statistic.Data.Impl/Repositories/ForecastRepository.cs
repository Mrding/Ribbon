using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Data;
using Luna.Infrastructure.Domain;
using Luna.Statistic.Data.Repositories;
using Luna.Statistic.Domain;
using NHibernate;
using Luna.Common;
using System.Diagnostics;

namespace Luna.Statistic.Data.Impl.Repositories
{
    public class ForecastRepository : Repository<Forecast>, IForecastRepository
    {
        public Forecast GetForecast(ServiceQueue serviceQueue, DateTime recordDate)
        {
            return Session.CreateQuery("from Forecast f where f.ServiceQueue=:serviceQueue and f.StartAt<=:date and f.EndAt>:date")
                .SetParameter("serviceQueue", serviceQueue)
                .SetDateTime("date", recordDate)
                .SetReadOnly(true)
                .SetCacheable(true)
                .SetCacheMode(CacheMode.Get)
                .UniqueResult<Forecast>();
        }

        public void DeleteForecast()
        {
            Session.CreateSQLQuery("delete ForecastTraffic where ForecastId is null").ExecuteUpdate();
            Session.CreateSQLQuery("delete ForecastServiceLevelGoal where ForecastId is null").ExecuteUpdate();
        }

        public void LoadForecastRaw(ServiceQueue[] serviceQueues, DateTime start, DateTime end, Action<IDailyObject> addTo)
        {
            if(serviceQueues.Length == 0) return;

            var x = new ActionableList<IDailyObject>(addTo);
            Session.CreateQuery(
                "from ForecastTraffic f where f.Forecast.ServiceQueue in (:serviceQueues) and f.Date >=:start and f.Date <:end")
                .SetParameterList("serviceQueues", serviceQueues)
                .SetDateTime("start", start.Date)
                .SetDateTime("end", end.Date)
                .SetReadOnly(true)
                .List(x);

            Session.CreateQuery(
               "from ForecastServiceLevelGoal f where f.Forecast.ServiceQueue in (:serviceQueues) and f.Date >=:start and f.Date <:end")
               .SetParameterList("serviceQueues", serviceQueues)
               .SetDateTime("start", start.Date)
               .SetDateTime("end", end.Date)
               .SetReadOnly(true)
               .List(x);
        }

        public void LoadActuralRaw(ServiceQueue[] serviceQueues, DateTime start, DateTime end, Action<IDailyObject> addTo)
        {
            if (serviceQueues.Length == 0) return;

            var x = new ActionableList<IDailyObject>(addTo);
            Session.CreateQuery(
                  "from ServiceQueueTraffic f where f.ServiceQueue in (:serviceQueue) and f.TrafficDate >=:start and f.TrafficDate <=:end")
                  .SetParameterList("serviceQueue", serviceQueues)
                  .SetDateTime("start", start.Date)
                  .SetDateTime("end", end.Date)
                  .List(x);
        }

        //public IDictionary<ServiceQueue, ActuralStaffData> GetActuralFrom(ServiceQueue[] serviceQueues, DateTime start, DateTime end)
        //{
        //    var acturalStaffDict = new Dictionary<ServiceQueue, ActuralStaffData>();
        //    var acturals = Session.CreateQuery(
        //            "from ServiceQueueTraffic f where f.ServiceQueue in (:serviceQueue) and f.TrafficDate >=:start and f.TrafficDate <=:end")
        //            .SetParameterList("serviceQueue", serviceQueues)
        //            .SetDateTime("start", start.Date)
        //            .SetDateTime("end", end.Date)
        //            .SetReadOnly(true)
        //            .SetCacheable(true)
        //            .SetCacheMode(CacheMode.Get)
        //            .List<ServiceQueueTraffic>();

        //    foreach (var serviceQueue in serviceQueues)
        //    {
        //        acturalStaffDict[serviceQueue] = new ActuralStaffData(acturals.Where(o => o.ServiceQueue.Id == serviceQueue.Id), start, end);
        //    }
        //    return acturalStaffDict;
        //}

        public void Update(Forecast record)
        {
            if (record.Id == 0)
            {
                Session.Save(record);
            }
            else
            {
                Session.CreateQuery("Update Forecast f set f.Name=:name,f.Description=:des,f.StartAt=:start,f.EndAt=:end,f.ServiceSecond=:ss,f.AbandonRate=:ar where f.Id=:id")
                    .SetString("name", record.Name)
                    .SetString("des", record.Description)
                    .SetDateTime("start", record.StartAt)
                    .SetDateTime("end", record.EndAt)
                    .SetInt32("ss", record.ServiceSecond)
                    .SetInt32("ar", record.AbandonRate)
                    .SetInt32("id", record.Id)
                    .ExecuteUpdate();
            }
        }
    }
}
