using System;
using System.Collections.Generic;
using Luna.Common;

namespace Luna.Shifts.Domain.Model
{
    [IgnoreRegister]
    public interface ILaborHoursCountingModel
    {
        IList<Exception> SummarizeLaborRule(IAgent agent);

        /// <summary>
        /// For custom terms range use
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="terms"></param>
        /// <returns></returns>
        IList<Exception> SummarizeLaborRule(IAgent agent, IEnumerable<Term> terms);

     
        void SumLaborRuleHours(IAgent agent, IEnumerable<Term> terms);
    }
}
