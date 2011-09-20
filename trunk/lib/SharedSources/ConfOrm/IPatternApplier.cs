namespace ConfOrm
{
	public interface IPatternApplier<TSubject, TApplyTo> : IPattern<TSubject>
	{
		void Apply(TSubject subject, TApplyTo applyTo);
	}
}