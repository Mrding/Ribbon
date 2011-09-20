using System;
using ConfOrm.Mappers;
using NHibernate.Persister.Entity;

namespace ConfOrm.NH.CustomizersImpl
{
	public class UnionSubclassCustomizer<TEntity> : PropertyContainerCustomizer<TEntity>, IUnionSubclassMapper<TEntity> where TEntity : class
	{
		public UnionSubclassCustomizer(ICustomizersHolder customizersHolder) : base(customizersHolder, null) {}

		#region Implementation of IEntityAttributesMapper

		public void EntityName(string value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IUnionSubclassAttributesMapper m) => m.EntityName(value));
		}

		public void Proxy(Type proxy)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IUnionSubclassAttributesMapper m) => m.Proxy(proxy));
		}

		public void Lazy(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IUnionSubclassAttributesMapper m) => m.Lazy(value));
		}

		public void DynamicUpdate(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IUnionSubclassAttributesMapper m) => m.DynamicUpdate(value));
		}

		public void DynamicInsert(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IUnionSubclassAttributesMapper m) => m.DynamicInsert(value));
		}

		public void BatchSize(int value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IUnionSubclassAttributesMapper m) => m.BatchSize(value));
		}

		public void SelectBeforeUpdate(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IUnionSubclassAttributesMapper m) => m.SelectBeforeUpdate(value));
		}

		public void Persister<T>() where T : IEntityPersister
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IUnionSubclassAttributesMapper m) => m.Persister<T>());
		}

		#endregion

		#region Implementation of IEntitySqlsMapper

		public void Loader(string namedQueryReference)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IUnionSubclassAttributesMapper m) => m.Loader(namedQueryReference));
		}

		public void SqlInsert(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IUnionSubclassAttributesMapper m) => m.SqlInsert(sql));
		}

		public void SqlUpdate(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IUnionSubclassAttributesMapper m) => m.SqlUpdate(sql));
		}

		public void SqlDelete(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IUnionSubclassAttributesMapper m) => m.SqlDelete(sql));
		}

		#endregion

		#region Implementation of IUnionSubclassAttributesMapper<TEntity>

		public void Table(string tableName)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IUnionSubclassAttributesMapper m) => m.Table(tableName));
		}

		public void Catalog(string catalogName)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IUnionSubclassAttributesMapper m) => m.Catalog(catalogName));
		}

		public void Schema(string schemaName)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IUnionSubclassAttributesMapper m) => m.Schema(schemaName));
		}

		#endregion
	}
}