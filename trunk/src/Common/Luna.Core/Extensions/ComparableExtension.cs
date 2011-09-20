using System;

namespace Luna.Core.Extensions
{
    public static class ComparableExtension
    {
        public static bool LessThan<T>(this T left, T right)
            where T : IComparable<T>
        {
            return left.CompareTo(right) < 0;
        }

        public static bool GreaterThan<T>(this T left, T right)
            where T : IComparable<T>
        {
            return left.CompareTo(right) > 0;
        }

        public static bool LessThanEqual<T>(this T left, T right)
            where T : IComparable<T>
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool GreaterThanEqual<T>(this T left, T right)
            where T : IComparable<T>
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool EqualWith<T>(this T left, T right)
            where T : IComparable<T>
        {
            return left.CompareTo(right) == 0;
        }
    }
}
