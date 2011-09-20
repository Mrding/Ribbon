using System;
using System.Linq.Expressions;
using ConfOrm.Mappers;
using NHibernate.UserTypes;

namespace ConfOrm.NH.CustomizersImpl
{
	public class CollectionPropertiesCustomizer<TEntity, TElement> : ICollectionPropertiesMapper<TEntity, TElement>
		where TEntity : class
	{
		private readonly IKeyMapper<TEntity> keyMapper;

		public CollectionPropertiesCustomizer(PropertyPath propertyPath, ICustomizersHolder customizersHolder)
		{
			PropertyPath = propertyPath;
			CustomizersHolder = customizersHolder;
			keyMapper = new CollectionKeyCustomizer<TEntity>(propertyPath, customizersHolder);
		}

		public ICustomizersHolder CustomizersHolder { get; private set; }
		public PropertyPath PropertyPath { get; private set; }

		#region Implementation of ICollectionPropertiesMapper<TEntity,TElement>

		public void Inverse(bool value)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (ICollectionPropertiesMapper x)=> x.Inverse(value));
		}

		public void Mutable(bool value)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (ICollectionPropertiesMapper x) => x.Mutable(value));
		}

		public void Where(string sqlWhereClause)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (ICollectionPropertiesMapper x) => x.Where(sqlWhereClause));
		}

		public void BatchSize(int value)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (ICollectionPropertiesMapper x) => x.BatchSize(value));
		}

		public void Lazy(CollectionLazy collectionLazy)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (ICollectionPropertiesMapper x) => x.Lazy(collectionLazy));
		}

		public void Key(Action<IKeyMapper<TEntity>> keyMapping)
		{
			keyMapping(keyMapper);
		}

		public void OrderBy<TProperty>(Expression<Func<TElement, TProperty>> property)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(property);
			CustomizersHolder.AddCustomizer(PropertyPath, (ICollectionPropertiesMapper x) => x.OrderBy(member));
		}

		public void OrderBy(string sqlOrderByClause)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (ICollectionPropertiesMapper x) => x.OrderBy(sqlOrderByClause));
		}

		public void Sort()
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (ICollectionPropertiesMapper x) => x.Sort());
		}

		public void Sort<TComparer>()
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (ICollectionPropertiesMapper x) => x.Sort<TComparer>());
		}

		public void Cascade(Cascade cascadeStyle)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (ICollectionPropertiesMapper x) => x.Cascade(cascadeStyle));
		}

		public void Type<TCollection>() where TCollection : IUserCollectionType
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (ICollectionPropertiesMapper x) => x.Type<TCollection>());
		}

		public void Type(Type collectionType)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (ICollectionPropertiesMapper x) => x.Type(collectionType));
		}

		public void Table(string tableName)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (ICollectionPropertiesMapper x) => x.Table(tableName));
		}

		public void Catalog(string catalogName)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (ICollectionPropertiesMapper x) => x.Catalog(catalogName));
		}

		public void Schema(string schemaName)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (ICollectionPropertiesMapper x) => x.Schema(schemaName));
		}

		public void Cache(Action<ICacheMapper> cacheMapping)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (ICollectionPropertiesMapper x) => x.Cache(cacheMapping));
		}

		public void Filter(string filterName, Action<IFilterMapper> filterMapping)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (ICollectionPropertiesMapper x) => x.Filter(filterName, filterMapping));
		}

		public void Fetch(CollectionFetchMode fetchMode)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (ICollectionPropertiesMapper x) => x.Fetch(fetchMode));
		}

		#endregion

		public void Access(Accessor accessor)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (ICollectionPropertiesMapper x) => x.Access(accessor));
		}

		public void Access(Type accessorType)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (ICollectionPropertiesMapper x) => x.Access(accessorType));
		}

		public void OptimisticLock(bool takeInConsiderationForOptimisticLock)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (ICollectionPropertiesMapper x) => x.OptimisticLock(takeInConsiderationForOptimisticLock));
		}
	}
}