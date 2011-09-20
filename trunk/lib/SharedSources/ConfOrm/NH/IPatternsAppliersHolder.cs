using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.NH
{
	public interface IPatternsAppliersHolder
	{
		ICollection<IPatternApplier<Type, IClassAttributesMapper>> RootClass { get; }
		ICollection<IPatternApplier<Type, IJoinedSubclassAttributesMapper>> JoinedSubclass { get; }
		ICollection<IPatternApplier<Type, ISubclassAttributesMapper>> Subclass { get; }
		ICollection<IPatternApplier<Type, IUnionSubclassAttributesMapper>> UnionSubclass { get; }
		ICollection<IPatternApplier<Type, IComponentAttributesMapper>> Component	{ get; }

		ICollection<IPatternApplier<MemberInfo, IIdMapper>> Poid { get; }
		ICollection<IPatternApplier<MemberInfo, IVersionMapper>> Version { get; }

		ICollection<IPatternApplier<MemberInfo, IPropertyMapper>> Property { get; }
		ICollection<IPatternApplier<PropertyPath, IPropertyMapper>> PropertyPath { get; }

		ICollection<IPatternApplier<MemberInfo, IManyToOneMapper>> ManyToOne { get; }
		ICollection<IPatternApplier<PropertyPath, IManyToOneMapper>> ManyToOnePath { get; }

		ICollection<IPatternApplier<MemberInfo, IOneToOneMapper>> OneToOne { get; }
		ICollection<IPatternApplier<PropertyPath, IOneToOneMapper>> OneToOnePath { get; }

		ICollection<IPatternApplier<MemberInfo, IAnyMapper>> Any { get; }
		ICollection<IPatternApplier<PropertyPath, IAnyMapper>> AnyPath { get; }

		#region Collection properties

		ICollection<IPatternApplier<MemberInfo, ICollectionPropertiesMapper>> Collection { get; }
		ICollection<IPatternApplier<PropertyPath, ICollectionPropertiesMapper>> CollectionPath { get; }
		ICollection<IPatternApplier<MemberInfo, IBagPropertiesMapper>> Bag { get; }
		ICollection<IPatternApplier<PropertyPath, IBagPropertiesMapper>> BagPath { get; }
		ICollection<IPatternApplier<MemberInfo, ISetPropertiesMapper>> Set { get; }
		ICollection<IPatternApplier<PropertyPath, ISetPropertiesMapper>> SetPath { get; }
		ICollection<IPatternApplier<MemberInfo, IListPropertiesMapper>> List { get; }
		ICollection<IPatternApplier<PropertyPath, IListPropertiesMapper>> ListPath { get; }
		ICollection<IPatternApplier<MemberInfo, IMapPropertiesMapper>> Map { get; }
		ICollection<IPatternApplier<PropertyPath, IMapPropertiesMapper>> MapPath { get; }

		#endregion

		#region Componenets

		ICollection<IPatternApplier<MemberInfo, IComponentParentMapper>> ComponentParent { get; }
		ICollection<IPatternApplier<MemberInfo, IComponentAttributesMapper>> ComponentProperty { get; }
		ICollection<IPatternApplier<PropertyPath, IComponentAttributesMapper>> ComponentPropertyPath { get; }
	
		#endregion

		#region Collection Element relations

		ICollection<IPatternApplier<MemberInfo, IManyToManyMapper>> ManyToMany { get; }
		ICollection<IPatternApplier<PropertyPath, IManyToManyMapper>> ManyToManyPath { get; }

		ICollection<IPatternApplier<MemberInfo, IElementMapper>> Element { get; }
		ICollection<IPatternApplier<PropertyPath, IElementMapper>> ElementPath { get; }

		ICollection<IPatternApplier<MemberInfo, IOneToManyMapper>> OneToMany { get; }
		ICollection<IPatternApplier<PropertyPath, IOneToManyMapper>> OneToManyPath { get; }
		#endregion

		#region Dictionary key relations

		ICollection<IPatternApplier<MemberInfo, IMapKeyManyToManyMapper>> MapKeyManyToMany { get; }
		ICollection<IPatternApplier<PropertyPath, IMapKeyManyToManyMapper>> MapKeyManyToManyPath { get; }

		ICollection<IPatternApplier<MemberInfo, IMapKeyMapper>> MapKey { get; }
		ICollection<IPatternApplier<PropertyPath, IMapKeyMapper>> MapKeyPath { get; }

		#endregion
	}
}