using System;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Type;

namespace ConfOrm.NH
{
	public static class TypeNameUtil
	{
		public static string GetNhTypeName(this Type type)
		{
			string typeName;
			var nhType = TypeFactory.HeuristicType(type.AssemblyQualifiedName);
			if (nhType != null)
			{
				typeName = nhType.Name;
			}
			else
			{
				typeName = type.FullName + ", " + type.Assembly.GetName().Name;
			}
			return typeName;
		}

		public static string GetShortClassName(this Type type, HbmMapping mapDoc)
		{
			if (type == null)
			{
				return null;
			}
			if (mapDoc == null)
			{
				return type.AssemblyQualifiedName;
			}
			var typeAssembly = type.Assembly.GetName().Name;
			var typeAssemblyFullName = type.Assembly.FullName;
			var typeNameSpace = type.Namespace;
			string assembly = null;
			if (!typeAssembly.Equals(mapDoc.assembly) && !typeAssemblyFullName.Equals(mapDoc.assembly))
			{
				assembly = typeAssembly;
			}
			string @namespace = null;
			if (!typeNameSpace.Equals(mapDoc.@namespace))
			{
				@namespace = typeNameSpace;
			}
			if (!string.IsNullOrEmpty(assembly) && !string.IsNullOrEmpty(@namespace))
			{
				return type.AssemblyQualifiedName;
			}
			if (!string.IsNullOrEmpty(assembly) && string.IsNullOrEmpty(@namespace))
			{
				return string.Concat(type.Name, ", ", assembly);
			}
			if (string.IsNullOrEmpty(assembly) && !string.IsNullOrEmpty(@namespace))
			{
				return type.FullName;
			}

			return type.Name;
		}
	}
}