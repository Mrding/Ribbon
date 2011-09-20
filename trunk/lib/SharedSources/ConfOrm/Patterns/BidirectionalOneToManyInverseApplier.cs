using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class BidirectionalOneToManyInverseApplier : BidirectionalOneToManyMemberPattern, IPatternApplier<MemberInfo, ICollectionPropertiesMapper>
	{
		public BidirectionalOneToManyInverseApplier(IDomainInspector domainInspector) : base(domainInspector) {}

		public override bool Match(MemberInfo subject)
		{
			var relation = GetRelation(subject);
			if (relation == null || !DomainInspector.IsEntity(relation.To))
			{
				return false;
			}

			return base.Match(subject);
		}

		#region Implementation of IPatternApplier<MemberInfo,ICollectionPropertiesMapper>

		public void Apply(MemberInfo subject, ICollectionPropertiesMapper applyTo)
		{
			applyTo.Inverse(true);
		}

		#endregion
	}
}