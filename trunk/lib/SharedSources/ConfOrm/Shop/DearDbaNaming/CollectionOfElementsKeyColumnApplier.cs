using System;
using System.Linq;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;

namespace ConfOrm.Shop.DearDbaNaming
{
	public class CollectionOfElementsKeyColumnApplier : CollectionOfElementsOnlyPattern, IPatternApplier<PropertyPath, ICollectionPropertiesMapper>
	{

		#region Implementation of IPattern<PropertyPath>

		public CollectionOfElementsKeyColumnApplier(IDomainInspector domainInspector) : base(domainInspector) {}

		public bool Match(PropertyPath subject)
		{
			return base.Match(subject.LocalMember);
		}

		#endregion

		#region Implementation of IPatternApplier<PropertyPath,ICollectionPropertiesMapper>

		public void Apply(PropertyPath subject, ICollectionPropertiesMapper applyTo)
		{
			applyTo.Key(km => km.Column(GetKeyColumnName(subject)));
		}

		#endregion

		protected virtual string GetKeyColumnName(PropertyPath subject)
		{
			Type entity = subject.GetContainerEntity(DomainInspector).GetRootEntity(DomainInspector);
			return entity.GetPoidColumnName();
		}
	}
}