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

namespace Luna.GUI.Views
{
    /// <summary>
    /// Interaction logic for InputTextBoxView.xaml
    /// </summary>
    public partial class DialogBoxView
    {
        public static RoutedEvent ConfirmingEvent = EventManager.RegisterRoutedEvent
            ("Confirming", RoutingStrategy.Bubble, typeof(RoutedEvent), typeof(DialogBoxView));

        public static void AddConfirmingHandler(DependencyObject d, RoutedEventHandler h)
        {
            var e = d as UIElement;
            if (e != null)
            {
                e.AddHandler(DialogBoxView.ConfirmingEvent, h);
            }
        }

        public void RemoveConfirmingHandler(DependencyObject d, RoutedEventHandler h)
        {
            var e = d as UIElement;
            if (e != null)
            {
                e.RemoveHandler(DialogBoxView.ConfirmingEvent, h);
            }
        }

        public DialogBoxView()
        {
            InitializeComponent();
        }
    }
}
