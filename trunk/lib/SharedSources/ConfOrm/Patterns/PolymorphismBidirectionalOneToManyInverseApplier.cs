using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class PolymorphismBidirectionalOneToManyInverseApplier : PolymorphismBidirectionalOneToManyMemberPattern, IPatternApplier<MemberInfo, ICollectionPropertiesMapper>
	{
		public PolymorphismBidirectionalOneToManyInverseApplier(IDomainInspector domainInspector) : base(domainInspector) {}

		#region Implementation of IPatternApplier<MemberInfo,ICollectionPropertiesMapper>

		public void Apply(MemberInfo subject, ICollectionPropertiesMapper applyTo)
		{
			applyTo.Inverse(true);
		}

		#endregion
	}
}