using System;
using System.Collections.Generic;
using Luna.Infrastructure.Domain;

namespace Luna.Statistic.Domain
{
    public interface IDailyObject
    {
        DateTime Date { get; }

        T GroupBy<T>() where T : class;

    }



    public class Forecast
    {
        public Forecast()
        {
            ForecastTraffics = new List<ForecastTraffic>();
            ForecastServiceLevelGoals = new List<ForecastServiceLevelGoal>();
        }
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime StartAt { get; set; }
        public virtual DateTime EndAt { get; set; }
        public virtual int ServiceSecond { get; set; }
        public virtual int AbandonRate { get; set; }
        public virtual ServiceQueue ServiceQueue { get; set; }
        public virtual int ServiceLevelGoalId { get; set; }
        public virtual ICollection<ForecastTraffic> ForecastTraffics { get; set; }
        public virtual ICollection<ForecastServiceLevelGoal> ForecastServiceLevelGoals { get; set; }
    }
}
