using System;
using System.Linq.Expressions;
using ConfOrm.Mappers;

namespace ConfOrm.NH.CustomizersImpl
{
	public class CollectionKeyCustomizer<TEntity>: IKeyMapper<TEntity> 
		where TEntity : class
	{
		private readonly PropertyPath propertyPath;

		public CollectionKeyCustomizer(PropertyPath propertyPath, ICustomizersHolder customizersHolder)
		{
			this.propertyPath = propertyPath;
			CustomizersHolder = customizersHolder;
		}

		public ICustomizersHolder CustomizersHolder { get; private set; }

		#region Implementation of IKeyMapper<TEntity>

		public void Column(string columnName)
		{
			CustomizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper m) => m.Key(x => x.Column(columnName)));
		}

		public void OnDelete(OnDeleteAction deleteAction)
		{
			CustomizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper m) => m.Key(x => x.OnDelete(deleteAction)));
		}

		public void PropertyRef<TProperty>(Expression<Func<TEntity, TProperty>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			CustomizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper m) => m.Key(x => x.PropertyRef(member)));
		}

		public void Update(bool consideredInUpdateQuery)
		{
			CustomizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper m) => m.Key(x => x.Update(consideredInUpdateQuery)));
		}

		public void ForeignKey(string foreingKeyName)
		{
			CustomizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper m) => m.Key(x => x.ForeignKey(foreingKeyName)));
		}

		#endregion
	}
}