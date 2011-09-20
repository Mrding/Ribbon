using System;
using System.Collections.Generic;
using Luna.Common;

namespace Luna.Shifts.Domain
{
    public class SeatEvent : Occupation, IWritableTerm
    {
        protected Dictionary<string, object> _snapshot;

        public SeatEvent() { }

        public SeatEvent(Seat seat, DateTime start, DateTime end)
            : base(start, end)
        {
            Seat = seat;
            Start = start;
            End = end;
        }

        public override int Level
        {
            get
            {
                return 1;
            }
        }

        public virtual string Category { get; set; }

        public virtual string Description { get; set; }

        public virtual string GetBriefDescription()
        {
            return string.Format("{0} {1: MM/dd HH:mm} - {2:HH:mm}", Category, Start, End);
        }

        public virtual void Snapshot()
        {
            if (_snapshot == null) _snapshot = new Dictionary<string, object>();
            _snapshot["Start"] = Start;
            _snapshot["End"] = End;
            _snapshot["Category"] = Category;
            _snapshot["Description"] = Description;
        }

        public virtual void Reset()
        {
            if (_snapshot == null || !_snapshot.ContainsKey("Start")) return;
            Start = (DateTime)_snapshot["Start"];
            End = (DateTime)_snapshot["End"];
            Category = (string)_snapshot["Category"];
            Description = (string)_snapshot["Description"];

            _snapshot.Clear();
            _snapshot = null;
        }

        public virtual void EndEdit()
        {
            if (_snapshot == null) return;

            _snapshot.Clear();
            _snapshot = null;
        }

        //public override void Cancel(SeatBox seatBox)
        //{
        //    seatBox.RemoveOccupation(this);
        //}

        public override string Text
        {
            get { return Category; }
        }
    }
}
