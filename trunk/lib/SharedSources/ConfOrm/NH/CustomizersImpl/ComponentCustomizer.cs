using System;
using System.Linq.Expressions;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.NH.CustomizersImpl
{
	public class ComponentCustomizer<TComponent> : PropertyContainerCustomizer<TComponent>, IComponentMapper<TComponent>
		where TComponent : class
	{
		public ComponentCustomizer(ICustomizersHolder customizersHolder) : base(customizersHolder, null) { }
		public ComponentCustomizer(ICustomizersHolder customizersHolder, PropertyPath propertyPath) : base(customizersHolder, propertyPath) { }

		#region Implementation of IComponentMapper<TComponent>

		public void Parent<TProperty>(Expression<Func<TComponent, TProperty>> parent) where TProperty : class
		{
			Parent(parent, x=> { });
		}

		public void Parent<TProperty>(Expression<Func<TComponent, TProperty>> parent, Action<IComponentParentMapper> parentMapping) where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(parent);
			AddCustomizer(m => m.Parent(member, parentMapping));
		}

		public void Update(bool consideredInUpdateQuery)
		{
			AddCustomizer(m => m.Update(consideredInUpdateQuery));
		}

		public void Insert(bool consideredInInsertQuery)
		{
			AddCustomizer(m => m.Insert(consideredInInsertQuery));
		}

		public void Lazy(bool isLazy)
		{
			AddCustomizer(m => m.Lazy(isLazy));
		}

		public void Class<TConcrete>() where TConcrete : TComponent
		{
			AddCustomizer(m=> m.Class(typeof(TConcrete)));
		}

		#endregion

		public void Access(Accessor accessor)
		{
			AddCustomizer(m => m.Access(accessor));
		}

		public void Access(Type accessorType)
		{
			AddCustomizer(m => m.Access(accessorType));
		}

		public void OptimisticLock(bool takeInConsiderationForOptimisticLock)
		{
			AddCustomizer(m => m.OptimisticLock(takeInConsiderationForOptimisticLock));
		}

		private void AddCustomizer(Action<IComponentAttributesMapper> classCustomizer)
		{
			if (PropertyPath == null)
			{
				CustomizersHolder.AddCustomizer(typeof(TComponent), classCustomizer);
			}
			else
			{
				CustomizersHolder.AddCustomizer(PropertyPath, classCustomizer);				
			}
		}
	}
}