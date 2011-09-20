using System.Collections.Generic;

namespace ConfOrm.Patterns
{
	public class PropertyToFieldPatterns
	{
		private static readonly List<AbstractPropertyToFieldPattern> DefaultsPatterns =
			new List<AbstractPropertyToFieldPattern>
				{
					new PropertyToFieldCamelCasePattern(),
					new PropertyToFieldUnderscorePascalCasePattern(),
					new PropertyToFieldMUnderscorePascalCasePattern(),
					new PropertyToFieldUnderscoreCamelCasePattern()
				};

		public static IEnumerable<AbstractPropertyToFieldPattern> Defaults
		{
			get { return DefaultsPatterns; }
		}
	}
}