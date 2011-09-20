using System.Collections.Generic;
using Luna.Infrastructure.Data.Repositories;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain.Model;
using uNhAddIns.Adapters;
using Luna.Common;
using System.Linq;
using Luna.Infrastructure.Domain;
using Luna.Common.Extensions;

namespace Luna.Shifts.Domain.Impl
{
    [PersistenceConversational(MethodsIncludeMode = MethodsIncludeMode.Implicit)]
    public class SeatingPrioritySetupModel : ISeatingPrioritySetupModel
    {
        private readonly IAreaRepository _areaRepository;
        private readonly IEntityFactory _entityFactory;
        private readonly IEmployeeRepository _employeeRepository;

        public SeatingPrioritySetupModel(IAreaRepository areaRepository,
            IEmployeeRepository employeeRepository, IEntityFactory entityFactory)
        {
            _areaRepository = areaRepository;
            _entityFactory = entityFactory;
            _employeeRepository = employeeRepository;
        }

        [PersistenceConversation(ConversationEndMode = EndMode.CommitAndContinue)]
        public Area ReloadWithSeat(Area area, out IEnumerable<IArrangeSeatRule> availableOrganizations)
        {
            var result = _areaRepository.Get(area.Id);
            var exists = _areaRepository.GetOrganizationSeatingArea(area.Id).ToDictionary(o => o.Object);

            var all = new List<IArrangeSeatRule>();

            var defaultTargetSeat = result.Seats.FirstOrDefault();

            foreach (var item in ((Campaign)result.Campaign).Organizations)
            {
              
                if (exists.ContainsKey(item))
                {
                    all.Add(exists[item]);
                }
                else
                {
                    var o = defaultTargetSeat == null ? null : _entityFactory.Create<OrganizationSeatingArea>(new Dictionary<string, object> { { "Area", result }, { "Object", item }, { "TargetSeat", defaultTargetSeat } });
                    if (o != null)
                    {
                        _areaRepository.MakePersistent(o);
                        all.Add(o);
                    }
                }
            }

            availableOrganizations = all.ToRebuildPriorityList<IArrangeSeatRule, IArrangeSeatRule>(true, null);
            return result;
        }

        public Core.Tuple<IList<Employee>, Dictionary<ISeat, List<PriorityEmployee>>> GetPriorityEmployeeMisc(Entity area)
        {
            var employees = _employeeRepository.GetAll();
            var list = _areaRepository.GetPriorityEmployees(new[] { area }).ToUsefulDictionary();

            return new Core.Tuple<IList<Employee>, Dictionary<ISeat, List<PriorityEmployee>>>(employees, list);
        }

        [PersistenceConversation(ConversationEndMode = EndMode.CommitAndContinue)]
        public void AddPriorityEmployee(PriorityEmployee entity)
        {
            _areaRepository.SaveOrUpdate(entity);
        }

        [PersistenceConversation(ConversationEndMode = EndMode.CommitAndContinue)]
        public void DeletePriorityEmployee(PriorityEmployee entity)
        {
            _areaRepository.Delete(entity);
        }

        [PersistenceConversation(ConversationEndMode = EndMode.End)]
        public void Save()
        {
        }
    }
}
