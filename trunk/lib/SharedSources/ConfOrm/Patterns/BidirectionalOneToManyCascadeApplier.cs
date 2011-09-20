using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class BidirectionalOneToManyCascadeApplier : BidirectionalOneToManyMemberPattern, IPatternApplier<MemberInfo, ICollectionPropertiesMapper>
	{
		public BidirectionalOneToManyCascadeApplier(IDomainInspector domainInspector) : base(domainInspector)
		{
		}

		public override bool Match(MemberInfo subject)
		{
			// does not match even when an explicit cascade is provided by DomainInspector
			var relation = GetRelation(subject);
			return relation != null && !DomainInspector.ApplyCascade(relation.From, subject, relation.To).HasValue && base.Match(relation);
		}

		public void Apply(MemberInfo subject, ICollectionPropertiesMapper applyTo)
		{
			applyTo.Cascade(Cascade.All | Cascade.DeleteOrphans);
		}
	}
}