using ConfOrm.Mappers;

namespace ConfOrm.NH.CustomizersImpl
{
	public class MapPropertiesCustomizer<TEntity, TKey, TElement> : CollectionPropertiesCustomizer<TEntity, TElement>, IMapPropertiesMapper<TEntity, TKey, TElement> where TEntity : class
	{
		public MapPropertiesCustomizer(PropertyPath propertyPath, ICustomizersHolder customizersHolder) : base(propertyPath, customizersHolder) {}
	}
}