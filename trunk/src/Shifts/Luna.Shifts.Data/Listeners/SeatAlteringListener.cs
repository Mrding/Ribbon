using Luna.Data.Event;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain;
using Microsoft.Practices.ServiceLocation;
using NHibernate.Event;

namespace Luna.Shifts.Data.Listeners
{
    public class SeatAlteringListener : IPostDeleteEventListener, IPostInsertEventListener
    {
        private ISeatBoxRepository _seatBoxRepository;

        #region IPostDeleteEventListener Members

        public void OnPostDelete(PostDeleteEvent @event)
        {
            var seat = default(Seat);

            if (@event.TryCatchEntity(ref seat))
            {
                if(_seatBoxRepository==null)
                    _seatBoxRepository = ServiceLocator.Current.GetInstance<ISeatBoxRepository>();
                // = @event.Session.SessionFactory.OpenSession();
                _seatBoxRepository.MakeTransient(seat);
                //@event.Session.SessionFactory.GetCurrentSession();
            }
        }

        #endregion

        #region IPostInsertEventListener Members

        public void OnPostInsert(PostInsertEvent @event)
        {
            var seat = default(Seat);

            if (@event.TryCatchEntity(ref seat))
            {
                if (_seatBoxRepository == null)
                    _seatBoxRepository = ServiceLocator.Current.GetInstance<ISeatBoxRepository>();

                _seatBoxRepository.MakePersistent(new SeatBox(seat));
            }
        }

        #endregion
    }
}
