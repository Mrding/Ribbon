using System.Collections.Generic;
using System.Linq;


namespace Luna.Statistic.Domain
{
    public class SimplexStaffing
    {
        /// <summary>
        /// Solve 
        ///     mtxA*result = RHS
        ///     where all element of result >= 0
        /// 1. solve mtxA normaly
        /// 2. if any element of result less than 0
        /// 3. compute corresponding simplex and try let  (ax + by) / k0 - (cx +dz) /k1 reach min
        /// </summary>
        /// <param name="mtxA"></param>
        /// Parameter Matrix
        /// # Must put number const at first
        ///   [ a b 0 0 0]
        /// [ [ 0 0 c d e] ] x = b
        ///   [ ratial eq]
        /// <param name="dim"></param>
        /// <param name="groupAmount"> amount of Group </param>
        /// <param name="rhs"></param>
        /// <param name="info">
        /// return :
        ///     info = -3 mtxA is bad condiction result is all zero
        ///     info = -1 dim is less or equal zero
        ///     info =  1 success
        /// </param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool FS(double[,] mtxA, int groupAmount, double[] rhs, out int info, out double[] result)
        {
            alglib.densesolverreport rep;
            alglib.rmatrixsolve( mtxA, rhs.Length, rhs, out info, out rep, out result);

            return false;

            if (info == -3 || info == -1)
                return false;
           

            //// check result of negaive
            //var neResultIdx = new List<int>();
            //for (var i = 0; i < dim; i++)
            //{
            //    if (result[i] < 0)
            //        neResultIdx.Add(i);
            //}
            //if (neResultIdx.Count == 0)
            //    return true;

            //// group negative by number const
            //var groupIdx = new List<int>[groupAmount];
            //#region Generate GroupIdx
            //for (var i = 0; i < groupAmount; i++)
            //{
            //    groupIdx[i] = new List<int>();
            //    for (int j = 0; j < dim; j++)
            //    {
            //        if (mtxA[i, j] != 0)
            //        {
            //            groupIdx[i].Add(j);
            //        }
            //    }
            //}
            //#endregion
            //// tweak negative result
            //foreach (var group in groupIdx)
            //{
            //    var pickedIdx = neResultIdx.Where(o => group.Contains(o));
            //    var otherIdx = group.Where(o => !neResultIdx.Contains(o));
            //    double negativeSum = 0;
            //    foreach (int idx in pickedIdx)
            //        negativeSum += result[idx];
            //    if (otherIdx.Count() == 1)
            //    {
            //        // add negative value
            //        result[otherIdx.First()] += negativeSum;
            //        foreach (int idx in pickedIdx)
            //            result[idx] = 0;
            //    }
            //    else
            //    {
            //        // compute negative rational
            //        double othersum = 0;
            //        foreach (int idx in otherIdx)
            //            othersum += result[idx];
            //        // add negative value (project on other positive solution)
            //        foreach (int idx in otherIdx)
            //            result[idx] += (result[idx] / othersum) * negativeSum;
            //        foreach (int idx in pickedIdx)
            //            result[idx] = 0;
            //    }
            //}
            //return true;
        }
    }
}
