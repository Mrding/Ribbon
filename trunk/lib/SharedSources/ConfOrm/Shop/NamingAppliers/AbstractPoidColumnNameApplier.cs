using System;
using ConfOrm.Mappers;

namespace ConfOrm.Shop.NamingAppliers
{
	public abstract class AbstractPoidColumnNameApplier : IPatternApplier<Type, IClassAttributesMapper>
	{
		public bool Match(Type subject)
		{
			// this patter is called only for root-entities
			return subject != null;
		}

		public void Apply(Type subject, IClassAttributesMapper applyTo)
		{
			applyTo.Id(idm => idm.Column(GetPoidColumnName(subject)));
		}

		public abstract string GetPoidColumnName(Type subject);
	}
}