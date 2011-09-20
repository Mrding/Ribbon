using System;
using System.Collections.Generic;
using ConfOrm.Mappers;
using ConfOrm.NH;

namespace ConfOrm.Shop.CoolNaming
{
	public class CoolTablesNamingPack : EmptyPatternsAppliersHolder
	{
		public CoolTablesNamingPack(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}

			collectionPath = new List<IPatternApplier<PropertyPath, ICollectionPropertiesMapper>>
			                 	{
			                 		new ManyToManyInCollectionTableApplier(domainInspector),
			                 		new CollectionOfElementsTableApplier(domainInspector),
													new CollectionOfComponentsTableApplier(domainInspector),
			                 	};
		}
	}
}