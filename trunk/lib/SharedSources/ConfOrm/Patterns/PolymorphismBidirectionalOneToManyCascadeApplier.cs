using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class PolymorphismBidirectionalOneToManyCascadeApplier: PolymorphismBidirectionalOneToManyMemberPattern, IPatternApplier<MemberInfo, ICollectionPropertiesMapper>
	{
		public PolymorphismBidirectionalOneToManyCascadeApplier(IDomainInspector domainInspector) : base(domainInspector) {}
		
		public void Apply(MemberInfo subject, ICollectionPropertiesMapper applyTo)
		{
			Cascade? explicitPolymorphismCascade = GetExplicitPolymorphismCascade(subject);
			if(explicitPolymorphismCascade.HasValue)
			{
				applyTo.Cascade(explicitPolymorphismCascade.Value);								
			}
			else
			{
				applyTo.Cascade(Cascade.All | Cascade.DeleteOrphans);				
			}
		}
	}
}