namespace ConfOrm.Shop.CoolNaming
{
	public class UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier : ConfOrm.Patterns.UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier
	{
		public UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier(IDomainInspector domainInspector) : base(domainInspector)
		{
		}

		protected override string GetColumnName(NH.PropertyPath subject)
		{
			return GetBaseColumnName(subject) + "Id";
		}
	}
}