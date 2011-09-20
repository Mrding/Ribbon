using System.Collections.Generic;
using Iesi.Collections.Generic;
using Luna.Common;
using Luna.Infrastructure.Domain;

namespace Luna.Shifts.Domain
{
    public class SeatConsolidationRule : IIndexable, IArrangeSeatRule
    {
        private int _start = 480;
        private int _end = 960;
        private bool[] _appliedDayOfWeeks = new[] { true, true, true, true, true, true, true };
        private string _title = "Untitled";
        private Iesi.Collections.Generic.ISet<Entity> _organizations = new HashedSet<Entity>();

        public virtual long Id { get; set; }

        public virtual Entity Site { get; set; }

        public virtual string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public virtual Iesi.Collections.Generic.ISet<Entity> Organizations
        {
            get { return _organizations; }
            set { _organizations = value; }
        }

        public virtual ArrangeSeatMethodology Methodology { get; set; }

        public virtual ISeat TargetSeat { get; set; }

        public virtual int StartValue { get { return _start; } set { _start = value; } }

        public virtual int EndValue { get { return _end; } set { _end = value; } }

        public virtual int Index { get; set; }

        public virtual bool[] AppliedDayOfWeeks
        {
            get { return _appliedDayOfWeeks; }
            set { _appliedDayOfWeeks = value; }
        }

        private Iesi.Collections.Generic.ISet<AssignmentType> _assignmentTypes = new HashedSet<AssignmentType>();
        public virtual Iesi.Collections.Generic.ISet<AssignmentType> AssignmentTypes
        {
            get { return _assignmentTypes; }
            set { _assignmentTypes = value; }
        }

        private Iesi.Collections.Generic.ISet<Skill> _skills = new HashedSet<Skill>();
        public virtual Iesi.Collections.Generic.ISet<Skill> Skills
        {
            get { return _skills; }
            set { _skills = value; }
        }

        public virtual bool MatchWholeSkills { get; set; }

        private int _maxRank = 10;
        public virtual int MaxRank
        {
            get { return _maxRank; }
            set
            {
                if (value > 10 || value < _minRank) return;
                _maxRank = value;
            }
        }

        private int _minRank = 1;
        public virtual int MinRank
        {
            get { return _minRank; }
            set
            {
                if (value < 1 || value > _maxRank) return;
                _minRank = value;
            }
        }

        public virtual ICollection<ISeat> GetSeatSiblings()
        {
            return ((IArea)TargetSeat.Area).Seats;
        }
    }
}
