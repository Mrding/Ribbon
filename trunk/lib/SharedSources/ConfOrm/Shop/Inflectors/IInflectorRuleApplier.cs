namespace ConfOrm.Shop.Inflectors
{
	public interface IInflectorRuleApplier
	{
		/// <summary>
		/// Apply the rule to a word.
		/// </summary>
		/// <param name="word">The source word.</param>
		/// <returns>The result of the rule or null where the rule does not apply. </returns>
		string Apply(string word);
	}
}