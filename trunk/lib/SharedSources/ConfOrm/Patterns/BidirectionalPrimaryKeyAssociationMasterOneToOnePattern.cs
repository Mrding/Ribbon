using System;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class BidirectionalPrimaryKeyAssociationMasterOneToOnePattern : BidirectionalUnaryAssociationPattern
	{
		public BidirectionalPrimaryKeyAssociationMasterOneToOnePattern(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			DomainInspector = domainInspector;
		}

		public IDomainInspector DomainInspector { get; private set; }

		public override bool Match(MemberInfo subject)
		{
			bool isBidirectionalUnaryAssociation = base.Match(subject);
			if (isBidirectionalUnaryAssociation)
			{
				Type from = subject.DeclaringType;
				Type to = subject.GetPropertyOrFieldType();
				return DomainInspector.IsOneToOne(from, to) && DomainInspector.IsMasterOneToOne(from, to)
				       && DomainInspector.IsOneToOne(to, from);
			}
			return false;
		}
	}
}