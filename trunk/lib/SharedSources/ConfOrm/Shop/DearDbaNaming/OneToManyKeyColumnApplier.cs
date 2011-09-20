using System;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;

namespace ConfOrm.Shop.DearDbaNaming
{
	public class OneToManyKeyColumnApplier: OneToManyPattern, IPatternApplier<PropertyPath, ICollectionPropertiesMapper>
	{
		public OneToManyKeyColumnApplier(IDomainInspector domainInspector) : base(domainInspector) { }

		#region Implementation of IPattern<PropertyPath>

		public bool Match(PropertyPath subject)
		{
			return Match(subject.LocalMember);
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
			Type propertyType = subject.LocalMember.GetPropertyOrFieldType();
			Type childType = propertyType.DetermineCollectionElementType();
			var entity = subject.GetContainerEntity(DomainInspector);
			var parentPropertyInChild = childType.GetFirstPropertyOfType(entity);
			var baseName = parentPropertyInChild == null ? subject.PreviousPath == null ? entity.Name : entity.Name + subject.PreviousPath : parentPropertyInChild.Name;
			return GetKeyColumnName(baseName);
		}

		protected virtual string GetKeyColumnName(string baseName)
		{
			return string.Format("{0}_ID", baseName.ToUpperInvariant());
		}
	}
}