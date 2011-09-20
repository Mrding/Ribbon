//using Castle.Core.Interceptor;
//using Luna.Core.Extensions;
//using Luna.NHibernate;
//using Microsoft.Practices.ServiceLocation;
//using NHibernate;

//namespace Luna.Common.Attributes
//{
//    public class TransactionCompleteAttribute : BeforeAttribute
//    {
//        private readonly bool _runWhenTransctionFail;
//        private ISessionFactory _sessionFactory;

//        public TransactionCompleteAttribute(string methodName)
//            : this(methodName, false)
//        { }

//        public TransactionCompleteAttribute(string methodName, bool runWhenTransctionFail)
//            : base(methodName)
//        {
//            _runWhenTransctionFail = runWhenTransctionFail;
//        }

//        public override void Action(IInvocation invocation)
//        {
//            CurrentSession.Transaction.RegisterSynchronization(new DefaultSynchronization(s =>
//            {
//                if (s && !_runWhenTransctionFail) base.Action(invocation);
//            }));
//        }

//        public ISession CurrentSession
//        {
//            get
//            {
//                if (_sessionFactory.IsNull())
//                    _sessionFactory = ServiceLocator.Current.GetInstance<ISessionFactory>();
//                return _sessionFactory.GetCurrentSession();
//            }
//        }
//    }
//}
