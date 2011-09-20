using System;
using ConfOrm.Mappers;

namespace ConfOrm.Shop.Subclassing
{
	public class DiscriminatorColumnNameApplier : IPatternApplier<Type, IClassAttributesMapper>
	{
		private readonly IDomainInspector domainInspector;

		public DiscriminatorColumnNameApplier(IDomainInspector domainInspector)
		{
			this.domainInspector = domainInspector;
		}

		#region IPatternApplier<Type,IClassAttributesMapper> Members

		public bool Match(Type subject)
		{
			return domainInspector.IsTablePerClassHierarchy(subject);
		}

		public void Apply(Type subject, IClassAttributesMapper applyTo)
		{
			applyTo.Discriminator(dm => dm.Column("EntityType"));
		}

		#endregion
	}
}