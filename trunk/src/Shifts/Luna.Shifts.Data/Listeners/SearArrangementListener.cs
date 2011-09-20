using Luna.Data.Event;
using Luna.Shifts.Domain;
using NHibernate.Event;

namespace Luna.Shifts.Data.Listeners
{
    public class SearArrangementListener : IPreDeleteEventListener
    {
        public bool OnPreDelete(PreDeleteEvent @event)
        {
            var seatArrangement = default(SeatArrangement);

            if (@event.TryCatchEntity(ref seatArrangement))
            {
                //seatArrangement.Cancel(null);
                //_occupationRepository.DeleteSeatArrangements(seatArrangement..Id);
           }
            return false;
        }

        //private static void Set(IEntityPersister persister, object[] state, string propertyName, object value)
        //{
        //    var index = Array.IndexOf(persister.PropertyNames, propertyName);
        //    if (index == -1)
        //        return;
        //    state[index] = value;
        //}
    }
}
