using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Markup;

namespace Luna.WPF.ApplicationFramework
{
    public class BlendMergedDictionary : ResourceDictionary
    {
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //public Uri Path
        //{
        //    get { return base.Source; }
        //    set
        //    {
        //        //if (!Caliburn.PresentationFramework.PresentationFrameworkModule.IsInDesignMode)
        //        //    return;
        //        //Debug.WriteLine("Setting Source = " + value);
        //        base.Source = value;
        //    }
        //}

        public new String Source
        {
            get
            {
                return base.Source.ToString();
            }
            set
            {
                if (!Caliburn.PresentationFramework.PresentationFrameworkModule.IsInDesignMode)
                    return;
               

                base.Source = new Uri(value, UriKind.RelativeOrAbsolute);
            }
        }
    }
}