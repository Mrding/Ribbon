using ConfOrm.Shop.Inflectors;

namespace ConfOrm.Shop.DearDbaNaming
{
	public class ClassPluralizedTableApplier : InflectorNaming.ClassPluralizedTableApplier
	{
		public ClassPluralizedTableApplier(IInflector inflector) : base(inflector)
		{
		}

		public override string GetTableName(System.Type subject)
		{
			return base.GetTableName(subject).ToUpperInvariant();
		}
	}
}