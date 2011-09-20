using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConfOrm.Shop.Inflectors
{
	public static class InflectorExtensions
	{
		private static readonly HashSet<IInflectorRuleApplier> UnaccentRules = new HashSet<IInflectorRuleApplier>();
		public const string UppercaseAccentedCharacters = "ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖØÙÚÛÜÝÞ";
		public const string LowercaseAccentedCharacters = "ßàáâãäåæçèéêëìíîïðñòóôõöøùúûüýþÿ";

		private static readonly Regex WordsSpliter =
			new Regex(string.Format(@"([A-Z{0}]+[a-z{1}\d]*)|[_\s]", UppercaseAccentedCharacters, LowercaseAccentedCharacters),
								RegexOptions.Compiled);

		static InflectorExtensions()
		{
			AddUnaccent("([ÀÁÂÃÄÅÆ])", "A");
			AddUnaccent("([Ç])", "C");
			AddUnaccent("([ÈÉÊË])", "E");
			AddUnaccent("([ÌÍÎÏ])", "I");
			AddUnaccent("([Ð])", "D");
			AddUnaccent("([Ñ])", "N");
			AddUnaccent("([ÒÓÔÕÖØ])", "O");
			AddUnaccent("([ÙÚÛÜ])", "U");
			AddUnaccent("([Ý])", "Y");
			AddUnaccent("([Þ])", "T");
			AddUnaccent("([ß])", "s");
			AddUnaccent("([àáâãäåæ])", "a");
			AddUnaccent("([ç])", "c");
			AddUnaccent("([èéêë])", "e");
			AddUnaccent("([ìíîï])", "i");
			AddUnaccent("([ð])", "e");
			AddUnaccent("([ñ])", "n");
			AddUnaccent("([òóôõöø])", "o");
			AddUnaccent("([ùúûü])", "u");
			AddUnaccent("([ý])", "y");
			AddUnaccent("([þ])", "t");
			AddUnaccent("([ÿ])", "y");
		}

		private static void AddUnaccent(string rule, string replacement)
		{
			UnaccentRules.Add(new CaseSensitiveRule(rule, replacement));
		}

		public static string Unaccent(this string word)
		{
			if (word == null)
			{
				return word;
			}
			return UnaccentRules.Aggregate(word, (current, rule) => rule.Apply(current));
		}

		public static IEnumerable<string> SplitWords(this string composedPascalCaseWords)
		{
			return from Match regex in WordsSpliter.Matches(composedPascalCaseWords) select regex.Value;
		}
	}
}