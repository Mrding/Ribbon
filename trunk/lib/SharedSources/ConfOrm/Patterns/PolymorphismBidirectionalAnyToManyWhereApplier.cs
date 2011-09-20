using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class PolymorphismBidirectionalAnyToManyWhereApplier : PolymorphismBidirectionalAnyToManyPattern, IPatternApplier<MemberInfo, ICollectionPropertiesMapper>
	{
		public PolymorphismBidirectionalAnyToManyWhereApplier(IDomainInspector domainInspector) : base(domainInspector) { }

		public void Apply(MemberInfo subject, ICollectionPropertiesMapper applyTo)
		{
			var bidirectionaAnyMember = GetCadidatedBidirectional(subject);
			// Note: This implementation does not take in account possibile customization of the columns; it take only the default column name implemented in the AnyMapper
			// TODO: read the mapping of the element to know the column of the property (second-pass)
			string columnNameForClass = bidirectionaAnyMember.Name + "Class";
			applyTo.Where(string.Format("{0} = '{1}'", columnNameForClass, subject.ReflectedType.FullName));
		}
	}
}