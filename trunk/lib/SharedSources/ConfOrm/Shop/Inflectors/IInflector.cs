namespace ConfOrm.Shop.Inflectors
{
	/// <summary>
	/// Inflector for pluralize and singularize nouns.
	/// </summary>
	public interface IInflector
	{
		/// <summary>
		/// Pluralizes nouns.
		/// </summary>
		/// <param name="word">Singular noun.</param>
		/// <returns>Plural noun.</returns>
		string Pluralize(string word);

		/// <summary>
		/// Singularizes nouns.
		/// </summary>
		/// <param name="word">Plural noun.</param>
		/// <returns>Singular noun.</returns>
		string Singularize(string word);

	}
}