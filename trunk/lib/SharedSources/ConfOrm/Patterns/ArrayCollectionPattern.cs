using System;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class ArrayCollectionPattern : AbstractCollectionPattern
	{
		#region Implementation of AbstractCollectionPattern

		protected override bool MemberMatch(MemberInfo subject)
		{
			Type memberType = subject.GetPropertyOrFieldType();
			return memberType.IsArray && memberType.GetElementType() != typeof(byte);
		}

		#endregion
	}
}