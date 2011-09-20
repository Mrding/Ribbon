using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Infrastructure.Domain;

namespace Luna.Shifts.Domain
{
    public class SeatArrangement : Occupation, IAgentRelativeObject
    {
        protected SeatArrangement() { }

        public SeatArrangement(ISimpleEmployee agent, DateTime start, DateTime end, Term shift, ISeat seat)
            : this(shift, agent, start, end)
        {
            this.Seat = seat;
        }

        public SeatArrangement(Term source,ISimpleEmployee agent, DateTime start, DateTime end)
        {
            Agent = agent;
            Start = start;
            End = end;
            Source = source;
        }

        public SeatArrangement( ISimpleEmployee agent, DateTime start, DateTime end, ISeat seat)
        {
            Agent = agent;
            Start = start;
            End = end;
            this.Seat = seat;
        }

        //public SeatArrangement(AssignmentBase shift, ISimpleEmployee agent)
        //    : base(shift.Start, shift.End)
        //{
        //    Shift = shift;
        //    Agent = agent;
        //}

        public virtual ISimpleEmployee Agent { get; set; }

        //private ISeatingTerm _shift;
        //public virtual ISeatingTerm Shift
        //{
        //    get { return _shift; }
        //    set
        //    {
        //        //if (value is AssignmentBase)
        //        _shift = value;
        //        //else if (value != null)
        //        //throw new Exception("SeatArrangement's shift is not AssignmentBase");
        //    }
        //}

        private ISeatingTerm _sourceParent;
        public ISeatingTerm GetSourceParent()
        {
            _sourceParent = _sourceParent ?? Source.GetLowestTerm() as ISeatingTerm;
            return _sourceParent;
        }


        public virtual Term Source
        {
            get;
            set;
        }

        public virtual string DisplayText
        {
            get
            {
                var text = Source.Text;
                Source.DiveInto(t =>
                {
                    if (t.IsNeedSeat)
                    {
                        text = t.Text;
                        return true;
                    }
                    return false;
                });
                return text;
            }
        }

        public override ISeat Seat
        {
            get
            {
                return base.Seat;
            }
            set
            {
                base.Seat = value;
                if (base.Seat == null)
                {
                    Source.Seat = null;
                }
                else
                {
                    var seatId = base.Seat.Id.ToString();
                    if (Source != null && seatId != Source.Seat)
                    {
                        Source.Seat = seatId;
                        if(!Source.IsNeedSeat && Source.Level > 0) return; //為unlabour subevent就是切割點, 不可以認assignment為source
                        Source.DiveInto(o =>
                                            {
                                                if (o.Start == Source.Start && o.SeatIsEmpty() && o.IsNeedSeat)
                                                {
                                                    o.Seat = seatId;
                                                    Source.Seat = null;
                                                }
                                                    
                                                return true;
                                            });
                    }
                }
            }
        }
       
        public override string Text
        {
            get { return Agent.Name; }
        }
    }

    public static class SeatArrangementEx
    {
        /// <summary>
        /// 取范围碰到的
        /// </summary>
        public static IEnumerable<SeatArrangement> CollideSeatArrangements(this IEnumerable<SeatArrangement> terms, DateRange range)
        {
            return terms.Where(o => (o.Start < range.End && o.End > range.Start) || (o.Start == range.Start && o.End == range.End));
        }
    }
}
