using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;

using Luna.Common;
using Luna.Common.Domain;
using Luna.Common.Extensions;
using Luna.Core.Extensions;
using Luna.Infrastructure.Domain;

namespace Luna.Shifts.Domain
{
    public class Agent : IEquatable<Agent>, IEquatable<ISimpleEmployee>, IEqualityComparer<Agent>, ISelectable, ITermContainer, IEnumerable<Term>, IWorkingAgent
    {
        #region Fields

        private const int CellUnit = 5;
        
        //private List<Term> _fetchTermSetCache;
        private readonly Attendance _laborRule;
        private readonly DateRange _enquiryRange;
        private bool[] _onlines;
        private TimeBox _schedule;
        private Dictionary<DateTime, HeaderContainer<DateTime, IAssignment, int>> _dailyContainer;

        #endregion Fields

        #region Constructors

        public Agent()
        {
        }

        public Agent(TimeBox timeBox, Attendance attendance, DateRange enquiryRange)
        {
            timeBox.Boundary = new DateRange(attendance.Start, attendance.End);
            _dailyContainer = new Dictionary<DateTime, HeaderContainer<DateTime, IAssignment, int>>(Convert.ToInt32(timeBox.Boundary.Duration.TotalDays));
            Schedule = timeBox;
            _laborRule = attendance;
            _enquiryRange = enquiryRange;
        }

        public Agent(TimeBox timeBox)
        {
            Schedule = timeBox;
        }

        #endregion Constructors

        #region Properties

        private ISeat _currentSeat;

        public virtual ISeat CurrentSeat
        {
            get { return _currentSeat; }
            set
            {
                _currentSeat = (value != null && !value.IsNew<Guid>()) ? value : null;
            }
        }

        public Attendance LaborRule
        {
            get { return _laborRule; }
        }

       
        public bool[] Onlines
        {
            get { return _onlines; }
        }

        public IAgent TransferPropertiesFrom(IAgent original)
        {
            return this;
        }

        public virtual bool? OperationFail { get; set; }

        public ISimpleEmployee Profile
        {
            get { return Schedule.Agent; }
        }

        public IList Result
        {
            get { return Schedule.TermSet.ToList(); }
        }

        public TimeBox Schedule
        {
            get { return _schedule; }
            set { _schedule = value; }
        }

        public virtual bool? IsSelected
        {
            get
            {
                bool? isSelected = false;
                Profile.SaftyInvoke<ISelectable>(o => { isSelected = o.IsSelected; });

                return isSelected;
            }
            set
            {
                Profile.SaftyInvoke<ISelectable>(o => { o.IsSelected = value; });
            }
        }

        #endregion Properties

        #region Methods

        public void BuildOnlines()
        {
            //未修正 延伸的Schedule.Boundary, 会引发bug
            _onlines = Schedule.TermSet.ConvertToCell(_enquiryRange, CellUnit, t => t.OnService);
            
            //var cellLength = Convert.ToInt32(Schedule.Boundary.Duration.TotalDays) * OneDayCellCapacity;
            //if (_onlines.Length != cellLength)
            //    throw new Exception();
        }

        public virtual bool Equals(Agent other)
        {
            if (ReferenceEquals(this, other)) return true;
            return this.Schedule.Equals(other.Schedule);
        }

        public virtual bool Equals(ISimpleEmployee other)
        {
            return Profile.Equals(other);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Agent;
            if (other == null)
            {
                var otherEmployee = obj as ISimpleEmployee;
                if (otherEmployee == null) return false;

                return this.Equals(otherEmployee);
            }
            return this.Equals(other); //other.LaborRule.Equals(LaborRule) &&
        }

        public bool Equals(Agent x, Agent y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            return x.Equals(y);
        }

        private IList<Occupation> _occupations = new List<Occupation>();
        public IList<Occupation> Occupations
        {
            get { return _occupations; }
            set { _occupations = value; }
        }

        public IEnumerable Fetch(DateTime start, DateTime end)
        {
            return Schedule.TermSet.Retrieve<Term>(start, end)
                .OrderBy(o => o.Level).ThenBy(o => o.Start).ToList();
            //return _fetchTermSetCache;
        }

        public IEnumerator<Term> GetEnumerator()
        {
            //if (_fetchTermSetCache == null)
            return Schedule.TermSet.GetEnumerator();
            //else
            //    return _fetchTermSetCache.GetEnumerator();
        }

        private int _hashCode;

        public override int GetHashCode()
        {
            if (Schedule != null)
                _hashCode = Schedule.GetHashCode();

            return _hashCode;
        }

        public virtual int GetHashCode(Agent obj)
        {
            return obj.GetHashCode();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //void ITermContainer.SetTime(ITerm term, DateTime start, DateTime end, Action<ITerm, bool> callback)
        //{
        //    Schedule.SetTime(term, start, end, callback, false);
        //}

        public IEnumerable<Term> SelectTargetTerms(Func<Term, bool> predicate)
        {
            return Schedule.TermSet.Where(predicate);
        }

        #endregion Methods
    }
}