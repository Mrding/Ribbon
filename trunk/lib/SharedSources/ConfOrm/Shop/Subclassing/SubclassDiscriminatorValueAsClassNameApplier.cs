using System;
using ConfOrm.Mappers;

namespace ConfOrm.Shop.Subclassing
{
	public class SubclassDiscriminatorValueAsClassNameApplier : IPatternApplier<Type, ISubclassAttributesMapper>
	{
		#region IPatternApplier<Type,ISubclassMapper> Members

		public bool Match(Type subject)
		{
			// because this applier will be called only for subclasses we don't have to check anything
			return true;
		}

		public void Apply(Type subject, ISubclassAttributesMapper applyTo)
		{
			applyTo.DiscriminatorValue(subject.Name);
		}

		#endregion
	}
}