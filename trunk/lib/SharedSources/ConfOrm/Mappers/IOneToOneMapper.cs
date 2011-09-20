using System.Reflection;

namespace ConfOrm.Mappers
{
	public interface IOneToOneMapper: IEntityPropertyMapper
	{
		void Cascade(Cascade cascadeStyle);
		void Lazy(LazyRelation lazyRelation);
		void Constrained(bool value);
		void PropertyReference(MemberInfo propertyInTheOtherSide);
		void Formula(string formula);
	}
}