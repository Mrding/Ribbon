using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Infrastructure.Domain;
using Luna.Common.Extensions;
using Luna.Core.Extensions;

namespace Luna.Shifts.Domain
{
    public partial class TimeBox : Entity, IAgentRelativeObject
    {
        static readonly Dictionary<Type, Func<TimeBox, Term, IOrderedEnumerable<Term>, bool>> CreateValidations;

        static TimeBox()
        {
            #region validation registration

            CreateValidations = new Dictionary<Type, Func<TimeBox, Term, IOrderedEnumerable<Term>, bool>>();
            //AlterValidations = new Dictionary<Type, Func<TimeBox, Term, IOrderedEnumerable<Term>, bool>>();

            // create support types
            CreateValidations.Add(typeof(Assignment), IndependenceTermCreation);
            CreateValidations.Add(typeof(OvertimeAssignment), IndependenceTermCreation);
            CreateValidations.Add(typeof(DayOff), HolidayTermCreation);
            CreateValidations.Add(typeof(TimeOff), HolidayTermCreation);

            CreateValidations.Add(typeof(AbsentEvent), AbsentEventCreation);
            CreateValidations.Add(typeof(Shrink), DependencyTermCreation);
            CreateValidations.Add(typeof(RegularSubEvent), DependencyTermCreation);
            CreateValidations.Add(typeof(OvertimeSubEvent), DependencyTermCreation);
            CreateValidations.Add(typeof(UnlaboredSubEvent), DependencyTermCreation);

            // alter support types
            //AlterValidations.Add(typeof(Assignment), GenericTermModification);
            //AlterValidations.Add(typeof(OvertimeAssignment), GenericTermModification);
            //AlterValidations.Add(typeof(TimeOff), HolidayTermModification);

            //AlterValidations.Add(typeof(RegularSubEvent), GenericTermModification);
            //AlterValidations.Add(typeof(OvertimeSubEvent), GenericTermModification);
            //AlterValidations.Add(typeof(UnlaboredSubEvent), GenericTermModification);
            //AlterValidations.Add(typeof(AbsentEvent), AbsentEventTermModification);
            //AlterValidations.Add(typeof(Shrink), GenericTermModification);

            #endregion
        }

        private Iesi.Collections.Generic.ISet<Term> _termSet = new Iesi.Collections.Generic.SortedSet<Term>();
        private ISimpleEmployee _agent;

        #region Constructors

        public TimeBox(DateRange period, Employee employee)
            : this()
        {
            _agent = employee;
            Boundary = period;
        }

        protected TimeBox() { }

        #endregion Constructors

        #region Properties

        public virtual ISimpleEmployee Agent
        {
            get { return _agent; }
            protected set { _agent = value; }
        }

        private DateRange _boundary;
        public virtual DateRange Boundary
        {
            get
            {
                if (_boundary == default(DateRange))
                {
                    if (TermSet != null && TermSet.Count() > 0)
                    {
                        var sortedTermSet = TermSet.OrderBy(o => o.Start);
                        _boundary = new DateRange(sortedTermSet.First().Start, sortedTermSet.Last().End);
                    }
                }
                return _boundary;
            }
            set { _boundary = value; }
        }

        public virtual IEnumerable<Term> TermSet
        {
            get { return _termSet; }
            set { _termSet = value as Iesi.Collections.Generic.ISet<Term>; }
        }

