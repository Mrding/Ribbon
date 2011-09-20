using System;
using System.Collections.Generic;
using Luna.Data;
using Luna.Infrastructure.Domain;

namespace Luna.Infrastructure.Data.Repositories
{
    public interface ICampaignScheduleRepository : IRepository<Schedule>
    {
        IList<Schedule> GetAll(int month);

        bool ValidateCross(ISchedule schedule);

        Schedule FindLast(Campaign campaign);

        Schedule GetScheduleDetail(Guid scheduleId);

        Schedule GetScheduleWithServiceQueues(Schedule schedule);

        void List(Action<Schedule> action);
    }
}