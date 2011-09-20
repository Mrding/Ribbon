using System;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class BidirectionalOneToManyMemberPattern : BidirectionalOneToManyPattern, IPattern<MemberInfo>
	{
		public BidirectionalOneToManyMemberPattern(IDomainInspector domainInspector) : base(domainInspector) {}

		#region Implementation of IPattern<MemberInfo>

		public virtual bool Match(MemberInfo subject)
		{
			var relation = GetRelation(subject);
			return relation != null && base.Match(relation);
		}

		#endregion

		protected Relation GetRelation(MemberInfo collectionMember)
		{
			var propertyType = collectionMember.GetPropertyOrFieldType();
			Type many = propertyType.DetermineCollectionElementType();
			Type one = collectionMember.ReflectedType;
			if (many == null)
			{
				return null;
			}
			return new Relation(one, many);
		}
	}
}