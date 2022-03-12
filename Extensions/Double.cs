using System;

namespace FEM
{
    public static class Double
    {
        public static float ToFloat(this double d)
        {
            float.TryParse(d.ToString(), out var output);
            return output;
        }
        /// <summary>
        /// Возведение в степень
        /// </summary>
        /// <param name="d"></param>
        /// <param name="level">Степень</param>
        /// <returns></returns>
        public static double Pow(this double d, double level)
        {
            return Math.Pow(d, level);
        }
        public static double Abs(this double d)
        {
            return Math.Abs(d);
        }

        /// <summary>
        /// Округление
        /// </summary>
        /// <param name="d"></param>
        /// <param name="det"></param>
        /// <returns></returns>
        public static double Round(this double d, int det)
        {
            var n = Math.Round(d, det);

            return n;
        }

      
    }
}
