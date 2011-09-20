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
using ActiproSoftware.Windows.Controls.Ribbon.Input;
using ActiproSoftware.Windows.Controls.Ribbon;
using System.Reflection;
using ActiproSoftware.Windows.Controls.Ribbon.UI;
using ActiproSoftware.Windows.Controls.Ribbon.Controls;
using Luna.WPF.ApplicationFramework;
using Luna.WPF.ApplicationFramework.Converters;
using ActiproSoftware.Windows;
using ActiproSoftware.Windows.Controls.Editors;
using System.Collections;
namespace Luna.GUI.Views.Shifts
{
    /// <summary>
    /// Interaction logic for SubEventTypeDetailDialog.xaml
    /// </summary>
    public partial class SubEventTypeDetailDialog
    {
     

        public SubEventTypeDetailDialog()
        {
            //this.Loaded += new RoutedEventHandler(SubEventTypeDetailView_Loaded);
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                
                Element_SubeventTypes.Items.Filter = new Predicate<object>((object obj) =>
                {

                    if (obj.GetType().GetProperty("Key").GetValue(obj, null).ToString().ToLower().Contains("absent")) return false;
                    return true;
                });
            };
            
        }

        void SubEventTypeDetailView_Loaded(object sender, RoutedEventArgs e)
        {
        
            //Binding binding = new Binding("Entity.Background");
            //binding.Converter = Application.Current.FindResource("NameToColorConverter") as IValueConverter;
            //binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //Background_Element.SetBinding(ColorEditBox.ValueProperty, binding);
            //ColorPickerGallery gallery = new ColorPickerGallery();
            //gallery.HorizontalAlignment = HorizontalAlignment.Left;
            //gallery.Width = 280;
            //gallery.ItemsSource =StaticObjectConverter.GetObjectList(typeof(Brushes));
            //gallery.SelectedItemChanged += ColorPickerGallery_SelectedItemChanged;
            //Background_Element.DropDownContent = gallery;
            //Background_Element.Format = "";
            
        }

        private void ColorPickerGallery_SelectedItemChanged(object sender, ObjectPropertyChangedRoutedEventArgs e)
        {
            //var brush = e.NewValue as SolidColorBrush;
            //if (brush == null) return;
            //Background_Element.Value = brush.Color;
            //Background_Element.IsDropDownOpen = false;
        }
    }

}
