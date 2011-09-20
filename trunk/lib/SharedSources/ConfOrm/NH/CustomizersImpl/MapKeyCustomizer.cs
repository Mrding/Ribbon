using System;
using ConfOrm.Mappers;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace ConfOrm.NH.CustomizersImpl
{
	public class MapKeyCustomizer : IMapKeyMapper
	{
		private readonly PropertyPath propertyPath;
		private readonly ICustomizersHolder customizersHolder;

		public MapKeyCustomizer(PropertyPath propertyPath, ICustomizersHolder customizersHolder)
		{
			this.propertyPath = propertyPath;
			this.customizersHolder = customizersHolder;
		}

		public void Column(Action<IColumnMapper> columnMapper)
		{
			customizersHolder.AddCustomizer(propertyPath, (IMapKeyMapper x) => x.Column(columnMapper));
		}

		public void Columns(params Action<IColumnMapper>[] columnMapper)
		{
			customizersHolder.AddCustomizer(propertyPath, (IMapKeyMapper x) => x.Columns(columnMapper));
		}

		public void Column(string name)
		{
			customizersHolder.AddCustomizer(propertyPath, (IMapKeyMapper x) => x.Column(name));
		}

		public void Type(IType persistentType)
		{
			customizersHolder.AddCustomizer(propertyPath, (IMapKeyMapper x) => x.Type(persistentType));
		}

		public void Type<TPersistentType>()
		{
			customizersHolder.AddCustomizer(propertyPath, (IMapKeyMapper x) => x.Type<TPersistentType>());
		}

		public void Type(Type persistentType)
		{
			customizersHolder.AddCustomizer(propertyPath, (IMapKeyMapper x) => x.Type(persistentType));
		}

		public void Length(int length)
		{
			customizersHolder.AddCustomizer(propertyPath, (IMapKeyMapper x) => x.Length(length));
		}

		public void Formula(string formula)
		{
			customizersHolder.AddCustomizer(propertyPath, (IMapKeyMapper x) => x.Formula(formula));
		}
	}
}