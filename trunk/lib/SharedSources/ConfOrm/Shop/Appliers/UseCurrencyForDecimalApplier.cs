using System.Reflection;
using ConfOrm.Mappers;
using NHibernate;

namespace ConfOrm.Shop.Appliers
{
	public class UseCurrencyForDecimalApplier : IPatternApplier<MemberInfo, IPropertyMapper>
	{
		public bool Match(MemberInfo subject)
		{
			if (subject == null)
			{
				return false;
			}
			return subject.GetPropertyOrFieldType() == typeof (decimal);
		}

		public void Apply(MemberInfo subject, IPropertyMapper applyTo)
		{
			applyTo.Type(NHibernateUtil.Currency);
		}
	}
}