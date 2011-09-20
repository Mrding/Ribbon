using System;
using System.Windows;
using System.Windows.Markup;

namespace Luna.Globalization
{
    [MarkupExtensionReturnType(typeof(object)), Localizability(LocalizationCategory.NeverLocalize)]
    public class ResourceExtension : MarkupExtension
    {
        private const string PreFix = "Luna.GUI.Views.";

        public ResourceExtension()
        { }

        public ResourceExtension(object key)
        {
            Key = key;
        }

        [ConstructorArgument("Key")]
        public object Key
        { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            //if (Key != null)
            //{
            //    var strKey = Key.ToString();
            //    //特殊字符，则自动拼装Language的Key
            //    if (!String.IsNullOrEmpty(strKey) && strKey.StartsWith("~", StringComparison.Ordinal))
            //    {
            //        if (serviceProvider != null)
            //        {
            //            var target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            //            if (target == null) throw new SystemException("null");
            //            var dependencyObj = target.TargetObject as DependencyObject;
            //            if (dependencyObj == null) throw new SystemException("null");
            //            var result = dependencyObj.FindAncestor<Window>() as DependencyObject;
                        
            //            if (result == null) throw new SystemException("null");
            //            if (result == null)
            //                result = dependencyObj.FindAncestor<UserControl>();
            //            if (result != null)
            //            {
            //                Key = result.GetType().FullName.Remove(0, PreFix.Length).Replace('.', '_') + strKey.Replace('~', '_');
            //                System.Diagnostics.Debug.Print(Key.ToString());
            //            }
            //            //Key = Compose(target.BaseUri, strKey);
            //            //Key = "Administration_LoginView_Login";
            //        }
            //    }
            //}

            return LanguageReader.GetValue(Key);
        }

        //private static string Compose(Uri uri, string key)
        //{
        //    //Output: /Luna.GUI;component/views/administration/loginview.xaml
        //    var pointIndex = uri.LocalPath.LastIndexOf('.');
        //    //Output: /Luna.GUI;component/views/administration/loginview
        //    var strExcludexaml = uri.LocalPath.Remove(pointIndex, uri.LocalPath.Length - pointIndex);
        //    //Output: administration/loginview
        //    var view = strExcludexaml.Remove(0, PreFix.Length);
        //    var array = view.Split('/');
        //    //Output: A
        //    var firstLetter = array[0][0].ToString().ToUpper();
        //    //Output: Administration
        //    var first = firstLetter + array[0].Remove(0, 1);
        //    //Output: L
        //    var secondLetter = array[1][0].ToString().ToUpper();
        //    //Output: Loginview
        //    var second = secondLetter + array[1].Remove(0, 1);
        //    //Output: Administration_Loginview_Key
        //    return first + "_" + second + key.Replace('~', '_');
        //}
    }
}
