using System;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;

namespace ConfOrm.Shop.CoolNaming
{
	public class CollectionOfComponentsTableApplier : CollectionOfComponentsPattern,
	                                                  IPatternApplier<PropertyPath, ICollectionPropertiesMapper>
	{
		public CollectionOfComponentsTableApplier(IDomainInspector domainInspector) : base(domainInspector)
		{
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
			Type entity = subject.GetContainerEntity(DomainInspector);
			return entity.Name + subject.ToColumnName();
		}
	}
}