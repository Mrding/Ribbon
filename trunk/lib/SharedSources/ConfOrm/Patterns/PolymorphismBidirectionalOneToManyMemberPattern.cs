using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfOrm.Patterns
{
	/// <summary>
	/// Match only where the member is a generic-collection, the element has a property of the type of the collection container 
	/// applying polymorphism based on interface or classes.
	/// </summary>
	public class PolymorphismBidirectionalOneToManyMemberPattern : IPattern<MemberInfo>
	{
		private readonly IDomainInspector domainInspector;

		public PolymorphismBidirectionalOneToManyMemberPattern(IDomainInspector domainInspector)
		{
			this.domainInspector = domainInspector;
		}

		protected IDomainInspector DomainInspector
		{
			get { return domainInspector; }
		}

		public virtual bool Match(MemberInfo subject)
		{
			if (subject == null)
			{
				return false;
			}
			var declaredMany = subject.GetPropertyOrFieldType().DetermineCollectionElementType();
			if (declaredMany == null)
			{
				return false;
			}
			
			var implementorsOfMany = domainInspector.GetBaseImplementors(declaredMany).ToArray();
			if (implementorsOfMany.Length != 1)
			{
				// short
				return false;
			}

			var declaredOne = subject.ReflectedType;
			var implementorsOfOne = domainInspector.GetBaseImplementors(declaredOne).ToArray();
			if (implementorsOfOne.Length != 1)
			{
				return false;
			}

			var many = implementorsOfMany[0];
			var one = implementorsOfOne[0];
			if (domainInspector.IsManyToMany(one, many))
			{
				return false;
			}

			List<Type> candidateAncestorsOfOne = GetCandidateAncestorsOf(one).ToList();

			bool isPolymorphicRelation = !declaredMany.Equals(many) || !declaredOne.Equals(one) || candidateAncestorsOfOne.Count > 0;
			return isPolymorphicRelation && (many.HasPublicPropertyOf(one, p=> DomainInspector.IsPersistentProperty(p)) || HasPublicPropertyOf(many, candidateAncestorsOfOne));
		}

		protected IEnumerable<Type> GetCandidateAncestorsOf(Type one)
		{
			return from ancestor in one.GetBaseTypes()
			       let implementors = domainInspector.GetBaseImplementors(ancestor)
			       where implementors.IsSingle(t => t.Equals(one))
			       select ancestor;
		}

		protected bool HasPublicPropertyOf(Type many, IEnumerable<Type> candidateAncestors)
		{
			return candidateAncestors.Any(candidateAncestor => many.HasPublicPropertyOf(candidateAncestor, p => DomainInspector.IsPersistentProperty(p)));
		}

		protected IEnumerable<Relation> GetRelations(MemberInfo subject)
		{
			var declaredMany = subject.GetPropertyOrFieldType().DetermineCollectionElementType();
			var declaredOne = subject.ReflectedType;
			yield return new Relation(declaredOne, declaredMany);
			var implementorsOfMany = DomainInspector.GetBaseImplementors(declaredMany).ToArray();
			var implementorsOfOne = DomainInspector.GetBaseImplementors(declaredOne).ToArray();

			var many = implementorsOfMany[0];
			var one = implementorsOfOne[0];
			if (declaredOne != one || declaredMany != many)
			{
				yield return new Relation(one, many);
			}
			foreach (var type in GetCandidateAncestorsOf(one))
			{
				yield return new Relation(type, declaredMany);
				yield return new Relation(type, many);
			}
		}

		protected Cascade? GetExplicitPolymorphismCascade(MemberInfo subject)
		{
			return (from relation in GetRelations(subject)
							let cascade = DomainInspector.ApplyCascade(relation.From, subject, relation.To)
							where cascade.HasValue
							select cascade).FirstOrDefault();
		}
	}
}