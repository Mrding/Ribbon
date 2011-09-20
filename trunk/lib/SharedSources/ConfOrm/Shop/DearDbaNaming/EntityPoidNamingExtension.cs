using System;

namespace ConfOrm.Shop.DearDbaNaming
{
	public static class EntityPoidNamingExtension
	{
		public static string GetPoidColumnName(this Type subject)
		{
			return subject.Name.ToUpperInvariant() + "_ID";
		}
	}
}