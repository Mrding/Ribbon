using System;
using ConfOrm.Mappers;

namespace ConfOrm.NH.CustomizersImpl
{
	public class ListPropertiesCustomizer<TEntity, TElement> : CollectionPropertiesCustomizer<TEntity, TElement>, IListPropertiesMapper<TEntity, TElement> where TEntity : class
	{
		public ListPropertiesCustomizer(PropertyPath propertyPath, ICustomizersHolder customizersHolder) : base(propertyPath, customizersHolder) {}

		#region Implementation of IListPropertiesMapper<TEntity,TElement>

		public void Index(Action<IListIndexMapper> listIndexMapping)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (IListPropertiesMapper x) => x.Index(listIndexMapping));
		}

		#endregion
	}
}