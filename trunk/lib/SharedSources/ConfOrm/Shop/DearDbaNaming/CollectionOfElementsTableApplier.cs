using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;
using ConfOrm.Shop.Inflectors;

namespace ConfOrm.Shop.DearDbaNaming
{
	public class CollectionOfElementsTableApplier : CollectionOfElementsOnlyPattern, IPatternApplier<PropertyPath, ICollectionPropertiesMapper>
	{
		private readonly IInflector inflector;

		public CollectionOfElementsTableApplier(IDomainInspector domainInspector, IInflector inflector)
			: base(domainInspector)
		{
			this.inflector = inflector;
		}

		public bool Match(PropertyPath subject)
		{
			return base.Match(subject.LocalMember);
		}

		public void Apply(PropertyPath subject, ICollectionPropertiesMapper applyTo)
		{
			applyTo.Table(GetTableName(subject));
		}

		protected virtual string GetTableName(PropertyPath subject)
		{
			var entity = subject.GetContainerEntity(DomainInspector);
			return string.Format("{0}_{1}", inflector.Pluralize(entity.Name).ToUpperInvariant(), subject.ToColumnName("_").ToUpperInvariant());
		}
	}
}