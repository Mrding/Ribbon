namespace ConfOrm.Shop.Inflectors
{
	public interface IInflectorReplacementRule : IInflectorRuleApplier
	{
		string Replacement { get; }
		string Pattern { get; }
	}
}