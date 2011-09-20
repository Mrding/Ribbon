using System;
using ConfOrm.Mappers;

namespace ConfOrm.Shop.Subclassing
{
	public class ClassDiscriminatorValueAsClassNameApplier : IPatternApplier<Type, IClassAttributesMapper>
	{
		private readonly IDomainInspector domainInspector;

		public ClassDiscriminatorValueAsClassNameApplier(IDomainInspector domainInspector)
		{
			this.domainInspector = domainInspector;
		}

		#region IPatternApplier<Type,IClassMapper> Members

		public bool Match(Type subject)
		{
			return domainInspector.IsTablePerClassHierarchy(subject);
		}

		public virtual void Apply(Type subject, IClassAttributesMapper applyTo)
		{
			applyTo.DiscriminatorValue(subject.Name);
		}

		#endregion
	}
}