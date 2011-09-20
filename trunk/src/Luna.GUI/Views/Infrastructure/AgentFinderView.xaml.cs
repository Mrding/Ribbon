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

namespace Luna.GUI.Views.Infrastructure
{
    /// <summary>
    /// Interaction logic for AgentFinderView.xaml
    /// </summary>
    public partial class AgentFinderView
    {
        public AgentFinderView()
        {
            InitializeComponent();
        }

        public Button SearchElement
        {
            get { return Element_Search; }
        }
    }
}
