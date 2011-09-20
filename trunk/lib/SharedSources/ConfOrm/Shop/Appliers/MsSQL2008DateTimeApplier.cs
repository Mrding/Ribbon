using System;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate;

namespace ConfOrm.Shop.Appliers
{
	public class MsSQL2008DateTimeApplier : IPatternApplier<MemberInfo, IPropertyMapper>
	{
		#region Implementation of IPattern<MemberInfo>

		public virtual bool Match(MemberInfo subject)
		{
			if (subject == null)
			{
				return false;
			}
			Type propertyOrFieldType = subject.GetPropertyOrFieldType();
			string name = subject.Name;
			return !IsDate(name)
			       && (propertyOrFieldType == typeof (DateTime) || propertyOrFieldType == typeof (DateTime?));
		}

		#endregion

		#region Implementation of IPatternApplier<MemberInfo,IPropertyMapper>

		public void Apply(MemberInfo subject, IPropertyMapper applyTo)
		{
			applyTo.Type(NHibernateUtil.DateTime2);
		}

		#endregion

		protected virtual bool IsDate(string name)
		{
			return name.EndsWith("Date") || name.StartsWith("Date") || name.EndsWith("Day") || name.EndsWith("day") || name.StartsWith("Day");
		}
	}
}