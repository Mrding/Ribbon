using System;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class BidirectionalManyToManyInverseApplier : BidirectionalManyToManyPattern,
	                                                     IPatternApplier<MemberInfo, ICollectionPropertiesMapper>
	{
		private readonly IDomainInspector domainInspector;

		public BidirectionalManyToManyInverseApplier(IDomainInspector domainInspector)
		{
			this.domainInspector = domainInspector;
		}

		#region IPatternApplier<MemberInfo,ICollectionPropertiesMapper> Members

		public override bool Match(MemberInfo subject)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			Relation relation = GetRelation(subject);
			bool isInverseEnd = !domainInspector.IsMasterManyToMany(relation.From, relation.To);
			return isInverseEnd && !IsCircularManyToMany(subject) && base.Match(subject);
		}

		public void Apply(MemberInfo subject, ICollectionPropertiesMapper applyTo)
		{
			applyTo.Inverse(true);
		}

		#endregion
	}
}