using System;

namespace ConfOrm.Mappers
{
	public interface IUnionSubclassAttributesMapper : IEntityAttributesMapper, IEntitySqlsMapper
	{
		void Table(string tableName);
		void Catalog(string catalogName);
		void Schema(string schemaName);
		void Extends(Type baseType);
	}
	public interface IUnionSubclassMapper : IUnionSubclassAttributesMapper, IPropertyContainerMapper
	{

	}

	public interface IUnionSubclassAttributesMapper<TEntity> : IEntityAttributesMapper, IEntitySqlsMapper where TEntity : class
	{
		void Table(string tableName);
		void Catalog(string catalogName);
		void Schema(string schemaName);
	}

	public interface IUnionSubclassMapper<TEntity> : IUnionSubclassAttributesMapper<TEntity>, IPropertyContainerMapper<TEntity> where TEntity : class
	{
	}

}