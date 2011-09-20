using System.Linq;
using Luna.Common;
using Luna.Common.Extensions;

namespace Luna.Shifts.Domain
{
    public partial class TimeBox
    {
        private static bool AbsentEventCreation(TimeBox sender, Term absent, IOrderedEnumerable<Term> orderedTerms)
        {
            if (orderedTerms.Any(t => t.CanNotOverlapWithAbsent && t.OverlapNotEnclosed(absent))) 
                return false;

            var exists = orderedTerms.Where(absent.NeedRelyOn)
                                     .Any(t =>
                                     {
                                         if (t.EnclosedWithTypeVerification(absent))
                                         {
                                             absent.Bottom = t.GetLowestTerm() as Term;
                                         
                                             //if (absent.IsNeedSeat != t.IsNeedSeat && t is AssignmentBase)
                                             //    ((AssignmentBase)t).CancelSeatArrangement(absent);

                                             return true;
                                         }
                                         return false;
                                     });
            return exists;
        }

        private static bool IndependenceTermCreation(TimeBox sender, Term other, IOrderedEnumerable<Term> orderedTerms)
        {
            //bugs might be here
            var result = !orderedTerms.Any(t =>  {
                var found = false;
                if (t.Level > other.Level)
                {
                    found = other.OverlapNotEnclosed(t);
                    other.Exception = new TermException(t, "OverlapNotEnclosed");
                }//upper term

                

                if (t.Level == other.Level && !(t is IOffWork || other is IOffWork) && t.HrDateEq(other)) // 只有非IOffWork不可相交
                {
                    found = t.IsCoverd(other);
                    other.Exception = new TermException(t, "IsCoverd");
                }// same level term

                return found;
            });

            return result;
        }

        private static bool HolidayTermCreation(TimeBox sender, Term other, IOrderedEnumerable<Term> orderedTerms)
        {
            return !orderedTerms.OfType<IIndependenceTerm>().Any(o => other.HrDateEq(o));
        }

        private static bool DependencyTermCreation(TimeBox sender, Term other, IOrderedEnumerable<Term> orderedTerms)
        {
            var newTermType = other.GetType();
            var closestBottom = sender.GetClosestBottom(other);
            var overlapping = false;
            var canLandingOn = false;

            foreach (var t in orderedTerms)
            {
                var insiede = other.IsInTheRange(t.Start, t.End);
              
                overlapping = t.AnyOverlap(other) && !insiede; //oh! No~

                if (overlapping)
                {
                    other.Exception = new TermException(t, "AnyOverlap");
                    break;
                }

                canLandingOn = t.CanBeOverlap(newTermType);
                
                if (!canLandingOn && insiede)
                {
                    other.Exception = new TermException(t, "RelationIncorrect");
                     break;
                }// find right position but no right relation
                   
                if (canLandingOn && insiede)
                {
                    other.Bottom = closestBottom;
                    break;
                }
            }
          
            return overlapping == false && canLandingOn;
        }

       

    }
}
