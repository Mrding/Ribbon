using Castle.DynamicProxy;
using Luna.Common;
using uNhAddIns.ComponentBehaviors;

namespace Luna.ComponentBehaviors
{
    [Behavior(3, typeof(IEditingObject))]
    public class EditingBehavior : IInterceptor
    {
        private bool _isNew;
        private bool _isEditing;
        private bool _isEnablePropertyChanged = true;

        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.DeclaringType.Equals(typeof(IEditing)))
            {
                switch (invocation.Method.Name)
                {
                    case "get_IsNew":
                        invocation.ReturnValue = _isNew;
                        break;
                    case "set_IsNew":
                        _isNew = (bool)invocation.Arguments[0];
                        break;
                    case "get_IsEditing":
                        invocation.ReturnValue = _isEditing;
                        break;
                    case "set_IsEditing":
                        _isEditing = (bool)invocation.Arguments[0];
                        break;
                    case "get_IsEnablePropertyChanged":
                        invocation.ReturnValue = _isEnablePropertyChanged;
                        break;
                    case "set_IsEnablePropertyChanged":
                        _isEnablePropertyChanged = (bool)invocation.Arguments[0];
                        break;
                }
            }
            else
            {
                invocation.Proceed();
            }
        }
    }
}
