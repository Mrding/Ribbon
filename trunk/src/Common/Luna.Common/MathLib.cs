using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luna.Common
{
    public static class MathLib
    {
        public static double StandardDeviation(double[] num)
        {
            double sum = 0.0, sumOfSqrs = 0.0;
            foreach (var t in num)
            {
                sum += t;
                sumOfSqrs += Math.Pow(t, 2);
            }
            var topSum = (num.Length * sumOfSqrs) - (Math.Pow(sum, 2));
            var n = (double)num.Length;
            return Math.Sqrt(topSum / (n * (n - 1)));
        }

        public static RatioSelector<T> CreateRatioSelector<T>(Dictionary<T, double> balls)
        {
            return new RatioSelector<T>(balls);
        }
    }

    /// <summary>
    /// 按比例概率抽取
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RatioSelector<T>
    {
        private double _sum;
        private Dictionary<T, double> _dictionary;
        private Random _random = new Random();

        /// <summary>
        /// 按比例概率抽取，参数举例： {"A球", 50}, {"B球", 20}, {"C球", 30}
        /// </summary>
        /// <param name="dictionary">double为数量，比如A球50个，B球20个，C球30个，那么A球概率0.5</param>
        public RatioSelector(Dictionary<T, double> dictionary)
        {
            _dictionary = dictionary;
            _sum = dictionary.Values.Sum();
        }

        /// <summary>
        /// 随机以概率方式抽取其中一个实体
        /// </summary>
        /// <returns></returns>
        public T Pick()
        {
            var random = _random.NextDouble();
            foreach (var item in _dictionary)
            {
                var ratio = item.Value / _sum;
                random -= ratio;
                if (random < 0)
                    return item.Key;
            }
            return default(T);
        }

     

    }
}
