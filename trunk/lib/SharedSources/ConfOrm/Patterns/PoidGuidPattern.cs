using System;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class PoidGuidPattern : IPattern<MemberInfo>
	{
		public bool Match(MemberInfo subject)
		{
			if (subject == null)
			{
				return false;
			}
			var propertyOrFieldType = subject.GetPropertyOrFieldType();
			return propertyOrFieldType == typeof(Guid);
		}
	}
}