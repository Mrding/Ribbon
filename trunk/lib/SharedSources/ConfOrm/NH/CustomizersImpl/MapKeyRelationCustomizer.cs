using System;
using ConfOrm.Mappers;

namespace ConfOrm.NH.CustomizersImpl
{
	public class MapKeyRelationCustomizer<TKey> : IMapKeyRelation<TKey>
	{
		private readonly PropertyPath propertyPath;
		private readonly ICustomizersHolder customizersHolder;

		public MapKeyRelationCustomizer(PropertyPath propertyPath, ICustomizersHolder customizersHolder)
		{
			this.propertyPath = propertyPath;
			this.customizersHolder = customizersHolder;
		}

		public void Element(Action<IMapKeyMapper> mapping)
		{
			var mapKeyCustomizer = new MapKeyCustomizer(propertyPath, customizersHolder);
			mapping(mapKeyCustomizer);
		}

		public void ManyToMany(Action<IMapKeyManyToManyMapper> mapping)
		{
			var manyToManyCustomizer = new MapKeyManyToManyCustomizer(propertyPath, customizersHolder);
			mapping(manyToManyCustomizer);
		}

		public void Component(Action<IComponentMapKeyMapper<TKey>> mapping)
		{
			var manyToManyCustomizer = new MapKeyComponentCustomizer<TKey>(propertyPath, customizersHolder);
			mapping(manyToManyCustomizer);
		}
	}
}