using System;
using ConfOrm.Mappers;

namespace ConfOrm.NH.CustomizersImpl
{
	public class MapKeyManyToManyCustomizer : IMapKeyManyToManyMapper
	{
		private readonly PropertyPath propertyPath;
		private readonly ICustomizersHolder customizersHolder;

		public MapKeyManyToManyCustomizer(PropertyPath propertyPath, ICustomizersHolder customizersHolder)
		{
			this.propertyPath = propertyPath;
			this.customizersHolder = customizersHolder;
		}

		public void Column(Action<IColumnMapper> columnMapper)
		{
			customizersHolder.AddCustomizer(propertyPath, (IMapKeyManyToManyMapper x) => x.Column(columnMapper));
		}

		public void Columns(params Action<IColumnMapper>[] columnMapper)
		{
			customizersHolder.AddCustomizer(propertyPath, (IMapKeyManyToManyMapper x) => x.Columns(columnMapper));
		}

		public void Column(string name)
		{
			customizersHolder.AddCustomizer(propertyPath, (IMapKeyManyToManyMapper x) => x.Column(name));
		}

		public void ForeignKey(string foreignKeyName)
		{
			customizersHolder.AddCustomizer(propertyPath, (IMapKeyManyToManyMapper x) => x.ForeignKey(foreignKeyName));
		}

		public void Formula(string formula)
		{
			customizersHolder.AddCustomizer(propertyPath, (IMapKeyManyToManyMapper x) => x.Formula(formula));
		}
	}
}