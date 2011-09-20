using System.Reflection;
using ConfOrm.Mappers;
using ConfOrm.NH;
using NHibernate;
using NHibernate.Type;

namespace ConfOrm.Patterns
{
	public class NoPoidGuidApplier: IPatternApplier<MemberInfo, IIdMapper>
	{
		#region Implementation of IPattern<MemberInfo>

		public bool Match(MemberInfo subject)
		{
			return subject == null;
		}

		#endregion

		#region Implementation of IPatternApplier<MemberInfo,IIdMapper>

		public void Apply(MemberInfo subject, IIdMapper applyTo)
		{
			applyTo.Generator(Generators.GuidComb);
			applyTo.Type((IIdentifierType)NHibernateUtil.Guid);
		}

		#endregion
	}
}