using System;
using System.Collections.Generic;
using Luna.Common;
using Luna.Infrastructure.Domain;

namespace Luna.Shifts.Domain.Model
{
    public interface IBoxPairSwapModel
    {     
        /// <summary>
        /// 基本代换班
        /// </summary>
       void SwapingForTerm(Pair<Guid> agentPair, Pair<long?> assignmentPair, bool isLaborRule);
       /// <summary>
       /// 基本代换班验证
       /// </summary>
       Dictionary<Guid, Dictionary<string, bool>> SwapingForTermValidate(Pair<Guid> agentPair, Pair<long?> assignmentPair, Pair<DateRange> dateRange);

        /// <summary>
        /// 事件代换班
        /// </summary>
        void SwapingForSubEvent(Pair<Guid> agentPair, Pair<long> assignmentPair);

        Dictionary<Guid, Dictionary<string, bool>> SwapingForSubEventValidate(Pair<Guid> agentPair, Pair<long> assignmentPair);

        /// <summary>
        /// 多日代换班
        /// </summary>
        void SwapingForMultiDay(Pair<Guid> agentPair, DateRange dateRange, bool isLaborRule);
        /// <summary>
        /// 多日代换班验证
        /// </summary>
        Dictionary<Guid, Dictionary<string, bool>> SwapingForMultiDayValidate(Pair<Guid> agentPair, DateRange dateRange);

        /// <summary>
        /// 新时段代换班
        /// </summary>
        void SwapingForDateRange(Pair<Guid> agentPair, Pair<DateRange> dateRange, bool isLaborRule);
        /// <summary>
        /// 新时段代换班验证
        /// </summary>
        Dictionary<Guid, Dictionary<string, bool>> SwapingForDateRangeValidate(Pair<Guid> agentPair, Pair<DateRange> dateRange);
        /// <summary>
        /// 新验证
        /// </summary>
        Dictionary<string, bool> PostForNewRangeValidate(Guid agent, DateRange dateRange);

        /// <summary>
        /// 获取劳动政策
        /// </summary>
        Pair<Attendance> GetLaborRule();

        string LaborRuleInfo { get; }
        IList<Employee> GetAllEmployee();

        void SaveLog();
        void SaveLogs();
        string Submit();
   }
}
