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
    public partial class AutoCompleteTextBox : Control, IUpdateVisualState
    {

        #region public bool IsDropDownOpen
        /// <summary>
        /// Gets or sets a value indicating whether the drop down is open.
        /// </summary>
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        /// <summary>
        /// Identifies the IsDropDownOpen dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(
                "IsDropDownOpen",
                typeof(bool),
                typeof(AutoCompleteTextBox),
                new PropertyMetadata(false, OnIsDropDownOpenPropertyChanged));

        /// <summary>
        /// IsDropDownOpenProperty property changed handler.
        /// </summary>
        /// <param name="d">AutoCompleteTextBox that changed its IsDropDownOpen.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIsDropDownOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteTextBox source = d as AutoCompleteTextBox;

            // Ignore the change if requested
            if (source.IgnorePropertyChange)
            {
                source.IgnorePropertyChange = false;
                return;
            }

            bool oldValue = (bool)e.OldValue;
            bool newValue = (bool)e.NewValue;
            bool delayedClosingVisual = source.PopupClosedVisualState;
            RoutedPropertyChangingEventArgs<bool> args = new RoutedPropertyChangingEventArgs<bool>(e.Property, oldValue, newValue, true);

            AutoCompleteBoxAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(source) as AutoCompleteBoxAutomationPeer;
            if (peer != null)
            {
                peer.RaiseExpandCollapseAutomationEvent(oldValue, newValue);
            }

            if (newValue)
            {
                // Opening
                source.OnDropDownOpening(args);

                // Opened
                if (!args.Cancel)
                {
                    source.OpenDropDown(oldValue, newValue);
                }
            }
            else
            {
                // Closing
                source.OnDropDownClosing(args);

                if (source.View == null || source.View.Count == 0)
                {
                    delayedClosingVisual = false;
                }

                // Immediately close the drop down window:
                // When a popup closed visual state is present, the code path is 
                // slightly different and the actual call to CloseDropDown will 
                // be called only after the visual state's transition is done
                if (!args.Cancel && !delayedClosingVisual)
                {
                    source.CloseDropDown(oldValue, newValue);
                }
            }

            // If canceled, revert the value change
            if (args.Cancel)
            {
                source.IgnorePropertyChange = true;
                source.SetValue(e.Property, oldValue);
            }

            // Closing call when visual states are in use
            if (delayedClosingVisual)
            {
                source.UpdateVisualState(true);
            }
        }
        #endregion public bool IsDropDownOpen

        #region Popup

        /// <summary>
        /// Handles MaxDropDownHeightChanged by re-arranging and updating the 
        /// popup arrangement.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newValue", Justification = "This makes it easy to add validation or other changes in the future.")]
        private void OnMaxDropDownHeightChanged(double newValue)
        {
            ArrangePopup();
            UpdateVisualState(true);
        }

        /// <summary>
        /// Private method that directly opens the popup, checks the expander 
        /// button, and then fires the Opened event.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OpenDropDown(bool oldValue, bool newValue)
        {
            if (DropDownPopup != null)
            {
                DropDownPopup.IsOpen = true;
            }
            if (DropDownToggleButton != null)
            {
                DropDownToggleButton.IsChecked = true;
            }

            RoutedPropertyChangedEventArgs<bool> e = new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue);
            OnDropDownOpened(e);
        }

        /// <summary>
        /// Private method that directly closes the popup, flips the Checked 
        /// value, and then fires the Closed event.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void CloseDropDown(bool oldValue, bool newValue)
        {
            if (SelectionAdapter != null)
            {
                SelectionAdapter.SelectedItem = null;
            }
            if (DropDownPopup != null)
            {
                DropDownPopup.IsOpen = false;
            }
            if (DropDownToggleButton != null)
            {
                DropDownToggleButton.IsChecked = false;
            }

            RoutedPropertyChangedEventArgs<bool> e = new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue);
            OnDropDownClosed(e);
        }

        /// <summary>
        /// The popup child has received focus.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void PopupChild_GotFocus(object sender, RoutedEventArgs e)
        {
            FocusChanged(HasFocus());
        }

        /// <summary>
        /// The popup child has lost focus.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void PopupChild_LostFocus(object sender, RoutedEventArgs e)
        {
            FocusChanged(HasFocus());
        }

        /// <summary>
        /// The popup child has had the mouse enter its bounds.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void PopupChild_MouseEnter(object sender, MouseEventArgs e)
        {
            UpdateVisualState(true);
        }

        /// <summary>
        /// The mouse has left the popup child's bounds.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void PopupChild_MouseLeave(object sender, MouseEventArgs e)
        {
            UpdateVisualState(true);
        }

        /// <summary>
        /// The size of the popup child has changed.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void PopupChild_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ArrangePopup();
        }

        /// <summary>
        /// The mouse has clicked outside of the popup.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OutsidePopup_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsDropDownOpen = false;
        }

        /// <summary>
        /// Arrange the drop down popup.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This try-catch pattern is used by other popup controls to keep the runtime up.")]
        private void ArrangePopup()
        {
        }

        #endregion

        /// <summary>
        /// Toggles the drop down visibility. If visible, the text updated 
        /// method will be called, eventually refreshing the selection adapter's
        /// view and making the appropriate visibility call on the drop down. 
        /// If no items are in the filtered view, the drop down will not be 
        /// displayed.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void ToggleDropDown(object sender, RoutedEventArgs e)
        {
            if (IsEnabled)
            {
                if (IsDropDownOpen)
                {
                    IsDropDownOpen = false;
                }
                else
                {
                    // If the drop down is current closed, the TextUpdated 
                    // method is called to re-evaluate the Text and display the 
                    // drop down only if items are available in the view.
                    TextUpdated(Text, true);
                }
            }
        }
    }
}
