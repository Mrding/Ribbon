using System;
using System.Reflection;

namespace ConfOrm.Patterns
{
	/// <summary>
	/// Discover a unirectional one-to-many relation.
	/// </summary>
	public class UnidirectionalOneToManyMemberPattern: IPattern<MemberInfo>
	{
		public UnidirectionalOneToManyMemberPattern(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			DomainInspector = domainInspector;
		}

		public IDomainInspector DomainInspector { get; private set; }

		/// <summary>
		/// Check if the given <paramref name="subject"/> represent a collection of one-to-many relation.
		/// </summary>
		/// <param name="subject">A generic collection.</param>
		/// <returns>true if the element of the collection has a many-to-one relation with the collection owner.</returns>
		public bool Match(MemberInfo subject)
		{
			if (subject == null)
			{
				return false;
			}
			var elementType = subject.GetPropertyOrFieldType().DetermineCollectionElementOrDictionaryValueType();
			if (elementType == null)
			{
				return false;
			}
			if(DomainInspector.IsOneToMany(subject.DeclaringType, elementType))
			{
				if(elementType.GetFirstPropertyOfType(subject.DeclaringType) == null)
				{
					return true;
				}
			}
			return false;
		}
	}
}