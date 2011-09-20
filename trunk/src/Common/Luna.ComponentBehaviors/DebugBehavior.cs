using System.Diagnostics;
using Castle.DynamicProxy;
using Luna.Common.Attributes;
using uNhAddIns.ComponentBehaviors;

namespace Luna.ComponentBehaviors
{
    [Behavior(5)]
    public class DebugBehavior : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (invocation.MethodInvocationTarget.IsDefined(typeof(WatchTimeAttribute), true) ||
                invocation.InvocationTarget.GetType().IsDefined(typeof(WatchTimeAttribute), true))
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                invocation.Proceed();
                watch.Stop();
                Debug.Print("{0}.{1}() cost {2} seconds", invocation.TargetType.Name, invocation.Method.Name, watch.Elapsed.TotalSeconds);
            }
            else
            {
                invocation.Proceed();
            }
        }
    }
}
