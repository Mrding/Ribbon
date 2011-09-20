using System;
using ConfOrm.Shop.NamingAppliers;

namespace ConfOrm.Shop.DearDbaNaming
{
	public class PoidColumnNameApplier : AbstractPoidColumnNameApplier
	{
		public override string GetPoidColumnName(Type subject)
		{
			return subject.GetPoidColumnName();
		}
	}
}