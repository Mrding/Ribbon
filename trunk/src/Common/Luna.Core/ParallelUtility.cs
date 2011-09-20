using System;
using System.Threading;

namespace Luna.Core
{
    public static class ParallelUtility
    {
        public static Tuple<int, int>[] GetStartEndIndex(int startIndex, int endIndex, int divisor)
        {
            if (startIndex >= endIndex)
                throw new IndexOutOfRangeException();
            if (divisor == 0)
                throw new DivideByZeroException();

            if (divisor == 1)
                return new Tuple<int, int>[] { new Tuple<int, int>(startIndex, endIndex) };

            Tuple<int, int>[] result = new Tuple<int, int>[divisor];

            int perCount = (endIndex - startIndex + 1) / divisor;
            int increase = perCount - 1;

            //first one
            result[0] = new Tuple<int, int>(startIndex, startIndex + increase);

            for (int i = 1; i < divisor - 1; i++)
            {
                int start = result[i - 1].Item2 + 1;
                result[i] = new Tuple<int, int>(start, start + increase);
            }

            //last one
            result[divisor - 1] = new Tuple<int, int>(result[divisor - 2].Item2 + 1, endIndex);

            return result;
        }

        public static AutoResetEvent[] GetProcessCountAutoResetEvents()
        {
            AutoResetEvent[] waitHandles = new AutoResetEvent[Environment.ProcessorCount];
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                waitHandles[i] = new AutoResetEvent(false);
            }
            return waitHandles;
        }

        public static void WaitAll(WaitHandle[] waitHandles)
        {
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                // 使用foreach，在多线程中等待每一个句柄
                foreach (var handle in waitHandles)
                {
                    WaitHandle.WaitAny(new[] { handle });
                }
            }
            else
            {
                WaitHandle.WaitAll(waitHandles);
            }
        }
    }
}
