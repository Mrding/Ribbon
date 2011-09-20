using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class BidirectionalPrimaryKeyAssociationMasterOneToOneApplier :
		BidirectionalPrimaryKeyAssociationMasterOneToOnePattern,
		IPatternApplier<MemberInfo, IOneToOneMapper>
	{
		public BidirectionalPrimaryKeyAssociationMasterOneToOneApplier(IDomainInspector domainInspector)
			: base(domainInspector)
		{
		}

		#region Implementation of IPatternApplier<MemberInfo,IOneToOneMapper>

		public void Apply(MemberInfo subject, IOneToOneMapper applyTo)
		{
			applyTo.Cascade(Cascade.All);
		}

		#endregion
	}
}