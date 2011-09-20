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
        #region Keyboard

        /// <summary>
        /// Provides class handling for the KeyDown event that occurs when a 
        /// key is pressed while the control has focus.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            base.OnPreviewKeyDown(e);

            if (e.Handled || !IsEnabled)
            {
                return;
            }

            // TODO: CONSIDER: What about the Adapter interface: should it 
            // offer the ability to always handle events from the text box 
            // key down?
            if (IsDropDownOpen)
            {
                OnDropDownKeyDown(e);
            }
            else
            {
                OnTextBoxKeyDown(e);
            }

        }

        /// <summary>
        /// Occurs when the KeyDown event fires and the drop down is not open.
        /// </summary>
        /// <param name="e">The key event data.</param>
        protected void OnTextBoxKeyDown(KeyEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            else if (e.Handled)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Down:
                    if (!IsDropDownOpen)
                    {
                        ToggleDropDown(this, e);
                        e.Handled = true;
                    }
                    break;

                case Key.F4:
                    ToggleDropDown(this, e);
                    e.Handled = true;
                    break;

                case Key.Enter:
                    OnAdapterSelectionComplete(this, new RoutedEventArgs());
                    e.Handled = true;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Raises the DropDownKeyDown event when a key down event occurs 
        /// when the drop down is open.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected void OnDropDownKeyDown(KeyEventArgs e)
        {
            if (e == null || e.Handled)
            {
                return;
            }

            if (SelectionAdapter != null)
            {
                SelectionAdapter.HandleKeyDown(e);
                if (e.Handled)
                {
                    return;
                }
            }

            switch (e.Key)
            {
                case Key.Enter:
                    OnAdapterSelectionComplete(this, new RoutedEventArgs());
                    e.Handled = true;
                    break;

                case Key.Escape:
                    OnAdapterSelectionCanceled(this, new RoutedEventArgs());
                    e.Handled = true;
                    break;

                case Key.F4:
                    ToggleDropDown(this, e);
                    e.Handled = true;
                    break;

                default:
                    break;
            }
        }

        #endregion
    }
}
