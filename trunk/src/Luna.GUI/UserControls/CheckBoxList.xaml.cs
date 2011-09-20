using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Luna.Core.Extensions;
using System.Collections;
using Luna.WPF.ApplicationFramework.Converters;

namespace Luna.GUI.UserControls
{
    /// <summary>
    /// Interaction logic for CheckBoxList.xaml
    /// </summary>
    public partial class CheckBoxList : UserControl
    {
        public CheckBoxList()
        {
            InitializeComponent();
        }

        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(CheckBoxList), new UIPropertyMetadata(null,
                (sender, e) =>
                {
                }));


        public int ArrayInt
        {
            get { return (int)GetValue(ArrayStringProperty); }
            set { SetValue(ArrayStringProperty, value); }
        }

        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Columns.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(int), typeof(CheckBoxList), new UIPropertyMetadata(7));

        // Using a DependencyProperty as the backing store for ArrayString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ArrayStringProperty =
            DependencyProperty.Register("ArrayInt", typeof(int), typeof(CheckBoxList),
            new UIPropertyMetadata(127));




        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(CheckBoxList), new UIPropertyMetadata(null));



        private void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ItemsSource == null) return;

            var boolArray = ItemsSource.As<IEnumerable>().OfType<Common.ISelectable>()
                .Select(o => (o.IsSelected == true)).ToArray();
            
            var newValue = boolArray.ToInteger();
            
            if (newValue == ArrayInt) return;

            ArrayInt = newValue;
        }
    }
}
