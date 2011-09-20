using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Patterns;

namespace ConfOrm.Shop.Packs
{
	public class BidirectionalManyToManyRelationPack : EmptyPatternsAppliersHolder
	{
		public BidirectionalManyToManyRelationPack(IDomainInspector domainInspector)
		{
			collection = new List<IPatternApplier<MemberInfo, ICollectionPropertiesMapper>>
			             	{
			             		new BidirectionalManyToManyInverseApplier(domainInspector),
			             	};
		}
	}
}