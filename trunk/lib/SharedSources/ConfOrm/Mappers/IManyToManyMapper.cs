using System;

namespace ConfOrm.Mappers
{
	public interface IManyToManyMapper: IColumnsMapper
	{
		void Class(Type entityType);
		void EntityName(string entityName);
		void NotFound(NotFoundMode mode);
		void Formula(string formula);
		void Lazy(LazyRelation lazyRelation);
	}
}