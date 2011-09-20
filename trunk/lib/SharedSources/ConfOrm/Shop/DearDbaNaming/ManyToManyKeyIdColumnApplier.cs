using System;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;

namespace ConfOrm.Shop.DearDbaNaming
{
	public class ManyToManyKeyIdColumnApplier: ManyToManyPattern, IPatternApplier<PropertyPath, ICollectionPropertiesMapper>
	{
		public ManyToManyKeyIdColumnApplier(IDomainInspector domainInspector) : base(domainInspector) {}

		#region Implementation of IPattern<PropertyPath>

		public bool Match(PropertyPath subject)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			return Match(subject.LocalMember);
		}

		#endregion

		#region Implementation of IPatternApplier<PropertyPath,ICollectionPropertiesMapper>

		public void Apply(PropertyPath subject, ICollectionPropertiesMapper applyTo)
		{
			applyTo.Key(km => km.Column(GetColumnNameForCollectionKey(subject)));
		}

		#endregion

		protected virtual string GetColumnNameForCollectionKey(PropertyPath subject)
		{
			var entityType = subject.GetContainerEntity(DomainInspector).GetRootEntity(DomainInspector);
			return entityType.GetPoidColumnName();
		}
	}
}