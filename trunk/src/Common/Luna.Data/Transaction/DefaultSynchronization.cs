using System;
using NHibernate.Transaction;

namespace Luna.Data.Transaction
{
    public class DefaultSynchronization : ISynchronization
    {
        private readonly Action _beforeCompletion;
        private readonly Action<bool> _afterCompletion;

        public DefaultSynchronization(Action<bool> after)
            : this(default(Action), after)
        { }

        public DefaultSynchronization(Action before, Action<bool> after)
        {
            _beforeCompletion = before;
            _afterCompletion = after;
        }

        public void AfterCompletion(bool success)
        {
            if (_afterCompletion != default(Action<bool>))
                _afterCompletion(success);
        }

        public void BeforeCompletion()
        {
            if (_beforeCompletion != default(Action))
                _beforeCompletion();
        }
    }
}
