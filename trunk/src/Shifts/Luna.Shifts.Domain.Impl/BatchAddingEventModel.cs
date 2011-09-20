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
    [Notificable(AdditionalType = typeof(IBatchAlterModel))]
    [PersistenceConversational(MethodsIncludeMode = MethodsIncludeMode.Explicit, ConversationId = "IBatchAlterModel")]
    public class BatchAddingEventModel : IBatchAddingEventModel
    {
        private readonly ITermStyleRepository _termStyleRepository;
        private Action<ITerm, string, bool> _dispatchedCallback;
        private int _length;
        private Func<Term, bool> _customValidation;

        public BatchAddingEventModel(DateTime watchPoint, ITermStyleRepository termStyleRepository)
        {
            SearchDate = watchPoint;
            _termStyleRepository = termStyleRepository;
        }

        public string Category
        {
            get { return LanguageReader.GetValue("Event"); }
        }

        public string Title
        {
            get { return LanguageReader.GetValue("BatchInsertEvent"); }
        }

        public DateTime SearchDate { get; set; }

        public TermStyle SelectedEventType { get; set; }

        public IEnumerable<TermStyle> EventTypes { get; set; }

        public IEnumerable<AssignmentType> AssignmentTypes { get; set; }

        public IEnumerable<ICustomAction> OptionalActions
        {
            get { return new ICustomAction[0]; }
        }

        [PersistenceConversation(ConversationEndMode = EndMode.Continue)]
        public void Initial()
        {
            EventTypes = _termStyleRepository.GetEventTypes();
            SelectedEventType = EventTypes.FirstOrDefault();
            AssignmentTypes = _termStyleRepository.GetAssignmentTypesWithInsertRules();
        }

        public Action<Term, TimeBox> GetDefaultAction(TimeSpan length)
        {
            _length = (int)length.TotalMinutes;
            return AddSubEvent;
        }

        public void OnDispatching() { }

        public void TryDispatch(IAgent agent, Func<Term, bool> filter, Action<Term, TimeBox> action, Action<ITerm, string, bool> callback)
        {
            _dispatchedCallback = callback;
            _customValidation = filter;

            var newSubEvent = Term.New(SearchDate, SelectedEventType, _length);
            //newSubEvent.Tag = comments;
            action(newSubEvent, agent.Schedule);
        }

        private void AddSubEvent(Term t, TimeBox tb)
        {
            var lowestTerm = tb.GetOrderedBottoms(t).LastOrDefault();
            if (lowestTerm != null)
            {
                //AssignmentTypes.Cast<ISelectable>().Any(o => o.IsSelected == true & lowestTerm.Style.Equals(o))
                if (_customValidation(lowestTerm))
                {
                    tb.Create(t, (terms, success) => _dispatchedCallback(t, "", success), false);
                    return;
                }
            }
            _dispatchedCallback(t, "BottomTermNotFound", false);
        }

        public void Comment(string content)
        {
            TermAlteringListener.Batching(content);
        }

        public void TearDown()
        {
            EventTypes = null;
            AssignmentTypes = null;
            this.SaftyInvoke<IDisposable>(d => d.Dispose());
        }
    }
}