using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;

namespace ConfOrm.Shop.Packs
{
	public class EnumAsStringPack: EmptyPatternsAppliersHolder
	{
		public EnumAsStringPack()
		{
			property = new List<IPatternApplier<MemberInfo, IPropertyMapper>>
			           	{
			           		new EnumPropertyAsStringApplier()
			           	};
			element = new List<IPatternApplier<MemberInfo, IElementMapper>>
			          	{
			          		new EnumElementAsStringApplier()
			          	};
		}
	}
}