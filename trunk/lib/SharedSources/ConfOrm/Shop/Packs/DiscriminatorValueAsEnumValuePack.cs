using System;
using System.Collections.Generic;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Subclassing;

namespace ConfOrm.Shop.Packs
{
	public class DiscriminatorValueAsEnumValuePack<TRootEntity, TEnum> : EmptyPatternsAppliersHolder
		where TRootEntity : class
		where TEnum : struct
	{
		public DiscriminatorValueAsEnumValuePack(IDomainInspector domainInspector)
		{
			rootClass = new List<IPatternApplier<Type, IClassAttributesMapper>>
			            	{
			            		new DiscriminatorColumnNameApplier(domainInspector),
											new DiscriminatorIndexNameApplier(domainInspector),
			            		new ClassDiscriminatorValueAsEnumValueApplier<TRootEntity, TEnum>(domainInspector)
			            	};
			subclass = new List<IPatternApplier<Type, ISubclassAttributesMapper>>
			           	{
			           		new SubclassDiscriminatorValueAsEnumValueApplier<TRootEntity, TEnum>()
			           	};
		}
	}
}