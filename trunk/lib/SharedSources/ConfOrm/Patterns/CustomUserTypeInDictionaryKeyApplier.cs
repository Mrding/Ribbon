using System;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.UserTypes;

namespace ConfOrm.Patterns
{
	public class CustomUserTypeInDictionaryKeyApplier : IPatternApplier<MemberInfo, IMapKeyMapper>
	{
		private readonly Type userComplexType;
		private readonly Type nhibernateUserType;

		public CustomUserTypeInDictionaryKeyApplier(Type userComplexType, Type nhibernateUserType)
		{
			if (userComplexType == null)
			{
				throw new ArgumentNullException("userComplexType");
			}
			if (nhibernateUserType == null)
			{
				throw new ArgumentNullException("nhibernateUserType");
			}
			if (!typeof (IUserType).IsAssignableFrom(nhibernateUserType))
			{
				throw new ArgumentOutOfRangeException("nhibernateUserType",
				                                      "Expected a type implementing " + typeof (IUserType).FullName);
			}
			this.userComplexType = userComplexType;
			this.nhibernateUserType = nhibernateUserType;
		}

		public bool Match(MemberInfo subject)
		{
			Type dictionaryKeyType = subject.GetPropertyOrFieldType().DetermineDictionaryKeyType();
			if (dictionaryKeyType == null)
			{
				return false;
			}
			return dictionaryKeyType == userComplexType;
		}

		public void Apply(MemberInfo subject, IMapKeyMapper applyTo)
		{
			applyTo.Type(nhibernateUserType);
		}
	}
}