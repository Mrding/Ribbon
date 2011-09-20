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
    /// <summary>
    /// Represents a control that combines a text box and a drop down popup 
    /// containing a selection control. AutoCompleteBox allows users to filter 
    /// an items list.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public partial class AutoCompleteTextBox : Control, IUpdateVisualState
    {
        #region Field

        /// <summary>
        /// Gets or sets a local cached copy of the items data.
        /// </summary>
        private List<object> Items { get; set; }

        /// <summary>
        /// Gets or sets the observable collection that contains references to 
        /// all of the items in the generated view of data that is provided to 
        /// the selection-style control adapter.
        /// </summary>
        private ObservableCollection<object> View { get; set; }

        /// <summary>
        /// Gets or sets a value to ignore a number of pending change handlers. 
        /// The value is decremented after each use. This is used to reset the 
        /// value of properties without performing any of the actions in their 
        /// change handlers.
        /// </summary>
        /// <remarks>The int is important as a value because the TextBox 
        /// TextChanged event does not immediately fire, and this will allow for
        /// nested property changes to be ignored.</remarks>
        private int IgnoreTextPropertyChange { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore calling a pending 
        /// change handlers. 
        /// </summary>
        private bool IgnorePropertyChange { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore the selection 
        /// changed event.
        /// </summary>
        private bool IgnoreTextSelectionChange { get; set; }

        /// <summary>
        /// Gets or sets the last observed text box selection start location.
        /// </summary>
        private int TextSelectionStart { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user initiated the 
        /// current populate call.
        /// </summary>
        private bool UserCalledPopulate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a visual popup state is 
        /// being used in the current template for the Closed state. Setting 
        /// this value to true will delay the actual setting of Popup.IsOpen 
        /// to false until after the visual state's transition for Closed is 
        /// complete.
        /// </summary>
        private bool PopupClosedVisualState { get; set; }

        /// <summary>
        /// Gets or sets the DispatcherTimer used for the MinimumPopulateDelay 
        /// condition for auto completion.
        /// </summary>
        private DispatcherTimer DelayTimer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a read-only dependency 
        /// property change handler should allow the value to be set.  This is 
        /// used to ensure that read-only properties cannot be changed via 
        /// SetValue, etc.
        /// </summary>
        private bool AllowWrite { get; set; }

        /// <summary>
        /// Gets or sets the helper that provides all of the standard
        /// interaction functionality. Making it internal for subclass access.
        /// </summary>
        internal InteractionHelper Interaction { get; set; }

        /// <summary>
        /// Gets a value indicating whether the content is editable. The 
        /// default Silverlight Toolkit AutoCompleteBox is always editable.
        /// </summary>
        public virtual bool IsEditable { get { return true; } }

        #endregion

        /// <summary>
        /// Initializes a new instance of the AutoCompleteBox control class.
        /// </summary>
        public AutoCompleteTextBox()
        {
            DefaultStyleKey = typeof(AutoCompleteTextBox);

            Loaded += (sender, e) => ApplyTemplate();
            IsEnabledChanged += ControlIsEnabledChanged;

            Interaction = new InteractionHelper(this);
            
            //// Creating the view here ensures that View is always != null
            ClearView();
        }

        /// <summary>
        /// Arranges and sizes the auto complete control and its contents.
        /// </summary>
        /// <param name="finalSize">The provided arrangement bounds object.</param>
        /// <returns>Returns the arrangement bounds, unchanged.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Size r = base.ArrangeOverride(finalSize);
            ArrangePopup();
            return r;
        }

        #region GroupPopupVisualStates

        private void RegisterGroupPopupClosedStateChangedEvent()
        {
            // Unhook the event handler for the popup closed visual state group.
            // This code is used to enable visual state transitions before 
            // actually setting the underlying Popup.IsOpen property to false.
            VisualStateGroup groupPopupClosed = VisualStates.TryGetVisualStateGroup(this, VisualStates.GroupPopup);
            if (null != groupPopupClosed)
            {
                groupPopupClosed.CurrentStateChanged -= OnPopupClosedStateChanged;
                PopupClosedVisualState = false;
            }

            groupPopupClosed = VisualStates.TryGetVisualStateGroup(this, VisualStates.GroupPopup);
            if (null != groupPopupClosed)
            {
                groupPopupClosed.CurrentStateChanged += OnPopupClosedStateChanged;
                PopupClosedVisualState = true;
            }
        }

        /// <summary>
        /// Actually closes the popup after the VSM state animation completes.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void OnPopupClosedStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            switch (e.NewState.Name)
            {
                // Delayed closing of the popup until now
                case VisualStates.StatePopupClosed:
                    CloseDropDown(true, false);
                    break;

                default:
                    break;
            }
        }

        #endregion

        /// <summary>
        /// Builds the visual tree for the AutoCompleteBox control when a 
        /// new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            RegisterGroupPopupClosedStateChangedEvent();
            
            // Explicit set of the popup to closed. This is in line with other 
            // drop down controls, including the Silverlight ComboBox's 
            // implementation.
            IsDropDownOpen = false;
            // Set the template parts. Individual part setters remove and add 
            // any event handlers.
            DropDownToggleButton = GetTemplateChild(ElementDropDownToggle) as ToggleButton;
            DropDownPopup = GetTemplateChild(ElementPopup) as Popup;
            SelectionAdapter = TryGetSelectionAdapter(GetTemplateChild(ElementSelectionAdapter));
            TextBox = GetTemplateChild(AutoCompleteTextBox.ElementTextBox) as TextBox;
            Interaction.OnApplyTemplateBase();
        }

        /// <summary>
        /// Creates the AutomationPeer for the AutoCompleteBox.
        /// </summary>
        /// <returns>Returns a new AutoCompleteBoxAutomationPeer.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new AutoCompleteBoxAutomationPeer(this);
        }

        #region Focus

        /// <summary>
        /// Handles the FocusChanged event.
        /// </summary>
        /// <param name="hasFocus">A value indicating whether the control 
        /// currently has the focus.</param>
        private void FocusChanged(bool hasFocus)
        {
            // The OnGotFocus & OnLostFocus are asynchronously and cannot 
            // reliably tell you that have the focus.  All they do is let you 
            // know that the focus changed sometime in the past.  To determine 
            // if you currently have the focus you need to do consult the 
            // FocusManager (see HasFocus()).

            if (hasFocus)
            {
                if (TextBox != null && TextBox.SelectionLength == 0)
                {
                    TextBox.SelectAll();
                }
            }
            else
            {
                IsDropDownOpen = false;
                UserCalledPopulate = false;
                if (TextBox != null)
                {
                    TextBox.Select(TextBox.Text.Length, 0);
                }
            }
        }

        /// <summary>
        /// Checks to see if the control has focus currently.
        /// </summary>
        /// <returns>Returns a value indicating whether the control or its popup
        /// have focus.</returns>
        private bool HasFocus()
        {
            DependencyObject focused = FocusManager.GetFocusedElement(this) as DependencyObject;
            while (focused != null)
            {
                if (object.ReferenceEquals(focused, this))
                {
                    return true;
                }

                // This helps deal with popups that may not be in the same 
                // visual tree
                DependencyObject parent = VisualTreeHelper.GetParent(focused);
                if (parent == null)
                {
                    // Try the logical parent.
                    FrameworkElement element = focused as FrameworkElement;
                    if (element != null)
                    {
                        parent = element.Parent;
                    }
                }
                focused = parent;
            }
            return false;
        }

        /// <summary>
        /// Provides class handling for the GotFocus event.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            FocusChanged(HasFocus());
        }

        /// <summary>
        /// Provides handling for the LostFocus event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            FocusChanged(HasFocus());
        }

        #endregion

        /// <summary>
        /// Handle the change of the IsEnabled property.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void ControlIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isEnabled = (bool)e.NewValue;
            if (!isEnabled)
            {
                IsDropDownOpen = false;
            }
        }

        /// <summary>
        /// Attempts to return an ISelectionAdapter wrapper for a specified object.
        /// </summary>
        /// <param name="value">The object value.</param>
        /// <returns>Returns an IItemsSelector wrapping the value.</returns>
        /// <remarks>
        /// The specified object will be returned if it implements 
        /// ISelectionAdapter. If the specified object can be placed in a known 
        /// implementation of ISelectionAdapter, one containing the specified 
        /// object will be returned. Otherwise null will be returned.
        /// 
        /// Custom adapters can be added by deriving a new control from 
        /// AutoCompleteBox and overriding the TryGetSelectionAdapter method.</remarks>
        protected virtual ISelectionAdapter TryGetSelectionAdapter(object value)
        {
            // Check if it is already an IItemsSelector
            ISelectionAdapter asAdapter = value as ISelectionAdapter;
            if (asAdapter != null)
            {
                return asAdapter;
            }

            // Built in support for wrapping a Selector control
            Selector asSelector = value as Selector;
            if (asSelector != null)
            {
                return new SelectorSelectionAdapter(asSelector);
            }

            // TODO: PLANNING: Consider an Attribute to expose extensibility

            return null;
        }

        /// <summary>
        /// Formats an Item for text comparisons based on Converter 
        /// and ConverterCulture properties.
        /// </summary>
        /// <param name="value">The object to format.</param>
        /// <returns>Formatted Value.</returns>
        protected virtual string FormatValue(object value)
        {
            // TODO: PLANNING: In the future, check for and use TypeConverters

            if (Converter != null)
            {
                return Converter.Convert(value, typeof(string), ConverterParameter, ConverterCulture) as string ?? string.Empty;
            }
            else
            {
                return value == null ? string.Empty : value.ToString();
            }
        }

        /// <summary>
        /// Handles the timer tick when using a populate delay.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event arguments.</param>
        private void PopulateDropDown(object sender, EventArgs e)
        {
            if (DelayTimer != null)
            {
                DelayTimer.Stop();
            }

            // Update the prefix/search text.
            SearchText = Text;

            // The Populated event enables advanced, custom filtering. The 
            // client needs to directly update the ItemsSource collection or
            // call the Populate method on the control to continue the 
            // display process if Cancel is set to true.
            PopulatingEventArgs populating = new PopulatingEventArgs(SearchText);
            OnPopulating(populating);
            if (!populating.Cancel)
            {
                PopulateComplete();
            }
        }

        /// <summary>
        /// Notifies AutoCompleteBox that ItemsSource has been populated and 
        /// suggestions can now be computed using that data.
        /// </summary>
        /// <remarks>
        /// Allows a developer to continue the population event after setting 
        /// the Cancel property to True. This allows for custom, 
        /// developer-driven AutoCompleteBox scenarios.
        /// </remarks>
        public void PopulateComplete()
        {
            // Apply the search filter
            RefreshView();

            // Fire the Populated event containing the read-only view data.
            PopulatedEventArgs populated = new PopulatedEventArgs(new ReadOnlyCollection<object>(View));
            OnPopulated(populated);

            if (SelectionAdapter != null && SelectionAdapter.ItemsSource != View)
            {
                SelectionAdapter.ItemsSource = View;
            }

            IsDropDownOpen = UserCalledPopulate && (View.Count > 0);
            if (IsDropDownOpen)
            {
                ArrangePopup();
            }

            UpdateTextCompletion(UserCalledPopulate);
        }

        /// <summary>
        /// Performs text completion, if enabled, and a lookup on the underlying
        /// item values for an exact match. Will update the SelectedItem value.
        /// </summary>
        /// <param name="userInitiated">A value indicating whether the operation
        /// was user initiated. Text completion will not be performed when not 
        /// directly initiated by the user.</param>
        private void UpdateTextCompletion(bool userInitiated)
        {
            // By default this method will clear the selected value
            object newSelectedItem = null;
            string text = Text;

            // Text search is StartsWith explicit and only when enabled, in 
            // line with WPF's ComboBox lookup. When in use it will associate 
            // a Value with the Text if it is found in ItemsSource. This is 
            // only valid when there is data and the user initiated the action.
            if (View.Count > 0)
            {
                if (IsTextCompletionEnabled && TextBox != null && userInitiated)
                {
                    int currentLength = TextBox.Text.Length;
                    int selectionStart = TextBox.SelectionStart;
                    if (selectionStart == text.Length && selectionStart > TextSelectionStart)
                    {
                        // When the SearchMode dependency property is set to 
                        // either StartsWith or StartsWithCaseSensitive, the 
                        // first item in the view is used. This will improve 
                        // performance on the lookup. It assumes that the 
                        // SearchMode the user has selected is an acceptable 
                        // case sensitive matching function for their scenario.
                        object top = SearchMode == AutoCompleteSearchMode.StartsWith || SearchMode == AutoCompleteSearchMode.StartsWithCaseSensitive
                            ? View[0]
                            : TryGetMatch(text, View, AutoCompleteSearch.GetFilter(AutoCompleteSearchMode.StartsWith));

                        // If the search was successful, update SelectedItem
                        if (top != null)
                        {
                            newSelectedItem = top;
                            string topString = FormatValue(top);

                            // Only replace partially when the two words being the same
                            int minLength = Math.Min(topString.Length, Text.Length);
                            if (AutoCompleteSearch.Equals(Text.Substring(0, minLength), topString.Substring(0, minLength)))
                            {
                                // Update the text
                                UpdateTextValue(topString);

                                // Select the text past the user's caret
                                TextBox.SelectionStart = currentLength;
                                TextBox.SelectionLength = topString.Length - currentLength;
                            }
                        }
                    }
                }
                else
                {
                    // Perform an exact string lookup for the text. This is a 
                    // design change from the original Toolkit release when the 
                    // IsTextCompletionEnabled property behaved just like the 
                    // WPF ComboBox's IsTextSearchEnabled property.
                    //
                    // This change provides the behavior that most people expect
                    // to find: a lookup for the value is always performed.
                    newSelectedItem = TryGetMatch(text, View, AutoCompleteSearch.GetFilter(AutoCompleteSearchMode.EqualsCaseSensitive));
                }
            }

            // Update the selected item property
            SelectedItem = newSelectedItem;

            // Restore updates for TextSelection
            if (IgnoreTextSelectionChange)
            {
                IgnoreTextSelectionChange = false;
                if (TextBox != null)
                {
                    TextSelectionStart = TextBox.SelectionStart;
                }
            }
        }

        /// <summary>
        /// Attempts to look through the view and locate the specific exact 
        /// text match.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="view">The view reference.</param>
        /// <param name="predicate">The predicate to use for the partial or 
        /// exact match.</param>
        /// <returns>Returns the object or null.</returns>
        private object TryGetMatch(string searchText, ObservableCollection<object> view, AutoCompleteSearchPredicate<string> predicate)
        {
            if (view != null && view.Count > 0)
            {
                foreach (object o in view)
                {
                    if (predicate(searchText, FormatValue(o)))
                    {
                        return o;
                    }
                }
            }

            return null;
        }

        #region View Operation

        /// <summary>
        /// A simple helper method to clear the view and ensure that a view 
        /// object is always present and not null.
        /// </summary>
        private void ClearView()
        {
            if (View == null)
            {
                View = new ObservableCollection<object>();
            }
            else
            {
                View.Clear();
            }
        }

        /// <summary>
        /// Walks through the items enumeration. Performance is not going to be 
        /// perfect with the current implementation.
        /// </summary>
        private void RefreshView()
        {
            if (Items == null)
            {
                ClearView();
                return;
            }

            // Cache the current text value
            string text = Text ?? string.Empty;

            // Determine if any filtering mode is on
            bool stringFiltering = TextFilter != null;
            bool objectFiltering = SearchMode == AutoCompleteSearchMode.Custom && TextFilter == null;

            int view_index = 0;
            int view_count = View.Count;
            List<object> items = Items;
            foreach (object item in items)
            {
                bool inResults = !(stringFiltering || objectFiltering);
                if (!inResults)
                {
                    inResults = stringFiltering ? TextFilter(text, FormatValue(item)) : ItemFilter(text, item);
                }

                if (view_count > view_index && inResults && View[view_index] == item)
                {
                    // Item is still in the view
                    view_index++;
                }
                else if (inResults)
                {
                    // Insert the item
                    if (view_count > view_index && View[view_index] != item)
                    {
                        // Replace item
                        // Unfortunately replacing via index throws a fatal 
                        // exception: View[view_index] = item;
                        // Cost: O(n) vs O(1)
                        View.RemoveAt(view_index);
                        View.Insert(view_index, item);
                        view_index++;
                    }
                    else
                    {
                        // Add the item
                        if (view_index == view_count)
                        {
                            // Constant time is preferred (Add).
                            View.Add(item);
                        }
                        else
                        {
                            View.Insert(view_index, item);
                        }
                        view_index++;
                        view_count++;
                    }
                }
                else if (view_count > view_index && View[view_index] == item)
                {
                    // Remove the item
                    View.RemoveAt(view_index);
                    view_count--;
                }
            }
        }

        #endregion

        #region IUpdateVisualState

        /// <summary>
        /// Update the visual state of the control.
        /// </summary>
        /// <param name="useTransitions">
        /// A value indicating whether to automatically generate transitions to
        /// the new state, or instantly transition to the new state.
        /// </param>
        void IUpdateVisualState.UpdateVisualState(bool useTransitions)
        {
            UpdateVisualState(useTransitions);
        }

        /// <summary>
        /// Update the current visual state of the button.
        /// </summary>
        /// <param name="useTransitions">
        /// True to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param>
        internal virtual void UpdateVisualState(bool useTransitions)
        {

            // Popup
            VisualStateManager.GoToState(this, IsDropDownOpen ? VisualStates.StatePopupOpened : VisualStates.StatePopupClosed, useTransitions);

            // Handle the Common and Focused states
            Interaction.UpdateVisualStateBase(useTransitions);
        }

        #endregion
    }
}
