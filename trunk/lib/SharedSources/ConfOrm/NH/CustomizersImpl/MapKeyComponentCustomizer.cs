using System;
using System.Linq.Expressions;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.NH.CustomizersImpl
{
	public class MapKeyComponentCustomizer<TKey> : IComponentMapKeyMapper<TKey>
	{
		private readonly ICustomizersHolder customizersHolder;
		private readonly PropertyPath propertyPath;

		public MapKeyComponentCustomizer(PropertyPath propertyPath, ICustomizersHolder customizersHolder)
		{
			this.propertyPath = propertyPath;
			this.customizersHolder = customizersHolder;
		}

		#region IComponentMapKeyMapper<TKey> Members

		public void Property<TProperty>(Expression<Func<TKey, TProperty>> property, Action<IPropertyMapper> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			customizersHolder.AddCustomizer(new PropertyPath(propertyPath, member), mapping);
		}

		public void ManyToOne<TProperty>(Expression<Func<TKey, TProperty>> property, Action<IManyToOneMapper> mapping) where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			customizersHolder.AddCustomizer(new PropertyPath(propertyPath, member), mapping);
		}

		#endregion
	}
}