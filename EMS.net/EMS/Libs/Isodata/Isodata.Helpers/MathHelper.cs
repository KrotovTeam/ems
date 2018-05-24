using System;

namespace Isodata.Helpers
{
    public class MathHelper
    {
        /// <summary>
        /// Рассчитывание Евклидового расстояния между точками
        /// </summary>
        /// <param name="pointA">Значения точки А</param>
        /// <param name="pointB">Значения точки B</param>
        /// <returns></returns>
        public static double EuclideanDistance(double[] pointA, double[] pointB)
        {
            var length = pointA.Length;
            var result = 0.0;
            for (var i = 0; i < length; i++)
            {
                result += Math.Pow(pointA[i] - pointB[i], 2);
            }
            return Math.Sqrt(result);
        }
    }
}
