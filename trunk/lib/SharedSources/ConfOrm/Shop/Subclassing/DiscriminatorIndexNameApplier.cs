using System;
using ConfOrm.Mappers;

namespace ConfOrm.Shop.Subclassing
{
	public class DiscriminatorIndexNameApplier : IPatternApplier<Type, IClassAttributesMapper>
	{
		private readonly IDomainInspector domainInspector;

		public DiscriminatorIndexNameApplier(IDomainInspector domainInspector)
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
			applyTo.Discriminator(dm => dm.Column(cm => cm.Index("Ix" + subject.Name + "EntityType")));
		}

		#endregion
	}
}