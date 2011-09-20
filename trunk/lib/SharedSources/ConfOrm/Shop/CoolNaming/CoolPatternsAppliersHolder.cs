using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Patterns;
using ConfOrm.Shop.NamingAppliers;

namespace ConfOrm.Shop.CoolNaming
{
	public class CoolPatternsAppliersHolder : EmptyPatternsAppliersHolder
	{
		public CoolPatternsAppliersHolder(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			rootClass = new List<IPatternApplier<Type, IClassAttributesMapper>>();
			joinedSubclass = new List<IPatternApplier<Type, IJoinedSubclassAttributesMapper>>
			                 	{new JoinedSubclassOnDeleteApplier(), new JoinedSubclassKeyAsRootIdColumnApplier(domainInspector)};
			subclass = new List<IPatternApplier<Type, ISubclassAttributesMapper>>();
			unionSubclass = new List<IPatternApplier<Type, IUnionSubclassAttributesMapper>>();
			poid = new List<IPatternApplier<MemberInfo, IIdMapper>>
			       	{
			       		new MemberNoSetterToFieldAccessorApplier<IIdMapper>(),
			       		new NoPoidGuidApplier(),
			       		new BidirectionalOneToOneAssociationPoidApplier(domainInspector)
			       	};
			property = new List<IPatternApplier<MemberInfo, IPropertyMapper>>
			           	{
			           		new MemberReadOnlyAccessorApplier<IPropertyMapper>(),
			           		new MemberNoSetterToFieldAccessorApplier<IPropertyMapper>(),
			           		new MemberToFieldAccessorApplier<IPropertyMapper>(),
			           	};
			propertyPath = new List<IPatternApplier<PropertyPath, IPropertyMapper>> {new ComponentPropertyColumnNameApplier(),};
			collection = new List<IPatternApplier<MemberInfo, ICollectionPropertiesMapper>>
			             	{
			             		new MemberReadOnlyAccessorApplier<ICollectionPropertiesMapper>(),
			             		new MemberNoSetterToFieldAccessorApplier<ICollectionPropertiesMapper>(),
			             		new MemberToFieldAccessorApplier<ICollectionPropertiesMapper>(),
			             		new BidirectionalOneToManyCascadeApplier(domainInspector),
			             		new BidirectionalOneToManyInverseApplier(domainInspector),
			             		new BidirectionalOneToManyOnDeleteConstraintApplier(domainInspector),
			             		new BidirectionalManyToManyInverseApplier(domainInspector),
			             		new PolymorphismBidirectionalOneToManyInverseApplier(domainInspector),
			             		new PolymorphismBidirectionalOneToManyCascadeApplier(domainInspector),
			             		new PolymorphismBidirectionalOneToManyOnDeleteConstraintApplier(domainInspector),
			             		new PolymorphismBidirectionalAnyToManyKeyColumnApplier(domainInspector),
			             		new PolymorphismBidirectionalAnyToManyWhereApplier(domainInspector),
			             	};

			collectionPath = new List<IPatternApplier<PropertyPath, ICollectionPropertiesMapper>>
			                 	{
			                 		new ManyToManyInCollectionTableApplier(domainInspector),
			                 		new ManyToManyKeyIdColumnApplier(domainInspector),
			                 		new OneToManyKeyColumnApplier(domainInspector),
													new CollectionOfElementsTableApplier(domainInspector),
													new CollectionOfElementsKeyColumnApplier(domainInspector),
													new CollectionOfComponentsTableApplier(domainInspector),
													new CollectionOfComponentsKeyColumnApplier(domainInspector),
			                 	};
			listPath = new List<IPatternApplier<PropertyPath, IListPropertiesMapper>>
			           	{
			           		new ListIndexAsPropertyPosColumnNameApplier(),
			           	};
			manyToOne = new List<IPatternApplier<MemberInfo, IManyToOneMapper>>
			            	{
			            		new MemberReadOnlyAccessorApplier<IManyToOneMapper>(),
			            		new MemberNoSetterToFieldAccessorApplier<IManyToOneMapper>(),
			            		new MemberToFieldAccessorApplier<IManyToOneMapper>(),
			            		new BidirectionalForeignKeyAssociationManyToOneApplier(domainInspector),
			            		new UnidirectionalOneToOneUniqueCascadeApplier(domainInspector),
											new PolymorphismManyToOneClassApplier(domainInspector),
			            	};
			manyToOnePath = new List<IPatternApplier<PropertyPath, IManyToOneMapper>> { new ManyToOneColumnApplier() };
			oneToMany = new List<IPatternApplier<MemberInfo, IOneToManyMapper>>
			            {
			            	new PolymorphismOneToManyClassApplier(domainInspector)
			            };

			oneToOne = new List<IPatternApplier<MemberInfo, IOneToOneMapper>>
			           	{
			           		new MemberReadOnlyAccessorApplier<IOneToOneMapper>(),
			           		new MemberNoSetterToFieldAccessorApplier<IOneToOneMapper>(),
			           		new MemberToFieldAccessorApplier<IOneToOneMapper>(),
			           		new BidirectionalForeignKeyAssociationOneToOneApplier(domainInspector),
			           		new BidirectionalPrimaryKeyAssociationMasterOneToOneApplier(domainInspector),
			           		new BidirectionalPrimaryKeyAssociationSlaveOneToOneApplier(domainInspector)
			           	};
			oneToOnePath = new List<IPatternApplier<PropertyPath, IOneToOneMapper>>();
			any = new List<IPatternApplier<MemberInfo, IAnyMapper>>
			      	{
			      		new MemberReadOnlyAccessorApplier<IAnyMapper>(),
			      		new MemberNoSetterToFieldAccessorApplier<IAnyMapper>(),
			      		new MemberToFieldAccessorApplier<IAnyMapper>()
			      	};
			anyPath = new List<IPatternApplier<PropertyPath, IAnyMapper>>();
			manyToMany = new List<IPatternApplier<MemberInfo, IManyToManyMapper>>();
			manyToManyPath = new List<IPatternApplier<PropertyPath, IManyToManyMapper>>
			                 	{new ManyToManyColumnApplier(domainInspector),};
			component = new List<IPatternApplier<Type, IComponentAttributesMapper>>
			            {
			            	new PolymorphismComponentClassApplier(domainInspector),
			            };
			componentParent = new List<IPatternApplier<MemberInfo, IComponentParentMapper>>
			                  	{
			                  		new MemberNoSetterToFieldAccessorApplier<IComponentParentMapper>(),
			                  		new MemberToFieldAccessorApplier<IComponentParentMapper>(),
			                  	};
			componentProperty = new List<IPatternApplier<MemberInfo, IComponentAttributesMapper>>
			                    	{
			                    		new MemberReadOnlyAccessorApplier<IComponentAttributesMapper>(),
			                    		new MemberNoSetterToFieldAccessorApplier<IComponentAttributesMapper>(),
			                    		new MemberToFieldAccessorApplier<IComponentAttributesMapper>()
			                    	};
			elementPath = new List<IPatternApplier<PropertyPath, IElementMapper>>
										{
											new CollectionOfElementsColumnApplier(domainInspector),
										};
		}
	}
}