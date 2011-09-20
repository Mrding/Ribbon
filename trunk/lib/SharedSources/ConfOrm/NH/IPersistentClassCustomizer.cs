using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ConfOrm.Mappers;

namespace ConfOrm.NH
{
	public interface IPersistentClassCustomizer<TPersistent> where TPersistent: class
	{
		void Property<TProperty>(Expression<Func<TPersistent, TProperty>> property, Action<IPropertyMapper> mapping);

		void ManyToOne<TProperty>(Expression<Func<TPersistent, TProperty>> property, Action<IManyToOneMapper> mapping) where TProperty : class;
		void OneToOne<TProperty>(Expression<Func<TPersistent, TProperty>> property, Action<IOneToOneMapper> mapping) where TProperty : class;

		void Collection<TElement>(Expression<Func<TPersistent, IEnumerable<TElement>>> property,
											 Action<ICollectionPropertiesMapper<TPersistent, TElement>> collectionMapping);
		void Any<TProperty>(Expression<Func<TPersistent, TProperty>> property, Action<IAnyMapper> mapping) where TProperty : class;

		void Component<TComponent>(Expression<Func<TPersistent, TComponent>> property,
															 Action<IComponentAttributesMapper<TComponent>> mapping) where TComponent : class;
	}
}