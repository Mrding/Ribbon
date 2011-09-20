using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Luna.WPF.ApplicationFramework.Controls
{
    /// <summary>
    /// support design UI in visual studio when element don't have enough dynamtic data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DesignElementDraw<T> : ElementDraw<T> where T : FrameworkElement
    {
        public override bool CanDraw()
        {
            return Caliburn.PresentationFramework.PresentationFrameworkModule.IsInDesignMode;
        }
    }
}
