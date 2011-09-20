using Luna.Infrastructure.Domain;
using System.Collections.Generic;
using System;
using Luna.Core.Extensions;
using Luna.Common;
using Luna.Common.Extensions;

namespace Luna.Shifts.Domain
{
    public static class SeatingExt
    {
        public static IList<SeatArrangement> GenSeatArrangements(this IEnumerable<Term> terms, ISimpleEmployee agent, Func<string, Seat> getSeat, Func<string, SeatArrangement, bool> add)
        {
            var prv = default(Term);
            return terms.GetSeatArrangement((dateRange, term, isNeedSeat) =>
            {
                if (term == null || !term.IsNeedSeat)
                {
                    prv = term;
                    return default(SeatArrangement);
                }

                if (prv != null && !ReferenceEquals(prv, term) && prv.Level == 0 && term.Level == 0)
                {
                    prv = null;
                }

                var source = default(Term);
                TermExt.X(prv, term, ref source);

                var instance = default(SeatArrangement);

                var seatObj = source.If(o => o.SeatIsEmpty()) == true ? default(Seat) : getSeat(source.Seat);
                if (seatObj != null)
                {
                    instance = new SeatArrangement(agent, dateRange.Start, dateRange.End, seatObj) { Source = source };
                    add(source.Seat, instance);
                }

                prv = term; 
                return instance;
            });
        }
    }
}