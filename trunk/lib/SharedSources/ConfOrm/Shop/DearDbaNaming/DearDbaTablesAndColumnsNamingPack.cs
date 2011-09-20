using System;
using System.Collections.Generic;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Inflectors;
using ConfOrm.Shop.NamingAppliers;

namespace ConfOrm.Shop.DearDbaNaming
{
	public class DearDbaTablesAndColumnsNamingPack : EmptyPatternsAppliersHolder
	{
		public DearDbaTablesAndColumnsNamingPack(IDomainInspector domainInspector, IInflector inflector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			if (inflector == null)
			{
				throw new ArgumentNullException("inflector");
			}
			rootClass = new List<IPatternApplier<Type, IClassAttributesMapper>>
			            	{
			            		new ClassPluralizedTableApplier(inflector),
			            		new PoidColumnNameApplier()
			            	};
			joinedSubclass = new List<IPatternApplier<Type, IJoinedSubclassAttributesMapper>>
			                 	{
			                 		new JoinedSubclassPluralizedTableApplier(inflector),
			                 		new JoinedSubclassKeyAsRootIdColumnApplier(domainInspector)
			                 	};
			unionSubclass = new List<IPatternApplier<Type, IUnionSubclassAttributesMapper>>
			                	{
			                		new UnionSubclassPluralizedTableApplier(inflector)
			                	};
			collectionPath = new List<IPatternApplier<PropertyPath, ICollectionPropertiesMapper>>
			                 	{
			                 		new ManyToManyPluralizedTableApplier(domainInspector, inflector),
			                 		new ManyToManyKeyIdColumnApplier(domainInspector),
			                 		new OneToManyKeyColumnApplier(domainInspector),
			                 		new CollectionOfElementsTableApplier(domainInspector, inflector),
			                 		new CollectionOfElementsKeyColumnApplier(domainInspector),
			                 		new CollectionOfComponentsTableApplier(domainInspector, inflector),
			                 		new CollectionOfComponentsKeyColumnApplier(domainInspector),
			                 	};
			propertyPath = new List<IPatternApplier<PropertyPath, IPropertyMapper>>
			               	{
			               		new ComponentPropertyColumnNameApplier(),
			               	};
			listPath = new List<IPatternApplier<PropertyPath, IListPropertiesMapper>>
			           	{
			           		new ListIndexAsPropertyPosColumnNameApplier(),
			           	};
			manyToOnePath = new List<IPatternApplier<PropertyPath, IManyToOneMapper>>
			                	{
			                		new ManyToOneColumnApplier()
			                	};
			manyToManyPath = new List<IPatternApplier<PropertyPath, IManyToManyMapper>>
			                 	{
			                 		new ManyToManyColumnApplier(domainInspector),
			                 	};
		}
	}
}