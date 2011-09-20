using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Patterns;

namespace ConfOrm.Shop.Packs
{
	public class PolymorphismPack : EmptyPatternsAppliersHolder
	{
		public PolymorphismPack(IDomainInspector domainInspector)
		{
			collection = new List<IPatternApplier<MemberInfo, ICollectionPropertiesMapper>>
			                             	{
																			new PolymorphismBidirectionalOneToManyInverseApplier(domainInspector),
																			new PolymorphismBidirectionalOneToManyCascadeApplier(domainInspector),
																			new PolymorphismBidirectionalOneToManyOnDeleteConstraintApplier(domainInspector),
																			new PolymorphismBidirectionalAnyToManyKeyColumnApplier(domainInspector),
																			new PolymorphismBidirectionalAnyToManyWhereApplier(domainInspector),
			                             	};
			manyToOne = new List<IPatternApplier<MemberInfo, IManyToOneMapper>>
			            	{
											new PolymorphismManyToOneClassApplier(domainInspector),
			            	};
			oneToMany = new List<IPatternApplier<MemberInfo, IOneToManyMapper>>
									{
										new PolymorphismOneToManyClassApplier(domainInspector)
									};
			component = new List<IPatternApplier<Type, IComponentAttributesMapper>>
			            {
			            	new PolymorphismComponentClassApplier(domainInspector),
			            };

		}
	}
}