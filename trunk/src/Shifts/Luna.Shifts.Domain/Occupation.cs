using System;
using Luna.Common;

namespace Luna.Shifts.Domain
{
    public abstract class Occupation : AbstractEntity<Int64>, IVisibleTerm, IComparable, IComparable<Occupation>, IHierarchicalTerm
    {

        protected Occupation() { }


        public Occupation(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public override long Id { get; protected set; }

        public virtual DateTime Start { get; set; }

        public virtual DateTime End { get; set; }

        public virtual bool HasChanged(bool compareWithDateOnly, out DateTime oldStart, out DateTime oldEnd)
        {
            throw new NotImplementedException();
        }

        public abstract string Text { get; }

        public virtual string Remark { get; set; }

        public virtual ISeat Seat { get; set; }

        public virtual int Level
        {
            get { return 0; }
        }

        int IComparable.CompareTo(object obj)
        {
            var other = obj as Occupation;
            if (other == null) return -1;
            //var cLevel = other.Level.CompareTo(Level);
            //if (cLevel == 0)
            //{
            return CompareTo(other);
        }

        public virtual int CompareTo(Occupation other)
        {
            return Start.CompareTo(other.Start);
        }
    }
}
