using System;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class ListIndexMapper : IListIndexMapper
	{
		private readonly Type ownerEntityType;
		private readonly HbmListIndex mapping;

		public ListIndexMapper(Type ownerEntityType, HbmListIndex mapping)
		{
			this.ownerEntityType = ownerEntityType;
			this.mapping = mapping;
		}

		#region Implementation of IListIndexMapper

		public void Column(string columnName)
		{
			mapping.column1 = columnName;
		}

		public void Base(int baseIndex)
		{
			if (baseIndex <= 0)
			{
				throw new ArgumentOutOfRangeException("baseIndex", "The baseIndex should be positive value");
			}

			mapping.@base = baseIndex.ToString();
		}

		#endregion
	}
}