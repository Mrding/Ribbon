using System;
using System.Collections.Generic;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.NamingAppliers;

namespace ConfOrm.Shop.CoolNaming
{
	public class CoolTablesAndColumnsNamingPack : EmptyPatternsAppliersHolder
	{
		public CoolTablesAndColumnsNamingPack(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			joinedSubclass = new List<IPatternApplier<Type, IJoinedSubclassAttributesMapper>>
			                 	{
			                 		new JoinedSubclassKeyAsRootIdColumnApplier(domainInspector)
			                 	};
			propertyPath = new List<IPatternApplier<PropertyPath, IPropertyMapper>>
			               	{
			               		new ComponentPropertyColumnNameApplier(),
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