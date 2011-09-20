using System;

namespace ConfOrm.Mappers
{
	public interface IListPropertiesMapper : ICollectionPropertiesMapper
	{
		void Index(Action<IListIndexMapper> listIndexMapping);
	}

	public interface IListPropertiesMapper<TEntity, TElement> : ICollectionPropertiesMapper<TEntity, TElement> where TEntity : class
	{
		void Index(Action<IListIndexMapper> listIndexMapping);
	}
}