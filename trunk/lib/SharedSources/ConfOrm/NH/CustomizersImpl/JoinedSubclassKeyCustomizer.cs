using System;
using System.Linq.Expressions;
using ConfOrm.Mappers;

namespace ConfOrm.NH.CustomizersImpl
{
	public class JoinedSubclassKeyCustomizer<TEntity>: IKeyMapper<TEntity> 
		where TEntity : class
	{
		public JoinedSubclassKeyCustomizer(ICustomizersHolder customizersHolder)
		{
			CustomizersHolder = customizersHolder;
		}

		public ICustomizersHolder CustomizersHolder { get; private set; }

		#region Implementation of IKeyMapper<TEntity>

		public void Column(string columnName)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinedSubclassAttributesMapper m) => m.Key(x => x.Column(columnName)));
		}

		public void OnDelete(OnDeleteAction deleteAction)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinedSubclassAttributesMapper m) => m.Key(x => x.OnDelete(deleteAction)));
		}

		public void PropertyRef<TProperty>(Expression<Func<TEntity, TProperty>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinedSubclassAttributesMapper m) => m.Key(x => x.PropertyRef(member)));
		}

		public void Update(bool consideredInUpdateQuery)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinedSubclassAttributesMapper m) => m.Key(x => x.Update(consideredInUpdateQuery)));
		}

		public void ForeignKey(string foreingKeyName)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinedSubclassAttributesMapper m) => m.Key(x => x.ForeignKey(foreingKeyName)));
		}

		#endregion
	}
}