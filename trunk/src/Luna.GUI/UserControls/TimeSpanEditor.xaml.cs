using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Caliburn.PresentationFramework;

namespace Luna.GUI.UserControls
{
    /// <summary>
    /// Interaction logic for TimeSpanEditor.xaml
    /// </summary>
    public partial class TimeSpanEditor : UserControl
    {
        public static HourFormat HourFormat = new HourFormat();

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.RegisterAttached("Value", typeof(int), typeof(TimeSpanEditor), new UIPropertyMetadata(60));

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(TimeSpanEditor), new UIPropertyMetadata(string.Empty));

        public TimeSpanEditor()
        {
            InitializeComponent();
            if (!PresentationFrameworkModule.IsInDesignMode)
            {
                panel.SetBinding(DataContextProperty, new Binding { Source = this });
                hourNumeric.StringFormatInfo = HourFormat;
                minNumeric.StringFormatInfo = HourFormat;
            }  
        }

        public static int GetValue(DependencyObject obj)
        {
            return (int)obj.GetValue(ValueProperty);
        }

        public static void SetValue(DependencyObject obj, int value)
        {
            obj.SetValue(ValueProperty, value);
        }

        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public decimal Minimum
        {
            get { return (decimal)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum", typeof(decimal), typeof(TimeSpanEditor),
                new FrameworkPropertyMetadata((decimal)0));

        public decimal Maximum
        {
            get { return (decimal)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum", typeof(decimal), typeof(TimeSpanEditor),
                new FrameworkPropertyMetadata(decimal.MaxValue));

        
    }
}
