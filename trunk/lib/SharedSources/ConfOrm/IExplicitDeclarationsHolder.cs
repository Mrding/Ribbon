using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConfOrm
{
	public interface IExplicitDeclarationsHolder
	{
		HashSet<Type> DomainClasses { get; }
		ICollection<Type> RootEntities { get; }
		ICollection<Type> TablePerClassEntities { get; }
		ICollection<Type> TablePerClassHierarchyEntities { get; }
		ICollection<Type> TablePerConcreteClassEntities { get; }
		HashSet<Type> ClassExclusions { get; }
		ICollection<Relation> ManyToOneRelations { get; }
		ICollection<Relation> OneToManyRelations { get; }
		ICollection<Relation> OneToOneRelations { get; }
		ICollection<Relation> ManyToManyRelations { get; }
		ICollection<MemberInfo> Sets { get; }
		ICollection<MemberInfo> Bags { get; }
		ICollection<MemberInfo> Lists { get; }
		ICollection<MemberInfo> Arrays { get; }
		ICollection<MemberInfo> Dictionaries { get; }
		ICollection<Type> Components { get; }
		IDictionary<Relation, Cascade> Cascades { get; }
		ICollection<MemberInfo> PersistentProperties { get; }
		ICollection<Type> ComplexTypes { get; }
		ICollection<MemberInfo> ComplexTypeMembers { get; }
		ICollection<MemberInfo> Poids { get; }
		ICollection<MemberInfo> VersionProperties { get; }
		ICollection<MemberInfo> HeterogeneousAssociations { get; }
		ICollection<MemberInfo> NaturalIds { get; }
		ICollection<MemberInfo> ExclusionProperties { get; }
		IDictionary<RelationOn, MemberInfo> BidirectionalMembers { get; }
	}
}