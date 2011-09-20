using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;
using NHibernate.UserTypes;

namespace ConfOrm.NH
{
	public class MapMapper : IMapPropertiesMapper
	{
		private readonly KeyMapper keyMapper;
		private readonly HbmMap mapping;
		private readonly HbmMapping mapDoc;
		private readonly IAccessorPropertyMapper entityPropertyMapper;
		private ICacheMapper cacheMapper;

		public MapMapper(Type ownerType, Type keyType, Type valueType, HbmMap mapping, HbmMapping mapDoc)
		{
			if (ownerType == null)
			{
				throw new ArgumentNullException("ownerType");
			}
			if (keyType == null)
			{
				throw new ArgumentNullException("keyType");
			}
			if (valueType == null)
			{
				throw new ArgumentNullException("valueType");
			}
			if (mapping == null)
			{
				throw new ArgumentNullException("mapping");
			}
			OwnerType = ownerType;
			KeyType = keyType;
			ValueType = valueType;
			this.mapping = mapping;
			this.mapDoc = mapDoc;
			if (mapping.Key == null)
			{
				mapping.key = new HbmKey();
			}
			keyMapper = new KeyMapper(ownerType, mapping.Key);

			if (KeyType.IsValueType || KeyType == typeof(string))
			{
				mapping.Item = new HbmMapKey { type = KeyType.GetNhTypeName() };
			}
			else
			{
				mapping.Item = new HbmMapKeyManyToMany { @class = KeyType.GetShortClassName(mapDoc) };
			}
			entityPropertyMapper = new AccessorPropertyMapper(ownerType, mapping.Name, x => mapping.access = x);
		}

		public Type OwnerType { get; private set; }
		public Type KeyType { get; private set; }
		public Type ValueType { get; private set; }

		#region Implementation of IMapPropertiesMapper

		public void Inverse(bool value)
		{
			mapping.inverse = value;
		}

		public void Mutable(bool value)
		{
			mapping.mutable = value;
		}

		public void Where(string sqlWhereClause)
		{
			mapping.where = sqlWhereClause;
		}

		public void BatchSize(int value)
		{
			if (value > 0)
			{
				mapping.batchsize = value;
				mapping.batchsizeSpecified = true;
			}
			else
			{
				mapping.batchsize = 0;
				mapping.batchsizeSpecified = false;
			}
		}

		public void Lazy(CollectionLazy collectionLazy)
		{
			mapping.lazySpecified = true;
			switch (collectionLazy)
			{
				case CollectionLazy.Lazy:
					mapping.lazy = HbmCollectionLazy.True;
					break;
				case CollectionLazy.NoLazy:
					mapping.lazy = HbmCollectionLazy.False;
					break;
				case CollectionLazy.Extra:
					mapping.lazy = HbmCollectionLazy.Extra;
					break;
			}
		}

		public void Key(Action<IKeyMapper> keyMapping)
		{
			keyMapping(keyMapper);
		}

		public void OrderBy(MemberInfo property)
		{
			// TODO: read the mapping of the element to know the column of the property (second-pass)
			mapping.orderby = property.Name;
		}

		public void OrderBy(string sqlOrderByClause)
		{
			mapping.orderby = sqlOrderByClause;
		}

		public void Sort()
		{
			mapping.sort = "natural";
		}

		public void Sort<TComparer>()
		{
			
		}

		public void Cascade(Cascade cascadeStyle)
		{
			mapping.cascade = cascadeStyle.ToCascadeString();
		}

		public void Type<TCollection>() where TCollection : IUserCollectionType
		{
			mapping.collectiontype = typeof(TCollection).AssemblyQualifiedName;
		}

		public void Type(Type collectionType)
		{
			if (collectionType == null)
			{
				throw new ArgumentNullException("collectionType");
			}
			if (!typeof(IUserCollectionType).IsAssignableFrom(collectionType))
			{
				throw new ArgumentOutOfRangeException("collectionType",
																							string.Format(
																														"The collection type should be an implementation of IUserCollectionType.({0})",
																														collectionType));
			}
			mapping.collectiontype = collectionType.AssemblyQualifiedName;
		}

		public void Table(string tableName)
		{
			mapping.table = tableName;
		}

		public void Catalog(string catalogName)
		{
			mapping.catalog = catalogName;
		}

		public void Schema(string schemaName)
		{
			mapping.schema = schemaName;
		}

		public void Cache(Action<ICacheMapper> cacheMapping)
		{
			if (cacheMapper == null)
			{
				var hbmCache = new HbmCache();
				mapping.cache = hbmCache;
				cacheMapper = new CacheMapper(hbmCache);
			}
			cacheMapping(cacheMapper);
		}

		public void Filter(string filterName, Action<IFilterMapper> filterMapping)
		{
			if (filterMapping == null)
			{
				filterMapping = x => { };
			}
			var hbmFilter = new HbmFilter();
			var filterMapper = new FilterMapper(filterName, hbmFilter);
			filterMapping(filterMapper);
			var filters = mapping.filter != null ? mapping.filter.ToDictionary(f => f.name, f => f) : new Dictionary<string, HbmFilter>(1);
			filters[filterName] = hbmFilter;
			mapping.filter = filters.Values.ToArray();
		}

		public void Fetch(CollectionFetchMode fetchMode)
		{
			if (fetchMode == null)
			{
				return;
			}
			mapping.fetch = fetchMode.ToHbm();
			mapping.fetchSpecified = mapping.fetch != HbmCollectionFetchMode.Select;
		}

		#endregion

		#region Implementation of IEntityPropertyMapper

		public void Access(Accessor accessor)
		{
			entityPropertyMapper.Access(accessor);
		}

		public void Access(Type accessorType)
		{
			entityPropertyMapper.Access(accessorType);
		}

		public void OptimisticLock(bool takeInConsiderationForOptimisticLock)
		{
			mapping.optimisticlock = takeInConsiderationForOptimisticLock;
		}

		#endregion
	}
}