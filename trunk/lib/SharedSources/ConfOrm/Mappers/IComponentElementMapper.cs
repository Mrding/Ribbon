using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ConfOrm.Mappers
{
	public interface IComponentElementMapper: IComponentAttributesMapper
	{
		void Property(MemberInfo property, Action<IPropertyMapper> mapping);

		void Component(MemberInfo property, Action<IComponentElementMapper> mapping);

		void ManyToOne(MemberInfo property, Action<IManyToOneMapper> mapping);
	}

	public interface IComponentElementMapper<TComponent> : IComponentAttributesMapper<TComponent>
	{
		void Property<TProperty>(Expression<Func<TComponent, TProperty>> property, Action<IPropertyMapper> mapping);

		void Component<TNestedComponent>(Expression<Func<TComponent, TNestedComponent>> property,
																		 Action<IComponentElementMapper<TNestedComponent>> mapping)
			where TNestedComponent : class;

		void ManyToOne<TProperty>(Expression<Func<TComponent, TProperty>> property, Action<IManyToOneMapper> mapping) where TProperty : class;
	}
}