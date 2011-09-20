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

namespace Luna.GUI.Views.Shifts
{
    /// <summary>
    /// Interaction logic for ShiftImportView.xaml
    /// </summary>
    public partial class ShiftImportView : ShiftView
    {
        public ShiftImportView()
        {
            InitializeComponent();
        }

        private void PART_ShowHideCheckBox_Checked(object sender, RoutedEventArgs e)
        {

            base.TopRowHeight = new GridLength(150);
        }

        private void PART_ShowHideCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            base.TopRowHeight = new GridLength(0);
        }

       
    }
}
