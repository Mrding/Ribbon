using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class DictionaryCollectionPattern : AbstractCollectionPattern
	{
		#region Overrides of AbstractCollectionPattern

		protected override bool MemberMatch(MemberInfo subject)
		{
			Type memberType = subject.GetPropertyOrFieldType();
			if (typeof(IDictionary).IsAssignableFrom(memberType))
			{
				return true;
			}
			if (memberType.IsGenericType)
			{
				return memberType.GetGenericIntercafesTypeDefinitions().Contains(typeof(IDictionary<,>));
			}
			return false;
		}

		#endregion
	}
}