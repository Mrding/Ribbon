using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.UserTypes;

namespace ConfOrm.Patterns
{
	public class CustomUserTypeInCollectionElementApplier : IPatternApplier<MemberInfo, IElementMapper>
	{
		private readonly Type userComplexType;
		private readonly Type nhibernateUserType;

		public CustomUserTypeInCollectionElementApplier(Type userComplexType, Type nhibernateUserType)
		{
			if (userComplexType == null)
			{
				throw new ArgumentNullException("userComplexType");
			}
			if (nhibernateUserType == null)
			{
				throw new ArgumentNullException("nhibernateUserType");
			}
			if (!typeof(IUserType).IsAssignableFrom(nhibernateUserType))
			{
				throw new ArgumentOutOfRangeException("nhibernateUserType",
				                                      "Expected a type implementing " + typeof (IUserType).FullName);
			}
			this.userComplexType = userComplexType;
			this.nhibernateUserType = nhibernateUserType;
		}

		#region IPatternApplier<MemberInfo,IElementMapper> Members

		public bool Match(MemberInfo subject)
		{
			Type collectionElementType = subject.GetPropertyOrFieldType().DetermineCollectionElementType();
			if (collectionElementType == null)
			{
				return false;
			}
			if (collectionElementType.IsGenericType && collectionElementType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
			{
				collectionElementType = collectionElementType.GetGenericArguments()[1];
			}
			return collectionElementType == userComplexType;
		}

		public void Apply(MemberInfo subject, IElementMapper applyTo)
		{
			applyTo.Type(nhibernateUserType, null);
		}

		#endregion
	}
}