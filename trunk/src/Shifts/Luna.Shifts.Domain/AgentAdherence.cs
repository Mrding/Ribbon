namespace Luna.Shifts.Domain
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Luna.Common;
    using Luna.Common.Domain;
    using Luna.Infrastructure.Domain;

    public class AgentAdherence : IEnumerable<ITerm>, ITermContainer, IEquatable<AgentAdherence>, IEqualityComparer<AgentAdherence>
    {
        #region Constructors

        public AgentAdherence(ISimpleEmployee profile)
        {
            Profile = profile;
        }

        #endregion Constructors

        #region Properties

        public IEnumerable<ITerm> AdherenceTerms
        {
            get;
            set;
        }

        public ISimpleEmployee Profile
        {
            get;
            private set;
        }

        public IList Result
        {
            get { return AdherenceTerms.ToList(); }
        }

        public IList<RtaaSlicedTerm> SlicedTerms
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public virtual bool Equals(AgentAdherence other)
        {
            if (ReferenceEquals(this, other)) return true;
            return this.Profile.Equals(other.Profile);
        }

        public override bool Equals(object obj)
        {
            var other = obj as AgentAdherence;
            if (other == null) return false;
            return this.Equals(other);
        }

        public bool Equals(AgentAdherence x, AgentAdherence y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            return x.Equals(y);
        }

        public IEnumerable Fetch(DateTime start, DateTime end)
        {
            return AdherenceTerms;
        }

        public IEnumerable GetAllTerms()
        {
            return AdherenceTerms;
        }

        public IEnumerator<ITerm> GetEnumerator()
        {
            return AdherenceTerms.GetEnumerator();
        }

        public override int GetHashCode()
        {
            return this.Profile.GetHashCode();
        }

        public virtual int GetHashCode(AgentAdherence obj)
        {
            return obj.GetHashCode();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //void ITermContainer.SetTime(ITerm term, DateTime start, DateTime end, Action<ITerm, bool> callback)
        //{
        //}

        #endregion Methods
    }
}