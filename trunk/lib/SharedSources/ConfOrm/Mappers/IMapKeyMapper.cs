using System;
using NHibernate.Type;

namespace ConfOrm.Mappers
{
	public interface IMapKeyMapper : IColumnsMapper
	{
		void Type(IType persistentType);
		void Type<TPersistentType>();
		void Type(Type persistentType);
		void Length(int length);
		void Formula(string formula);
	}
}