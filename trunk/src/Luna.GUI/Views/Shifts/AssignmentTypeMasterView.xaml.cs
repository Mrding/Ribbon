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
using System.Windows.Threading;
using Luna.WPF.ApplicationFramework.Threads;

namespace Luna.GUI.Views.Shifts
{
    /// <summary>
    /// Interaction logic for AssignmentTypeEditView.xaml
    /// </summary>
    public partial class AssignmentTypeMasterView
    {
        public AssignmentTypeMasterView()
        {
            InitializeComponent();

            this.Unloaded += new RoutedEventHandler(OnViewUnloaded);
        }

        void OnViewUnloaded(object sender, RoutedEventArgs e)
        {
            ItemList.SelectionChanged -= OnItemListOnSelectionChanged;
            ItemList.Loaded -= ItemList_Loaded;
            Unloaded -= OnViewUnloaded;
        }

        private void ItemList_Loaded(object sender, RoutedEventArgs e)
        {
            ItemList.SelectionChanged += OnItemListOnSelectionChanged;

       
        }

        private void OnItemListOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UIThread.BeginInvoke(() =>
                                     {
                                         var lb = (ListBoxItem) (ItemList.ItemContainerGenerator.ContainerFromItem(ItemList.SelectedItem));
                                         if (lb == null)
                                             return;

                                         lb.BringIntoView();
                                     }, DispatcherPriority.Input);
        }
    }
}
