using System;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class JoinedSubclassOnDeleteApplier : IPatternApplier<Type, IJoinedSubclassAttributesMapper>
	{
		#region Implementation of IPattern<Type>

		public bool Match(Type subject)
		{
			return true;
		}

		#endregion

		#region Implementation of IPatternApplier<Type,IJoinedSubclassAttributesMapper>

		public void Apply(Type subject, IJoinedSubclassAttributesMapper applyTo)
		{
			applyTo.Key(km=> km.OnDelete(OnDeleteAction.Cascade));
		}

		#endregion
	}
}