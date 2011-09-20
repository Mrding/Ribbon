using Castle.DynamicProxy;
using Luna.Common;
using Luna.Core.Extensions;
using uNhAddIns.ComponentBehaviors;

namespace Luna.ComponentBehaviors
{
    [Behavior(1, typeof(ISelectable))]
    public class SelectableBehavior : IInterceptor
    {
        private bool? _isSelected = false;

        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.DeclaringType.Equals(typeof(ISelectable)))
            {
                if (invocation.Method.Name.IsEqual("get_IsSelected"))
                {
                    invocation.ReturnValue = _isSelected;
                }
                else if (invocation.Method.Name.IsEqual("set_IsSelected"))
                {
                    _isSelected = (bool?)invocation.Arguments[0];
                }
            }
            else
            {
                invocation.Proceed();
            }
        }
    }
}