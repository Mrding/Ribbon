using System.Text.RegularExpressions;

namespace ConfOrm.Shop.Inflectors
{
	public class CaseSensitiveRule : AbstractInflectorRule
	{
		public CaseSensitiveRule(string pattern, string replacement) : base(pattern, replacement)
		{
		}

		public override string Apply(string word)
		{
			return Regex.Replace(word, Replacement);
		}

		protected override Regex CreateRegex()
		{
			return new Regex(Pattern, RegexOptions.Compiled);
		}
	}
}