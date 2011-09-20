namespace ConfOrm.Mappers
{
	public interface IEntitySqlsMapper
	{
		void Loader(string namedQueryReference);
		void SqlInsert(string sql);
		void SqlUpdate(string sql);
		void SqlDelete(string sql);
	}
}