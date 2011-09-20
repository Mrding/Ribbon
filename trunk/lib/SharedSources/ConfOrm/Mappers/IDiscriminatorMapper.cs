using System;
using NHibernate.Type;

namespace ConfOrm.Mappers
{
	public interface IDiscriminatorMapper
	{
		void Column(string column);
		void Column(Action<IColumnMapper> columnMapper);
		void Type(IType persistentType);
		void Type(IDiscriminatorType persistentType);
		void Type<TPersistentType>() where TPersistentType : IDiscriminatorType;
		void Type(Type persistentType);
		void Formula(string formula);
		void Force(bool force);
		void Insert(bool applyOnApplyOnInsert);
		void NotNullable(bool isNotNullable);
		void Length(int length);
	}
}