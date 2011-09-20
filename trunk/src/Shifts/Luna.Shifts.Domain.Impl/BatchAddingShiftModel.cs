using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Core.Extensions;
using Luna.Globalization;
using Luna.Shifts.Data.Listeners;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain.Model;
using uNhAddIns.Adapters;

namespace Luna.Shifts.Domain.Impl
{
    [Notificable]
    [PersistenceConversational(MethodsIncludeMode = MethodsIncludeMode.Explicit, ConversationId = "IBatchAlterModel")]
    public class BatchAddingShiftModel : IBatchAddingShiftModel
    {
        private readonly ITermStyleRepository _termStyleRepository;

        private DateTime _newAssignmentStart;
        private DateTime _newAssignmentEnd;

        public BatchAddingShiftModel(DateTime watchPoint, ITermStyleRepository termStyleRepository)
        {
            SearchDate = watchPoint;
            _termStyleRepository = termStyleRepository;
        }

        public string Category
        {
            get { return LanguageReader.GetValue("Assignment"); }
        }

        public IEnumerable<AssignmentType> Types { get; set; }

        [PersistenceConversation(ConversationEndMode = EndMode.Continue)]
        public void Initial()
        {
            Types = _termStyleRepository.GetAssignmentTypes();
            SelectedType = Types.FirstOrDefault();
        }

        public IEnumerable<ICustomAction> OptionalActions
        {
            get { return new ICustomAction[0]; }
        }

        public void OnDispatching()
        {
            _newAssignmentStart = SearchDate.Date.AddMinutes(SelectedType.TimeRange.StartValue);
            _newAssignmentEnd = _newAssignmentStart.AddMinutes(SelectedType.TimeRange.Length);
        }

        public void TryDispatch(IAgent agent, Func<Term, bool> filter, Action<Term, TimeBox> action, Action<ITerm, string, bool> callback)
        {
            var newAssignment = Term.NewAssignment(_newAssignmentStart, _newAssignmentEnd, SelectedType);

            agent.Schedule.Create(newAssignment, (terms, success) =>
            {
                if (success)
                    agent.Schedule.ArrangeSubEvent(newAssignment as IAssignment, SelectedType.GetSubEventInsertRules(), (t, result) => { });
                callback(newAssignment, "", success);
            }, false);
        }

        public string Title { get { return LanguageReader.GetValue("BatchInsertAssignment"); } }

        public virtual DateTime SearchDate { get; set; }

        public virtual AssignmentType SelectedType { get; set; }

        public void Comment(string content)
        {
            TermAlteringListener.Batching(content);
        }

        public void TearDown()
        {
            Types = null;
            this.SaftyInvoke<IDisposable>(d => d.Dispose());
        }
    }
}
