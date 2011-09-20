using System;
using System.Collections;
using System.Collections.Generic;
using Luna.Common;
using Luna.Common.Extensions;
using Luna.Core.Extensions;
using Luna.Infrastructure.Data.Repositories;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain.Model;
using uNhAddIns.Adapters;

namespace Luna.Shifts.Domain.Impl
{
    [PersistenceConversational(MethodsIncludeMode = MethodsIncludeMode.Implicit)]
    public class AssignmentTypeModel : IAssignmentTypeModel
    {
        private readonly ITermStyleRepository _termStyleRepository;
        private readonly IServiceQueueRepository _serviceQueueRepository;
        private readonly IEntityFactory _entityFactory;

        public AssignmentTypeModel(ITermStyleRepository termStyleRepository, IServiceQueueRepository serviceQueueRepository, IEntityFactory entityFactory)
        {
            _termStyleRepository = termStyleRepository;
            _serviceQueueRepository = serviceQueueRepository;
            _entityFactory = entityFactory;
        }

        public IEnumerable<BasicAssignmentType> GetAllAssignmentTypes()
        {
            return _termStyleRepository.GetWholeAssignmentTypes();
        }

        public IList<ServiceQueue> GetAllServiceQueues()
        {
            return _serviceQueueRepository.GetAll();
        }

        public void Reload(ref BasicAssignmentType entity)
        {
            if (entity.SaftyGetProperty<bool, IEditingObject>(o => o.IsNew))
            {
                _termStyleRepository.Evict(entity);
            }
            else
            {
                _termStyleRepository.Refresh(ref entity);
                entity.Rebuild();
            }
        }

        [PersistenceConversation(ConversationEndMode = EndMode.CommitAndContinue)]
        public void Save(BasicAssignmentType entity)
        {
            //if (entity.IsNew())
            _termStyleRepository.MakePersistent(entity);
        }

        [PersistenceConversation(Exclude = true)]
        public BasicAssignmentType SaveAsNew(string newName, Type type, ref BasicAssignmentType source)
        {
            var newEntity = _entityFactory.Create<BasicAssignmentType>(type);

            newEntity.GapGuaranteed = source.GapGuaranteed;
            newEntity.Occupied = source.Occupied;
            newEntity.IgnoreAdherence = source.IgnoreAdherence;
            newEntity.AsAWork = source.AsAWork;
            newEntity.AsARest = source.AsARest;

            newEntity.Name = newName;
            newEntity.Type = source.Type;
            newEntity.Background = source.Background;
            newEntity.SetNewTime(source.Start, source.End);

            foreach (var o in source.GetSubEventInsertRules())
            {
                var copy = o;
                _entityFactory.Create<SubEventInsertRule>().Self(
                    n =>
                    {
                        n.TimeRange = copy.TimeRange;
                        n.SubEvent = copy.SubEvent;
                        n.SubEventLength = copy.SubEventLength;
                        newEntity.AddSubEventInsertRule(n);
                    });
            }

            if (type == typeof(AssignmentType))
            {
                var copy = source.As<AssignmentType>();
                newEntity.Self<AssignmentType>(n =>
                {
                    n.ServiceQueue = copy == null ? null: copy.ServiceQueue;
                    n.EstimationPriority = copy == null ? 1 : copy.EstimationPriority;
                    n.Template1 = copy == null ? null : copy.Template1;
                    n.Template2 = copy == null ? null : copy.Template2;
                    n.Template3 = copy == null ? null : copy.Template3;
                    n.TimeZoneInfoId = copy == null ? "Taipei Standard Time" : copy.TimeZone.Id;
                    n.Country = copy == null ? "Taiwan" :  copy.Country;

                    if (copy != null)
                    {
                        n.WorkingDayMask.Weekdays = copy.WorkingDayMask.Weekdays;
                        n.WorkingDayMask.Weekdays2 = copy.WorkingDayMask.Weekdays2;
                        n.WorkingDayMask.Monthdays = copy.WorkingDayMask.Monthdays;
                    }
                });
            }

            //_termStyleRepository.MakePersistent(newEntity);

            //Reload(ref source);
            return newEntity;
        }

        [PersistenceConversation(ConversationEndMode = EndMode.CommitAndContinue)]
        public void Delete(BasicAssignmentType entity)
        {
            if (!entity.IsNew())
            {
                _termStyleRepository.MakeTransient(entity);
            }
        }

        public bool DuplicationChecking(TermStyle entity,string name)
        {
            var example = Activator.CreateInstance(entity.GetType()).As<TermStyle>();
            example.Name = name;
            var count =  _termStyleRepository.Count(example);

            return 0 < count;
        }

        [PersistenceConversation(ConversationEndMode = EndMode.Abort)]
        public void Release()
        {
            _termStyleRepository.Clear();
        }
    }
}