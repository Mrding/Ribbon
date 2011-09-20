using System;

namespace Luna.Core.Extensions
{
    public static class CommonExtension
    {
        public static bool[] ToBooleanArray(this int integer)
        {
            var charArray = Convert.ToString(integer, 2).ToCharArray();
            var booleanArray = new bool[charArray.Length];
            for (int i = 0; i < charArray.Length; i++)
            {
                booleanArray[i] = charArray[i] == '1';
            }
            return booleanArray;
        }

        public static bool[] ToBooleanArray(this int integer, int length)
        {
            var charArray = Convert.ToString(integer, 2).ToCharArray();
            var booleanArray = new bool[length];
            for (int i = charArray.Length - 1, j = 0; j < length; i--, j++)
            {
                booleanArray[j] = j >= charArray.Length ? false : charArray[i] == '1';
            }
            return booleanArray;
        }

        public static int ToInteger(this bool[] booleanArray)
        {
            var length = booleanArray.Length;
            var charArray = new char[length];
            for (int i = 0; i < length; i++)
            {
                charArray[length - i - 1] = booleanArray[i] ? '1' : '0';
            }
            return Convert.ToInt32(new string(charArray), 2);
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return String.IsNullOrEmpty(str);
        }

        public static bool IsNotNullOrEmpty(this string str)
        {
            return !String.IsNullOrEmpty(str);
        }

        public static bool IsEqual(this string left, string right)
        {
            return String.CompareOrdinal(left, right) == 0;
        }

        public static bool IsNotEqual(this string left, string right)
        {
            return String.CompareOrdinal(left, right) != 0;
        }

        public static bool IsNull(this object obj)
        {
            return obj == null;
        }

        public static bool IsNotNull(this object obj)
        {
            return !obj.IsNull();
        }

        public static T As<T>(this object obj)
        {
            return obj is T ? (T)obj : default(T);
        }

        public static T Do<T>(this T target, Action<T> action)
        {
            action(target);
            return target;
        }

        public static R Let<T, R>(this T target, Func<T, R> function)
        {
            return function(target);
        }

        public static Lazy<T> AsLazy<T>(this T t)
           where T : class
        {
            return new Lazy<T>(() => t, false);
        }

        public static T Self<T>(this T obj, Action<T> method)
        {
           
            method(obj);
            return obj;
        }

        public static T Self<T>(this object obj, Action<T> method)
        {
            if (obj is T)
                method((T)obj);
            else
                return default(T);
            return (T)obj;
        }

    }
}
