using System;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class BidirectionalOneToManyOnDeleteConstraintApplier : BidirectionalOneToManyPattern, IPatternApplier<MemberInfo, ICollectionPropertiesMapper>
	{
		public BidirectionalOneToManyOnDeleteConstraintApplier(IDomainInspector domainInspector) : base(domainInspector) {}

		public bool Match(MemberInfo subject)
		{
			var propertyType = subject.GetPropertyOrFieldType();
			Type many = propertyType.DetermineCollectionElementType();
			
			Type one = subject.DeclaringType;
			if (many == null)
			{
				return false;
			}
			if (one.Equals(many))
			{
				// Circular references
				return false;
			}
			Cascade? applyCascade = DomainInspector.ApplyCascade(one, subject, many);
			if(applyCascade.HasValue && !applyCascade.Value.Has(Cascade.DeleteOrphans)
				&& !applyCascade.Value.Has(Cascade.Remove) && !applyCascade.Value.Has(Cascade.All))
			{
				return false;
			}
			if (!DomainInspector.IsEntity(many))
			{
				return false;
			}
			if (DomainInspector.IsTablePerClass(many) && !DomainInspector.IsRootEntity(many))
			{
				return false;
			}
			return base.Match(new Relation(one, many));
		}

		#region Implementation of IPatternApplier<MemberInfo,ICollectionPropertiesMapper>

		public void Apply(MemberInfo subject, ICollectionPropertiesMapper applyTo)
		{
			applyTo.Key(km => km.OnDelete(OnDeleteAction.Cascade));
		}

		#endregion
	}
}