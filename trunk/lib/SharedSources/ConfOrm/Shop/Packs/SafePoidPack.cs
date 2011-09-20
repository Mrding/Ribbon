using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Patterns;

namespace ConfOrm.Shop.Packs
{
	public class SafePoidPack : EmptyPatternsAppliersHolder
	{
		public SafePoidPack()
		{
			poid = new List<IPatternApplier<MemberInfo, IIdMapper>>
			       	{
			       		new MemberNoSetterToFieldAccessorApplier<IIdMapper>(),
			       		new NoPoidGuidApplier(),
			       	};
		}
	}
}