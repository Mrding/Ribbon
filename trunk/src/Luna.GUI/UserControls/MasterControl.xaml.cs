using System.Windows;
using System.Windows.Controls;

namespace Luna.GUI.UserControls
{
    /// <summary>
    /// 用于创建Master,Detail页面之用
    /// </summary>
    public partial class MasterControl : UserControl
    {
        public MasterControl()
        {
            InitializeComponent();
        }

        public string FunctionName
        {
            get { return (string)GetValue(FunctionNameProperty); }
            set { SetValue(FunctionNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FunctionName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FunctionNameProperty =
            DependencyProperty.Register("FunctionName", typeof(string), typeof(MasterControl), new UIPropertyMetadata(null));





        public DataTemplate ItemDisplayTemplate
        {
            get { return (DataTemplate)GetValue(ItemDisplayTemplateProperty); }
            set { SetValue(ItemDisplayTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemDisplayTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemDisplayTemplateProperty =
            DependencyProperty.Register("ItemDisplayTemplate", typeof(DataTemplate), typeof(MasterControl), new UIPropertyMetadata(null));




    }
}
