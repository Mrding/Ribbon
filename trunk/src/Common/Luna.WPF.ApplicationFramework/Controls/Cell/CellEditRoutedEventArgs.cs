using System;
using System.Collections.Generic;
using System.Windows;
using Luna.Common;
using Luna.Core;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public class CellEditRoutedEventArgs : RoutedEventArgs
    {
        public CellEditRoutedEventArgs(RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
        
        }

        public bool HasChanged { get; set; }
    }
}