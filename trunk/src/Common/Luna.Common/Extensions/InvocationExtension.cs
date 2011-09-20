using System;
using System.Collections.Generic;
using System.Reflection;
using Luna.Core.Extensions;
using Castle.DynamicProxy;

namespace Luna.Common.Extensions
{
    public static class InvocationExtension
    {
        public static void InvokeMethod(this IInvocation invocation, string methodName, params object[] args)
        {
            if (invocation.IsNull())
                throw new ArgumentNullException("invocation");
            if (methodName.IsNullOrEmpty())
                throw new ArgumentNullException("methodName");
            if (invocation.InvocationTarget.GetType().GetMethod(methodName).ReturnType != typeof(void))
                throw new ArgumentException("The method has a return value, so please use \"InvokeMethod<TRet>\" instead.");

            var targetParams = invocation.MethodInvocationTarget.GetParameters();
            var methodInfo = invocation.InvocationTarget.GetType().GetMethod(methodName);
            var parameters = methodInfo.GetParameters();

            var arguments = GetArguments(args, targetParams, parameters);
            Invoke(methodInfo, invocation.InvocationTarget, arguments);
        }

        public static TRet InvokeMethod<TRet>(this IInvocation invocation, string methodName, params object[] args)
        {
            if (invocation.IsNull())
                throw new ArgumentNullException("invocation");
            if (methodName.IsNullOrEmpty())
                throw new ArgumentNullException("methodName");
            if (invocation.InvocationTarget.GetType().GetMethod(methodName).ReturnType == typeof(void))
                throw new ArgumentException("The method dose not have a return value, so please use \"InvokeMethod\" instead.");

            var targetParams = invocation.MethodInvocationTarget.GetParameters();
            var methodInfo = invocation.InvocationTarget.GetType().GetMethod(methodName);
            var parameters = methodInfo.GetParameters();

            var arguments = GetArguments(args, targetParams, parameters);
            return Invoke<TRet>(methodInfo, invocation.InvocationTarget, arguments);
        }

        private static void Invoke(MethodInfo methodInfo, object target, params object[] args)
        {
            if (args.IsNull())
            {
                methodInfo.FastInvoke(target);
            }
            else
            {
                switch (args.Length)
                {
                    case 0:
                        methodInfo.FastInvoke(target);
                        break;
                    case 1:
                        methodInfo.FastInvoke(target, args[0]);
                        break;
                    case 2:
                        methodInfo.FastInvoke(target, args[0], args[1]);
                        break;
                    case 3:
                        methodInfo.FastInvoke(target, args[0], args[1], args[2]);
                        break;
                    case 4:
                        methodInfo.FastInvoke(target, args[0], args[1], args[2], args[3]);
                        break;
                    case 5:
                        methodInfo.FastInvoke(target, args[0], args[1], args[2], args[3], args[4]);
                        break;
                    default:
                        methodInfo.FastInvoke(target);
                        break;
                }
            }
        }

        private static TRet Invoke<TRet>(MethodInfo methodInfo, object target, params object[] args)
        {
            if (args.IsNull())
            {
                return methodInfo.FastInvoke<TRet>(target);
            }
            else
            {
                switch (args.Length)
                {
                    case 0:
                        return methodInfo.FastInvoke<TRet>(target);
                    case 1:
                        return methodInfo.FastInvoke<TRet, object>(target, args[0]);
                    case 2:
                        return methodInfo.FastInvoke<TRet, object, object>(target, args[0], args[1]);
                    case 3:
                        return methodInfo.FastInvoke<TRet, object, object, object>(target, args[0], args[1], args[2]);
                    case 4:
                        return methodInfo.FastInvoke<TRet, object, object, object, object>(target, args[0], args[1], args[2], args[3]);
                    case 5:
                        return methodInfo.FastInvoke<TRet, object, object, object, object, object>(target, args[0], args[1], args[2], args[3], args[4]);
                    default:
                        return methodInfo.FastInvoke<TRet>(target);
                }
            }
        }

        /// <summary>
        /// 组装AOP调用方法的参数
        /// </summary>
        /// <param name="args">全部参数</param>
        /// <param name="targetParams">原始方法的参数</param>
        /// <param name="parameters">需要调用AOP方法的参数</param>
        /// <returns></returns>
        public static object[] GetArguments(object[] args, ParameterInfo[] targetParams, ParameterInfo[] parameters)
        {
            List<object> arguments = new List<object>(args.Length);
            for (int j = 0; j < parameters.Length; j++)
            {
                for (int i = j; i < targetParams.Length; i++)
                {
                    if (targetParams[i].ParameterType.Equals(parameters[j].ParameterType)
                        && targetParams[i].Name.IsEqual(parameters[j].Name))
                    {
                        arguments.Add(args[i]);
                        break;
                    }
                }
            }

            if (arguments.Count < parameters.Length)
                arguments.AddRange(args.GetFromEnd(parameters.Length - arguments.Count));
            return arguments.ToArray();
        }
    }
}
