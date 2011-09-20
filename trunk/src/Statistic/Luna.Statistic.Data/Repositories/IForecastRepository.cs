using System;
using System.Collections.Generic;
using Luna.Data;
using Luna.Infrastructure.Domain;
using Luna.Statistic.Domain;

namespace Luna.Statistic.Data.Repositories
{
    public interface IForecastRepository : IRepository<Forecast>
    {
        Forecast GetForecast(ServiceQueue serviceQueue, DateTime recordDate);
        void DeleteForecast();

        void LoadActuralRaw(ServiceQueue[] serviceQueues, DateTime start, DateTime end, Action<IDailyObject> addTo);

        void Update(Forecast record);

        void LoadForecastRaw(ServiceQueue[] serviceQueues, DateTime start, DateTime end, Action<IDailyObject> addTo);
    }
}
