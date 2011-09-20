using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Patterns;

namespace ConfOrm.Shop.Packs
{
	public class OneToOneRelationPack : EmptyPatternsAppliersHolder
	{
		public OneToOneRelationPack(IDomainInspector domainInspector)
		{
			poid = new List<IPatternApplier<MemberInfo, IIdMapper>>
			       	{
			       		new BidirectionalOneToOneAssociationPoidApplier(domainInspector)
			       	};
			manyToOne = new List<IPatternApplier<MemberInfo, IManyToOneMapper>>
			            	{
			            		new BidirectionalForeignKeyAssociationManyToOneApplier(domainInspector),
			            		new UnidirectionalOneToOneUniqueCascadeApplier(domainInspector)
			            	};
			oneToOne = new List<IPatternApplier<MemberInfo, IOneToOneMapper>>
			           	{
			           		new BidirectionalForeignKeyAssociationOneToOneApplier(domainInspector),
			           		new BidirectionalPrimaryKeyAssociationMasterOneToOneApplier(domainInspector),
			           		new BidirectionalPrimaryKeyAssociationSlaveOneToOneApplier(domainInspector)
			           	};
		}
	}
}