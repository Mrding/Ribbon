namespace Luna.Globalization
{
    public static class LanguageKeyExtension
    {
        public static string GetLanguageValue(this string key)
        {
            return LanguageReader.GetValue(key);
        }

        public static object GetLanguageValue(this object key)
        {
            return LanguageReader.GetValue(key);
        }

        public static T GetLanguageValue<T>(this object key)
        {
            return LanguageReader.GetValue<T>(key);
        }
    }
}
