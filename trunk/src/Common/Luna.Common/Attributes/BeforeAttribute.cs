using Castle.DynamicProxy;

namespace Luna.Common.Attributes
{
    /// <summary>
    /// 定义开始动作
    /// </summary>
    public class BeforeAttribute : MethodCallAttribute, IPredicate<IInvocation>
    {
        protected BeforeAttribute()
        { }

        public BeforeAttribute(string methodName)
            : base(methodName)
        { }

        public virtual bool Predicate(IInvocation invocation)
        {
            base.Action(invocation);
            return true;
        }
    }
}
