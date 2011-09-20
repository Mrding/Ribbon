using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Patterns;

namespace ConfOrm.Shop.Packs
{
	public class BidirectionalOneToManyRelationPack : EmptyPatternsAppliersHolder
	{
		public BidirectionalOneToManyRelationPack(IDomainInspector domainInspector)
		{
			collection = new List<IPatternApplier<MemberInfo, ICollectionPropertiesMapper>>
			             	{
			             		new BidirectionalOneToManyCascadeApplier(domainInspector),
			             		new BidirectionalOneToManyInverseApplier(domainInspector),
			             		new BidirectionalOneToManyOnDeleteConstraintApplier(domainInspector),
			             	};

		}
	}
}