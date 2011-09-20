using System;
using System.Collections.Generic;
using Luna.Common;
using Luna.Data;
using Luna.Shifts.Domain;

namespace Luna.Shifts.Data.Repositories
{
    public interface ITimeBoxRepository : IRepository<TimeBox>
    {
        IList<TimeBox> GetTimeBoxes(string[] seatIds, Guid[] excludedEmployeeIds, DateTime start, DateTime end);

        TAgent GetAgentFrom<TAgent>(string[] text, DateTime start, DateTime end) where TAgent : IAgent;

        

        IList<IAgent> GetAgentsFrom<TAgent>(Entity campaign, DateTime start, DateTime end);

        void DeletedTerms(string[] employees,DateTime from, DateTime to);
     
        IList<TimeBox> GetTimeBoxesFrom(Entity campaign, DateTime start, DateTime end);
        IAgent GetAgent(Attendance attendance,Type agentInstanceType);

        IList<TimeBox> GetTimeBoxesByRange(string[] agentAcdids, DateTime start, DateTime end);
        IList<TimeBox> GetTimeBoxesByRange(Guid[] employeeIds, DateTime start, DateTime end);

        IList<TimeBox> Find(DateRange watchRange, DateRange editRange);
        TimeBox GetTermsWithSiblings(Guid timeBoxId, DateTime time);
        TimeBox GetTermsWithEndSibling(Guid timeBoxId, DateTime start, DateTime end);
        TimeBox GetTermsWithSiblings(Guid timeBoxId, DateTime start, DateTime end);
        TimeBox GetTermsWithMiddleRange(Guid employeeId, DateTime start, DateTime end);
        TimeBox GetTermsWithTouchRange(Guid employeeId, DateTime start, DateTime end);
        TimeBox GetTermsWithCurrentDay(Guid employeeId, DateTime date);
        IList<TimeBox> GetTermsWithCurrentDay(Guid employeeAId, Guid employeeBId, DateTime date);
        TimeBox GetTimeBoxByTermId(long termId);
        //IEnumerable<TimeBox> Find(Entity[] employees, DateRange watchRange, DateRange editRange, TermStyle style);
        IList<long> GetExsitTerms(List<long> ids);
        void EnableBackup(bool value);

        void Detach(object entity);

        void SaveLog(Term term, TimeBox timeBox);

        string BatchDeleteTerm(TimeBox timeBox ,string @operator,long[] ids);
    }
}
