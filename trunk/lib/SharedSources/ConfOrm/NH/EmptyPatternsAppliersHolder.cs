using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.NH
{
	public class EmptyPatternsAppliersHolder : IPatternsAppliersHolder
	{
		protected List<IPatternApplier<Type, IClassAttributesMapper>> rootClass;
		protected List<IPatternApplier<Type, IJoinedSubclassAttributesMapper>> joinedSubclass;
		protected List<IPatternApplier<Type, ISubclassAttributesMapper>> subclass;
		protected List<IPatternApplier<Type, IUnionSubclassAttributesMapper>> unionSubclass;
		protected List<IPatternApplier<MemberInfo, IIdMapper>> poid;
		protected List<IPatternApplier<MemberInfo, IPropertyMapper>> property;
		protected List<IPatternApplier<MemberInfo, IManyToOneMapper>> manyToOne;
		protected List<IPatternApplier<MemberInfo, IOneToOneMapper>> oneToOne;
		protected List<IPatternApplier<MemberInfo, ICollectionPropertiesMapper>> collection;
		protected List<IPatternApplier<PropertyPath, IPropertyMapper>> propertyPath;
		protected List<IPatternApplier<PropertyPath, IManyToOneMapper>> manyToOnePath;
		protected List<IPatternApplier<PropertyPath, ICollectionPropertiesMapper>> collectionPath;
		protected List<IPatternApplier<PropertyPath, IOneToOneMapper>> oneToOnePath;
		protected List<IPatternApplier<MemberInfo, IAnyMapper>> any;
		protected List<IPatternApplier<PropertyPath, IAnyMapper>> anyPath;
		protected List<IPatternApplier<MemberInfo, IManyToManyMapper>> manyToMany;
		protected List<IPatternApplier<PropertyPath, IManyToManyMapper>> manyToManyPath;
		protected List<IPatternApplier<MemberInfo, IElementMapper>> element;
		protected List<IPatternApplier<PropertyPath, IElementMapper>> elementPath;
		protected List<IPatternApplier<MemberInfo, IOneToManyMapper>> oneToMany;
		protected List<IPatternApplier<PropertyPath, IOneToManyMapper>> oneToManyPath;
		protected List<IPatternApplier<MemberInfo, IVersionMapper>> version;

		protected List<IPatternApplier<MemberInfo, IMapKeyManyToManyMapper>> mapKeyManyToMany;
		protected List<IPatternApplier<PropertyPath, IMapKeyManyToManyMapper>> mapKeyManyToManyPath;

		protected List<IPatternApplier<MemberInfo, IMapKeyMapper>> mapKey;
		protected List<IPatternApplier<PropertyPath, IMapKeyMapper>> mapKeyPath;
		protected List<IPatternApplier<Type, IComponentAttributesMapper>> component;
		protected List<IPatternApplier<MemberInfo, IComponentAttributesMapper>> componentProperty;
		protected List<IPatternApplier<PropertyPath, IComponentAttributesMapper>> componentPropertyPath;
		protected List<IPatternApplier<MemberInfo, IComponentParentMapper>> componentParent;
		protected List<IPatternApplier<MemberInfo, IBagPropertiesMapper>> bag;
		protected List<IPatternApplier<PropertyPath, IBagPropertiesMapper>> bagPath;
		protected List<IPatternApplier<MemberInfo, ISetPropertiesMapper>> set;
		protected List<IPatternApplier<PropertyPath, ISetPropertiesMapper>> setPath;
		protected List<IPatternApplier<MemberInfo, IListPropertiesMapper>> list;
		protected List<IPatternApplier<PropertyPath, IListPropertiesMapper>> listPath;
		protected List<IPatternApplier<MemberInfo, IMapPropertiesMapper>> map;
		protected List<IPatternApplier<PropertyPath, IMapPropertiesMapper>> mapPath;

		public EmptyPatternsAppliersHolder()
		{
			rootClass = new List<IPatternApplier<Type, IClassAttributesMapper>>();
			joinedSubclass = new List<IPatternApplier<Type, IJoinedSubclassAttributesMapper>>();
			subclass = new List<IPatternApplier<Type, ISubclassAttributesMapper>>();
			unionSubclass = new List<IPatternApplier<Type, IUnionSubclassAttributesMapper>>();
			component = new List<IPatternApplier<Type, IComponentAttributesMapper>>();
			componentProperty = new List<IPatternApplier<MemberInfo, IComponentAttributesMapper>>();
			componentPropertyPath = new List<IPatternApplier<PropertyPath, IComponentAttributesMapper>>();

			poid = new List<IPatternApplier<MemberInfo, IIdMapper>>();
			property = new List<IPatternApplier<MemberInfo, IPropertyMapper>>();
			collection = new List<IPatternApplier<MemberInfo, ICollectionPropertiesMapper>>();
			propertyPath = new List<IPatternApplier<PropertyPath, IPropertyMapper>>();
			componentParent = new List<IPatternApplier<MemberInfo, IComponentParentMapper>>();

			collectionPath = new List<IPatternApplier<PropertyPath, ICollectionPropertiesMapper>>();
			bag = new List<IPatternApplier<MemberInfo, IBagPropertiesMapper>>();
			bagPath = new List<IPatternApplier<PropertyPath, IBagPropertiesMapper>>();
			set = new List<IPatternApplier<MemberInfo, ISetPropertiesMapper>>();
			setPath = new List<IPatternApplier<PropertyPath, ISetPropertiesMapper>>();
			list = new List<IPatternApplier<MemberInfo, IListPropertiesMapper>>();
			listPath = new List<IPatternApplier<PropertyPath, IListPropertiesMapper>>();
			map = new List<IPatternApplier<MemberInfo, IMapPropertiesMapper>>();
			mapPath = new List<IPatternApplier<PropertyPath, IMapPropertiesMapper>>();

			manyToOne = new List<IPatternApplier<MemberInfo, IManyToOneMapper>>();
			manyToOnePath = new List<IPatternApplier<PropertyPath, IManyToOneMapper>>();
			oneToOne = new List<IPatternApplier<MemberInfo, IOneToOneMapper>>();
			oneToOnePath = new List<IPatternApplier<PropertyPath, IOneToOneMapper>>();
			any = new List<IPatternApplier<MemberInfo, IAnyMapper>>();
			anyPath = new List<IPatternApplier<PropertyPath, IAnyMapper>>();
			manyToMany = new List<IPatternApplier<MemberInfo, IManyToManyMapper>>();
			manyToManyPath = new List<IPatternApplier<PropertyPath, IManyToManyMapper>>();
			element = new List<IPatternApplier<MemberInfo, IElementMapper>>();
			elementPath = new List<IPatternApplier<PropertyPath, IElementMapper>>();
			oneToMany = new List<IPatternApplier<MemberInfo, IOneToManyMapper>>();
			oneToManyPath = new List<IPatternApplier<PropertyPath, IOneToManyMapper>>();
			version = new List<IPatternApplier<MemberInfo, IVersionMapper>>();

			mapKey = new List<IPatternApplier<MemberInfo, IMapKeyMapper>>();
			mapKeyPath = new List<IPatternApplier<PropertyPath, IMapKeyMapper>>();
			mapKeyManyToMany = new List<IPatternApplier<MemberInfo, IMapKeyManyToManyMapper>>();
			mapKeyManyToManyPath = new List<IPatternApplier<PropertyPath, IMapKeyManyToManyMapper>>();
		}

		public ICollection<IPatternApplier<Type, IClassAttributesMapper>> RootClass
		{
			get { return rootClass; }
		}

		public ICollection<IPatternApplier<Type, IJoinedSubclassAttributesMapper>> JoinedSubclass
		{
			get { return joinedSubclass; }
		}

		public ICollection<IPatternApplier<Type, ISubclassAttributesMapper>> Subclass
		{
			get { return subclass; }
		}

		public ICollection<IPatternApplier<Type, IUnionSubclassAttributesMapper>> UnionSubclass
		{
			get { return unionSubclass; }
		}

		public ICollection<IPatternApplier<Type, IComponentAttributesMapper>> Component
		{
			get { return component; }
		}

		public ICollection<IPatternApplier<MemberInfo, IComponentAttributesMapper>> ComponentProperty
		{
			get { return componentProperty; }
		}

		public ICollection<IPatternApplier<PropertyPath, IComponentAttributesMapper>> ComponentPropertyPath
		{
			get { return componentPropertyPath; }
		}

		public ICollection<IPatternApplier<MemberInfo, IIdMapper>> Poid
		{
			get { return poid; }
		}

		public ICollection<IPatternApplier<MemberInfo, IVersionMapper>> Version
		{
			get { return version; }
		}

		public ICollection<IPatternApplier<MemberInfo, IPropertyMapper>> Property
		{
			get { return property; }
		}

		public ICollection<IPatternApplier<MemberInfo, IManyToOneMapper>> ManyToOne
		{
			get { return manyToOne; }
		}

		public ICollection<IPatternApplier<MemberInfo, ICollectionPropertiesMapper>> Collection
		{
			get { return collection; }
		}

		public ICollection<IPatternApplier<PropertyPath, IPropertyMapper>> PropertyPath
		{
			get { return propertyPath; }
		}

		public ICollection<IPatternApplier<PropertyPath, IManyToOneMapper>> ManyToOnePath
		{
			get { return manyToOnePath; }
		}

		public ICollection<IPatternApplier<PropertyPath, ICollectionPropertiesMapper>> CollectionPath
		{
			get { return collectionPath; }
		}

		public ICollection<IPatternApplier<MemberInfo, IBagPropertiesMapper>> Bag
		{
			get { return bag; }
		}

		public ICollection<IPatternApplier<PropertyPath, IBagPropertiesMapper>> BagPath
		{
			get { return bagPath; }
		}

		public ICollection<IPatternApplier<MemberInfo, ISetPropertiesMapper>> Set
		{
			get { return set; }
		}

		public ICollection<IPatternApplier<PropertyPath, ISetPropertiesMapper>> SetPath
		{
			get { return setPath; }
		}

		public ICollection<IPatternApplier<MemberInfo, IListPropertiesMapper>> List
		{
			get { return list; }
		}

		public ICollection<IPatternApplier<PropertyPath, IListPropertiesMapper>> ListPath
		{
			get { return listPath; }
		}

		public ICollection<IPatternApplier<MemberInfo, IMapPropertiesMapper>> Map
		{
			get { return map; }
		}

		public ICollection<IPatternApplier<PropertyPath, IMapPropertiesMapper>> MapPath
		{
			get { return mapPath; }
		}

		public ICollection<IPatternApplier<MemberInfo, IComponentParentMapper>> ComponentParent
		{
			get { return componentParent; }
		}

		public ICollection<IPatternApplier<MemberInfo, IOneToOneMapper>> OneToOne
		{
			get { return oneToOne; }
		}

		public ICollection<IPatternApplier<PropertyPath, IOneToOneMapper>> OneToOnePath
		{
			get { return oneToOnePath; }
		}

		public ICollection<IPatternApplier<MemberInfo, IAnyMapper>> Any
		{
			get { return any; }
		}

		public ICollection<IPatternApplier<PropertyPath, IAnyMapper>> AnyPath
		{
			get { return anyPath; }
		}

		public ICollection<IPatternApplier<MemberInfo, IManyToManyMapper>> ManyToMany
		{
			get { return manyToMany; }
		}

		public ICollection<IPatternApplier<PropertyPath, IManyToManyMapper>> ManyToManyPath
		{
			get { return manyToManyPath; }
		}

		public ICollection<IPatternApplier<MemberInfo, IElementMapper>> Element
		{
			get { return element; }
		}

		public ICollection<IPatternApplier<PropertyPath, IElementMapper>> ElementPath
		{
			get { return elementPath; }
		}

		public ICollection<IPatternApplier<MemberInfo, IOneToManyMapper>> OneToMany
		{
			get { return oneToMany; }
		}

		public ICollection<IPatternApplier<PropertyPath, IOneToManyMapper>> OneToManyPath
		{
			get { return oneToManyPath; }
		}

		public ICollection<IPatternApplier<MemberInfo, IMapKeyManyToManyMapper>> MapKeyManyToMany
		{
			get { return mapKeyManyToMany; }
		}

		public ICollection<IPatternApplier<PropertyPath, IMapKeyManyToManyMapper>> MapKeyManyToManyPath
		{
			get { return mapKeyManyToManyPath; }
		}

		public ICollection<IPatternApplier<MemberInfo, IMapKeyMapper>> MapKey
		{
			get { return mapKey; }
		}

		public ICollection<IPatternApplier<PropertyPath, IMapKeyMapper>> MapKeyPath
		{
			get { return mapKeyPath; }
		}
	}
}