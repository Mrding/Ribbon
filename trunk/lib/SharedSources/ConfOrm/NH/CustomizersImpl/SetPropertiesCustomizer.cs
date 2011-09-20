using ConfOrm.Mappers;

namespace ConfOrm.NH.CustomizersImpl
{
	public class SetPropertiesCustomizer<TEntity, TElement> : CollectionPropertiesCustomizer<TEntity, TElement>, ISetPropertiesMapper<TEntity, TElement> where TEntity : class
	{
		public SetPropertiesCustomizer(PropertyPath propertyPath, ICustomizersHolder customizersHolder) : base(propertyPath, customizersHolder) {}
	}
}