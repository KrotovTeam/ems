using System;

namespace Common.Helpers
{
    public static class MathHelper
    {
        public static double Fraction(double value)
        {
            var absValue = Math.Abs(value);
            return absValue - Math.Truncate(absValue);
        }
    }
}
