using System;

namespace ConfOrm.Patterns
{
	public class DelegatedPattern<TSubject> : IPattern<TSubject>
	{
		private readonly Predicate<TSubject> matcher;

		public DelegatedPattern(Predicate<TSubject> matcher)
		{
			if (matcher == null)
			{
				throw new ArgumentNullException("matcher");
			}
			this.matcher = matcher;
		}

		#region IPattern<TSubject> Members

		public bool Match(TSubject subject)
		{
			return matcher(subject);
		}

		#endregion
	}
}