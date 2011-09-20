namespace ConfOrm.Patterns
{
	public class PropertyToFieldUnderscorePascalCasePattern : AbstractPropertyToFieldPattern
	{
		protected override string GetFieldName(string propertyName)
		{
			return "_" + propertyName.Substring(0, 1).ToUpperInvariant() + propertyName.Substring(1);
		}
	}
}