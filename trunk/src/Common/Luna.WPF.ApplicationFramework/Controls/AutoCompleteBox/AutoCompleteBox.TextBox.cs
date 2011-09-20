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
        /// <summary>
        /// The TextBox template part.
        /// </summary>
        private TextBox _text;

        /// <summary>
        /// Gets or sets the Text template part.
        /// </summary>
        internal TextBox TextBox
        {
            get { return _text; }
            set
            {
                // Detach existing handlers
                if (_text != null)
                {
                    _text.SelectionChanged -= OnTextBoxSelectionChanged;
                    _text.TextChanged -= OnTextBoxTextChanged;
                }

                _text = value;

                // Attach handlers
                if (_text != null)
                {
                    _text.SelectionChanged += OnTextBoxSelectionChanged;
                    _text.TextChanged += OnTextBoxTextChanged;

                    if (Text != null)
                    {
                        UpdateTextValue(Text);
                    }
                }
            }
        }

        #region TextBoxChanged

        /// <summary>
        /// Handle the TextChanged event that is directly attached to the 
        /// TextBox part. This ensures that only user initiated actions will 
        /// result in an AutoCompleteBox suggestion and operation.
        /// </summary>
        /// <param name="sender">The source TextBox object.</param>
        /// <param name="e">The TextChanged event data.</param>
        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            // Call the central updated text method as a user-initiated action
            TextUpdated(_text.Text, true);
        }

        /// <summary>
        /// When selection changes, save the location of the selection start.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OnTextBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            // If ignoring updates. This happens after text is updated, and 
            // before the PopulateComplete method is called. Required for the 
            // IsTextCompletionEnabled feature.
            if (IgnoreTextSelectionChange)
            {
                return;
            }

            TextSelectionStart = _text.SelectionStart;
        }

        /// <summary>
        /// Updates both the text box value and underlying text dependency 
        /// property value if and when they change. Automatically fires the 
        /// text changed events when there is a change.
        /// </summary>
        /// <param name="value">The new string value.</param>
        private void UpdateTextValue(string value)
        {
            UpdateTextValue(value, null);
        }

        /// <summary>
        /// Updates both the text box value and underlying text dependency 
        /// property value if and when they change. Automatically fires the 
        /// text changed events when there is a change.
        /// </summary>
        /// <param name="value">The new string value.</param>
        /// <param name="userInitiated">A nullable bool value indicating whether
        /// the action was user initiated. In a user initiated mode, the 
        /// underlying text dependency property is updated. In a non-user 
        /// interaction, the text box value is updated. When user initiated is 
        /// null, all values are updated.</param>
        private void UpdateTextValue(string value, bool? userInitiated)
        {
            // Update the Text dependency property
            if ((userInitiated == null || userInitiated == true) && Text != value)
            {
                IgnoreTextPropertyChange++;
                Text = value;
                OnTextChanged(new RoutedEventArgs());
            }

            // Update the TextBox's Text dependency property
            if ((userInitiated == null || userInitiated == false) && TextBox != null && TextBox.Text != value)
            {
                IgnoreTextPropertyChange++;
                TextBox.Text = value ?? string.Empty;

                // Text dependency property value was set, fire event
                if (Text == value || Text == null)
                {
                    OnTextChanged(new RoutedEventArgs());
                }
            }
        }

        /// <summary>
        /// Handle the update of the text for the control from any source, 
        /// including the TextBox part and the Text dependency property.
        /// </summary>
        /// <param name="newText">The new text.</param>
        /// <param name="userInitiated">A value indicating whether the update 
        /// is a user-initiated action. This should be a True value when the 
        /// TextUpdated method is called from a TextBox event handler.</param>
        private void TextUpdated(string newText, bool userInitiated)
        {
            // Only process this event if it is coming from someone outside 
            // setting the Text dependency property directly.
            if (IgnoreTextPropertyChange > 0)
            {
                IgnoreTextPropertyChange--;
                return;
            }

            if (newText == null)
            {
                newText = string.Empty;
            }

            // The TextBox.TextChanged event was not firing immediately and 
            // was causing an immediate update, even with wrapping. If there is 
            // a selection currently, no update should happen.
            if (IsTextCompletionEnabled && TextBox != null && TextBox.SelectionLength > 0 
                && TextBox.SelectionStart != TextBox.Text.Length)
            {
                return;
            }

            // Evaluate the conditions needed for completion.
            // 1. Minimum prefix length
            // 2. If a delay timer is in use, use it
            bool populateReady = newText.Length >= MinimumPrefixLength && MinimumPrefixLength >= 0;
            UserCalledPopulate = populateReady ? userInitiated : false;

            // Update the interface and values only as necessary
            UpdateTextValue(newText, userInitiated);

            if (populateReady)
            {
                IgnoreTextSelectionChange = true;

                if (DelayTimer != null)
                {
                    DelayTimer.Start();
                }
                else
                {
                    PopulateDropDown(this, EventArgs.Empty);
                }
            }
            else
            {
                SearchText = null;
                SelectedItem = null;
                if (IsDropDownOpen)
                {
                    IsDropDownOpen = false;
                }
            }
        }

        #endregion
    }
}
