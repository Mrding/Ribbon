using System.Linq.Expressions;
using System.Reflection;
using Phydeaux.Utilities;

namespace Luna.Core.Extensions
{
    public static class ReflectionExtension
    {
        public static void FastInvoke(this MethodInfo methodInfo, object target)
        {
            Dynamic<object>.Instance.Procedure.Explicit.CreateDelegate(methodInfo)(target);
        }

        public static void FastInvoke<TArg1>(this MethodInfo methodInfo, object target, TArg1 arg1)
        {
            Dynamic<object>.Instance.Procedure.Explicit<TArg1>.CreateDelegate(methodInfo)(target, arg1);
        }

        public static void FastInvoke<TArg1, TArg2>(this MethodInfo methodInfo, object target, TArg1 arg1, TArg2 arg2)
        {
            Dynamic<object>.Instance.Procedure.Explicit<TArg1, TArg2>.CreateDelegate(methodInfo)(target, arg1, arg2);
        }

        public static void FastInvoke<TArg1, TArg2, TArg3>(this MethodInfo methodInfo, object target, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            Dynamic<object>.Instance.Procedure.Explicit<TArg1, TArg2, TArg3>.CreateDelegate(methodInfo)(target, arg1, arg2, arg3);
        }

        public static void FastInvoke<TArg1, TArg2, TArg3, TArg4>(this MethodInfo methodInfo, object target, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            Dynamic<object>.Instance.Procedure.Explicit<TArg1, TArg2, TArg3, TArg4>.CreateDelegate(methodInfo)(target, arg1, arg2, arg3, arg4);
        }

        public static void FastInvoke<TArg1, TArg2, TArg3, TArg4, TArg5>(this MethodInfo methodInfo, object target, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
        {
            Dynamic<object>.Instance.Procedure.Explicit<TArg1, TArg2, TArg3, TArg4, TArg5>.CreateDelegate(methodInfo)(target, arg1, arg2, arg3, arg4, arg5);
        }

        public static TRet FastInvoke<TRet>(this MethodInfo methodInfo, object target)
        {
            return Dynamic<object>.Instance.Function<TRet>.Explicit.CreateDelegate(methodInfo)(target);
        }

        public static TRet FastInvoke<TRet, TArg1>(this MethodInfo methodInfo, object target, TArg1 arg1)
        {
            return Dynamic<object>.Instance.Function<TRet>.Explicit<TArg1>.CreateDelegate(methodInfo)(target, arg1);
        }

        public static TRet FastInvoke<TRet, TArg1, TArg2>(this MethodInfo methodInfo, object target, TArg1 arg1, TArg2 arg2)
        {
            return Dynamic<object>.Instance.Function<TRet>.Explicit<TArg1, TArg2>.CreateDelegate(methodInfo)(target, arg1, arg2);
        }

        public static TRet FastInvoke<TRet, TArg1, TArg2, TArg3>(this MethodInfo methodInfo, object target, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            return Dynamic<object>.Instance.Function<TRet>.Explicit<TArg1, TArg2, TArg3>.CreateDelegate(methodInfo)(target, arg1, arg2, arg3);
        }

        public static TRet FastInvoke<TRet, TArg1, TArg2, TArg3, TArg4>(this MethodInfo methodInfo, object target, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            return Dynamic<object>.Instance.Function<TRet>.Explicit<TArg1, TArg2, TArg3, TArg4>.CreateDelegate(methodInfo)(target, arg1, arg2, arg3, arg4);
        }

        public static TRet FastInvoke<TRet, TArg1, TArg2, TArg3, TArg4, Targ5>(this MethodInfo methodInfo, object target, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, Targ5 arg5)
        {
            return Dynamic<object>.Instance.Function<TRet>.Explicit<TArg1, TArg2, TArg3, TArg4, Targ5>.CreateDelegate(methodInfo)(target, arg1, arg2, arg3, arg4, arg5);
        }

        public static TRet FastGetValue<TRet>(this PropertyInfo propertyInfo, object target)
        {
            return Dynamic<object>.Instance.Property<TRet>.Explicit.Getter.CreateDelegate(propertyInfo)(target);
        }

        public static TRet FastGetValue<TRet>(this FieldInfo fieldInfo, object target)
        {
            return Dynamic<object>.Instance.Field<TRet>.Getter.CreateDelegate(fieldInfo)(target);
        }

        public static TRet GetValue<TRet>(this object obj, Expression<Func<TRet>> expression)
        {
            var body = expression.Body;
            var memeber = body as MemberExpression;
            if (memeber != null)
            {
                var name = memeber.Member.Name;
                if (memeber.Member is PropertyInfo)
                    return obj.GetType().GetProperty(name).FastGetValue<TRet>(obj);
                else
                    return obj.GetType().GetField(name).FastGetValue<TRet>(obj);
            }
            else
            {
                var method = body as MethodCallExpression;
                if (method.Method.ReturnType == typeof(void))
                    return default(TRet);
                else
                    return obj.GetType().GetMethod(method.Method.Name).FastInvoke<TRet>(obj);
            }
        }

        public static object GetValue(this object obj, string parameter)
        {
            return GetValueByIndex(obj, parameter, parameter.Split('.').Length);
        }

        public static object GetValueByIndex(this object obj, string parameter, int index)
        {
            if (parameter.IsNullOrEmpty())
                return null;
            if (!parameter.Contains("."))
            {
                //For example. "GetType()"
                if (parameter.Contains("()"))
                {
                    //"GetType"
                    var methodName = parameter.Substring(0, parameter.Length - 2);
                    return GetValueByMethodName(obj, methodName);
                }
                //"Name"
                else
                {
                    return GetValueByPropertyName(obj, parameter);
                }
            }
            else
            {
                var paramArray = parameter.Split('.');
                if (index > paramArray.Length)
                    index = paramArray.Length;
                var target = obj;
                for (int i = 0; i < index; i++)
                {
                    target = target.GetValueByIndex(paramArray[i], i);
                }
                return target;
            }
        }

        public static void SetValue(this object obj, string parameter, object value)
        {
            if (parameter.IsNullOrEmpty())
                return;
            if (!parameter.Contains("."))
            {
                //"Name"
                SetValueByPropertyName(obj, parameter, value);
            }
            else
            {
                var paramArray = parameter.Split('.');
                var target = obj;
                for (int i = 0; i < paramArray.Length - 1; i++)
                {
                    target = target.GetValue(paramArray[i]);
                }
                target.SetValue(paramArray[paramArray.Length - 1], value);
            }
        }

        internal static object GetValueByMethodName(object obj, string methodName)
        {
            var methodInfo = obj.GetType().GetMethod(methodName);
            return methodInfo.FastInvoke<object>(obj);
        }

        internal static object GetValueByPropertyName(object obj, string propertyName)
        {
            var propertyInfo = GetPropertyInfo(obj, propertyName);
            return propertyInfo.FastGetValue<object>(obj);
        }

        internal static PropertyInfo GetPropertyInfo(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName);
        }

        internal static void SetValueByPropertyName(object obj, string propertyName, object value)
        {
            var propertyInfo = GetPropertyInfo(obj, propertyName);
            propertyInfo.SetValue(obj, value, null);
        }
    }
}
