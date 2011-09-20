using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ActiproSoftware.Windows.Media;
using ActiproSoftware.Windows.Controls.Ribbon.Controls.Primitives;
using ActiproSoftware.Windows.Controls.Ribbon;
using Luna.WPF.ApplicationFramework.Extensions;
using ActiproSoftware.Windows.Controls.Ribbon.Controls;
using Luna.Core.Extensions;

namespace Luna.GUI.Views.Shifts
{
    /// <summary>
    /// Interaction logic for ScheduleInfomationView.xaml
    /// </summary>
    public partial class ScheduleInfomationView
    {
        public ScheduleInfomationView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, ActiproSoftware.Windows.Controls.Ribbon.Controls.ExecuteRoutedEventArgs e)
        {
            ((DependencyObject) sender).FindAncestor<ActiproSoftware.Windows.Controls.Ribbon.Controls.Menu>().
                SaftyInvoke(menu =>
                                {
                                    menu.Parent.SaftyInvoke<PopupButton>(b=> b.IsPopupOpen = false);
                                });


            //popupButton.IsPopupOpen = false;

            //RibbonWindow ribbonWindow = VisualTreeHelperExtended.GetCurrentOrAncestor(this, typeof(RibbonWindow)) as RibbonWindow;
            //if (null != ribbonWindow)
            //{
            //    Ribbon button = VisualTreeHelperExtended.GetFirstDescendant(ribbonWindow, typeof(Ribbon)) as Ribbon;
            //    if (null != button)
            //        button.IsApplicationMenuOpen = true;
            //}
        }
    }
}
