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
        /// Handle any change to the ItemsSource dependency property, update 
        /// the underlying ObservableCollection view, and set the selection 
        /// adapter's ItemsSource to the view if appropriate.
        /// </summary>
        /// <param name="oldValue">The old enumerable reference.</param>
        /// <param name="newValue">The new enumerable reference.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "oldValue", Justification = "This makes it easy to add validation or other changes in the future.")]
        private void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            // Remove handler for oldValue.CollectionChanged (if present)
            INotifyCollectionChanged oldValueINotifyCollectionChanged = oldValue as INotifyCollectionChanged;
            if (null != oldValueINotifyCollectionChanged)
            {
                oldValueINotifyCollectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(ItemsSourceCollectionChanged);
            }

            // Add handler for newValue.CollectionChanged (if possible)
            INotifyCollectionChanged newValueINotifyCollectionChanged = newValue as INotifyCollectionChanged;
            if (null != newValueINotifyCollectionChanged)
            {
                newValueINotifyCollectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(ItemsSourceCollectionChanged);
            }

            // Store a local cached copy of the data
            Items = newValue == null ? null : new List<object>(newValue.Cast<object>().ToList());

            // Clear and set the view on the selection adapter
            ClearView();
            if (SelectionAdapter != null && SelectionAdapter.ItemsSource != View)
            {
                SelectionAdapter.ItemsSource = View;
            }
        }

        /// <summary>
        /// Method that handles the ObservableCollection.CollectionChanged event for the ItemsSource property.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Update the cache
            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                for (int index = 0; index < e.OldItems.Count; index++)
                {
                    Items.RemoveAt(e.OldStartingIndex);
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null && Items.Count >= e.NewStartingIndex)
            {
                for (int index = 0; index < e.NewItems.Count; index++)
                {
                    Items.Insert(e.NewStartingIndex + index, e.NewItems[index]);
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Replace && e.NewItems != null && e.OldItems != null)
            {
                for (int index = 0; index < e.NewItems.Count; index++)
                {
                    Items[e.NewStartingIndex] = e.NewItems[index];
                }
            }

            // Update the view
            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
            {
                for (int index = 0; index < e.OldItems.Count; index++)
                {
                    View.Remove(e.OldItems[index]);
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                ClearView();
            }
            else
            {
                RefreshView();
            }
        }

        #region Selection Adapter

        /// <summary>
        /// Handles the SelectionChanged event of the selection adapter.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The selection changed event data.</param>
        private void OnAdapterSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object o = _adapter.SelectedItem;
            string text;

            if (o == null)
            {
                text = SearchText;
            }
            else
            {
                text = FormatValue(o);
            }

            // Update the Text property and the TextBox values
            UpdateTextValue(text);

            // Set the selected value to the item
            SelectedItem = o;

            // Move the caret to the end of the text box
            if (TextBox != null && Text != null)
            {
                TextBox.SelectionStart = Text.Length;
            }
        }

        /// <summary>
        /// Handles the Commit event on the selection adapter.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OnAdapterSelectionComplete(object sender, RoutedEventArgs e)
        {
            if (IsDropDownOpen)
            {
                IsDropDownOpen = false;
            }

            // Completion will update the selected value
            UpdateTextCompletion(false);

            // Text should not be selected
            if (TextBox != null)
            {
                TextBox.Select(TextBox.Text.Length, 0);
            }
        }

        /// <summary>
        /// Handles the Cancel event on the selection adapter.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OnAdapterSelectionCanceled(object sender, RoutedEventArgs e)
        {
            UpdateTextValue(SearchText);

            // Completion will update the selected value
            UpdateTextCompletion(false);
        }

        #endregion
    }
}
