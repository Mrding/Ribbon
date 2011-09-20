namespace ConfOrm
{
	public interface IPatternValueGetter<TElement, TResult> : IPattern<TElement>
	{
		TResult Get(TElement element);
	}
}