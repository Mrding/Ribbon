using System;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class OneToManyMapper: IOneToManyMapper
	{
		private readonly Type collectionElementType;
		private readonly HbmOneToMany oneToManyMapping;
		private readonly HbmMapping mapDoc;

		public OneToManyMapper(Type collectionElementType, HbmOneToMany oneToManyMapping, HbmMapping mapDoc)
		{
			if (oneToManyMapping == null)
			{
				throw new ArgumentNullException("oneToManyMapping");
			}
			this.collectionElementType = collectionElementType;
			if(collectionElementType != null)
			{
				oneToManyMapping.@class = collectionElementType.GetShortClassName(mapDoc);
			}
			this.oneToManyMapping = oneToManyMapping;
			this.mapDoc = mapDoc;
		}

		#region Implementation of IOneToManyMapper

		public void Class(Type entityType)
		{
			if (!collectionElementType.IsAssignableFrom(entityType))
			{
				throw new ArgumentOutOfRangeException("entityType",
																							string.Format("The type is incompatible; expected assignable to {0}",
																													collectionElementType));
			}
			oneToManyMapping.@class = entityType.GetShortClassName(mapDoc);
		}

		public void EntityName(string entityName)
		{
			oneToManyMapping.entityname = entityName;
		}

		public void NotFound(NotFoundMode mode)
		{
			if (mode == null)
			{
				return;
			}
			oneToManyMapping.notfound = mode.ToHbm();
		}

		#endregion
	}
}