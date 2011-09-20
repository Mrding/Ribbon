using System;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	/// <summary>
	/// Apply PropertyRef in the one-to-one side of a bidirectional ono-to-one association based on FK
	/// </summary>
	public class BidirectionalForeignKeyAssociationOneToOneApplier : BidirectionalUnaryAssociationPattern,
	                                                                 IPatternApplier<MemberInfo, IOneToOneMapper>
	{
		private readonly IDomainInspector domainInspector;

		public BidirectionalForeignKeyAssociationOneToOneApplier(IDomainInspector domainInspector)
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
				return domainInspector.IsOneToOne(from, to) && domainInspector.IsManyToOne(to, from);
			}
			return false;
		}

		#endregion

		#region Implementation of IPatternApplier<MemberInfo,IOneToOneMapper>

		public void Apply(MemberInfo subject, IOneToOneMapper applyTo)
		{
			Type oneToOneSideType = subject.DeclaringType;
			Type manyToOneSideType = subject.GetPropertyOrFieldType();

			applyTo.PropertyReference(GetPropertyOf(manyToOneSideType, oneToOneSideType));
		}

		#endregion

		protected MemberInfo GetPropertyOf(Type manyToOneSideType, Type oneToOneSideType)
		{
			return manyToOneSideType.GetProperties(PublicPropertiesOfClass).First(p => p.PropertyType == oneToOneSideType);
		}
	}
}