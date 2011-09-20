using System;
using ConfOrm.Mappers;

namespace ConfOrm.NH.CustomizersImpl
{
	public class OneToManyCustomizer : IOneToManyMapper
	{
		private readonly PropertyPath propertyPath;
		private readonly ICustomizersHolder customizersHolder;

		public OneToManyCustomizer(PropertyPath propertyPath, ICustomizersHolder customizersHolder)
		{
			this.propertyPath = propertyPath;
			this.customizersHolder = customizersHolder;
		}

		public void Class(Type entityType)
		{
			customizersHolder.AddCustomizer(propertyPath, (IOneToManyMapper x) => x.Class(entityType));
		}

		public void EntityName(string entityName)
		{
			customizersHolder.AddCustomizer(propertyPath, (IOneToManyMapper x) => x.EntityName(entityName));
		}

		public void NotFound(NotFoundMode mode)
		{
			customizersHolder.AddCustomizer(propertyPath, (IOneToManyMapper x) => x.NotFound(mode));
		}
	}
}