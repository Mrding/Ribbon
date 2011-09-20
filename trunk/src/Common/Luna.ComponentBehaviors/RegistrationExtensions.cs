using Castle.Core;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Luna.Common;
using Luna.Common.Attributes;
using Luna.Common.Interfaces;


namespace Luna.ComponentBehaviors
{
    public static class RegistrationExtensions
    {
        /// <summary>
        /// 增加可被选中行为
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="componentRegistration">The component registration.</param>
        /// <returns></returns>
        public static ComponentRegistration<T> AddSelectableBehavior<T>(this ComponentRegistration<T> componentRegistration)
        {
            return componentRegistration.Proxy.AdditionalInterfaces(typeof(ISelectable)).Interceptors(new InterceptorReference(typeof(SelectableBehavior))).Last;
            //return componentRegistration.Proxy.AdditionalInterfaces(typeof(ISelectable));

        }

        /// <summary>
        ///增加索引行为
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="componentRegistration">The component registration.</param>
        /// <returns></returns>
        public static ComponentRegistration<T> AddIndexableBehavior<T>(this ComponentRegistration<T> componentRegistration)
        {
            return componentRegistration.Proxy.AdditionalInterfaces(typeof(IIndexable)).Interceptors(new InterceptorReference(typeof(IndexableBehavior))).Last;
        }

        /// <summary>
        /// 增加可编辑行为
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="componentRegistration">The component registration.</param>
        /// <returns></returns>
        public static ComponentRegistration<T> AddEditingBehavior<T>(this ComponentRegistration<T> componentRegistration)
        {
            return componentRegistration.Proxy.AdditionalInterfaces(typeof(IEditing)).Interceptors(new InterceptorReference(typeof(EditingBehavior))).Anywhere;
        }

        /// <summary>
        /// 增加默认AOP行为
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="componentRegistration">The component registration.</param>
        /// <returns></returns>
        public static ComponentRegistration<T> AddAopBehavior<T>(this ComponentRegistration<T> componentRegistration)
        {
            return componentRegistration.Proxy.AdditionalInterfaces(typeof(IAction<IInvocation>)).Interceptors(new InterceptorReference(typeof(AopBehavior))).Anywhere;
        }

        public static ComponentRegistration<object> TryAddAopBehavior(this ComponentRegistration<object> componentRegistration)
        {
            if (componentRegistration.Implementation.IsDefined(typeof(AopAttribute), true))
                return componentRegistration.AddAopBehavior();
            return componentRegistration;
        }

        /// <summary>
        /// 给AgentStatus增加Employee属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="componentRegistration">The component registration.</param>
        /// <returns></returns>
        public static ComponentRegistration<T> AddEmployeePropertyBehavior<T>(this ComponentRegistration<T> componentRegistration)
        {
            return componentRegistration.Proxy.AdditionalInterfaces(typeof(IExpandObject)).Interceptors(new InterceptorReference(typeof(AgentStatusBehavior))).Anywhere;
        }

        /// <summary>
        /// 增加Debug调试行为
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="componentRegistration">The component registration.</param>
        /// <returns></returns>
        public static ComponentRegistration<T> AddDebugBehavior<T>(this ComponentRegistration<T> componentRegistration)
        {
            return componentRegistration.Interceptors(new InterceptorReference(typeof(DebugBehavior))).Anywhere;
        }
    }
}
