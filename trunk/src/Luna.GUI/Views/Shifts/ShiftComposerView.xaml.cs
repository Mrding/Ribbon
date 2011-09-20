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
using ActiproSoftware.Windows.Controls.Ribbon.Controls;
using Luna.WPF.ApplicationFramework.Extensions;
using Luna.WPF.ApplicationFramework.Threads;
using System.ComponentModel;
using Luna.WPF.ApplicationFramework.Controls;
using System.Windows.Threading;
namespace Luna.GUI.Views.Shifts
{
    /// <summary>
    /// Interaction logic for ShiftComposer.xaml
    /// </summary>
    public partial class ShiftComposerView : ShiftView
    {
        private GridLength? _contentGrid2ndRowDefinitionHeight = new GridLength(1, GridUnitType.Star);
        private GridLength? _contentGrid1stRowDefinitionHeight = new GridLength(1, GridUnitType.Star);

        public ShiftComposerView()
        {
            InitializeComponent();

            this.Unloaded += OnViewUnloaded;

            var dpd = DependencyPropertyDescriptor.FromProperty(BlockGridLayerContainer.CellModeProperty, typeof(BlockGridLayerContainer));
            if (dpd != null)
            {
                dpd.AddValueChanged(Element_BlockGridLayerContainer, OnCellModeChangedHandler);
            }
        }

        private void OnCellModeChangedHandler(object sender, EventArgs e)
        {
            if (!Element_BlockGridLayerContainer.CellMode)
                _contentGrid2ndRowDefinitionHeight = new GridLength(ContentGrid.RowDefinitions[1].ActualHeight, GridUnitType.Star);

            if (!Element_BlockGridLayerContainer.CellMode)
                _contentGrid1stRowDefinitionHeight = new GridLength(ContentGrid.RowDefinitions[0].ActualHeight, GridUnitType.Star);

            ContentGrid.RowDefinitions[0].Height = _contentGrid1stRowDefinitionHeight.Value;
            ContentGrid.RowDefinitions[1].Height = Element_BlockGridLayerContainer.CellMode ? _contentGrid2ndRowDefinitionHeight.Value : new GridLength(0);

            if (Element_BlockGridLayerContainer.CellMode)
            {
                BindingOperations.SetBinding(AssignmentEstimationGrid, ScheduleGrid.ScreenStartProperty, new Binding("ScreenStart"));

                //xBindingOperations.SetBinding(LinechartDashboard, ScheduleGrid.ScreenStartProperty, new Binding("SelectedColumnDate"));
                //xBindingOperations.ClearBinding(LinechartDashboard, ScheduleGrid.ScreenStartProperty);
            }
            else
            {
                BindingOperations.ClearBinding(AssignmentEstimationGrid, ScheduleGrid.ScreenStartProperty);
                //xBindingOperations.SetBinding(LinechartDashboard, ScheduleGrid.ScreenStartProperty, new Binding("ScreenStart"));
                //x BindingOperations.SetBinding(LinechartDashboard, ScheduleGrid.ScreenStartProperty, new Binding("ScreenStart"){Source = DataContext, Mode = BindingMode.OneWay});
            }
        }

        private void OnViewUnloaded(object sender, RoutedEventArgs e)
        {
            var dpd = DependencyPropertyDescriptor.FromProperty(BlockGridLayerContainer.CellModeProperty, typeof(BlockGridLayerContainer));
            if (dpd != null)
            {
                dpd.RemoveValueChanged(Element_BlockGridLayerContainer, OnCellModeChangedHandler);
            }
            this.Unloaded -= OnViewUnloaded;
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
