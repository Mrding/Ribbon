using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using Luna.WPF.ApplicationFramework.Threads;

namespace Luna.WPF.ApplicationFramework
{
    public class FlowDocumentService
    {


        public static string GetRunText(DependencyObject obj)
        {
            return (string)obj.GetValue(RunTextProperty);
        }

        public static void SetRunText(DependencyObject obj, string value)
        {
            obj.SetValue(RunTextProperty, value);
        }

        public static readonly DependencyProperty RunTextProperty =
            DependencyProperty.RegisterAttached("RunText", typeof(string), typeof(FlowDocumentService),
            new UIPropertyMetadata(string.Empty, (o, a) =>
            {
                var element = o as Run;
                if (a.NewValue != null)
                {
                    UIThread.BeginInvoke(() => {
                        element.Text = a.NewValue.ToString();
                    });
                    //element.DataContext = null;
                    
                }
                
            }));
    }
}
