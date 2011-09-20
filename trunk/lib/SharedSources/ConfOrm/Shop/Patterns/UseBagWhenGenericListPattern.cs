using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Patterns;

namespace ConfOrm.Shop.Patterns
{
	public class UseBagWhenGenericListPattern : AbstractCollectionPattern
	{
		protected override bool MemberMatch(MemberInfo subject)
		{
			var memberType = subject.GetPropertyOrFieldType();
			return memberType.IsGenericType && memberType.GetGenericTypeDefinition() == typeof(IList<>);
		}
	}
}