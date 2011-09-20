using System;

namespace ConfOrm.Mappers
{
	public interface IOneToManyMapper
	{
		void Class(Type entityType);
		void EntityName(string entityName);
		void NotFound(NotFoundMode mode);
	}
}