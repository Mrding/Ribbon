using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConfOrm
{
	public interface IDomainInspector
	{
		bool IsRootEntity(Type type);
		bool IsComponent(Type type);
		bool IsEntity(Type type);

		bool IsTablePerClass(Type type);
		bool IsTablePerClassHierarchy(Type type);
		bool IsTablePerConcreteClass(Type type);

		bool IsOneToOne(Type from, Type to);
		bool IsMasterOneToOne(Type from, Type to);
		bool IsManyToOne(Type from, Type to);
		bool IsManyToMany(Type role1, Type role2);
		bool IsMasterManyToMany(Type from, Type to);
		bool IsOneToMany(Type from, Type to);
		bool IsHeterogeneousAssociation(MemberInfo member);
		Cascade? ApplyCascade(Type from, MemberInfo on, Type to);

		/// <summary>
		/// Provide the bidirectional member of a relation where available.
		/// </summary>
		/// <param name="from">The class containing the member of <paramref name="on"/>.</param>
		/// <param name="on">The member under inspection.</param>
		/// <param name="to">The other side of the relation.</param>
		/// <returns>The member representing the bidirectional side of the relation where available. null otherwise.</returns>
		/// <remarks>
		/// The bidirectional member even where present can be not explicitly provided by the implementor of <see cref="IDomainInspector"/>;
		/// in this case the user of the method should try to find it in its way.
		/// </remarks>
		MemberInfo GetBidirectionalMember(Type from, MemberInfo on, Type to);

		bool IsPersistentId(MemberInfo member);
		IPersistentIdStrategy GetPersistentIdStrategy(MemberInfo member);
		bool IsMemberOfNaturalId(MemberInfo member);

		bool IsPersistentProperty(MemberInfo role);

		bool IsSet(MemberInfo role);
		bool IsBag(MemberInfo role);
		bool IsList(MemberInfo role);
		bool IsArray(MemberInfo role);
		bool IsDictionary(MemberInfo role);
		bool IsComplex(MemberInfo member);
		bool IsVersion(MemberInfo member);

		IEnumerable<Type> GetBaseImplementors(Type ancestor);
		void AddToDomain(Type domainClass);
		void AddToDomain(IEnumerable<Type> domainClasses);
	}
}