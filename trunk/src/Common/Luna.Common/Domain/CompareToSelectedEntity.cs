using System;
using Luna.Common.Interfaces;
using Luna.Core.Extensions;

namespace Luna.Common.Domain
{
    public class CompareToSelectedEntity<TEntity> : ISelectable, IEntityDecker
    {
        protected readonly Func<TEntity, bool> _compareWith;
        protected readonly Action<CompareToSelectedEntity<TEntity>, bool?> _whenSelected;

        public CompareToSelectedEntity()
        {
        }

        public CompareToSelectedEntity(TEntity entity, Func<TEntity, bool> compareWith, Action<CompareToSelectedEntity<TEntity>, bool?> whenSelected)
        {
            Entity = entity;
            _compareWith = compareWith;
            _whenSelected = whenSelected;
        }

        public virtual TEntity Entity { get; private set; }

        public virtual bool? IsSelected
        {
            get { return _compareWith(Entity); }
            set { _whenSelected(this, value);
            }
        }

        public T GetEntity<T>() where T :Entity
        {
            return Entity.SaftyGetProperty<T, T>(e => e);
        }
    }
}