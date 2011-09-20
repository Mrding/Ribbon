//using Castle.Core.Interceptor;
//using Microsoft.Practices.ServiceLocation;
//using NHibernate;

//namespace Luna.Common.Attributes
//{
//    public class EnableFilterAttribute : BeforeAttribute
//    {
//        private string _filterName;
//        private string[] _parameters;

//        public EnableFilterAttribute(string filterName, params string[] parameters)
//        {
//            _filterName = filterName;
//            _parameters = parameters;
//        }

//        public override void Action(IInvocation invocation)
//        {
//            var factory = ServiceLocator.Current.GetInstance<ISessionFactory>();
//            var currentSession = factory.GetCurrentSession();
//            var filter = currentSession.EnableFilter(_filterName);
//            if (_parameters != null && _parameters.Length > 0)
//            {
//                for (int i = 0; i < _parameters.Length; i++)
//                {
//                    filter.SetParameter(_parameters[i], invocation.Arguments[i]);
//                }
//            }
//        }
//    }
//}
