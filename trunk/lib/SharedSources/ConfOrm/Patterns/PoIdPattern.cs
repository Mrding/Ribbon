using System;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class PoIdPattern : IPattern<MemberInfo>
	{
		#region Implementation of IPattern<MemberInfo>

		public bool Match(MemberInfo subject)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			var name = subject.Name;
			return name.Equals("id", StringComparison.InvariantCultureIgnoreCase)
						 || name.Equals("poid", StringComparison.InvariantCultureIgnoreCase)
						 || name.Equals("oid", StringComparison.InvariantCultureIgnoreCase)
						 || (name.StartsWith(subject.DeclaringType.Name) && name.Equals(subject.DeclaringType.Name + "id", StringComparison.InvariantCultureIgnoreCase));
		}

		#endregion
	}
}