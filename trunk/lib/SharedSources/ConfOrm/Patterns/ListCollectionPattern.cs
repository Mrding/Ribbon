using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class ListCollectionPattern : AbstractCollectionPattern
	{
		private readonly IDomainInspector domainInspector;

		public ListCollectionPattern(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			this.domainInspector = domainInspector;
		}

		#region Implementation of AbstractCollectionPattern

		protected override bool MemberMatch(MemberInfo subject)
		{
			Type memberType = subject.GetPropertyOrFieldType();
			if (IsNotSupportedAsList(memberType))
			{
				return false;
			}
			if (typeof (IList).IsAssignableFrom(memberType))
			{
				return true;
			}
			if (memberType.IsGenericType)
			{
				var isList = memberType.GetGenericIntercafesTypeDefinitions().Contains(typeof(IList<>));
				// a bidirectional one-to-many should use Bag or Set
				return isList && !(new BidirectionalOneToManyPattern(domainInspector).Match(new Relation(subject.DeclaringType, memberType.DetermineCollectionElementType())));
			}
			return false;
		}

		#endregion

		protected virtual bool IsNotSupportedAsList(Type memberType)
		{
			return memberType == typeof(byte[]);
		}
	}
}