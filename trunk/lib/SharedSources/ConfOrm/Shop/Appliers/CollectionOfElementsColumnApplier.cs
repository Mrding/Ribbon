using ConfOrm.Mappers;
using ConfOrm.NH;

namespace ConfOrm.Shop.Appliers
{
	public abstract class CollectionOfElementsColumnApplier : CollectionOfElementsOnlyPattern, IPatternApplier<PropertyPath, IElementMapper>
	{
		protected CollectionOfElementsColumnApplier(IDomainInspector domainInspector) : base(domainInspector) {}

		#region Implementation of IPattern<PropertyPath>

		public bool Match(PropertyPath subject)
		{
			return base.Match(subject.LocalMember);
		}

		#endregion

		#region IPatternApplier<PropertyPath,IElementMapper> Members

		public void Apply(PropertyPath subject, IElementMapper applyTo)
		{
			applyTo.Column(GetColumnName(subject));
		}

		#endregion

		protected abstract string GetColumnName(PropertyPath subject);
	}
}