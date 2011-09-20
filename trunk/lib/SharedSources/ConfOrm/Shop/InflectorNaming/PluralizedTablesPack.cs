using System;
using System.Collections.Generic;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Inflectors;

namespace ConfOrm.Shop.InflectorNaming
{
	public class PluralizedTablesPack : EmptyPatternsAppliersHolder
	{
		public PluralizedTablesPack(IDomainInspector domainInspector, IInflector inflector)
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
			            		new ClassPluralizedTableApplier(inflector)
			            	};
			joinedSubclass = new List<IPatternApplier<Type, IJoinedSubclassAttributesMapper>>
			                 	{
			                 		new JoinedSubclassPluralizedTableApplier(inflector)
			                 	};
			unionSubclass = new List<IPatternApplier<Type, IUnionSubclassAttributesMapper>>
			                	{
			                		new UnionSubclassPluralizedTableApplier(inflector)
			                	};
			collectionPath = new List<IPatternApplier<PropertyPath, ICollectionPropertiesMapper>>
			                 	{
			                 		new ManyToManyPluralizedTableApplier(domainInspector, inflector)
			                 	};
		}
	}
}