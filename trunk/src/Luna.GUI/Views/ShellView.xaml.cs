using System.Reflection;
using System.Windows;
using Luna.WPF.ApplicationFramework;
using Luna.WPF.ApplicationFramework.Extensions;

namespace Luna.GUI.Views
{

    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ShellView
    {
        public ShellView()
        {
            InitializeComponent();

            this.Loaded += (s, e) =>
            {
                //xthis.Title = string.Format("{0}({1})", Title, Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
                ribbon.IsApplicationMenuOpen = true;
                appMenu.SelectedIndex = 8;
            };
        }
    }
}