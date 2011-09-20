using System;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Shop.CoolNaming
{
	public class JoinedSubclassKeyAsRootIdColumnApplier: IPatternApplier<Type, IJoinedSubclassAttributesMapper>
	{
		private readonly IDomainInspector domainInspector;

		public JoinedSubclassKeyAsRootIdColumnApplier(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			this.domainInspector = domainInspector;
		}

		#region Implementation of IPattern<Type>

		public virtual bool Match(Type subject)
		{
			return true;
		}

		#endregion

		#region Implementation of IPatternApplier<Type,IJoinedSubclassAttributesMapper>

		public void Apply(Type subject, IJoinedSubclassAttributesMapper applyTo)
		{
			var idMember = GetPoidPropertyOrField(subject);
			if(idMember != null)
			{
				applyTo.Key(km => km.Column(GetColumnNameForPoid(idMember)));
			}
		}

		#endregion

		protected MemberInfo GetPoidPropertyOrField(Type type)
		{
			return type.GetProperties().Cast<MemberInfo>().Concat(type.GetFields()).FirstOrDefault(mi => domainInspector.IsPersistentId(mi));
		}

		protected virtual string GetColumnNameForPoid(MemberInfo poidMember)
		{
			return poidMember.Name;
		}
	}
}