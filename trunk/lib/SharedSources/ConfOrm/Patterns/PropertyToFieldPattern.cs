using System.Linq;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class PropertyToFieldPattern : IPattern<MemberInfo>
	{
		#region Implementation of IPattern<MemberInfo>

		public bool Match(MemberInfo subject)
		{
			var property = subject as PropertyInfo;
			if (property == null)
			{
				return false;
			}
			var fieldPattern = PropertyToFieldPatterns.Defaults.FirstOrDefault(pp => pp.Match(property));
			if (fieldPattern != null)
			{
				var fieldInfo = fieldPattern.GetBackFieldInfo(property);
				return fieldInfo.FieldType != property.PropertyType;
			}

			return false;
		}

		#endregion
	}
}