using System.Collections.Generic;

namespace Luna.Common.Model
{
    
    public interface IModelBase<TEntity> : IModel<TEntity>
    {
        IEnumerable<TEntity> GetAll();
    }
}