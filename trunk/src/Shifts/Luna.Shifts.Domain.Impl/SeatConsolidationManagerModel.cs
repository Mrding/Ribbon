using System.Collections.Generic;
using Luna.Common;
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
    public class SeatConsolidationManagerModel : ISeatConsolidationManagerModel
    {
        private readonly IRepository<Skill> _skillRepository;
        private readonly ITermStyleRepository _termStyleRepository;
        private readonly ISeatConsolidationRepository _seatConsolidationRepository;
        private readonly IAreaRepository _areaRepository;
        private readonly IOrganizationRepository _organizationRepository;


        public SeatConsolidationManagerModel(ISeatConsolidationRepository seatConsolidationRepository, IRepository<Skill> skillRepository, ITermStyleRepository termStyleRepository,
            IAreaRepository areaRepository, IOrganizationRepository organizationRepository)
        {
            _skillRepository = skillRepository;
            _termStyleRepository = termStyleRepository;
            _seatConsolidationRepository = seatConsolidationRepository;
            _areaRepository = areaRepository;
            _organizationRepository = organizationRepository;
        }

        public IEnumerable<Skill> GetSkills()
        {
            return _skillRepository.GetAll();
        }

        [PersistenceConversation(ConversationEndMode = EndMode.Continue)]
        public Luna.Core.Tuple<IEnumerable<Organization>, IEnumerable<Area>> GetDetails(Entity site, Entity campaign)
        {
            //reload entity from repository
            var organizations = _organizationRepository.GetByCampaign(campaign);
            var areas = _areaRepository.GetWithSeatPriorityDetails(site, campaign);
            return new Luna.Core.Tuple<IEnumerable<Organization>, IEnumerable<Area>>(organizations, areas);
        }


        public IEnumerable<AssignmentType> GetAssignmentTypes()
        {
            return _termStyleRepository.GetAssignmentTypes();
        }

        public IEnumerable<SeatConsolidationRule> GetAll(Entity site, Entity campaign)
        {
            return _seatConsolidationRepository.GetByStieAndCampaign(site, campaign);
        }

        public SeatConsolidationRule Reload(SeatConsolidationRule rule)
        {
            _seatConsolidationRepository.Evict(rule);

            return _seatConsolidationRepository.Get(rule.Id);
        }

        [PersistenceConversation(ConversationEndMode = EndMode.CommitAndContinue)]
        public void Save(SeatConsolidationRule rule)
        {
            _seatConsolidationRepository.MakePersistent(rule);
        }

        public void Reload(ref SeatConsolidationRule entity)
        {
            if (entity.SaftyGetProperty<bool, IEditingObject>(o => o.IsNew))
            {
                _seatConsolidationRepository.Evict(entity);
            }
            else
            {
                _seatConsolidationRepository.Refresh(ref entity);
            }
        }

        [PersistenceConversation(ConversationEndMode = EndMode.CommitAndContinue)]
        public void Delete(SeatConsolidationRule rule)
        {
            _seatConsolidationRepository.MakeTransient(rule);
        }

        [PersistenceConversation(ConversationEndMode = EndMode.End)]
        public void SaveAll(IEnumerable<SeatConsolidationRule> rules)
        {
            rules.ForEach(r => _seatConsolidationRepository.MakePersistent(r));
        }

        [PersistenceConversation(ConversationEndMode = EndMode.Continue)]
        public void SaveAllIndex(IEnumerable<SeatConsolidationRule> rules)
        {
            rules.ForEach(r => _seatConsolidationRepository.MakePersistent(r, () => r.Index));
        }
    }
}
