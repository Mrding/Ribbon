namespace ConfOrm.Mappers
{
	public interface INaturalIdAttributesMapper
	{
		void Mutable(bool isMutable);
	}

	public interface INaturalIdMapper : INaturalIdAttributesMapper, IBasePlainPropertyContainerMapper
	{
		
	}
}