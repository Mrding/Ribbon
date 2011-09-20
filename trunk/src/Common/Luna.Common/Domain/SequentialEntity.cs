namespace Luna.Common
{
    public class SequentialEntity<T> : IIndexable, ISelectable
    {
        protected SequentialEntity() { IsSelected = false; }

        
        public virtual T Object { get; set; }

        private int _index;

        public virtual int Index
        {
            get { return _index; }
            set
            {
                _index = value;
            }
        }

        public virtual bool? IsSelected { get; set; }

        public override int GetHashCode()
        {
            return Object.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as SequentialEntity<T>;
            if (other == null || Object == null || other.Object == null) return false;
            return other.Object.Equals(Object);
        }
    }
}
