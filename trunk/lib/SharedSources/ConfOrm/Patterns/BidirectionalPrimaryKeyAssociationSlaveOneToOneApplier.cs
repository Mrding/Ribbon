using System;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class BidirectionalPrimaryKeyAssociationSlaveOneToOneApplier : BidirectionalUnaryAssociationPattern,
																																	 IPatternApplier<MemberInfo, IOneToOneMapper>
	{
		private readonly IDomainInspector domainInspector;

		public BidirectionalPrimaryKeyAssociationSlaveOneToOneApplier(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			this.domainInspector = domainInspector;
		}

		#region IPatternApplier<MemberInfo,IOneToOneMapper> Members

		public override bool Match(MemberInfo subject)
		{
			bool isBidirectionalUnaryAssociation = base.Match(subject);
			if (isBidirectionalUnaryAssociation)
			{
				Type from = subject.DeclaringType;
				Type to = subject.GetPropertyOrFieldType();
				return domainInspector.IsOneToOne(from, to) && !domainInspector.IsMasterOneToOne(from, to)
				       && domainInspector.IsOneToOne(to, from);
			}
			return false;
		}

		#endregion

		#region Implementation of IPatternApplier<MemberInfo,IOneToOneMapper>

		public void Apply(MemberInfo subject, IOneToOneMapper applyTo)
		{
			applyTo.Constrained(true);
		}

		#endregion
	}
}