//using Castle.Core.Interceptor;
//using Microsoft.Practices.ServiceLocation;
//using NHibernate;

//namespace Luna.Common.Attributes
//{
//    public class DisableFilterAttribute : AfterAttribute
//    {
//        private string _filterName;

//        public DisableFilterAttribute(string filterName)
//        {
//            _filterName = filterName;
//        }

//        public override void Action(IInvocation invocation)
//        {
//            var factory = ServiceLocator.Current.GetInstance<ISessionFactory>();
//            var currentSession = factory.GetCurrentSession();
//            currentSession.DisableFilter(_filterName);
//        }
//    }
//}
