using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ConfOrm
{
	public static class ForClass<T>
	{
		private const BindingFlags DefaultFlags =
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

		public static MemberInfo Property(Expression<Func<T, object>> propertyGetter)
		{
			if (propertyGetter == null)
			{
				return null;
			}
			return TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
		}

		public static MemberInfo Field(string fieldName)
		{
			if (fieldName == null)
			{
				return null;
			}

			return GetField(typeof(T), fieldName);
		}

		private static MemberInfo GetField(Type type, string fieldName)
		{
			if(type == typeof(object) || type == null)
			{
				return null;
			}
			MemberInfo member = type.GetField(fieldName, DefaultFlags) ?? GetField(type.BaseType, fieldName);
			return member;
		}

		public static MemberInfo Property(string propertyName)
		{
			if (propertyName == null)
			{
				return null;
			}

			return GetProperty(typeof(T), propertyName);
		}

		private static MemberInfo GetProperty(Type type, string propertyName)
		{
			if (type == typeof(object) || type == null)
			{
				return null;
			}
			MemberInfo member = type.GetProperty(propertyName, DefaultFlags) ?? GetProperty(type.BaseType, propertyName);
			return member;
		}
	}
}