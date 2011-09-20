using System;
using System.Collections;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class BagCollectionPattern : AbstractCollectionPattern
	{
		#region Implementation of AbstractCollectionPattern

		protected override bool MemberMatch(MemberInfo subject)
		{
			Type memberType = subject.GetPropertyOrFieldType();
			// IEnumerable<T> inherits from IEnumerable
			return typeof(IEnumerable).IsAssignableFrom(memberType) && !IsNotSupportedAsBag(memberType);
		}

		#endregion

		private static bool IsNotSupportedAsBag(Type memberType)
		{
			return memberType == typeof(string) || memberType == typeof(byte[]);
		}
	}
}