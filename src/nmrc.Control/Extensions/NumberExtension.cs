using System;

namespace Nmrc.Control.Extensions
{
    public static class NumberExtension
    {
        public static bool In<T>(this T number, T left, T right) where T : struct,
            IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
        {
            return number.CompareTo(left) >= 0 && number.CompareTo(right) <= 0;
        }
    }
}
