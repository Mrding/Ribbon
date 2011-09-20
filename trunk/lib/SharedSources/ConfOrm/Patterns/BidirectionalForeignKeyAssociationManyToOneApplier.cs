using System;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	/// <summary>
	/// Apply Unique and Cascade in the many-to-one side of a bidirectional ono-to-one association based on FK
	/// </summary>
	public class BidirectionalForeignKeyAssociationManyToOneApplier: BidirectionalUnaryAssociationPattern, IPatternApplier<MemberInfo, IManyToOneMapper>
	{
		private readonly IDomainInspector domainInspector;

		public BidirectionalForeignKeyAssociationManyToOneApplier(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			this.domainInspector = domainInspector;
		}

		public override bool Match(MemberInfo subject)
		{
			var isBidirectionalUnaryAssociation = base.Match(subject);
			if (isBidirectionalUnaryAssociation)
			{
				var from = subject.DeclaringType;
				var to = subject.GetPropertyOrFieldType();
				return domainInspector.IsManyToOne(from, to) && domainInspector.IsOneToOne(to, from);
			}
			return false;
		}

		#region Implementation of IPatternApplier<MemberInfo,IManyToOneMapper>

		public void Apply(MemberInfo subject, IManyToOneMapper applyTo)
		{
			applyTo.Unique(true);
			applyTo.Cascade(Cascade.All);
		}

		#endregion
	}
}