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

namespace Luna.GUI.UserControls
{
    /// <summary>
    /// Interaction logic for TimeRangePicker.xaml
    /// </summary>
    public partial class TimeRangePicker : UserControl
    {
        public static HourFormat2 HourFormat = new HourFormat2();

        public TimeRangePicker()
        {
            InitializeComponent();
            StartValue = 100;
            EndValue = 300;
        }

        public int StartValue
        {
            get { return (int)GetValue(StartValueProperty); }
            set { SetValue(StartValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartValueProperty =
            DependencyProperty.Register("StartValue", typeof(int), typeof(TimeRangePicker), new UIPropertyMetadata(100));


        public int EndValue
        {
            get { return (int)GetValue(EndValueProperty); }
            set { SetValue(EndValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndValueProperty =
            DependencyProperty.Register("EndValue", typeof(int), typeof(TimeRangePicker), new UIPropertyMetadata(300));

        
    }
}
