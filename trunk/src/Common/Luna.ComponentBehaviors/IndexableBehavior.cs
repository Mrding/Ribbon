using Castle.DynamicProxy;
using Luna.Common;
using Luna.Core.Extensions;
using uNhAddIns.ComponentBehaviors;

namespace Luna.ComponentBehaviors
{
    [Behavior(2, typeof(IIndexable))]
    public class IndexableBehavior : IInterceptor
    {
        private int _index;

        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.DeclaringType.Equals(typeof(IIndexable)))
            {
                if (invocation.Method.Name.IsEqual("get_Index"))
                {
                    invocation.ReturnValue = _index;
                }
                else if (invocation.Method.Name.IsEqual("set_Index"))
                {
                    _index = (int)invocation.Arguments[0];
                }
            }
            else
            {
                invocation.Proceed();
            }
        }
    }
}