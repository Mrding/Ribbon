namespace ConfOrm.Patterns
{
	public class PropertyToFieldUnderscoreCamelCasePattern : AbstractPropertyToFieldPattern
	{
		protected override string GetFieldName(string propertyName)
		{
			return "_" + propertyName.Substring(0, 1).ToLowerInvariant() + propertyName.Substring(1);
		}
	}
}