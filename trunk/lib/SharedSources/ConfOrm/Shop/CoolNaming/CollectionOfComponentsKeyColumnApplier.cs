using System;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;

namespace ConfOrm.Shop.CoolNaming
{
	public class CollectionOfComponentsKeyColumnApplier : CollectionOfComponentsPattern,
	                                                      IPatternApplier<PropertyPath, ICollectionPropertiesMapper>
	{
		public CollectionOfComponentsKeyColumnApplier(IDomainInspector domainInspector) : base(domainInspector)
		{
		}

		#region IPatternApplier<PropertyPath,ICollectionPropertiesMapper> Members

		public bool Match(PropertyPath subject)
		{
			return Match(subject.LocalMember);
		}

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
			Type entity = subject.GetContainerEntity(DomainInspector);
			return entity.Name;
		}
	}
}