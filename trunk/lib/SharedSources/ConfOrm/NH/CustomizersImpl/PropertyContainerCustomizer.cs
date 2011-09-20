using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.NH.CustomizersImpl
{
	public class PropertyContainerCustomizer<TEntity> where TEntity : class
	{
		public PropertyContainerCustomizer(ICustomizersHolder customizersHolder, PropertyPath propertyPath)
		{
			CustomizersHolder = customizersHolder;
			PropertyPath = propertyPath;
		}

		protected ICustomizersHolder CustomizersHolder { get; private set; }
		protected PropertyPath PropertyPath { get; private set; }

		public void Property<TProperty>(Expression<Func<TEntity, TProperty>> property, Action<IPropertyMapper> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, member), mapping);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, memberOf), mapping);
		}

		public void Component<TComponent>(Expression<Func<TEntity, TComponent>> property,
		                                  Action<IComponentMapper<TComponent>> mapping) where TComponent : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			mapping(new ComponentCustomizer<TComponent>(CustomizersHolder, new PropertyPath(PropertyPath, member)));
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			mapping(new ComponentCustomizer<TComponent>(CustomizersHolder, new PropertyPath(PropertyPath, memberOf)));
		}

		public void ManyToOne<TProperty>(Expression<Func<TEntity, TProperty>> property, Action<IManyToOneMapper> mapping)
			where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, member), mapping);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, memberOf), mapping);
		}

		public void OneToOne<TProperty>(Expression<Func<TEntity, TProperty>> property, Action<IOneToOneMapper> mapping)
			where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, member), mapping);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, memberOf), mapping);
		}

		public void Any<TProperty>(Expression<Func<TEntity, TProperty>> property, Type idTypeOfMetaType, Action<IAnyMapper> mapping)
			where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, member), am => am.IdType(idTypeOfMetaType));
			CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, member), mapping);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, memberOf), mapping);
			CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, memberOf), am => am.IdType(idTypeOfMetaType));
		}

		public void Set<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
		                          Action<ISetPropertiesMapper<TEntity, TElement>> collectionMapping,
		                          Action<ICollectionElementRelation<TElement>> mapping)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(property);
			collectionMapping(new SetPropertiesCustomizer<TEntity, TElement>(new PropertyPath(null, member), CustomizersHolder));
			mapping(new CollectionElementRelationCustomizer<TElement>(new PropertyPath(PropertyPath, member), CustomizersHolder));

			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			collectionMapping(new SetPropertiesCustomizer<TEntity, TElement>(new PropertyPath(null, memberOf), CustomizersHolder));
			mapping(new CollectionElementRelationCustomizer<TElement>(new PropertyPath(PropertyPath, member), CustomizersHolder));
		}

		public void Bag<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
															Action<IBagPropertiesMapper<TEntity, TElement>> collectionMapping,
		                          Action<ICollectionElementRelation<TElement>> mapping)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(property);
			collectionMapping(new BagPropertiesCustomizer<TEntity, TElement>(new PropertyPath(null, member), CustomizersHolder));
			mapping(new CollectionElementRelationCustomizer<TElement>(new PropertyPath(PropertyPath, member), CustomizersHolder));

			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			collectionMapping(new BagPropertiesCustomizer<TEntity, TElement>(new PropertyPath(null, memberOf), CustomizersHolder));
			mapping(new CollectionElementRelationCustomizer<TElement>(new PropertyPath(PropertyPath, memberOf), CustomizersHolder));
		}

		public void List<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
															 Action<IListPropertiesMapper<TEntity, TElement>> collectionMapping,
		                           Action<ICollectionElementRelation<TElement>> mapping)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(property);
			collectionMapping(new ListPropertiesCustomizer<TEntity, TElement>(new PropertyPath(null, member), CustomizersHolder));
			mapping(new CollectionElementRelationCustomizer<TElement>(new PropertyPath(PropertyPath, member), CustomizersHolder));

			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			collectionMapping(new ListPropertiesCustomizer<TEntity, TElement>(new PropertyPath(null, memberOf), CustomizersHolder));
			mapping(new CollectionElementRelationCustomizer<TElement>(new PropertyPath(PropertyPath, member), CustomizersHolder));
		}

		public void Map<TKey, TElement>(Expression<Func<TEntity, IDictionary<TKey, TElement>>> property,
		                                Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping,
		                                Action<IMapKeyRelation<TKey>> keyMapping,
		                                Action<ICollectionElementRelation<TElement>> mapping)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(property);
			var memberPath = new PropertyPath(PropertyPath, member);
			collectionMapping(new MapPropertiesCustomizer<TEntity, TKey, TElement>(memberPath, CustomizersHolder));
			keyMapping(new MapKeyRelationCustomizer<TKey>(memberPath, CustomizersHolder));
			mapping(new CollectionElementRelationCustomizer<TElement>(memberPath, CustomizersHolder));

			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			var memberOfPath = new PropertyPath(PropertyPath, memberOf);
			collectionMapping(new MapPropertiesCustomizer<TEntity, TKey, TElement>(memberOfPath, CustomizersHolder));
			keyMapping(new MapKeyRelationCustomizer<TKey>(memberOfPath, CustomizersHolder));
			mapping(new CollectionElementRelationCustomizer<TElement>(memberOfPath, CustomizersHolder));
		}
	}
}