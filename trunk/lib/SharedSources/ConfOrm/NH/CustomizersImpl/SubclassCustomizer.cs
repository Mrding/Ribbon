using System;
using ConfOrm.Mappers;
using NHibernate.Persister.Entity;

namespace ConfOrm.NH.CustomizersImpl
{
	public class SubclassCustomizer<TEntity> : PropertyContainerCustomizer<TEntity>, ISubclassMapper<TEntity> where TEntity : class
	{
		public SubclassCustomizer(ICustomizersHolder customizersHolder) : base(customizersHolder, null) {}

		public void DiscriminatorValue(object value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassAttributesMapper m) => m.DiscriminatorValue(value));
		}

		#region Implementation of IEntityAttributesMapper

		public void EntityName(string value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassAttributesMapper m) => m.EntityName(value));
		}

		public void Proxy(Type proxy)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassAttributesMapper m) => m.Proxy(proxy));
		}

		public void Lazy(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassAttributesMapper m) => m.Lazy(value));
		}

		public void DynamicUpdate(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassAttributesMapper m) => m.DynamicUpdate(value));
		}

		public void DynamicInsert(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassAttributesMapper m) => m.DynamicInsert(value));
		}

		public void BatchSize(int value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassAttributesMapper m) => m.BatchSize(value));
		}

		public void SelectBeforeUpdate(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassAttributesMapper m) => m.SelectBeforeUpdate(value));
		}

		public void Persister<T>() where T : IEntityPersister
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassAttributesMapper m) => m.Persister<T>());
		}

		#endregion

		#region Implementation of IEntitySqlsMapper

		public void Loader(string namedQueryReference)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassAttributesMapper m) => m.Loader(namedQueryReference));
		}

		public void SqlInsert(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassAttributesMapper m) => m.SqlInsert(sql));
		}

		public void SqlUpdate(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassAttributesMapper m) => m.SqlUpdate(sql));
		}

		public void SqlDelete(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassAttributesMapper m) => m.SqlDelete(sql));
		}

		#endregion
	}
}