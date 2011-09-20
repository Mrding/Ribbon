using System;
using System.Collections.Generic;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;

namespace ConfOrm.Shop.Packs
{
	public class UseNoLazyForNoProxablePack: EmptyPatternsAppliersHolder
	{
		public UseNoLazyForNoProxablePack()
		{
			rootClass = new List<IPatternApplier<Type, IClassAttributesMapper>> { new UseNoLazyForNoProxableApplier<IClassAttributesMapper>() };
			joinedSubclass = new List<IPatternApplier<Type, IJoinedSubclassAttributesMapper>> { new UseNoLazyForNoProxableApplier<IJoinedSubclassAttributesMapper>() };
			subclass = new List<IPatternApplier<Type, ISubclassAttributesMapper>> { new UseNoLazyForNoProxableApplier<ISubclassAttributesMapper>() };
			unionSubclass = new List<IPatternApplier<Type, IUnionSubclassAttributesMapper>> { new UseNoLazyForNoProxableApplier<IUnionSubclassAttributesMapper>() };
		}
	}
}