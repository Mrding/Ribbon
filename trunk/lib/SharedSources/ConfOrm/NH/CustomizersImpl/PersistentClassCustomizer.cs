using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.NH.CustomizersImpl
{
	public class PersistentClassCustomizer<TPersistent> : IPersistentClassCustomizer<TPersistent> where TPersistent : class
	{
		private readonly ICustomizersHolder customizersHolder;

		public PersistentClassCustomizer(ICustomizersHolder customizersHolder)
		{
			this.customizersHolder = customizersHolder;
		}

		#region Implementation of IPersistentClassCustomizer<TPersistent>

		public void Property<TProperty>(Expression<Func<TPersistent, TProperty>> property, Action<IPropertyMapper> mapping)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(property);
			customizersHolder.AddCustomizer(new PropertyPath(null, member),mapping);
			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			customizersHolder.AddCustomizer(new PropertyPath(null, memberOf), mapping);
		}

		public void ManyToOne<TProperty>(Expression<Func<TPersistent, TProperty>> property, Action<IManyToOneMapper> mapping) where TProperty : class
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(property);
			customizersHolder.AddCustomizer(new PropertyPath(null, member), mapping);
			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			customizersHolder.AddCustomizer(new PropertyPath(null, memberOf), mapping);
		}

		public void OneToOne<TProperty>(Expression<Func<TPersistent, TProperty>> property, Action<IOneToOneMapper> mapping) where TProperty : class
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(property);
			customizersHolder.AddCustomizer(new PropertyPath(null, member), mapping);
			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			customizersHolder.AddCustomizer(new PropertyPath(null, memberOf), mapping);
		}

		public void Collection<TElement>(Expression<Func<TPersistent, IEnumerable<TElement>>> property, Action<ICollectionPropertiesMapper<TPersistent, TElement>> mapping)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(property);
			mapping(new CollectionPropertiesCustomizer<TPersistent, TElement>(new PropertyPath(null, member), customizersHolder));
			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			mapping(new CollectionPropertiesCustomizer<TPersistent, TElement>(new PropertyPath(null, memberOf), customizersHolder));
		}

		public void Any<TProperty>(Expression<Func<TPersistent, TProperty>> property, Action<IAnyMapper> mapping) where TProperty : class
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(property);
			customizersHolder.AddCustomizer(new PropertyPath(null, member), mapping);
			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			customizersHolder.AddCustomizer(new PropertyPath(null, memberOf), mapping);
		}

		public void Component<TProperty>(Expression<Func<TPersistent, TProperty>> property, Action<IComponentAttributesMapper<TProperty>> mapping) where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			mapping(new ComponentCustomizer<TProperty>(customizersHolder, new PropertyPath(null, member)));
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			mapping(new ComponentCustomizer<TProperty>(customizersHolder, new PropertyPath(null, memberOf)));
		}

		#endregion
	}
}