using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;

namespace ConfOrm.Shop.CoolNaming
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
			string baseName = GetBaseName(subject);
			return string.Format("{0}Id", baseName);
		}

		protected virtual string GetBaseName(PropertyPath subject)
		{
			var entity = subject.GetContainerEntity(DomainInspector);
			return entity.Name;
		}
	}
}