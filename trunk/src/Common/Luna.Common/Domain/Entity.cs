using System;

namespace Luna.Common
{
    public class Entity : AbstractEntity<Guid>
    {
        public override Guid Id { get; protected set; }
        public virtual string Name { get; set; }
        public virtual string GetUniqueKey()
        {
            return string.Format("{0}.{1}", GetType(), GetHashCode());
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Name) ? base.ToString() : Name;
        }

        /// <summary>
        /// 用于做NullItem
        /// </summary>
        public virtual Entity Value { get { return string.IsNullOrEmpty(Name) ? null : this; } }
    }
}
