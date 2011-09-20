using System;

namespace ConfOrm.Mappers
{
	public interface IMapPropertiesMapper : ICollectionPropertiesMapper
	{
	}

	public interface IMapPropertiesMapper<TEntity, TKey, TElement> : ICollectionPropertiesMapper<TEntity, TElement> where TEntity : class
	{
	}
}