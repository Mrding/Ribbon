using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Luna.Core.Extensions
{
    public static class TypeExtension
    {
        public static MethodInfo AsMethodInfo(this string methodName, Type targetType)
        {
            if (!methodName.IsNullOrEmpty())
            {
                var methodInfo = targetType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                if (methodInfo == null)
                    throw new Exception(string.Format("Method '{0}' could not be found on '{1}'.", methodName, targetType.FullName));
                return methodInfo;
            }
            return null;
        }

     

        public static TR SaftyGetProperty<TR, T>(this T obj, Func<T, TR> func) where T : class
        {
            if (func != null && obj !=null)
                return func(obj);
            return default(TR);
        }

        public static TR SaftyGetProperty<TR, T>(this object obj, Func<T, TR> func)
        {
            if (obj is T && func != null)
                return func((T)obj);
            return default(TR);
        }

        public static TR SaftyGetProperty<TR, T>(this object obj, Func<T, TR> func, Func<TR> defaultValue)
        {
            if (obj is T && func != null)
                return func((T)obj);
            return defaultValue == null ? default(TR) : defaultValue();
        }

        public static void SaftyInvoke<T>(this object obj, Action<T> action)
        {
            if (obj is T && action != null)
                action.Invoke((T)obj);
        }

        public static void SaftyInvoke<T>(this T obj, Action<T> action) where T : class
        {
            if (obj != default(T) && action != null)
                action.Invoke(obj);
        }

        public static bool If<T>(this object obj, Func<T, bool> func)
        {
            if (obj is T && func != null)
                return func((T)obj);
            return false;
        }

        public static bool? If<T>(this T obj, Func<T, bool> func)
        {
            if (ReferenceEquals(obj,default(T)) || func == null) return false;
            
            return func(obj);
        }

        /// <summary>
        /// Gets the member info represented by an expression.
        /// </summary>
        /// <param name="expression">The member expression.</param>
        /// <returns>The member info represeted by the expression.</returns>
        public static MemberInfo GetMemberInfo(this Expression expression)
        {
            var lambda = (LambdaExpression)expression;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else memberExpression = (MemberExpression)lambda.Body;

            return memberExpression.Member;
        }

        public static object GetValue(this Expression expression)
        {
            var lambda = (LambdaExpression)expression;
            return lambda.Compile().DynamicInvoke();
        }

        public static bool Is<T>(this object obj)
        {
            return obj is T;
        }


        public static bool IsNot<T>(this object obj)
        {
            return !(obj is T);
        }

        public static object GetDefaultValue(this Type type)
        {
            var typeCode = Type.GetTypeCode(type);
            return GetDefualtValue(typeCode);
        }

        public static object GetDefualtValue(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return default(Boolean);
                case TypeCode.Byte:
                    return default(Byte);
                case TypeCode.Char:
                    return default(Char);
                case TypeCode.DateTime:
                    return default(DateTime);
                case TypeCode.DBNull:
                    return default(DBNull);
                case TypeCode.Decimal:
                    return default(Decimal);
                case TypeCode.Double:
                    return default(Double);
                case TypeCode.Empty:
                    return null;
                case TypeCode.Int16:
                    return default(Int16);
                case TypeCode.Int32:
                    return default(Int32);
                case TypeCode.Int64:
                    return default(Int64);
                case TypeCode.Object:
                    return default(Object);
                case TypeCode.SByte:
                    return default(SByte);
                case TypeCode.Single:
                    return default(Single);
                case TypeCode.String:
                    return default(String);
                case TypeCode.UInt16:
                    return default(UInt16);
                case TypeCode.UInt32:
                    return default(UInt32);
                case TypeCode.UInt64:
                    return default(UInt64);
                default:
                    return null;
            }
        }
    }
}
