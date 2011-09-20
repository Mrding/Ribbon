namespace ConfOrm
{
	public interface IPattern<TSubject>
	{
		bool Match(TSubject subject);
	}
}