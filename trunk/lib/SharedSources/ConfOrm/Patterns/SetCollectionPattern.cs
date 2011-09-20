using System.Linq;
using System.Reflection;
using Iesi.Collections;
using Iesi.Collections.Generic;

namespace ConfOrm.Patterns
{
	public class SetCollectionPattern : AbstractCollectionPattern
	{
		#region Implementation of IPattern<MemberInfo>

		protected override bool MemberMatch(MemberInfo subject)
		{
			var memberType = subject.GetPropertyOrFieldType();
			if (typeof (ISet).IsAssignableFrom(memberType))
			{
				return true;
			}
			if (memberType.IsGenericType)
			{
				return memberType.GetGenericIntercafesTypeDefinitions().Contains(typeof (ISet<>));
			}
			return false;
		}

		#endregion
	}
}