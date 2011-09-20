using System;
using ConfOrm.Mappers;

namespace ConfOrm.Shop.DearDbaNaming
{
	public class JoinedSubclassKeyAsRootIdColumnApplier: IPatternApplier<Type, IJoinedSubclassAttributesMapper>
	{
		private readonly IDomainInspector domainInspector;

		public JoinedSubclassKeyAsRootIdColumnApplier(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			this.domainInspector = domainInspector;
		}


		public virtual bool Match(Type subject)
		{
			// this applier is called only for joined-subclasses
			return subject != null;
		}

		public void Apply(Type subject, IJoinedSubclassAttributesMapper applyTo)
		{
			var rootEntity = subject.GetRootEntity(domainInspector);
			applyTo.Key(km => km.Column(rootEntity.GetPoidColumnName()));
		}
	}
}