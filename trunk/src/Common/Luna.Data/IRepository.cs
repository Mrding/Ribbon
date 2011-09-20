using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Transform;

namespace Luna.Data
{
    public interface IRepository<T> : IQueryable<T>
    {
        T Get(object id);
        T Load(object id);
        IList<T> GetAll();
        IList<TConverted> GetAll<TConverted>(IResultTransformer resultTransformer);
        IEnumerable<T> DelayGetAll();
        T MakePersistent(T entity);
        T MakePersistent<TProperty>(T entity, Expression<Func<TProperty>> propertyExpression);
        void Refresh<TChild>(ref TChild entity) where TChild : T;
        void Clear();
        void Evict(T entity);
        void Evict<T2>(T2 obj);
        void Merge(T entity);
        void MakeTransient(T entity);
        void MakePersistent(T entity, Action<bool> complete);
        void MakePersistent(T entity, Action before, Action<bool> after);
        void MakePersistent(T entity, Action<T, Exception> exceptionCallBack);
        void BatchInsert<TEntity>(ICollection<TEntity> entities);
        void BatchDetele<TEntity>(ICollection<TEntity> entities);
        void LoadRelatedEntities();
        bool HaveAnyRelationships(T entity);
        T2 SaveOrUpdate<T2>(T2 entity);
        T2 Get<T2>(object id);
        void Delete<T2>(T2 entity);
        void ClearSecondLevelCache();

        void RegisterSynchronization(Action<bool> complete);
    }
}