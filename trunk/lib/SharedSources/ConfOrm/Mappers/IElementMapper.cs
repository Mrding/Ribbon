using NHibernate.Type;

namespace ConfOrm.Mappers
{
	public interface IElementMapper : IColumnsMapper
	{
		void Type(IType persistentType);
		void Type<TPersistentType>();
		void Type<TPersistentType>(object parameters);
		void Type(System.Type persistentType, object parameters);
		void Length(int length);
		void Precision(short precision);
		void Scale(short scale);
		void NotNullable(bool notnull);
		void Unique(bool unique);
		void Formula(string formula);
	}
}