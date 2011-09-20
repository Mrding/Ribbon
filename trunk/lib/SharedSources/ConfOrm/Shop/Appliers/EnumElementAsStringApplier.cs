using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Type;

namespace ConfOrm.Shop.Appliers
{
	public class EnumElementAsStringApplier : IPatternApplier<MemberInfo, IElementMapper>
	{
		#region IPatternApplier<MemberInfo,IElementMapper> Members

		public bool Match(MemberInfo subject)
		{
			var elementType = GetElementType(subject);
			return elementType.IsEnumOrNullableEnum() && !elementType.IsFlagEnumOrNullableFlagEnum();
		}

		public void Apply(MemberInfo subject, IElementMapper applyTo)
		{
			Type memberType = GetElementType(subject);
			applyTo.Type(typeof (EnumStringType<>).MakeGenericType(new[] {memberType}), null);
		}

		#endregion

		protected virtual Type GetElementType(MemberInfo subject)
		{
			Type memberType = subject.GetPropertyOrFieldType();
			if (!memberType.IsGenericCollection())
			{
				return null;
			}
			Type manyType = memberType.DetermineCollectionElementType();
			if (manyType.IsGenericType && typeof (KeyValuePair<,>) == manyType.GetGenericTypeDefinition())
			{
				return memberType.DetermineDictionaryValueType();
			}
			return manyType;
		}
	}
}