using System;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;
using ConfOrm.Shop.Inflectors;

namespace ConfOrm.Shop.DearDbaNaming
{
	public class CollectionOfComponentsTableApplier : CollectionOfComponentsPattern,
																										IPatternApplier<PropertyPath, ICollectionPropertiesMapper>
	{
		private readonly IInflector inflector;

		public CollectionOfComponentsTableApplier(IDomainInspector domainInspector, IInflector inflector)
			: base(domainInspector)
		{
			this.inflector = inflector;
		}

		#region IPatternApplier<PropertyPath,ICollectionPropertiesMapper> Members

		public bool Match(PropertyPath subject)
		{
			return Match(subject.LocalMember);
		}

		public void Apply(PropertyPath subject, ICollectionPropertiesMapper applyTo)
		{
			applyTo.Table(GetTableName(subject));
		}

		#endregion

		protected virtual string GetTableName(PropertyPath subject)
		{
			Type entity = subject.GetContainerEntity(DomainInspector).GetRootEntity(DomainInspector);
			return string.Format("{0}_{1}", inflector.Pluralize(entity.Name).ToUpperInvariant() , subject.ToColumnName("_").ToUpperInvariant());
		}
	}
}