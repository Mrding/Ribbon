using System;

namespace ConfOrm.Patterns
{
	public class DelegatedApplier<TSubject, TApplyTo> : IPatternApplier<TSubject, TApplyTo>
	{
		private readonly Action<TApplyTo> applier;
		private readonly Predicate<TSubject> matcher;

		public DelegatedApplier(Predicate<TSubject> matcher, Action<TApplyTo> applier)
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
			applier(applyTo);
		}

		#endregion
	}
}