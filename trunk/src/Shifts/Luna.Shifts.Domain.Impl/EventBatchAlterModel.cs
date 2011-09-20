using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Globalization;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain.Model;
using Luna.Common.Extensions;

namespace Luna.Shifts.Domain.Impl
{
    [Notificable]
    public class EventBatchAlterModel : BatchAlterModelBase, IEventBatchAlterModel
    {
        public EventBatchAlterModel(DateTime watchPoint, ITimeBoxRepository timeBoxRepository,
                                    ITermStyleRepository termStyleRepository)
            : base(watchPoint, timeBoxRepository, termStyleRepository)
        {
            _title = LanguageReader.GetValue("BatchAlterExistsOfSubEvent");

            OptionalFilters = new ICustomFilter[]
                                  {
                                      
                                 new CustomFilter("EqEventType", this,(t)=>
                                                   { 
                                                       return t.Text.Equals(QueryType.Text);
                                                   }),
                                      
                                 new CustomFilter("EqParentType", this,(t)=>
                                                   {
                                                       var bottomStyle = t.GetBottomStyle();
                                                       if(bottomStyle == null) return false;
                                                       return bottomStyle.Equals(QueryParentType.Text);
                                                   })   
                                  };

            _filters = new ICustomFilter[] {
                                 new CustomFilter("InTermRange", this,(t)=>
                                                                          {
                                                                              return t.IsInTheRange(SearchDate, End);
                                                                          }),
                                 
                                 new CustomFilter("EqTermStart", this ,(t)=> t.Start == SearchDate),
                                 
                                 new CustomFilter("EqTermEnd", this, (t)=> t.End == SearchDate)
                               };

            End = _searchDate.Date.AddDays(1).AddMinutes(-5);
        }

        public override string Category
        {
            get
            {
                return LanguageReader.GetValue("Event");
            }
        }

        public IEnumerable<TermStyle> Types { get; private set; }

        public IEnumerable<AssignmentType> ParentTypes { get; private set; }

        public IEnumerable<ICustomFilter> OptionalFilters { get; private set; }

        public AssignmentType QueryParentType { get; set; }

        public TermStyle QueryType { get; set; }

        private DateTime _end;
        public DateTime End
        {
            get { return DateTime.Parse(string.Format("{0:yyyy/MM/dd} {1:HH:mm}", _searchDate, _end)); }
            set { _end = value; }
        }

        public override void Initial()
        {
            ParentTypes = _termStyleRepository.GetAssignmentTypesWithInsertRules();
            Types = _termStyleRepository.GetEventTypes();

            QueryType = Types.FirstOrDefault();
            QueryParentType = ParentTypes.FirstOrDefault();
        }

        public override IEnumerable<Term> FindTargetTerms(IAgent agent, Func<Term, bool> predicate)
        {
            Func<Term, bool> whereClause = t => t.Level > 0 && t.Level < 3 && predicate(t);
            var results = agent.SelectTargetTerms(whereClause);

            return results;
        }
    }
}
