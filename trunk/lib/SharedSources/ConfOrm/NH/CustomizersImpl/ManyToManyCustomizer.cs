using System;
using ConfOrm.Mappers;

namespace ConfOrm.NH.CustomizersImpl
{
	public class ManyToManyCustomizer : IManyToManyMapper
	{
		private readonly PropertyPath propertyPath;
		private readonly ICustomizersHolder customizersHolder;

		public ManyToManyCustomizer(PropertyPath propertyPath, ICustomizersHolder customizersHolder)
		{
			this.propertyPath = propertyPath;
			this.customizersHolder = customizersHolder;
		}

		public void Column(Action<IColumnMapper> columnMapper)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToManyMapper x)=> x.Column(columnMapper));
		}

		public void Columns(params Action<IColumnMapper>[] columnMapper)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToManyMapper x) => x.Columns(columnMapper));
		}

		public void Column(string name)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToManyMapper x) => x.Column(name));
		}

		public void Class(Type entityType)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToManyMapper x) => x.Class(entityType));
		}

		public void EntityName(string entityName)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToManyMapper x) => x.EntityName(entityName));
		}

		public void NotFound(NotFoundMode mode)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToManyMapper x) => x.NotFound(mode));
		}

		public void Formula(string formula)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToManyMapper x) => x.Formula(formula));			
		}

		public void Lazy(LazyRelation lazyRelation)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToManyMapper x) => x.Lazy(lazyRelation));
		}
	}
}