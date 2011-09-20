using System.Reflection;
using Castle.DynamicProxy;
using Luna.Common.Interfaces;
using Luna.Core.Extensions;
using Microsoft.Practices.ServiceLocation;
using uNhAddIns.ComponentBehaviors;

namespace Luna.ComponentBehaviors
{
    [Behavior(4, typeof(IExpandObject))]
    public class AgentStatusBehavior : IInterceptor
    {
        private static PropertyInfo _agentAcdidPropertyInfo;

        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.Name.IsEqual("get_ExpandData"))
            {
                if (_agentAcdidPropertyInfo == null)
                    _agentAcdidPropertyInfo = invocation.Proxy.GetType().GetProperty("AgentAcdid");

                var arg = _agentAcdidPropertyInfo.FastGetValue<object>(invocation.Proxy);
                var temp = ServiceLocator.Current.GetInstance<IExpandMethod>("AgentStatus").GetExpandObject(arg);
                invocation.ReturnValue = temp;
            }
            else
            {
                invocation.Proceed();
            }
        }
    }
}
