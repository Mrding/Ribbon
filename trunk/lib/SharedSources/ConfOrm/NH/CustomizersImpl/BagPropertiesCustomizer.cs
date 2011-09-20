using ConfOrm.Mappers;

namespace ConfOrm.NH.CustomizersImpl
{
	public class BagPropertiesCustomizer<TEntity, TElement> : CollectionPropertiesCustomizer<TEntity, TElement>, IBagPropertiesMapper<TEntity, TElement> where TEntity : class
	{
		public BagPropertiesCustomizer(PropertyPath propertyPath, ICustomizersHolder customizersHolder) : base(propertyPath, customizersHolder) {}
	}
}