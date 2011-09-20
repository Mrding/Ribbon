using System;
using System.Linq;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class BidirectionalUnaryAssociationPattern : IPattern<MemberInfo>
	{
		protected const BindingFlags PublicPropertiesOfClass = BindingFlags.Public | BindingFlags.Instance;

		#region Implementation of IPattern<MemberInfo>

		public virtual bool Match(MemberInfo subject)
		{
			if (subject == null)
			{
				return false;
			}
			Type memberType = subject.GetPropertyOrFieldType();
			return HasPropertyOf(memberType, subject.DeclaringType);
		}

		#endregion

		protected bool HasPropertyOf(Type from, Type to)
		{
			return from.GetProperties(PublicPropertiesOfClass).Select(p => p.PropertyType).Any(t => t == to);
		}
	}
}