using System.Collections.Generic;

namespace ConfOrm.Shop.Inflectors
{
	/// <summary>
	/// Base implementation.
	/// </summary>
	/// <remarks>
	/// It is part of the originally implemented by http://andrewpeters.net/inflectornet/ as static class.
	/// copied from my (Fabio Maulo) implementation in http://code.google.com/p/unhaddins/
	/// </remarks>
	public class AbstractInflector : IInflector
	{
		private readonly List<IInflectorRuleApplier> plurals = new List<IInflectorRuleApplier>();
		private readonly List<IInflectorRuleApplier> singulars = new List<IInflectorRuleApplier>();
		private readonly HashSet<string> uncountables = new HashSet<string>();

		#region IInflector Members

		public virtual string Pluralize(string word)
		{
			return ApplyFirstMatchRule(plurals, word);
		}

		public virtual string Singularize(string word)
		{
			return ApplyFirstMatchRule(singulars, word);
		}

		#endregion

		public virtual void AddIrregular(string singular, string plural)
		{
			AddPlural("(" + singular[0] + ")" + singular.Substring(1) + "$", "$1" + plural.Substring(1));
			AddSingular("(" + plural[0] + ")" + plural.Substring(1) + "$", "$1" + singular.Substring(1));
		}

		protected virtual void AddUncountable(string word)
		{
			uncountables.Add(word.ToLower());
		}

		protected void AddPlural(string rule, string replacement)
		{
			plurals.Add(new NounsRule(rule, replacement));
		}

		protected void AddSingular(string rule, string replacement)
		{
			singulars.Add(new NounsRule(rule, replacement));
		}

		/// <summary>
		/// Applay first match starting from last rule.
		/// </summary>
		/// <param name="rules">The ordered list of rules.</param>
		/// <param name="word">The word where apply the rule</param>
		/// <returns>The result of rule applied or the <paramref name="word"/> where no rule found.</returns>
		protected virtual string ApplyFirstMatchRule(IList<IInflectorRuleApplier> rules, string word)
		{
			string result = null;

			if (!uncountables.Contains(word.ToLower()))
			{
				for (int i = rules.Count - 1; i >= 0 && result == null; i--)
				{
					result = rules[i].Apply(word);
				}
			}
			return result ?? word;
		}
	}
}