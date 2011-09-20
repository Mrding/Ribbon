using System;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class CollectionElementRelation : ICollectionElementRelation
	{
		private readonly Type collectionElementType;
		private readonly HbmMapping mapDoc;
		private readonly Action<object> elementRelationshipAssing;

		public CollectionElementRelation(Type collectionElementType, HbmMapping mapDoc, Action<object> elementRelationshipAssing)
		{
			this.collectionElementType = collectionElementType;
			this.mapDoc = mapDoc;
			this.elementRelationshipAssing = elementRelationshipAssing;
		}

		#region Implementation of ICollectionElementRelation

		public void Element(Action<IElementMapper> mapping)
		{
			var hbm = new HbmElement();
			mapping(new ElementMapper(collectionElementType, hbm));
			elementRelationshipAssing(hbm);
		}

		public void OneToMany(Action<IOneToManyMapper> mapping)
		{
			var hbm = new HbmOneToMany { @class = collectionElementType.GetShortClassName(mapDoc) };
			mapping(new OneToManyMapper(collectionElementType, hbm, mapDoc));
			elementRelationshipAssing(hbm);
		}

		public void ManyToMany(Action<IManyToManyMapper> mapping)
		{
			var hbm = new HbmManyToMany { @class = collectionElementType.GetShortClassName(mapDoc) };
			mapping(new ManyToManyMapper(collectionElementType, hbm, mapDoc));
			elementRelationshipAssing(hbm);
		}

		public void Component(Action<IComponentElementMapper> mapping)
		{
			var hbm = new HbmCompositeElement { @class = collectionElementType.GetShortClassName(mapDoc) };
			mapping(new ComponentElementMapper(collectionElementType, mapDoc, hbm));
			elementRelationshipAssing(hbm);
		}

		#endregion
	}
}