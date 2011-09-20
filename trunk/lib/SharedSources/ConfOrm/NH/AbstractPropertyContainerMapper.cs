using System;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public abstract class AbstractPropertyContainerMapper : AbstractBasePropertyContainerMapper, IPropertyContainerMapper
	{
		protected AbstractPropertyContainerMapper(Type container, HbmMapping mapDoc) : base(container, mapDoc)
		{}

		#region Implementation of IPropertyContainerMapper

		public virtual void OneToOne(MemberInfo property, Action<IOneToOneMapper> mapping)
		{
			var hbm = new HbmOneToOne { name = property.Name };
			mapping(new OneToOneMapper(property, hbm));
			AddProperty(hbm);
		}

		public virtual void Set(MemberInfo property, Action<ISetPropertiesMapper> collectionMapping, Action<ICollectionElementRelation> mapping)
		{
			var hbm = new HbmSet { name = property.Name };
			var propertyType = property.GetPropertyOrFieldType();
			var collectionElementType = propertyType.DetermineCollectionElementType();
			collectionMapping(new SetMapper(container, collectionElementType, hbm));
			mapping(new CollectionElementRelation(collectionElementType, MapDoc, rel => hbm.Item = rel));
			AddProperty(hbm);
		}

		public virtual void Bag(MemberInfo property, Action<IBagPropertiesMapper> collectionMapping, Action<ICollectionElementRelation> mapping)
		{
			var hbm = new HbmBag { name = property.Name };
			var propertyType = property.GetPropertyOrFieldType();
			var collectionElementType = propertyType.DetermineCollectionElementType();
			collectionMapping(new BagMapper(container, collectionElementType, hbm));
			mapping(new CollectionElementRelation(collectionElementType, MapDoc, rel => hbm.Item = rel));
			AddProperty(hbm);
		}

		public virtual void List(MemberInfo property, Action<IListPropertiesMapper> collectionMapping, Action<ICollectionElementRelation> mapping)
		{
			var hbm = new HbmList { name = property.Name };
			var propertyType = property.GetPropertyOrFieldType();
			var collectionElementType = propertyType.DetermineCollectionElementType();
			collectionMapping(new ListMapper(container, collectionElementType, hbm));
			mapping(new CollectionElementRelation(collectionElementType, MapDoc, rel => hbm.Item1 = rel));
			AddProperty(hbm);
		}

		public virtual void Map(MemberInfo property, Action<IMapPropertiesMapper> collectionMapping,
		                        Action<IMapKeyRelation> keyMapping, Action<ICollectionElementRelation> mapping)
		{
			var hbm = new HbmMap { name = property.Name };
			var propertyType = property.GetPropertyOrFieldType();
			Type dictionaryKeyType = propertyType.DetermineDictionaryKeyType();
			Type dictionaryValueType = propertyType.DetermineDictionaryValueType();

			collectionMapping(new MapMapper(container, dictionaryKeyType, dictionaryValueType, hbm, mapDoc));
			keyMapping(new MapKeyRelation(dictionaryKeyType, hbm, mapDoc));
			mapping(new CollectionElementRelation(dictionaryValueType, MapDoc, rel => hbm.Item1 = rel));
			AddProperty(hbm);
		}

		#endregion
	}
}