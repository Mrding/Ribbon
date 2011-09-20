using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Luna.WPF.ApplicationFramework.Controls;

namespace Luna.WPF.ApplicationFramework.Graphics
{
    public class ElementPanel : AxisControl
    {
        protected UIElementCollection Children { get; set; }

        public ElementPanel()
        {
            Children = new UIElementCollection(this, this);
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return Children.Count;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return Children[index];
        }
    }
}
