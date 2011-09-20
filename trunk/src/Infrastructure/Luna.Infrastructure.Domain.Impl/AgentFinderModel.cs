using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Core.Extensions;
using Luna.Data;
using Luna.Infrastructure.Data.Repositories;
using Luna.Infrastructure.Domain.Model;
using Luna.Shifts.Domain;
using uNhAddIns.Adapters;

namespace Luna.Infrastructure.Domain.Impl
{
    [PersistenceConversational(MethodsIncludeMode = MethodsIncludeMode.Implicit)]
    public class AgentFinderModel : IAgentFinderModel
    {
        private readonly IRepository<Skill> _skillRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private List<ICustomFilter> _filters;
        private ICustomFilter _timeBoxFilter;

        public AgentFinderModel(IRepository<Skill> skillRepository, IOrganizationRepository organizationRepository)
        {
            _skillRepository = skillRepository;
            _organizationRepository = organizationRepository;
        }

        public void CreateTimeBoxFilter(IEnumerable agents, DateTime targetDate)
        {
            var avaliableTypes = new List<string>();

            foreach (var item in agents)
            {
                var agent = item as IAgent;
                if (agent == null) break;
                avaliableTypes.AddRange(agent.Schedule.TermSet.Where(
                                                                        o =>
                                                                        o is IAssignment &&
                                                                        o.Start.Date == targetDate)
                                            .Select(o => o.Text));
            }
            if (avaliableTypes.Count == 0) return;

            _timeBoxFilter = new TimeBoxFilter(this, avaliableTypes.Distinct().OrderBy(o => o).ToList());
        }


        public IEnumerable<ICustomFilter> GetFilters()
        {
            _filters = new List<ICustomFilter>(new List<ICustomFilter>(new ICustomFilter[]
                           {
                               new AgentOnlyFilter(this),
                               new SkillMatchingFilter(_skillRepository.GetAll().ToList(),this),
                               new FieldRestrictionFilter(this),
                               
                               new EqEmployeeTypeFilter("EmployeeType1",this),
                               new OrganizationMatchingFilter(_organizationRepository.GetAll(),this),
                               new EqEmployeeTypeFilter("EmployeeType2",this),
                               new InRankFilter(this),
                               new EqEmployeeTypeFilter("EmployeeType3",this),
                               new InEnrollDateFilter(this)
                           }));
            if (_timeBoxFilter != null)
                _filters.Add(_timeBoxFilter);

            return _filters;
        }

        [PersistenceConversation(Exclude = true)]
        public void TearDown()
        {
            foreach (var item in _filters)
                item.Dispose();
            _filters = null;

            if(_timeBoxFilter != null)
            {
                _timeBoxFilter.Dispose();
                _timeBoxFilter = null;
            }
            this.SaftyInvoke<IDisposable>(d => d.Dispose());
        }
    }
}
