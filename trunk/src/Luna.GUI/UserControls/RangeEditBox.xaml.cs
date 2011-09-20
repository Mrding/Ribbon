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

namespace Luna.GUI.UserControls
{
    /// <summary>
    /// Interaction logic for RangeEditBox.xaml
    /// </summary>
    public partial class RangeEditBox : UserControl
    {
        public RangeEditBox()
        {
            InitializeComponent();
            StartValue = 1;
            EndValue = 5;
        }

        public int StartValue
        {
            get { return (int)GetValue(StartValueProperty); }
            set { SetValue(StartValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartValueProperty =
            DependencyProperty.Register("StartValue", typeof(int), typeof(RangeEditBox), new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public int EndValue
        {
            get { return (int)GetValue(EndValueProperty); }
            set { SetValue(EndValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndValueProperty =
            DependencyProperty.Register("EndValue", typeof(int), typeof(RangeEditBox), new FrameworkPropertyMetadata(5, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));




        public int Min
        {
            get { return (int)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Min.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(int), typeof(RangeEditBox), new UIPropertyMetadata(0));




        public int Max
        {
            get { return (int)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Max.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(int), typeof(RangeEditBox), new UIPropertyMetadata(5));


        
    }
}
