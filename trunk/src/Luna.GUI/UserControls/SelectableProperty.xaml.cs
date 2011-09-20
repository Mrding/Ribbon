using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Luna.GUI.UserControls
{
    /// <summary>
    /// SelectableProperty.xaml 的交互逻辑
    /// </summary>
    public partial class SelectableProperty : UserControl
    {
        public SelectableProperty()
        {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                Binding bind=new Binding(){ Source=this};
                panel.SetBinding(FrameworkElement.DataContextProperty, bind);
            }
        }

        public bool IsNew
        {
            get { return (bool)GetValue(IsNewProperty); }
            set { SetValue(IsNewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsNew.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsNewProperty =
            DependencyProperty.Register("IsNew", typeof(bool), typeof(SelectableProperty), new UIPropertyMetadata(true));



        public bool AllowAddItem
        {
            get { return (bool)GetValue(AllowAddItemProperty); }
            set { SetValue(AllowAddItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowAddItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowAddItemProperty =
            DependencyProperty.Register("AllowAddItem", typeof(bool), typeof(SelectableProperty), new UIPropertyMetadata(false));





        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(SelectableProperty), new UIPropertyMetadata(null));




        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(SelectableProperty), new UIPropertyMetadata(null));



        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SelectableProperty), new UIPropertyMetadata(null));
    }
}
