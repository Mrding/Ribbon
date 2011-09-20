using System;
using System.Collections.Generic;
using System.Linq;
using Iesi.Collections.Generic;
using Luna.Common;
using Luna.Common.Extensions;
using Luna.Infrastructure.Data.Repositories;
using Luna.Infrastructure.Domain.Model;
using Luna.Shifts.Data.Repositories;
using uNhAddIns.Adapters;

namespace Luna.Infrastructure.Domain.Impl
{
    [PersistenceConversational(MethodsIncludeMode = MethodsIncludeMode.Implicit)]
    public class ScheduleManagerModel : IScheduleManagerModel
    {
        private readonly ICampaignScheduleRepository _repository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IServiceQueueRepository _serviceQueueRepository;
        private readonly IEntityFactory _entityFactory;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IOrganizationRepository _organizationRepository;


        public ScheduleManagerModel(ICampaignScheduleRepository repository, ICampaignRepository campaignRepository,
                                    IServiceQueueRepository serviceQueueRepository, IAttendanceRepository attendanceRepository,
            IOrganizationRepository organizationRepository,
                                    IEntityFactory entityFactory)
        {
            _repository = repository;
            _campaignRepository = campaignRepository;
            _serviceQueueRepository = serviceQueueRepository;
            _entityFactory = entityFactory;
            _attendanceRepository = attendanceRepository;
            _organizationRepository = organizationRepository;
        }

        public Luna.Core.Tuple<IEnumerable<Campaign>, IList<Organization>> LoadMisc()
        {
            var misc = new Core.Tuple<IEnumerable<Campaign>, IList<Organization>>
          (_campaignRepository.GetAll(), _organizationRepository.GetHierarchicalTree());
            return misc;
        }

        public Schedule CreateNewSchedule(Campaign campaign, out Luna.Core.Tuple<IEnumerable<ServiceQueue>, IEnumerable<Organization>> relations)
        {
            var last = _repository.FindLast(campaign);

            relations = new Core.Tuple<IEnumerable<ServiceQueue>, IEnumerable<Organization>>(
               _serviceQueueRepository.GetServiceQueueByCampaign(campaign),
               campaign.Organizations.OfType<Organization>());

            IDictionary<ServiceQueue, int> defaultSvcQueues;
            ICollection<Entity> defaultOrgs;

            if (last != null)
            {
                defaultSvcQueues = new Dictionary<ServiceQueue, int>(last.ServiceQueues);
                defaultOrgs = new HashedSet<Entity>(last.Organizations);
            }
            else
            {
                defaultSvcQueues = relations.Item1.ToDictionary(o => o, o => 0);
                defaultOrgs = new HashedSet<Entity>(relations.Item2.Cast<Entity>().ToList());
            }

            var defaultStart = last == null ? DateTime.Today : last.End;

            var schedule = _entityFactory.Create<Schedule>(new Dictionary<string, object>
                                                               {
                                                                   {"Campaign", campaign},
                                                                   { "ServiceQueues", defaultSvcQueues },
                                                                   {"Organizations",defaultOrgs},
                                                                   {"Start", defaultStart},
                                                                   {"End", defaultStart.AddMonths(1)},
                                                                   {"Name", string.Format("{0:MMM}", defaultStart)}
                                                               });

            return schedule;
        }

        public bool CanEditScheduleDate(Schedule schedule)
        {
            return _attendanceRepository.CountAttendance(schedule) == 0;
        }

        public Schedule LoadDetails(Guid scheduleId)
        {
            return _repository.GetScheduleDetail(scheduleId);
        }

        [PersistenceConversation(ConversationEndMode = EndMode.Continue)]
        public Schedule EditSchedule(Guid scheduleId, out Luna.Core.Tuple<IEnumerable<ServiceQueue>, IEnumerable<Organization>> relations)
        {
            var schedule = _repository.Get(scheduleId);
            relations = new Core.Tuple<IEnumerable<ServiceQueue>, IEnumerable<Organization>>(
            _serviceQueueRepository.GetServiceQueueByCampaign(schedule.Campaign as Campaign),
            _organizationRepository.GetByCampaign(schedule.Campaign));
            return schedule;
        }

        //[PersistenceConversation(ConversationEndMode = EndMode.End)]
        //public void ListSchedule(Action<Schedule> action)
        //{
        //    _repository.List(action);
        //}

        [PersistenceConversation(ConversationEndMode = EndMode.Abort)]
        public void Abort()
        {

        }

        [PersistenceConversation(ConversationEndMode = EndMode.Continue)]
        public bool Validate(Schedule schedule)
        {
            return _repository.ValidateCross(schedule);
        }

        [PersistenceConversation(ConversationEndMode = EndMode.End)]
        public void Save(Schedule schedule)
        {
            if (schedule.IsNew())
                _repository.MakePersistent(schedule);
        }

        public IList<Schedule> GetAll(int pastMonths)
        {
            return _repository.GetAll(pastMonths);
        }
    }
}
