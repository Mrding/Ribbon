namespace ConfOrm.Patterns
{
	public class PropertyToFieldCamelCasePattern: AbstractPropertyToFieldPattern
	{
		protected override string GetFieldName(string propertyName)
		{
			return propertyName.Substring(0, 1).ToLowerInvariant() + propertyName.Substring(1);
		}
	}
}