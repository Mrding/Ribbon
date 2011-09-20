using System;
using System.Text.RegularExpressions;

namespace ConfOrm.Shop.Inflectors
{
	/// <summary>
	/// Base rule with equaly implementation (needed to avoid duplications).
	/// </summary>
	/// <remarks>
	/// copied from my (Fabio Maulo) implementation in http://code.google.com/p/unhaddins/
	/// </remarks>
	public abstract class AbstractInflectorRule : IInflectorReplacementRule
	{
		private readonly int hashCode;

		protected AbstractInflectorRule(string pattern, string replacement)
		{
			if (string.IsNullOrEmpty(pattern))
			{
				throw new ArgumentNullException("pattern");
			}
			if (replacement == null)
			{
				throw new ArgumentNullException("replacement");
			}
			Pattern = pattern;
			Replacement = replacement;
			hashCode = 397 ^ Replacement.GetHashCode() ^ Pattern.GetHashCode();
			Regex = CreateRegex();
		}

		public string Replacement { get; private set; }

		public string Pattern { get; private set; }
		public abstract string Apply(string word);

		protected Regex Regex { get; private set; }
		protected abstract Regex CreateRegex();

		public override bool Equals(object obj)
		{
			return Equals(obj as IInflectorReplacementRule);
		}

		public bool Equals(IInflectorReplacementRule other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other.Pattern, Pattern) && Equals(other.Replacement, Replacement);
		}

		public override int GetHashCode()
		{
			return hashCode;
		}
	}
}