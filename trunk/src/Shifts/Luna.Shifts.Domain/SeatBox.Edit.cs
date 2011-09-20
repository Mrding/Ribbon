using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Luna.Common;
using Luna.Common.Extensions;
using Luna.Common.Domain;

namespace Luna.Shifts.Domain
{
    public partial class SeatBox
    {
        public virtual void Create(ITerm newTerm, Action<ITerm, bool> callback)
        {
            var source = (Occupation)newTerm;

            // TODO: reconsider OrderBy clause

            var success = !_mixed.Retrieve<Occupation>(newTerm.Start, newTerm.End)
                .OfType<SeatEvent>()
                                .Any(t => t.IsCoverd(source));
            if (success)
            {
                AddOccupation(source);
            }
            callback(newTerm, success);
        }

        //todo don't forget
        //void ITermContainer.SetTime(ITerm term, DateTime start, DateTime end, Action<ITerm, bool> callback)
        //{
        //    var result = false;
        //    var source = (Occupation)term;
        //    //var isMoving = source.CanCauseMoved(start, end);
        //    var value = new DateRange(start, end);

        //    var coverage = term.MeasureCoverage(start, end);
        //    var relatedTerms = _mixed.Retrieve<Occupation>(coverage.Start, coverage.End, o => !ReferenceEquals(o, source)).ToList();

        //    if (!relatedTerms.OfType<SeatEvent>().Any(o => o.IsCoverd(value)))
        //    {
        //        //var offset = new { L = value.Start.Subtract(source.Start), R = value.End.Subtract(source.End) };
        //        source.Start = value.Start;
        //        source.End = value.End;
        //        result = true;
        //        _mixed.Rebuild<Occupation>();
        //    }

        //    callback(source, result);
        //}
    }
}
