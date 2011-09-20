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
using System.Windows.Shapes;

namespace Luna.GUI.Views.Shifts
{
    /// <summary>
    /// Interaction logic for CreateAssignmentTypeView.xaml
    /// </summary>
    public partial class NewAssignmentTypeDialog : UserControl
    {
        public NewAssignmentTypeDialog()
        {
            DataContextChanged += new DependencyPropertyChangedEventHandler(CreateAssignmentTypeView_DataContextChanged);
            Unloaded += new RoutedEventHandler(CreateAssignmentTypeView_Unloaded);
            InitializeComponent();


        }

        void CreateAssignmentTypeView_Unloaded(object sender, RoutedEventArgs e)
        {
            //DataContext = null;
            //throw new NotImplementedException();
        }



        void CreateAssignmentTypeView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
