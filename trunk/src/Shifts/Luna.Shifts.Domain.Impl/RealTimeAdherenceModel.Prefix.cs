using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Luna.Common.Constants;
using uNhAddIns.Adapters;
using Luna.Common;

namespace Luna.Shifts.Domain.Impl
{
    public partial class RealTimeAdherenceModel
    {
        private IList<string> _absenceTypes;

        [PersistenceConversation(Exclude = true)]
        public IList<string> GetAbsenceTypes()
        {
            return _absenceTypes;
        }

        [PersistenceConversation(ConversationEndMode = EndMode.Continue)]
        public IDictionary<Guid,IEnumerable> GetAgentAdherenceEvents(DateTime start, DateTime end)
        {
            _absenceTypes = _adherenceEventRepository.GetAbsenceTypes();

            var results = new Dictionary<Guid, IEnumerable>(_employeeIdsCache.Length);
            _adherenceEventRepository.Search(_employeeIdsCache, start, end, (o =>
                {
                    if (!results.ContainsKey(o.EmployeeId))
                        results[o.EmployeeId] = new List<AdherenceEvent>(10);

                    ((List<AdherenceEvent>)results[o.EmployeeId]).Add(o);
                }));
            return results;
        }

        [PersistenceConversation(ConversationEndMode = EndMode.CommitAndContinue)]
        public void AddAdherenceEvents(IEnumerable<AdherenceEvent> itmes)
        {
            var currentAgentId = ApplicationCache.Get<string>(Global.LoggerId);
            foreach (var o in itmes)
            {
                o.Assigner = currentAgentId;
                o.Remark = string.Empty;
                _adherenceEventRepository.MakePersistent(o);
            }
        }

        [PersistenceConversation(ConversationEndMode = EndMode.CommitAndContinue)]
        public void RemoveAdherenceEvents(IEnumerable<AdherenceEvent> itmes)
        {
            foreach (var o in itmes)
            {
                _adherenceEventRepository.MakeTransient(o);
            }
        }

        [PersistenceConversation(ConversationEndMode = EndMode.CommitAndContinue)]
        public void AlterAdherenceEvents()
        {
        }
    }
}
