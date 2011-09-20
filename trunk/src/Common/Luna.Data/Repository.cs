using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Luna.Data.Transaction;
using Microsoft.Practices.ServiceLocation;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using Luna.Core.Extensions;
using NHibernate.Transform;
using Expression = System.Linq.Expressions.Expression;

namespace Luna.Data
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected ISessionFactory Factory
        {
            get { return ServiceLocator.Current.GetInstance<ISessionFactory>(); }
        }

        protected ISession Session
        {
            get { return Factory.GetCurrentSession(); }
        }

        #region IRepository<T> Members

        public T Get(object id)
        {
            return Session.Get<T>(id);
        }

        public T Load(object id)
        {
            return Session.Load<T>(id);
        }

        public virtual IList<T> GetAll()
        {
            return Session.CreateCriteria<T>().List<T>();
        }

        public virtual IList<TConverted> GetAll<TConverted>(IResultTransformer resultTransformer)
        {
            return Session.CreateCriteria<T>().SetResultTransformer(resultTransformer).List<TConverted>();
        }

        public virtual IEnumerable<T> DelayGetAll()
        {
            return Session.CreateCriteria<T>().Future<T>();
        }

        public virtual bool HaveAnyRelationships(T entity)
        {
            return default(bool);
        }

        public virtual TEntity SaveOrUpdate<TEntity>(TEntity entity)
        {
            Session.SaveOrUpdate(entity);
            return entity;
        }
        public TEntity Get<TEntity>(object id)
        {
            return Session.Get<TEntity>(id);
        }

        public virtual void Delete<TEntity>(TEntity entity)
        {
            Session.Delete(entity);
        }

        public virtual T MakePersistent(T entity)
        {
            //Session.Persist(entity);
            Session.SaveOrUpdate(entity);
            return entity;
        }

        public virtual T MakePersistent<TProperty>(T entity, Expression<Func<TProperty>> propertyExpression)
        {
            var type = typeof(T);
            string tableName = type.Name;
            string propertyName = propertyExpression.GetMemberInfo().Name;
            var value = propertyExpression.GetValue();
            var id = type.GetProperty("Id").GetValue(entity, null);

            Session.CreateQuery(string.Format("update {0} set {1}=:value where Id=:id", tableName, propertyName))
                .SetParameter("value", value)
                .SetParameter("id", id)
                .ExecuteUpdate();
            return entity;
        }

        public virtual void MakePersistent(T entity, Action<bool> after)
        {
            MakePersistent(entity, default(Action), after);
        }

        public virtual void MakePersistent(T entity, Action before, Action<bool> after)
        {
            Session.Transaction.RegisterSynchronization(new DefaultSynchronization(before, after));
            Session.SaveOrUpdate(entity);
        }

        public virtual void RegisterSynchronization(Action<bool> complete)
        {
            Session.Transaction.RegisterSynchronization(new DefaultSynchronization(complete));
        }

        public virtual void MakePersistent(T entity, Action<T, Exception> exceptionCallBack)
        {
            try
            {
                MakePersistent(entity);
            }
            catch (Exception ex)
            {
                if (exceptionCallBack != null)
                    exceptionCallBack(entity, ex);
            }
        }

        public void BatchInsert<TEntity>(ICollection<TEntity> entities)
        {
            using (var s = Factory.OpenStatelessSession())
            {
                using (var tx = s.BeginTransaction())
                {
                    foreach (var entity in entities)
                    {
                        s.Insert(entity);
                    }
                    tx.Commit();
                }
            }
        }

        public void BatchDetele<TEntity>(ICollection<TEntity> entities)
        {
            using (var s = Factory.OpenStatelessSession())
            {
                using (var tx = s.BeginTransaction())
                {
                    foreach (var entity in entities)
                    {
                        s.Delete(entity);
                    }
                    tx.Commit();
                }
            }
        }

        public virtual void Refresh<TEntity>(ref TEntity entity) where TEntity : T
        {
            Session.Evict(entity);
            Session.Refresh(entity);
        }

        public virtual void Evict(T entity)
        {
            Session.Evict(entity);
        }

        public virtual void Evict<T2>(T2 obj)
        {
            Session.Evict(obj);
        }

        public virtual void Clear()
        {
            Session.Clear();
        }

        public void Merge(T entity)
        {
            Session.Merge(entity);
        }

        public virtual void MakeTransient(T entity)
        {
            Session.Delete(entity);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return GetLinq().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetLinq().GetEnumerator();
        }

        public Expression Expression
        {
            get { return GetLinq().Expression; }
        }

        public Type ElementType
        {
            get { return GetLinq().ElementType; }
        }

        public IQueryProvider Provider
        {
            get { return GetLinq().Provider; }
        }

        public virtual void LoadRelatedEntities()
        {
        }

        #endregion

        private IQueryable<T> GetLinq()
        {
            return Session.Query<T>();
        }

        public void ClearSecondLevelCache()
        {
            Factory.EvictQueries();
            foreach (var collectionMetadata in Factory.GetAllCollectionMetadata())
                Factory.EvictCollection(collectionMetadata.Key);
            foreach (var classMetadata in Factory.GetAllClassMetadata())
                Factory.EvictEntity(classMetadata.Key);
        }

        protected long DoCount(DetachedCriteria criteria)
        {
            return Convert.ToInt64(criteria.GetExecutableCriteria(Session)
                          .SetProjection(Projections.RowCountInt64())
                          .UniqueResult());
        }
    }
}