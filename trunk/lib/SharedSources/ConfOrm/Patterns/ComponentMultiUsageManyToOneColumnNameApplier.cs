using System;
using ConfOrm.Mappers;
using ConfOrm.NH;

namespace ConfOrm.Patterns
{
	public class ComponentMultiUsageManyToOneColumnNameApplier : ComponentMultiUsagePattern, IPatternApplier<PropertyPath, IManyToOneMapper>
	{
		#region Implementation of IPatternApplier<PropertyPath,IManyToOneMapper>

		public void Apply(PropertyPath subject, IManyToOneMapper applyTo)
		{
			applyTo.Column(subject.ToColumnName());
		}

		#endregion
	}
}