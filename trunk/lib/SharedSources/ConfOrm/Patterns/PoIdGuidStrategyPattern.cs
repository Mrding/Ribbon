using System;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class PoIdGuidStrategyPattern : IPattern<MemberInfo>
	{
		#region Implementation of IPattern<MemberInfo>

		public bool Match(MemberInfo subject)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			var propertyOrFieldType = subject.GetPropertyOrFieldType();
			return propertyOrFieldType  == typeof(Guid);
		}

		#endregion
	}
}