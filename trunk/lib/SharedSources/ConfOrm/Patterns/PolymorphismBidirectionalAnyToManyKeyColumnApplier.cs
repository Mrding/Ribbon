using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class PolymorphismBidirectionalAnyToManyKeyColumnApplier : PolymorphismBidirectionalAnyToManyPattern, IPatternApplier<MemberInfo, ICollectionPropertiesMapper>
	{
		public PolymorphismBidirectionalAnyToManyKeyColumnApplier(IDomainInspector domainInspector) : base(domainInspector) { }

		public void Apply(MemberInfo subject, ICollectionPropertiesMapper applyTo)
		{
			var bidirectionaAnyMember = GetCadidatedBidirectional(subject);
			// Note: This implementation does not take in account possibile customization of the columns; it take only the default column name implemented in the AnyMapper
			// TODO: read the mapping of the element to know the column of the property (second-pass)
			applyTo.Key(km =>
			            {
										km.Column(bidirectionaAnyMember.Name + "Id");
										km.ForeignKey("none");
			            });
		}
	}
}