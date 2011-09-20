using System;
using System.Linq;
using Luna.Core.Extensions;
using Castle.DynamicProxy;
using Luna.Common.Extensions;

namespace Luna.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class HandleExceptionAttribute : MethodCallAttribute, IPredicate<IInvocation, Exception>
    {
        protected HandleExceptionAttribute()
        { }

        public HandleExceptionAttribute(string methodName)
            : base(methodName)
        { }

        public virtual bool Predicate(IInvocation invocation, Exception ex)
        {
            if (invocation.TargetType.GetMethod(_methodName).ReturnType == typeof(bool))
                return base.Func<bool>(invocation, _methodName, GetParameters(invocation, ex));

            base.Action(invocation, _methodName, GetParameters(invocation, ex));

            return true;
        }

        private object[] GetParameters(IInvocation invocation, Exception ex)
        {
            var targetParams = invocation.MethodInvocationTarget.GetParameters();
            var parameters = invocation.TargetType.GetMethod(_methodName).GetParameters();
            var args = invocation.Arguments.Add(ex).ToArray();
            return InvocationExtension.GetArguments(args, targetParams, parameters);
        }
    }
}