        private object _currentSeat;
        public virtual object CurrentSeat
        {
            get { return _currentSeat; }
            set { _currentSeat = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// In Boundary terms
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAllTerm<T>(DateTime start, DateTime end)
        {
            return TermSet.Retrieve<Term>(start, end)
                    .Where(o => o.IsCoverd(Boundary.Start, Boundary.End))
                    .OfType<T>();
        }

        /// <summary>
        /// In Boundary terms
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAllTerm<T>(DateTime start, DateTime end, Predicate<Term> predicate)
        {
            return TermSet.Retrieve<Term>(start, end)
                    .Where(o => predicate(o) && o.IsCoverd(Boundary.Start, Boundary.End))
                    .OfType<T>();
        }

        /// <summary>
        /// In Boundary terms
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAllTermWithoutOffWork<T>(DateTime start, DateTime end, Predicate<Term> predicate)
        {
            return TermSet.Retrieve<Term>(start, end)
                    .Where(o => o.IsNot<IOffWork>() && predicate(o) && o.IsCoverd(Boundary.Start, Boundary.End))
                    .OfType<T>();
        }

        public virtual Term GetTopMostTerm(Term source)
        {
            var topTerm = GetCoveredTerms(source).OrderByDescending(o => o.Level).FirstOrDefault();
            return topTerm ?? source;
        }

        public static bool ExcludeSelf(Term self, Term other)
        {
            if (ReferenceEquals(self, other)) return false;

            return !self.SameTime(other);
        }

        /// <summary>
        /// Get bottoms order by Level DESC
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public virtual IOrderedEnumerable<Term> GetOrderedBottoms(Term source)
        {
            var results = TermSet.Retrieve<Term>(source.Start, source.End).Where(o => !ReferenceEquals(source, o) && source.IsInTheRange(o.Start, o.End)).OrderByDescending(o => o.Level);
            return results;
        }

        public virtual Term GetClosestBottom(Term source)
        {
            return GetOrderedBottoms(source).Where(o=> o.IsNot<IImmutableTerm>()).FirstOrDefault();
        }

        public virtual IEnumerable<Term> GetCoveredTermsWithAbsent(Term source)
        {
            var results = TermSet.Retrieve<Term>(source.Start, source.End)
                .Where(t => !ReferenceEquals(source, t) && t.Level > source.Level && source.EnclosedWithTypeVerification(t))
                .WhereFamilyTerms(source);

            return results;
        }

        public virtual IEnumerable<Term> GetCoveredTerms(Term source)
        {
            Func<Term, bool> excludeAbesent = t => !(t is AbsentEvent);

            var results = TermSet.Retrieve<Term>(source.Start, source.End).Where(t => !ReferenceEquals(source, t) && excludeAbesent(t) && source.EnclosedWithTypeVerification(t))
                                 .WhereFamilyTerms(source);

            return results;
        }

        public virtual IOrderedEnumerable<Term> GetEditableCoveredTerms(Term source)
        {
            return TermSet.Retrieve<Term>(source.Start, source.End).WhereEditableTerms(source)
                          .WhereFamilyTerms(source)
                          .OrderByStartAndLevel();
        }

        public virtual IOrderedEnumerable<Term> GetDeletableCoveredTerms(Term source)
        {
            return TermSet.Retrieve<Term>(source.Start, source.End)
                          .Where(t => t.Level > source.Level)
                          .WhereDeletableTerms(source)
                          .WhereFamilyTerms(source)
                          .OrderByStartAndLevel();
        }


        public override string GetUniqueKey()
        {
            return Agent.AgentId;
        }

        private void AdjustBoundary(AssignmentBase assignment)
        {
            var coveredTerms = GetCoveredTerms(assignment).OrderBy(o=> o.Level).ToArray();
            assignment.AdjustFromFinish(coveredTerms);
        }

        public virtual void EmptySeatArrangment<T>(Term term) where T : Term
        {
            if (typeof(T) == typeof(AssignmentBase))
            {
                term.IsNeedSeat = true;
                term.SaftyInvoke<AssignmentBase>(
                    x => GetCoveredTermsWithAbsent(x).Add(x).ForEach(o => o.ReleaseSeat()));
            }
        }

        public virtual void BalanceLabourHour(IAssignment term)
        {
            if (term == null) return;

            var coveredTerms = GetCoveredTerms(term as Term).Where(t => !(t is AbsentEvent)).OrderBy(o => o.Level);

            term.OvertimeTotals = term.Payment == WorkHourType.ExtraPaid ? term.End.Subtract(term.Start) : new TimeSpan();
            term.ShrinkageTotals = new TimeSpan();
            term.WorkingTotals = term.Payment == WorkHourType.Paid ? term.End.Subtract(term.Start) : new TimeSpan();

            foreach (var item in coveredTerms)
            {
                var timeSpan = item.End.Subtract(item.Start);
                if (item.Level == 1)
                {
                    if (term.Payment == WorkHourType.Paid)
                    {
                        if (item.Payment == WorkHourType.ExtraPaid)
                        {
                            term.OvertimeTotals += timeSpan;
                            term.WorkingTotals -= timeSpan;
                        }
                        else if (item.Payment == WorkHourType.Unpaid)
                            term.WorkingTotals -= timeSpan;
                        else if (item.Payment == WorkHourType.Shrink)
                            term.ShrinkageTotals += timeSpan;
                    }
                    else if (term.Payment == WorkHourType.ExtraPaid)
                    {
                        if (item.Payment == WorkHourType.Unpaid)
                            term.OvertimeTotals -= timeSpan;
                    }
                }
                else if(item.IsNot<IImmutableTerm>())//level2
                {
                    if (term.Payment == WorkHourType.Paid)
                    {
                        if (item.Payment == WorkHourType.ExtraPaid)
                        {
                            term.OvertimeTotals += timeSpan;
                            //term.WorkingTotals -= timeSpan;
                        }
                        else
                            term.WorkingTotals += timeSpan;
                    }
                    else //assignmentTerm.Payment == WorkHourTypes.ExtraPaid
                    {
                        term.OvertimeTotals += timeSpan;
                    }
                }
            }
        }

        #endregion Methods
    }


}
