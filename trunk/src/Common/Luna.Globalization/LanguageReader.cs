using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Linq;

namespace Luna.Globalization
{
    /// <summary>
    /// Read the Language
    /// </summary>
    public static class LanguageReader
    {
        #region Fields

        private const string LanguagePath = @".\Languages\";
        private static ResourceDictionary _currentLanguage;
        private static ResourceDictionary _defaultLanguage;

        #endregion Fields

        static LanguageReader()
        {
            var uri = new Uri("Luna.Globalization;component/Default.xaml", UriKind.RelativeOrAbsolute);
            _defaultLanguage = Application.LoadComponent(uri) as ResourceDictionary;
            _currentLanguage = _defaultLanguage;
        }

        private static bool CheckLanguageName(string languageName)
        {
            var allLanguageNames = Directory.GetFiles(LanguagePath);
            return allLanguageNames.Any(name => name.Contains(languageName));
        }

        /// <summary>
        /// Loads the specified culture.
        /// </summary>
        /// <param name="culture">The culture.</param>
        public static void Load(string culture)
        {
            var isExistLanguage = CheckLanguageName(culture);
            if (isExistLanguage)
            {
                var path = string.Format("{0}{1}.xaml", LanguagePath, culture);
                using (var fs = File.OpenRead(path))
                {
                    _currentLanguage = XamlReader.Load(fs) as ResourceDictionary;
                    //屏蔽此段代码，这样节省Applicaion资源字典的内存使用，加快查找速度
                    //缺点是无法使用StaticResource/DynamicResource,只能使用inf:Resource
                    //if (Application.Current.Resources.MergedDictionaries.Contains(_currentLanguage))
                    //{
                    //    Application.Current.Resources.MergedDictionaries.Remove(_currentLanguage);
                    //}

                    //Application.Current.Resources.MergedDictionaries.Add(_currentLanguage);
                }

                var cultureInfo = culture.ToLower().Contains("zh") ? new CultureInfo(culture)
                                      {
                                          DateTimeFormat = { AbbreviatedDayNames = GetValue<string>("AbbreviatedDayNames").Split(',') }
                                      } : CultureInfo.GetCultureInfo(culture);
                Application.Current.Resources.Add("CurrentCulture", cultureInfo);
                Application.Current.Dispatcher.Thread.CurrentCulture = cultureInfo;
                Application.Current.Dispatcher.Thread.CurrentUICulture = cultureInfo;
            }
            else
            {
                //set default Language
                Application.Current.Dispatcher.Thread.CurrentCulture = CultureInfo.GetCultureInfo("zh-CN");
                Application.Current.Dispatcher.Thread.CurrentUICulture = CultureInfo.GetCultureInfo("zh-CN");
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string GetValue(string key)
        {
            var value = GetValue((object)key);
            return value == null ? key : value.ToString();
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static object GetValue(object key)
        {
            if (key == null)
                return null;
            var value = _currentLanguage[key] ?? _defaultLanguage[key];
            return value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static T GetValue<T>(object key)
        {
            var value = GetValue(key);
            if (value == null)
                return default(T);
            return (T)value;
        }

        public static bool ContainsKey(string key)
        {
            return ContainsKey((object)key);
        }

        public static bool ContainsKey(object key)
        {
            return _currentLanguage.Contains(key) || _defaultLanguage.Contains(key);
        }
    }
}
