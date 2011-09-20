using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Common.Constants;
using Luna.Common.Extensions;
using Luna.Core.Extensions;
using Luna.Data;
using Luna.Infrastructure.Data.Repositories;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain.Model;
using uNhAddIns.Adapters;

namespace Luna.Shifts.Domain.Impl
{
    [PersistenceConversational(MethodsIncludeMode = MethodsIncludeMode.Implicit)]
    public class AttendanceManagerModel : IAttendanceManagerModel
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IRepository<Schedule> _scheduleRepository;
        private readonly IAttendanceRepository _attendanceRepository;

        private readonly IEntityFactory _entityFactory;

        public AttendanceManagerModel(IEntityFactory entityFactory, IEmployeeRepository employeeRepository, IRepository<Schedule> scheduleRepository, 
            IAttendanceRepository attendanceRepository)
        {
            _entityFactory = entityFactory;
            _employeeRepository = employeeRepository;
            _scheduleRepository = scheduleRepository;
            _attendanceRepository = attendanceRepository;
        }


        [PersistenceConversation(ConversationEndMode = EndMode.Continue)]
        public IList<Attendance> GetEmployees(Schedule schedule)
        {
            var reloadSchedule = _scheduleRepository.Get(schedule.Id);

            var employees = _employeeRepository.GetEmployeeByOrganizations(reloadSchedule.Organizations.ToArray(), schedule.Start);
            var involved = _attendanceRepository.GetAttendanceFrom(reloadSchedule).ToDictionary(o=> o.Agent);

            var results = new List<Attendance>(involved.Values);

            foreach (var o in employees)
            {
                if (involved.ContainsKey(o))
                    continue;
                var entity = _entityFactory.Create<Attendance>()
                    .SetProperties<Attendance>(o, reloadSchedule.Campaign, reloadSchedule.Start, reloadSchedule.End)
                    .CopyRule(o);
                entity.Joined = false;

                results.Add(entity);
            }
      
            return results;
        }

        [PersistenceConversation(ConversationEndMode = EndMode.Continue)]
        public Attendance CreateNewInstance(Attendance exist, Schedule schedule)
        {
            _attendanceRepository.Evict(exist);
            var o = _employeeRepository.Get(exist.Agent.Id);



            var entity = _entityFactory.Create<Attendance>()
                    .SetProperties<Attendance>(o, schedule.Campaign, schedule.Start, schedule.End);
            entity.SchedulingPayload = _entityFactory.Create<SchedulingPayload>();
            entity.CopyRule(o);
            entity.Joined = false;

            return entity;
        }

        [PersistenceConversation(ConversationEndMode = EndMode.CommitAndContinue)]
        public void Enroll(IEnumerable<Attendance> attendances)
        {
            attendances.ForEach(a => {
                if (!a.IsNew())
                    _attendanceRepository.Evict(a);

                _attendanceRepository.MakePersistent(a);
            }, c => c.Id == 0);
        }

        [PersistenceConversation(ConversationEndMode = EndMode.CommitAndContinue)]
        public void Evict(IEnumerable<Attendance> attendances)
        {
            attendances.ForEach(a => {
              
                _attendanceRepository.MakeTransient(a);
            }, c => c.Id != 0);
        }



        [PersistenceConversation(ConversationEndMode = EndMode.End)]
        public void AcceptAllChanges(IEnumerable<Attendance> attendances)
        {
            foreach (var item in attendances)
            {
                _attendanceRepository.MakePersistent(item);
            }
        }
    }
}
