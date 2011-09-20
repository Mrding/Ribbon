using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain.Model;
using uNhAddIns.Adapters;

namespace Luna.Shifts.Domain.Impl
{
    [PersistenceConversational(MethodsIncludeMode = MethodsIncludeMode.Explicit)]
    public class SeatEventManagerModel : ISeatEventManagerModel
    {
        private readonly ISeatEventRepository _seatEventRepository;
        private readonly ISeatBoxRepository _seatBoxRepository;

        public SeatEventManagerModel(ISeatEventRepository seatEventRepository, ISeatBoxRepository seatBoxRepository)
        {
            _seatEventRepository = seatEventRepository;
            _seatBoxRepository = seatBoxRepository;
        }

        [PersistenceConversation(ConversationEndMode = EndMode.End)]
        public IList<SeatEvent> Searh(Entity site, string category, DateRange range, out IList seatEventGroup)
        {

            var results = _seatEventRepository.Searh(site, category, range);

            seatEventGroup = (from s in results
                              group s by new { s.Start, Length = s.End.Subtract(s.Start), s.Description, s.Category } into g
                              select new { Header = g.Key, Details = g.ToList() }).ToList();

            return results;
        }
    }
}
