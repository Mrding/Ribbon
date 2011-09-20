namespace ConfOrm.Patterns
{
	public class PropertyToFieldMUnderscorePascalCasePattern : AbstractPropertyToFieldPattern
	{
		protected override string GetFieldName(string propertyName)
		{
			return "m_" + propertyName.Substring(0, 1).ToUpperInvariant() + propertyName.Substring(1);
		}
	}
}