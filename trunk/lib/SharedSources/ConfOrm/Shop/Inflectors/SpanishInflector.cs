using System;
using System.Collections.Generic;

namespace ConfOrm.Shop.Inflectors
{
	/// <summary>
	/// Inflector for pluralize and singularize Spanish nouns.
	/// </summary>
	/// <remarks>
	/// Inspired from Bermi Ferrer Martinez python implementation.
	/// </remarks>
	public class SpanishInflector: AbstractInflector
	{
		private class RuleReplacement
		{
			public string Rule { get; set; }
			public string Replacement { get; set; }
		}

		public SpanishInflector()
		{
		}

		public override void AddIrregular(string singular, string plural)
		{
			base.AddIrregular(singular, plural);
			base.AddIrregular(singular.Unaccent(), plural.Unaccent());
		}

		protected override void AddUncountable(string word)
		{
			base.AddUncountable(word);
			base.AddUncountable(word.Unaccent());
		}

		private void AddPluralForEach(string charectersToMatch, string charectersToReplace, char wildChar, string ruleTemplate,
																	string replacementTemplate)
		{
			IEnumerable<RuleReplacement> e = RulesReplacements(charectersToMatch, charectersToReplace, wildChar, ruleTemplate,
																												 replacementTemplate);
			foreach (var element in e)
			{
				AddPlural(element.Rule, element.Replacement);
			}
		}

		private void AddSingularForEach(string charectersToMatch, string charectersToReplace, char wildChar,
																		string ruleTemplate, string replacementTemplate)
		{
			IEnumerable<RuleReplacement> e = RulesReplacements(charectersToMatch, charectersToReplace, wildChar, ruleTemplate,
																												 replacementTemplate);
			foreach (var element in e)
			{
				AddSingular(element.Rule, element.Replacement);
			}
		}

		private static IEnumerable<RuleReplacement> RulesReplacements(string charectersToMatch, string charectersToReplace,
																																	char wildChar, string ruleTemplate,
																																	string replacementTemplate)
		{
			if (charectersToMatch.Length != charectersToReplace.Length)
			{
				throw new ArgumentException("charectersToMatch and charectersToReplace must have same length", charectersToReplace);
			}
			for (int i = 0; i < charectersToMatch.Length; i++)
			{
				string rule = ruleTemplate.Replace(wildChar, charectersToMatch[i]);
				string replacement = replacementTemplate.Replace(wildChar, charectersToReplace[i]);
				yield return new RuleReplacement { Rule = rule, Replacement = replacement };
			}
		}

	}
}