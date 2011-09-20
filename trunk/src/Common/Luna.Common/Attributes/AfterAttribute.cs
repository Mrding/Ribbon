namespace Luna.Common.Attributes
{
    /// <summary>
    /// 定义接受动作
    /// </summary>
    public class AfterAttribute : MethodCallAttribute
    {
        protected AfterAttribute()
        { }

        public AfterAttribute(string methodName)
            : base(methodName)
        { }
    }
}
