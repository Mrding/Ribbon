namespace ConfOrm
{
	public enum PoIdStrategy
	{
		HighLow,
		Sequence,
		Guid,
		GuidOptimized,
		Identity,
		Assigned,
		Native
	}

	public interface IPersistentIdStrategy
	{
		PoIdStrategy Strategy { get; }

		/// <summary>
		/// Any object as "property value" container
		/// </summary>
		object Params { get; }
	}
}