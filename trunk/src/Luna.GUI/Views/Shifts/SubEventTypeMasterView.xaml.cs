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
using Luna.Shifts.Presenters;

namespace Luna.GUI.Views.Shifts
{
    /// <summary>
    /// Interaction logic for SubEventTypeMasterView.xaml
    /// </summary>
    public partial class SubEventTypeMasterView
    {
        public SubEventTypeMasterView()
        {
            InitializeComponent();
        }

        public object DragItem
        {
            get { return (object)GetValue(DragItemProperty); }
            set { SetValue(DragItemProperty, value); }
        }

        public static readonly DependencyProperty DragItemProperty =
            DependencyProperty.Register("DragItem", typeof(object), typeof(SubEventTypeMasterView),
            new UIPropertyMetadata());
    }
}
