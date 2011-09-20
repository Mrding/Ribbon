using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Globalization;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain.Model;
using uNhAddIns.Adapters;

namespace Luna.Shifts.Domain.Impl
{
    [Notificable]
    public class AssignmentBatchAlterModel : BatchAlterModelBase, IAssignmentBatchAlterModel
    {

        public AssignmentBatchAlterModel(DateTime watchPoint, ITimeBoxRepository timeBoxRepository,
                                         ITermStyleRepository termStyleRepository)
            : base(watchPoint, timeBoxRepository, termStyleRepository)
        {
            _title = LanguageReader.GetValue("BatchAlterExistsOfAssignemnt");
            _filters = new ICustomFilter[]
                               {
                                 new CustomFilter("EqAssignmentType", this, (t)=>
                                                          {
                                                              return  t.Start.Date == SearchDate.Date && t.Text.Equals(QueryType.Text);
                                                          }),
                                 
                                 new CustomFilter("EqTermStart",this ,(t)=>
                                                     {
                                                         return t.Start == SearchDate;
                                                     }),
                                 
                                 new CustomFilter("EqTermEnd", this ,(t)=>
                                                   {
                                                       return   t.End == SearchDate;
                                                   }),
                                 
                                 new CustomFilter("Any", this, (t)=>
                                                   {
                                                       return   t.Start.Date == SearchDate.Date;
                                                   })
                               };

        }

        public override string Category
        {
            get
            {
                return LanguageReader.GetValue("Assignment");
            }
        }

        public virtual IEnumerable<AssignmentType> Types { get; set; }

        public virtual AssignmentType QueryType { get; set; }


        public override void Initial()
        {
            Types = _termStyleRepository.GetAssignmentTypesWithInsertRules();
            QueryType = Types.FirstOrDefault();
        }

        public override IEnumerable<Term> FindTargetTerms(IAgent agent, Func<Term, bool> predicate)
        {
            var results = agent.SelectTargetTerms(
                t => (t is Assignment || t is OvertimeAssignment) && predicate(t));

            return results;
        }
    }
}
