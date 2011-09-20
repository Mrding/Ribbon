using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ConfOrm.Mappers
{
	public interface IBasePlainPropertyContainerMapper
	{
		void Property(MemberInfo property, Action<IPropertyMapper> mapping);

		void Component(MemberInfo property, Action<IComponentMapper> mapping);

		void ManyToOne(MemberInfo property, Action<IManyToOneMapper> mapping);
		void Any(MemberInfo property, Type idTypeOfMetaType, Action<IAnyMapper> mapping);
	}

	public interface IPlainPropertyContainerMapper : IBasePlainPropertyContainerMapper
	{
		void OneToOne(MemberInfo property, Action<IOneToOneMapper> mapping);
	}

	public interface IPlainPropertyContainerMapper<TContainer>
	{
		void Property<TProperty>(Expression<Func<TContainer, TProperty>> property, Action<IPropertyMapper> mapping);

		void Component<TComponent>(Expression<Func<TContainer, TComponent>> property,
															 Action<IComponentMapper<TComponent>> mapping) where TComponent : class;

		void ManyToOne<TProperty>(Expression<Func<TContainer, TProperty>> property, Action<IManyToOneMapper> mapping) where TProperty : class;
		void OneToOne<TProperty>(Expression<Func<TContainer, TProperty>> property, Action<IOneToOneMapper> mapping) where TProperty : class;
		void Any<TProperty>(Expression<Func<TContainer, TProperty>> property, Type idTypeOfMetaType, Action<IAnyMapper> mapping) where TProperty : class;
	}
}