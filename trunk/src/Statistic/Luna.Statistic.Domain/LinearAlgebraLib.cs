using System;
using System.Collections.Generic;
using Luna.Core;
using System.Collections;
using System.Text;

namespace Luna.Statistic.Domain
{
    public static class LinearAlgebraLib
    {
        public static void Distribute<T>(this IDictionary<T, double> items, System.Func<T, double, bool> addingVerification,
            ref Dictionary<string, Group> identicalGroups)
            where T : class
        {
            var addToGroup = new System.Action<Group>(g => g.Add());
            var keyBuilder = new StringBuilder();

            foreach (var item in items)
            {
                var productivity = item.Value;
                var q = item.Key;
                if (addingVerification(q, productivity))
                {
                    addToGroup += g => g.Add(q, productivity);
                    keyBuilder.Append(item.GetHashCode());
                }
            }

            if (keyBuilder.Length == 0)
                return;

            var key = keyBuilder.ToString();

            if (!identicalGroups.ContainsKey(key))
            {
                identicalGroups[key] = new Group();
            }
            addToGroup(identicalGroups[key]);
        }

        public static void Build<T>(IEnumerable groups, out int mtxCurrentIndex, out double[,] mtx, out double[] rhs, out Dictionary<int, Tuple<T, double>> vEx)
        {
            var vCounter = 0;
            vEx = new Dictionary<int, Tuple<T, double>>();
            foreach (IGroup g in groups)
            {
                foreach (T q in g.GetKeyList())
                {
                    vEx[vCounter] = new Tuple<T, double>(q, g.GetE(q)); //fill a,b,c,d...
                    vCounter++;
                }
            }

            mtx = new double[vCounter, vCounter];
            rhs = new double[vCounter];
            mtxCurrentIndex = 0;
            var hCounter = 0;

            var j = 0;
            foreach (IGroup g in groups)
            {
                g.BuildF(ref mtx, j, ref hCounter);
                rhs[j] = g.Totals; // agents in group
                mtxCurrentIndex++;
                j++;
            }
        }

        public static void RatioX<TX>(List<TX> keys, IDictionary<TX, double> sources, ref int mtxCurrentIndex, ref double[,] mtx, IDictionary<int, Tuple<TX, double>> vEx)
            where TX : class
        {
            var dim = vEx.Count;
            var count = keys.Count - 1;

            for (var i = 0; i < count && mtxCurrentIndex < dim; i++)
            {
                var required = sources[keys[i]]; //denominator
                var numerator = Numerator(keys[i], vEx, false);

                Ratio(dim, ref mtxCurrentIndex, ref mtx, required, numerator,
                          sources[keys[i + 1]], Numerator(keys[i + 1], vEx, false));
            }

            //Add the ratio if length of matrix is less than dMtx.Length
            var remains = Math.Min(dim - mtxCurrentIndex, keys.Count - 1); // 防止发生keys阵列溢出, 正确性尚未验证
            if (remains <= 0) return;

            for (var i = 0; i < remains; i++)
            {
                var required = sources[keys[i]]; //denominator
                var numerator = Numerator(keys[i], vEx, true);

                Ratio(dim, ref mtxCurrentIndex, ref mtx, required, numerator,
                                                         sources[keys[i + 1]], Numerator(keys[i + 1], vEx, true));
            }
        }

        private static double[] Numerator<TX>(TX target, IDictionary<int, Tuple<TX, double>> vEx, bool escapWhenMatched) where TX : class
        {
            var count = vEx.Count;
            var numerator = new double[count];
            for (var i = 0; i < count; i++)
            { 
                // fill in the blanks
                var matched = vEx[i].Item1 == target;
                if (escapWhenMatched && matched)
                {
                    numerator[i] = vEx[i].Item2;
                    break;
                }
                numerator[i] = matched ? vEx[i].Item2 : 0;
            }
            return numerator;
        }

        private static void Ratio(int count, ref int mtxCurrentIndex, ref double[,] mtx, double denominatorA, double[] numeratorA, double denominatorB, double[] numeratorB)
        {
            if (denominatorA == 0 || denominatorB == 0 || numeratorA == null || numeratorB == null)
                return;

            for (var i = 0; i < count; i++)
            {
                mtx[mtxCurrentIndex, i] = denominatorB * numeratorA[i] - denominatorA * numeratorB[i];
            }
            mtxCurrentIndex++;
        }
    }
}