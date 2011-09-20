using NHibernate.Event;

namespace Luna.Data.Event
{
    public static class ListenerExt
    {
        public static bool TryCatchEntity<T>(this PostUpdateEvent @event, ref T entity)
        {
            if (@event.Entity is T)
            {
                entity = (T)@event.Entity;
                return true;
            }
            return false;
        }

        public static bool TryCatchEntity<T>(this PostDeleteEvent @event, ref T entity)
        {
            if (@event.Entity is T)
            {
                entity = (T)@event.Entity;
                return true;
            }
            return false;
        }

        public static bool TryCatchEntity<T>(this PostInsertEvent @event, ref T entity)
        {
            if (@event.Entity is T)
            {
                entity = (T)@event.Entity;
                return true;
            }
            return false;
        }

        public static bool TryCatchEntity<T>(this PreDeleteEvent @event, ref T entity)
        {
            if (@event.Entity is T)
            {
                entity = (T)@event.Entity;
                return true;
            }
            return false;
        }

        public static bool TryCatchEntity<T>(this PreInsertEvent @event, ref T entity)
        {
            if (@event.Entity is T)
            {
                entity = (T)@event.Entity;
                return true;
            }
            return false;
        }

        public static bool TryCatchEntity<T>(this PreUpdateEvent @event, ref T entity)
        {
            if (@event.Entity is T)
            {
                entity = (T)@event.Entity;
                return true;
            }
            return false;
        }
    }
}