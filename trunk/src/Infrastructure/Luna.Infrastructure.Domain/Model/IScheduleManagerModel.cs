using System;
using System.Collections.Generic;

namespace Luna.Infrastructure.Domain.Model
{
    public interface IScheduleManagerModel
    {
        Luna.Core.Tuple<IEnumerable<Campaign>, IList<Organization>> LoadMisc();

        bool Validate(Schedule schedule);

        IList<Schedule> GetAll(int pastMonths);

        void Save(Schedule schedule);

        Schedule CreateNewSchedule(Campaign campaign,out Luna.Core.Tuple<IEnumerable<ServiceQueue>, IEnumerable<Organization>> relations);

        void Abort();

        bool CanEditScheduleDate(Schedule schedule);

        Schedule EditSchedule(Guid scheduleId, out Luna.Core.Tuple<IEnumerable<ServiceQueue>, IEnumerable<Organization>> relations);

        //void ListSchedule(Action<Schedule> action);
    }
}