using Luna.Shifts.Domain.Model;
using uNhAddIns.Adapters;

namespace Luna.Shifts.Domain.Impl
{
    [PersistenceConversational(MethodsIncludeMode = MethodsIncludeMode.Implicit)]
    public class BrowseSeatOccupationModel : IBrowseSeatOccupationModel
    {
        //private readonly IOccupationRepository _occupationRepository;
        //public BrowseSeatOccupationModel(IOccupationRepository occupationRepository)
        //{
        //    _occupationRepository = occupationRepository;
        //}

        //[PersistenceConversation(ConversationEndMode = EndMode.End)]
        //public IList<SeatArrangement> GetTermSeatSeatArrangements(ISeatingTerm seatingTerm)
        //{
        //   return _occupationRepository.GetSeatArrangements(seatingTerm);
        //}
    }
}
