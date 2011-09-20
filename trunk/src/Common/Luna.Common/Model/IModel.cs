namespace Luna.Common.Model
{
    
    public interface IModel<TEntity>
    {
        void Save(TEntity entity);

        void Reload(ref TEntity entity);

        void Delete(TEntity entity);
    }
}