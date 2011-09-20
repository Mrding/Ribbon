using System;
using System.Collections;
using System.Collections.Generic;
using Luna.Core;
using Luna.Infrastructure.Domain;
using Luna.Common;
using Luna.Common.Domain;

namespace Luna.Shifts.Domain.Model
{
    public interface IShiftDispatcherModel
    {
        IList<IAgent> GetAgents(Schedule schedule);

        IDictionary<string, HeaderContainer<AssignmentType, DailyCounter<AssignmentType>, DateTime>>
            GetAssignmentTypeDailyCounter(IEnumerable assignmentTypes, Schedule dateRange);

        IList<IAgent> GetPlanningAgents(Entity campaign, ITerm dateRange);

        PlanningAgent GetPlanningAgent(string[] values, ITerm range);

        void UpdateSchedule(bool manually);

        //void ManuallyUpdateSchedule();

        bool DeleteShift(IAgent agent, Term shift);

        void Reload(ref IAgent agent);

        bool ApplyAbsent(IAgent agent, AssignmentBase assignment, TermStyle type, TimeSpan length);

        IEnumerable<TermStyle> GetAllAbsenetTypes();

        IEnumerable<TermStyle> GetAllEventTypes();

        IEnumerable<AssignmentType> GetAllShiftTypes();

        //void Abort();

        IEnumerable<SubEventInsertRule> LoadSubEventInsertRules(AssignmentType assignmentType);

        void Release();

        void Release(IEnumerable<IAgent> agents);
        

        AssignmentBase CreateAssignmentWithSenser(DateTime date, AssignmentType type);

    }
}
