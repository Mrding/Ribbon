using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Type;

namespace ConfOrm.Shop.Appliers
{
	public class EnumPropertyAsStringApplier : IPatternApplier<MemberInfo, IPropertyMapper>
	{
		public bool Match(MemberInfo subject)
		{
			if (subject == null)
			{
				return false;
			}
			var memberType = subject.GetPropertyOrFieldType();
			return memberType.IsEnumOrNullableEnum() && !memberType.IsFlagEnumOrNullableFlagEnum();
		}

		public void Apply(MemberInfo subject, IPropertyMapper applyTo)
		{
			var memberType = subject.GetPropertyOrFieldType();
			applyTo.Type(typeof (EnumStringType<>).MakeGenericType(new[] {memberType}), null);
		}
	}
}