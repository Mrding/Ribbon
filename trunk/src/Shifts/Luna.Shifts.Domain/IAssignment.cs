using System;
using System.Collections.Generic;
using Luna.Common;
using System.Linq;
using Luna.Core.Extensions;

namespace Luna.Shifts.Domain
{
    public interface IAssignment : ISeatingTerm, IIndependenceTerm
    {
        TimeSpan ShrinkageTotals { get; set; }

        TimeSpan OvertimeTotals { get; set; }

        TimeSpan WorkingTotals { get; set; }

        WorkHourType Payment { get; }

        DateTime From { get; }

        DateTime Finish { get; }

        bool AsAWork { get; }

        bool GapGuaranteed { get; }


        //xDateTime HrDate { get; set; }

        string NativeName { get; }
    }


    public static class AssignmentExt
    {
        //private static readonly string[] OccupationAssigned = new[] { "C", "S" };

        //public static bool AnyOccupationAssigned(this IAssignment obj)
        //{
        //    return OccupationAssigned.Contains(obj.OccupyStatus);
        //}


        //public static bool CanRearrangeSeat(this IAssignment obj)
        //{
        //    return new[] { "C", "S", "X", "N" }.Contains(obj.OccupyStatus);
        //}

        //public static bool CanCancelSeatArrangement(this IAssignment obj)
        //{
        //    return new[] { "C", "S", "X", "W" }.Contains(obj.OccupyStatus);
        //}

        public static DateTime SaftyGetHrDate(this IIndependenceTerm assignment)
        {
            return assignment.HrDate == default(DateTime) ? assignment.Start.Date : assignment.HrDate;
        }


        public static bool ArrangeSeatYet(this ISeatingTerm obj)
        {
            return obj.IsNeedSeat && obj.If<Term>(o => o.SeatIsEmpty());
            //return new[] { "W", "X" }.Contains(obj.OccupyStatus);
        }


        //public static bool NeedRemoveSeatArrangement(this IAssignment obj)
        //{

        //    //TODO:detect event has any changed



        //    // || new[] { "W", "N" }.Contains(OccupyStatus);
        //    return new[] { "W", "N" }.Contains(obj.OccupyStatus);
        //}
    }
}
