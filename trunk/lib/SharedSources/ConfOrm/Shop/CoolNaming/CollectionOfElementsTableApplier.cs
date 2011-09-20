using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;

namespace ConfOrm.Shop.CoolNaming
{
	public class CollectionOfElementsTableApplier : CollectionOfElementsOnlyPattern, IPatternApplier<PropertyPath, ICollectionPropertiesMapper>
	{
		public CollectionOfElementsTableApplier(IDomainInspector domainInspector) : base(domainInspector) {}

		#region Implementation of IPattern<PropertyPath>

		public bool Match(PropertyPath subject)
		{
			return base.Match(subject.LocalMember);
		}

		#endregion

		#region Implementation of IPatternApplier<PropertyPath,ICollectionPropertiesMapper>

		public void Apply(PropertyPath subject, ICollectionPropertiesMapper applyTo)
		{
			applyTo.Table(GetTableName(subject));
		}

		#endregion

		protected virtual string GetTableName(PropertyPath subject)
		{
			var entity = subject.GetContainerEntity(DomainInspector);
			return entity.Name + subject.ToColumnName();
		}
	}
}