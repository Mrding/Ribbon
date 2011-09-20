using System;

namespace ConfOrm.Patterns
{
	public class DelegatedAdvancedApplier<TSubject, TApplyTo> : IPatternApplier<TSubject, TApplyTo>
	{
		private readonly Action<TSubject, TApplyTo> applier;
		private readonly Predicate<TSubject> matcher;

		public DelegatedAdvancedApplier(Predicate<TSubject> matcher, Action<TSubject, TApplyTo> applier)
		{
			if (matcher == null)
			{
				throw new ArgumentNullException("matcher");
			}
			if (applier == null)
			{
				throw new ArgumentNullException("applier");
			}
			this.matcher = matcher;
			this.applier = applier;
		}

		#region Implementation of IPattern<MemberInfo>

		public bool Match(TSubject subject)
		{
			return matcher(subject);
		}

		#endregion

		#region Implementation of IPatternApplier<MemberInfo,TApplyTo>

		public void Apply(TSubject subject, TApplyTo applyTo)
		{
			applier(subject, applyTo);
		}

		#endregion
	}
}