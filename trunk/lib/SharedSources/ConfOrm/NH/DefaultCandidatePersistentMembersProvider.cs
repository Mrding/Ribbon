using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfOrm.NH
{
	public class DefaultCandidatePersistentMembersProvider: ICandidatePersistentMembersProvider
	{
		internal const BindingFlags SubClassPropertiesBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
		internal const BindingFlags RootClassPropertiesBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
		internal const BindingFlags ComponentPropertiesBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

		#region Implementation of ICandidatePersistentMembersProvider

		public IEnumerable<MemberInfo> GetEntityMembersForPoid(Type entityClass)
		{
			return entityClass.IsInterface
			       	? entityClass.GetInterfaceProperties()
			       	: entityClass.GetProperties(RootClassPropertiesBindingFlags).Concat(GetFieldsOfHierarchy(entityClass));
		}

		public IEnumerable<MemberInfo> GetRootEntityMembers(Type entityClass)
		{
			return GetCandidatePersistentProperties(entityClass, RootClassPropertiesBindingFlags);
		}

		public IEnumerable<MemberInfo> GetSubEntityMembers(Type entityClass, Type entitySuperclass)
		{
			const BindingFlags flattenHierarchyBindingFlag =
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

			if (!entitySuperclass.Equals(entityClass.BaseType))
			{
				var propertiesOfSubclass = GetCandidatePersistentProperties(entityClass, flattenHierarchyBindingFlag);
				var propertiesOfBaseClass = GetCandidatePersistentProperties(entitySuperclass, flattenHierarchyBindingFlag);
				return propertiesOfSubclass.Except(propertiesOfBaseClass, new PropertyNameEqualityComparer());
			}
			else
			{
				return GetCandidatePersistentProperties(entityClass, SubClassPropertiesBindingFlags);
			}
		}

		public IEnumerable<MemberInfo> GetComponentMembers(Type componentClass)
		{
			return GetCandidatePersistentProperties(componentClass, ComponentPropertiesBindingFlags);
		}

		#endregion
		
		private IEnumerable<MemberInfo> GetFieldsOfHierarchy(Type type)
		{
			Type analizing = type;
			while (analizing != null && analizing != typeof(object))
			{
				foreach (var fieldInfo in analizing.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
				{
					yield return fieldInfo;
				}
				analizing = analizing.BaseType;
			}
		}

		private IEnumerable<MemberInfo> GetCandidatePersistentProperties(Type type, BindingFlags propertiesBindingFlags)
		{
			return type.IsInterface ? type.GetInterfaceProperties() : type.GetProperties(propertiesBindingFlags);
		}

		private class PropertyNameEqualityComparer : IEqualityComparer<MemberInfo>
		{
			#region Implementation of IEqualityComparer<MemberInfo>

			public bool Equals(MemberInfo x, MemberInfo y)
			{
				return x.Name == y.Name;
			}

			public int GetHashCode(MemberInfo obj)
			{
				return obj.Name.GetHashCode();
			}

			#endregion
		}
	}
}