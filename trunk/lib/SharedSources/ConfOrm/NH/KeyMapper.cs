using System;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class KeyMapper: IKeyMapper
	{
		private readonly Type ownerEntityType;
		private readonly HbmKey mapping;

		public KeyMapper(Type ownerEntityType, HbmKey mapping)
		{
			this.ownerEntityType = ownerEntityType;
			this.mapping = mapping;
			this.mapping.column1 = DefaultColumnName(ownerEntityType);
		}

		public static string DefaultColumnName(Type ownerEntityType)
		{
			return ownerEntityType.Name.ToLowerInvariant() + "_key";
		}

		#region Implementation of IKeyMapper

		public void Column(string columnName)
		{
			if (string.IsNullOrEmpty(columnName))
			{
				throw new ArgumentNullException("columnName","Valid column name required.");
			}
			mapping.column1 = columnName;
		}

		public void OnDelete(OnDeleteAction deleteAction)
		{
			switch (deleteAction)
			{
				case OnDeleteAction.NoAction:
					mapping.ondelete = HbmOndelete.Noaction;
					break;
				case OnDeleteAction.Cascade:
					mapping.ondelete = HbmOndelete.Cascade;
					break;
			}
		}

		public void PropertyRef(MemberInfo property)
		{
			if (property == null)
			{
				mapping.propertyref = null;
				return;
			}
			if (!ownerEntityType.Equals(property.DeclaringType) && !ownerEntityType.Equals(property.ReflectedType))
			{
				throw new ArgumentOutOfRangeException("property", "Can't reference a property of another entity.");
			}
			mapping.propertyref = property.Name;
		}

		public void Update(bool consideredInUpdateQuery)
		{
			mapping.update = consideredInUpdateQuery;
			mapping.updateSpecified = true;
		}

		public void ForeignKey(string foreingKeyName)
		{
			if(foreingKeyName == null)
			{
				mapping.foreignkey = null;
				return;
			}
			string nameToAssign = foreingKeyName.Trim();
			if (string.Empty.Equals(nameToAssign))
			{
				mapping.foreignkey = "none";
				return;				
			}
			mapping.foreignkey = nameToAssign;
		}

		#endregion
	}
}