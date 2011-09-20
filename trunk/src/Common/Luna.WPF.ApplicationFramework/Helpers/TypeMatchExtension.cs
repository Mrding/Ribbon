using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Luna.WPF.ApplicationFramework.Helpers
{
    [MarkupExtensionReturnType(typeof(bool)), Localizability(LocalizationCategory.NeverLocalize)]
    public class TypeMatchExtension : MarkupExtension
    {
        public TypeMatchExtension(){}

        public TypeMatchExtension(Binding binding, Type type)
        {
            Type = type;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            
            return true;
        }

        [ConstructorArgument("Type")]
        public Type Type { get; set; }

        

    }
}
