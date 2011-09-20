namespace Luna.Common
{
    public abstract class AbstractEntity<TIdentity> : IGenericEntity<TIdentity>
    {
        private int? _requestedHashCode;
        public virtual TIdentity Id { get; protected set; }

        public virtual bool CustomEquals(object other)
        {
            return false;
        }

        public virtual bool Equals(IGenericEntity<TIdentity> other)
        {
            if (null == other)
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            var otherIsTransient = Equals(other.Id, default(TIdentity));
            var thisIsTransient = IsTransient();
            if (otherIsTransient && thisIsTransient)
            {
                return ReferenceEquals(other, this);
            }

            return other.Id.Equals(Id);
        }

        protected bool IsTransient()
        {
            return Equals(Id, default(TIdentity));
        }

        public override bool Equals(object obj)
        {
            var that = obj as IGenericEntity<TIdentity>;
            return Equals(that);
        }

        public override int GetHashCode()
        {
            if (!_requestedHashCode.HasValue)
            {
                _requestedHashCode = IsTransient() ? base.GetHashCode() : Id.GetHashCode();
            }
            return _requestedHashCode.Value;
        }
    }
}
