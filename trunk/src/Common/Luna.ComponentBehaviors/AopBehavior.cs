using System;
using System.Linq;
using Castle.DynamicProxy;
using Luna.Common;
using Luna.Common.Attributes;
using Luna.Core.Extensions;
using uNhAddIns.ComponentBehaviors;

namespace Luna.ComponentBehaviors
{
    [Behavior(0, typeof(IAction<IInvocation>), typeof(IPredicate<IInvocation, Exception>))]
    public class AopBehavior : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            var canProceed = PreProceed(invocation);
            try
            {
                if (canProceed)
                    invocation.Proceed();
                else if (invocation.MethodInvocationTarget.ReturnType != typeof(void))
                    invocation.ReturnValue = invocation.MethodInvocationTarget.ReturnType.GetDefaultValue();
            }
            catch (Exception ex)
            {
                bool needThrow = HandleException(invocation, ex);
                if (invocation.MethodInvocationTarget.ReturnType != typeof(void))
                    invocation.ReturnValue = invocation.MethodInvocationTarget.ReturnType.GetDefaultValue();
                if (needThrow)
                    throw;
            }
            PostProceed(invocation);
        }

        protected virtual bool PreProceed(IInvocation invocation)
        {
            if (invocation.MethodInvocationTarget.IsDefined(typeof(BeforeAttribute), true))
            {
                return (from IPredicate<IInvocation> memberInfo in invocation.MethodInvocationTarget.GetCustomAttributes(typeof (BeforeAttribute), true)
                        select memberInfo.Predicate(invocation)).All(canProceed => canProceed);
            }
            return true;
        }

        protected virtual void PostProceed(IInvocation invocation)
        {
            if (invocation.MethodInvocationTarget.IsDefined(typeof(AfterAttribute), true))
            {
                var afterAttributes = invocation.MethodInvocationTarget.GetCustomAttributes(typeof(AfterAttribute), true);
                if (afterAttributes != null && afterAttributes.Length > 0)
                {
                    foreach (IAction<IInvocation> after in afterAttributes)
                    {
                        after.Action(invocation);
                    }
                }
            }
        }

        protected virtual bool HandleException(IInvocation invocation, Exception ex)
        {
            if (invocation.MethodInvocationTarget.IsDefined(typeof(HandleExceptionAttribute), true))
            {
                var exceptionAttributes = invocation.MethodInvocationTarget.GetCustomAttributes(typeof(HandleExceptionAttribute), true);
                //只允许1个HandleExceptionAttribute
                if (exceptionAttributes != null && exceptionAttributes.Length == 1)
                {
                    return exceptionAttributes[0].As<IPredicate<IInvocation, Exception>>().Predicate(invocation, ex);
                }
            }
            return true;
        }
    }
}
