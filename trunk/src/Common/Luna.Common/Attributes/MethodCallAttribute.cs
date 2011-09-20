using System;
using Luna.Common.Extensions;
using Castle.DynamicProxy;

namespace Luna.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class MethodCallAttribute : Attribute, IAction<IInvocation>
    {
        protected readonly string _methodName;

        protected MethodCallAttribute()
        { }

        public MethodCallAttribute(string methodName)
        {
            _methodName = methodName;
        }

        public virtual void Action(IInvocation invocation)
        {
            Action(invocation, _methodName, invocation.Arguments);
        }

        protected virtual void Action(IInvocation invocation, string methodName, params object[] args)
        {
            invocation.InvokeMethod(methodName, args);
        }

        public virtual TResult Func<TResult>(IInvocation invocation)
        {
            return Func<TResult>(invocation, _methodName, invocation.Arguments);
        }

        protected virtual TResult Func<TResult>(IInvocation invocation, string methodName, params object[] args)
        {
            return invocation.InvokeMethod<TResult>(methodName, args);
        }
    }
}
