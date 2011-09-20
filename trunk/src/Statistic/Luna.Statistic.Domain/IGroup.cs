using System.Collections;

namespace Luna.Statistic.Domain
{
    public interface IGroup
    {
        int Totals { get; }

        IEnumerable GetKeyList();

        double GetE(object keyEntity);

        void BuildF(ref double[,] mtx, int position, ref int hCounter);
    }
}