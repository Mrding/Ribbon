using ConfOrm.NH;

namespace ConfOrm.Shop.CoolNaming
{
	public class CollectionOfElementsColumnApplier : Appliers.CollectionOfElementsColumnApplier
	{
		public CollectionOfElementsColumnApplier(IDomainInspector domainInspector) : base(domainInspector) {}

		protected override string GetColumnName(PropertyPath subject)
		{
			return subject.ToColumnName() + "Element";
		}
	}
}