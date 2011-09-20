using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Luna.Common.Domain;
using Luna.Common.Extensions;
using Luna.Common;
using Luna.Core.Extensions;

namespace Luna.Shifts.Domain
{
    public static class HeaderContainerExt
    {

        public static Dictionary<DateTime, int> CreateDateIndexer(this ITerm range )
        {
            return DateTimeExt.CreateDateIndexer(range.Start, range.End);
        }


        public static Func<DateTime, int> CreateDateIndexer<T>(this ITerm range, Dictionary<DateTime, int> dateToIndexTable, 
                                                                    Func<DateTime, T> construct, out T[] dates)
        {
            var viewDays = Convert.ToInt32(range.GetLength().TotalDays);

            var start = range.Start.Date;
            dates = new T[viewDays];


            for (var i = 0; i < viewDays; i++)
            {
                var date = start.AddDays(i);
                dateToIndexTable[date] = i;
                dates[i] = construct(date);
                dates[i].SaftyInvoke<IIndexable>(d => { d.Index = dateToIndexTable[date]; });
            }
            var indexer = new Func<DateTime, int>(date =>
                                                      {
                                                          if (!dateToIndexTable.ContainsKey(date))
                                                              return -1;
                                                          return dateToIndexTable[date];
                                                      });
            return indexer;
        } 
        
        
       

        public static HeaderContainer<T, DailyCounter<T>, DateTime> BuildDailyCounter<T>(this HeaderContainer<T, DailyCounter<T>, DateTime> obj, DailyCounter<T>[] sources,
            Action<DailyCounter<T>> instanceCreated, DateTime initial, int days)
        {
            var results = new DailyCounter<T>[days];

            var addedCount = 0;
            foreach (var item in sources)
            {
                var i = item.Date.GetIndex(initial, 1440, days);
                if (i < 0 || i >= results.Length)
                    continue;
                results[i] = item;
                addedCount++;
            }

            if (addedCount < days)
            {
                for (var i = 0; i < days; i++)
                {
                    if (results[i] == null)
                    {
                        results[i] = new DailyCounter<T>(obj.Header, initial.AddDays(i), 0);
                        instanceCreated(results[i]);
                    }
                }
            }

            obj.Items = new List<DailyCounter<T>>(results);
            return obj;
        }


        public static HeaderContainer<DateTime, IIndependenceTerm, int> Initial(this HeaderContainer<DateTime, IIndependenceTerm, int> obj,
            DateTime date, bool autoAddnewEmptyAssignment)
        {
            obj.Items = autoAddnewEmptyAssignment ? new List<IIndependenceTerm> { new UnknowAssignment(date, TimeSpan.FromDays(1)) } : new List<IIndependenceTerm>();
            return obj;
        }
    }
}
