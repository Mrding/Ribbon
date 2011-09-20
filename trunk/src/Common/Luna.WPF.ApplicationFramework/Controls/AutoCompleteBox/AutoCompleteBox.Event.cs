// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Luna.WPF.ApplicationFramework.Controls.Automation.Peers;
using Properties = Luna.WPF.ApplicationFramework.Controls;
namespace Luna.WPF.ApplicationFramework.Controls
{
    public partial class AutoCompleteTextBox
    {
        #region Event

        /// <summary>
        /// Occurs when the text changes.
        /// </summary>
        public event RoutedEventHandler TextChanged;

        /// <summary>
        /// Raises the TextChanged event when the text has changed.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnTextChanged(RoutedEventArgs e)
        {
            RoutedEventHandler handler = TextChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when the AutoCompleteBox is populating the selection adapter 
        /// with suggestions based on the text property. 
        /// </summary>
        /// <remarks>
        /// If Canceled, the control will not continue the automatic suggestion 
        /// process, which will be left to the handler.
        /// </remarks>
        public event PopulatingEventHandler Populating;

        /// <summary>
        /// Raises the Populating event when the AutoCompleteBox is populating the 
        /// selection adapter with suggestions based on the text property.
        /// </summary>
        /// <param name="e">The populating event data.</param>
        protected virtual void OnPopulating(PopulatingEventArgs e)
        {
            PopulatingEventHandler handler = Populating;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when the AutoCompleteBox has populated the selection adapter 
        /// with suggestions based on the text property.
        /// </summary>
        public event PopulatedEventHandler Populated;

        /// <summary>
        /// Raises the Populated event when the AutoCompleteBox has populated the 
        /// selection adapter with suggestions based on the text property.
        /// </summary>
        /// <param name="e">The populated event data.</param>
        protected virtual void OnPopulated(PopulatedEventArgs e)
        {
            PopulatedEventHandler handler = Populated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when the IsDropDownOpen property is changing from false to 
        /// true. The event can be cancelled.
        /// </summary>
        public event RoutedPropertyChangingEventHandler<bool> DropDownOpening;

        /// <summary>
        /// Raises an DropDownOpening event when the IsDropDownOpen property is 
        /// changing from false to true.
        /// </summary>
        /// <param name="e">
        /// Provides any observers the opportunity to cancel the operation and 
        /// halt opening the drop down.
        /// </param>
        protected virtual void OnDropDownOpening(RoutedPropertyChangingEventArgs<bool> e)
        {
            RoutedPropertyChangingEventHandler<bool> handler = DropDownOpening;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when the IsDropDownOpen property was changed from false to true.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> DropDownOpened;

        /// <summary>
        /// Raises an DropDownOpened event when the IsDropDownOpen property 
        /// changed from false to true.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnDropDownOpened(RoutedPropertyChangedEventArgs<bool> e)
        {
            RoutedPropertyChangedEventHandler<bool> handler = DropDownOpened;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when the IsDropDownOpen property is changing from true to 
        /// false. The event can be cancelled.
        /// </summary>
        public event RoutedPropertyChangingEventHandler<bool> DropDownClosing;

        /// <summary>
        /// Raises an DropDownClosing event when the IsDropDownOpen property is 
        /// changing from true to false.
        /// </summary>
        /// <param name="e">
        /// Provides any observers the opportunity to cancel the operation 
        /// and halt closing the drop down.
        /// </param>
        protected virtual void OnDropDownClosing(RoutedPropertyChangingEventArgs<bool> e)
        {
            RoutedPropertyChangingEventHandler<bool> handler = DropDownClosing;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when the IsDropDownOpen property was changed from true to 
        /// false.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> DropDownClosed;

        /// <summary>
        /// Raises an DropDownClosed event when the IsDropDownOpen property 
        /// changed from true to false.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnDropDownClosed(RoutedPropertyChangedEventArgs<bool> e)
        {
            RoutedPropertyChangedEventHandler<bool> handler = DropDownClosed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when the selected item has changed.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Raises the SelectionChanged event when the selected item has
        /// changed.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            SelectionChangedEventHandler handler = SelectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion
    }
}
