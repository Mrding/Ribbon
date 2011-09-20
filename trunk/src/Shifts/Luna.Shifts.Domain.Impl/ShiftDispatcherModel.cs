using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Common.Attributes;
using Luna.Common.Constants;
using Luna.Common.Extensions;
using Luna.Core.Extensions;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Data.Listeners;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain.Model;
using uNhAddIns.Adapters;
using System.Collections;
using Luna.Common.Domain;

namespace Luna.Shifts.Domain.Impl
{
    //[Aop]
    [PersistenceConversational(MethodsIncludeMode = MethodsIncludeMode.Implicit)]
    public class ShiftDispatcherModel : IShiftDispatcherModel
    {
        private readonly ITimeBoxRepository _timeBoxRepository;
        private readonly ITermStyleRepository _termStyleRepository;
        private readonly IEntityFactory _entityFactory;

        public ShiftDispatcherModel(ITimeBoxRepository timeBoxRepository, ITermStyleRepository termStyleRepository, IEntityFactory entityFactory)
        {
            _timeBoxRepository = timeBoxRepository;
            _termStyleRepository = termStyleRepository;
            _entityFactory = entityFactory;
        }

        public IEnumerable<TermStyle> GetAllAbsenetTypes()
        {
            return _termStyleRepository.GetInUseAbsentTypes();
        }

        public IEnumerable<TermStyle> GetAllEventTypes()
        {
            return _termStyleRepository.GetEventTypes();
        }

        public IEnumerable<AssignmentType> GetAllShiftTypes()
        {
            return _termStyleRepository.GetAssignmentTypes();
        }

        public IEnumerable<SubEventInsertRule> LoadSubEventInsertRules(AssignmentType assignmentType)
        {
            //_termStyleRepository.(assignmentType);
            return assignmentType.GetSubEventInsertRules();
        }

        public IList<IAgent> GetAgents(Schedule schedule)
        {
            return _timeBoxRepository.GetAgentsFrom<Agent>(schedule.Campaign, schedule.Start, schedule.End);
        }

        public IList<IAgent> GetPlanningAgents(Entity campaign, ITerm dateRange)
        {
            //多查询前期一天班表
            return _timeBoxRepository.GetAgentsFrom<PlanningAgent>(campaign, dateRange.Start, dateRange.End);
        }

        

        public PlanningAgent GetPlanningAgent(string[] values, ITerm range)
        {
            var proxyInstance = _timeBoxRepository.GetAgentFrom<PlanningAgent>(values, range.Start, range.End);

            return proxyInstance;
        }

        public IDictionary<string, HeaderContainer<AssignmentType, DailyCounter<AssignmentType>, DateTime>> GetAssignmentTypeDailyCounter(IEnumerable assignmentTypes, Schedule dateRange)
        {
            assignmentTypes.SaftyInvoke(list => list.ForEach<HeaderContainer<AssignmentType, DailyCounter<AssignmentType>, DateTime>>(o =>
            {
                foreach (var dailyCounter in o.Items)
                    _termStyleRepository.Evict(dailyCounter);
            }));
            return _termStyleRepository.GetAssignmentTypeDailyCounter(dateRange.Start, dateRange.End, dateRange.ServiceQueues.Keys);
        }

        [PersistenceConversation(ConversationEndMode = EndMode.CommitAndContinue)]
        public void UpdateSchedule(bool manually)
        {
        }

        //[PersistenceConversation(ConversationEndMode = EndMode.CommitAndContinue)]
        //public void ManuallyUpdateSchedule()
        //{
        //    TermAlteringListener.Manually();
        //}

        public void Reload(ref IAgent agent)
        {
            _timeBoxRepository.Detach(agent.Schedule);
            var reloadAgent = _timeBoxRepository.GetAgent(agent.LaborRule, agent.GetType()).TransferPropertiesFrom(agent);

            agent = reloadAgent;
        }

        public bool DeleteShift(IAgent agent, Term term)
        {
            return agent.Schedule.Delete(term, deletedTerms =>
                                                    {
                                                        foreach (var t in deletedTerms)
                                                            _timeBoxRepository.Detach(t);
                                                    }).Self(o =>
                                                                {
                                                                    if (o == false)
                                                                        agent.OperationFail = true;
                                                                });
        }

        [PersistenceConversation(ConversationEndMode = EndMode.CommitAndContinue)]
        public bool ApplyAbsent(IAgent agent, AssignmentBase assignment, TermStyle type, TimeSpan length)
        {
            TermAlteringListener.Manually();

            var newAbsent = new AbsentEvent(assignment.Start, length, type);
            var success = false;
            agent.Schedule.Create(newAbsent,
                (t, result) =>
                {
                    success = result;

                    //if (success)
                    //    assignment.OccupyStatus = "A";
                }, false);

            return success;
        }

        public void Release()
        {
            _timeBoxRepository.Clear();
        }

        public void Release(IEnumerable<IAgent> agents)
        {
            foreach (var agent in agents)
                _timeBoxRepository.Evict(agent.Schedule);
        }

        /// <summary>
        /// 日历智能感知, hrDate标记
        /// </summary>
        /// <param name="date"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [PersistenceConversation(Exclude = true)]
        public AssignmentBase CreateAssignmentWithSenser(DateTime date, AssignmentType type)
        {
            var applyType = type.Sense(date);

            return Term.New(date.AddMinutes(applyType.TimeRange.StartValue), applyType, applyType.TimeRange.Length)
                       .Self<AssignmentBase>(a =>
                                                 {
                                                     a.HrDate = date;
                                                     a.NativeName = type.Name;
                                                 });
        }
    }
}
