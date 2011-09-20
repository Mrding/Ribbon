namespace ConfOrm.Mappers
{
	public interface IEntityPropertyMapper : IAccessorPropertyMapper
	{
		void OptimisticLock(bool takeInConsiderationForOptimisticLock);
	}
}