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
        #region public int MinimumPrefixLength
        /// <summary>
        /// Gets or sets the minimum text length before the AutoCompleteBox can 
        /// display suggestions.
        /// </summary>
        /// <remarks>
        /// The default MinimumPrefixLength value is 1 character. Valid integers
        /// range from -1 to any reasonable maximum. -1 effectively disables the 
        /// AutoCompleteBox functionality of the control.
        /// </remarks>
        public int MinimumPrefixLength
        {
            get { return (int)GetValue(MinimumPrefixLengthProperty); }
            set { SetValue(MinimumPrefixLengthProperty, value); }
        }

        /// <summary>
        /// Identifies the MinimumPrefixLength dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumPrefixLengthProperty =
            DependencyProperty.Register(
                "MinimumPrefixLength",
                typeof(int),
                typeof(AutoCompleteTextBox),
                new PropertyMetadata(1, OnMinimumPrefixLengthPropertyChanged));

        /// <summary>
        /// MinimumPrefixLengthProperty property changed handler.
        /// </summary>
        /// <param name="d">AutoCompleteBox that changed its MinimumPrefixLength.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMinimumPrefixLengthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteTextBox source = d as AutoCompleteTextBox;
            int newValue = (int)e.NewValue;

            // If negative, coerce the value to -1
            if (newValue < 0 && newValue != -1)
            {
                source.SetValue(e.Property, -1);
            }
        }
        #endregion public int MinimumPrefixLength

        #region public int MinimumPopulateDelay
        /// <summary>
        /// Gets or sets the minimum delay required, in milliseconds, before 
        /// the AutoCompleteBox control will lookup and provide suggestions for 
        /// the current Text.
        /// </summary>
        public int MinimumPopulateDelay
        {
            get { return (int)GetValue(MinimumPopulateDelayProperty); }
            set { SetValue(MinimumPopulateDelayProperty, value); }
        }

        /// <summary>
        /// Identifies the MinimumPopulateDelay dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumPopulateDelayProperty =
            DependencyProperty.Register(
                "MinimumPopulateDelay",
                typeof(int),
                typeof(AutoCompleteTextBox),
                new PropertyMetadata(OnMinimumPopulateDelayPropertyChanged));

        // TODO: PLANNING: We should consider MinimumPopulateDelay as a TimeSpan

        /// <summary>
        /// MinimumPopulateDelayProperty property changed handler. Any current 
        /// dispatcher timer will be stopped. The timer will not be restarted 
        /// until the next TextUpdate call by the user.
        /// </summary>
        /// <param name="d">AutoCompleteTextBox that changed its 
        /// MinimumPopulateDelay.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "The exception is most likely to be called through the CLR property setter.")]
        private static void OnMinimumPopulateDelayPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteTextBox source = d as AutoCompleteTextBox;

            if (source.IgnorePropertyChange)
            {
                source.IgnorePropertyChange = false;
                return;
            }

            int newValue = (int)e.NewValue;
            if (newValue < 0)
            {
                source.IgnorePropertyChange = true;
                d.SetValue(e.Property, e.OldValue);

                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0}", newValue), "value");
            }

            // Stop any existing timer
            if (source.DelayTimer != null)
            {
                source.DelayTimer.Stop();

                if (newValue == 0)
                {
                    source.DelayTimer = null;
                }
            }

            // Create or clear a dispatcher timer instance
            if (newValue > 0 && source.DelayTimer == null)
            {
                source.DelayTimer = new DispatcherTimer();
                source.DelayTimer.Tick += source.PopulateDropDown;
            }

            // Set the new tick interval
            if (newValue > 0 && source.DelayTimer != null)
            {
                source.DelayTimer.Interval = TimeSpan.FromMilliseconds(newValue);
            }
        }
        #endregion public int MinimumPopulateDelay

        #region public bool IsTextCompletionEnabled
        /// <summary>
        /// Gets or sets a value indicating whether the first suggestion found 
        /// during a lookup will be automatically displayed in the TextBox.
        /// </summary>
        /// <remarks>
        /// Additionally, performs a lookup for the associated item value 
        /// belonging to the first suggestion.
        /// </remarks>
        public bool IsTextCompletionEnabled
        {
            get { return (bool)GetValue(IsTextCompletionEnabledProperty); }
            set { SetValue(IsTextCompletionEnabledProperty, value); }
        }

        /// <summary>
        /// Identifies the IsTextCompletionEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTextCompletionEnabledProperty =
            DependencyProperty.Register(
                "IsTextCompletionEnabled",
                typeof(bool),
                typeof(AutoCompleteTextBox),
                new PropertyMetadata(false, null));

        #endregion public bool IsTextCompletionEnabled

        #region public DataTemplate ItemTemplate
        /// <summary>
        /// Gets or sets the DataTemplate used to display each item in the 
        /// drop down.
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return GetValue(ItemTemplateProperty) as DataTemplate; }
            set { SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(
                "ItemTemplate",
                typeof(DataTemplate),
                typeof(AutoCompleteTextBox),
                new PropertyMetadata(null));

        #endregion public DataTemplate ItemTemplate

        #region public Style ItemContainerStyle
        /// <summary>
        /// Gets or sets the Style that is applied to the selection adapter.
        /// </summary>
        public Style ItemContainerStyle
        {
            get { return GetValue(ItemContainerStyleProperty) as Style; }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemContainerStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register(
                "ItemContainerStyle",
                typeof(Style),
                typeof(AutoCompleteTextBox),
                new PropertyMetadata(null, null));

        #endregion public Style ItemContainerStyle

        #region public Style TextBoxStyle
        /// <summary>
        /// Gets or sets the Style that is applied to the TextBox.
        /// </summary>
        public Style TextBoxStyle
        {
            get { return GetValue(TextBoxStyleProperty) as Style; }
            set { SetValue(TextBoxStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the TextBoxStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty TextBoxStyleProperty =
            DependencyProperty.Register(
                "TextBoxStyle",
                typeof(Style),
                typeof(AutoCompleteTextBox),
                new PropertyMetadata(null));

        #endregion public Style TextBoxStyle

        #region public double MaxDropDownHeight
        /// <summary>
        /// Gets or sets the maximum drop down height.
        /// </summary>
        public double MaxDropDownHeight
        {
            get { return (double)GetValue(MaxDropDownHeightProperty); }
            set { SetValue(MaxDropDownHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the MaxDropDownHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register(
                "MaxDropDownHeight",
                typeof(double),
                typeof(AutoCompleteTextBox),
                new FrameworkPropertyMetadata(200.0,FrameworkPropertyMetadataOptions.AffectsRender));
        //, OnMaxDropDownHeightPropertyChanged
        /// <summary>
        /// MaxDropDownHeightProperty property changed handler.
        /// </summary>
        /// <param name="d">AutoCompleteTextBox that changed its MaxDropDownHeight.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "The exception will be called through a CLR setter in most cases.")]
        private static void OnMaxDropDownHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteTextBox source = d as AutoCompleteTextBox;
            if (source.IgnorePropertyChange)
            {
                source.IgnorePropertyChange = false;
                return;
            }

            double newValue = (double)e.NewValue;

            // Revert to the old value if invalid (negative)
            if (newValue < 0)
            {
                source.IgnorePropertyChange = true;
                source.SetValue(e.Property, e.OldValue);

                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0}", e.NewValue), "value");
            }

            source.OnMaxDropDownHeightChanged(newValue);
        }
        #endregion public double MaxDropDownHeight

        #region public IEnumerable ItemsSource
        /// <summary>
        /// Gets or sets a collection that is used to generate the content of 
        /// the control.
        /// </summary>
        /// <remarks>
        /// AutoCompleteBox does not properly support the INotifyCollectionChanged 
        /// interface. Directly set ItemsSource to a new value if the source 
        /// data has changed.</remarks>
        public IEnumerable ItemsSource
        {
            get { return GetValue(ItemsSourceProperty) as IEnumerable; }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(AutoCompleteTextBox),
                new PropertyMetadata(OnItemsSourcePropertyChanged));

        /// <summary>
        /// ItemsSourceProperty property changed handler.
        /// </summary>
        /// <param name="d">AutoCompleteBox that changed its ItemsSource.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteTextBox autoComplete = d as AutoCompleteTextBox;
            autoComplete.OnItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
        }

        #endregion public IEnumerable ItemsSource

        #region public object SelectedItem
        /// <summary>
        /// Gets the selected item's value. This is a read-only property.
        /// </summary>
        /// <remarks>
        /// The IsTextCompletionEnabled property of the control impacts the 
        /// SelectedItem behavior: if the property is set to false, and the 
        /// user enters a valid items' textual representation, without 
        /// selection, the SelectedItem value will be null. The lookup between 
        /// text and items is only done when the IsTextCompletionEnabled property 
        /// is true (the default value). This is the same behavior that the 
        /// ComboBox control has in WPF.
        /// </remarks>
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty) as object; }
             set
            {
                try
                {
                    AllowWrite = true;
                    SetValue(SelectedItemProperty, value);
                }
                finally
                {
                    AllowWrite = false;
                }
            }
        }

        /// <summary>
        /// Identifies the SelectedItem dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                "SelectedItem",
                typeof(object),
                typeof(AutoCompleteTextBox),
                new PropertyMetadata(OnSelectedItemPropertyChanged));

        /// <summary>
        /// SelectedItemProperty property changed handler. Fires the 
        /// SelectionChanged event. The event data will contain any non-null
        /// removed items and non-null additions.
        /// </summary>
        /// <param name="d">AutoCompleteBox that changed its SelectedItem.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteTextBox source = d as AutoCompleteTextBox;

            if (source.IgnorePropertyChange)
            {
                source.IgnorePropertyChange = false;
                return;
            }

            // Ensure the property is only written when expected
            if (!source.AllowWrite)
            {
                // Reset the old value before it was incorrectly written
                source.IgnorePropertyChange = true;
                source.SetValue(e.Property, e.OldValue);

                throw new InvalidOperationException("");
            }

            List<object> removed = new List<object>();
            if (e.OldValue != null)
            {
                removed.Add(e.OldValue);
            }

            List<object> added = new List<object>();
            if (e.NewValue != null)
            {
                added.Add(e.NewValue);
            }

            // source.OnSelectionChanged(new SelectionChangedEventArgs(null,removed, added));
        }
        #endregion public object SelectedItem

        #region public string Text
        /// <summary>
        /// Gets or sets the contents of the TextBox.
        /// </summary>
        public string Text
        {
            get { return GetValue(TextProperty) as string; }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Identifies the Text dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(AutoCompleteTextBox),
                new PropertyMetadata(OnTextPropertyChanged));

        /// <summary>
        /// TextProperty property changed handler.
        /// </summary>
        /// <param name="d">AutoCompleteBox that changed its Text.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteTextBox source = d as AutoCompleteTextBox;
            source.TextUpdated((string)e.NewValue, false);
        }

        #endregion public string Text

        #region public string SearchText
        /// <summary>
        /// Gets the text value used to search. This is a read-only dependency 
        /// property.
        /// </summary>
        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }

            private set
            {
                try
                {
                    AllowWrite = true;
                    SetValue(SearchTextProperty, value);
                }
                finally
                {
                    AllowWrite = false;
                }
            }
        }

        /// <summary>
        /// Identifies the SearchText dependency property.
        /// </summary>
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register(
                "SearchText",
                typeof(string),
                typeof(AutoCompleteTextBox),
                new PropertyMetadata(string.Empty, OnSearchTextPropertyChanged));

        /// <summary>
        /// OnSearchTextProperty property changed handler.
        /// </summary>
        /// <param name="d">AutoCompleteBox that changed its SearchText.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSearchTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteTextBox source = d as AutoCompleteTextBox;
            if (source.IgnorePropertyChange)
            {
                source.IgnorePropertyChange = false;
                return;
            }

            // Ensure the property is only written when expected
            if (!source.AllowWrite)
            {
                // Reset the old value before it was incorrectly written
                source.IgnorePropertyChange = true;
                source.SetValue(e.Property, e.OldValue);

                throw new InvalidOperationException("");
            }
        }
        #endregion public string SearchText

        #region public AutoCompleteSearchMode SearchMode
        /// <summary>
        /// Gets or sets the built-in, predefined search mode to use for 
        /// searching the ItemsSource.
        /// </summary>
        public AutoCompleteSearchMode SearchMode
        {
            get { return (AutoCompleteSearchMode)GetValue(SearchModeProperty); }
            set { SetValue(SearchModeProperty, value); }
        }

        /// <summary>
        /// Identifies the SearchMode dependency property.
        /// </summary>
        public static readonly DependencyProperty SearchModeProperty =
            DependencyProperty.Register(
                "SearchMode",
                typeof(AutoCompleteSearchMode),
                typeof(AutoCompleteTextBox),
                new PropertyMetadata(AutoCompleteSearchMode.StartsWith, OnSearchModePropertyChanged));

        /// <summary>
        /// SearchModeProperty property changed handler.
        /// </summary>
        /// <param name="d">AutoCompleteBox that changed its SearchMode.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "The exception will be thrown when the CLR setter is used in most situations.")]
        private static void OnSearchModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteTextBox source = d as AutoCompleteTextBox;
            AutoCompleteSearchMode mode = (AutoCompleteSearchMode)e.NewValue;

            if (mode != AutoCompleteSearchMode.Contains &&
                mode != AutoCompleteSearchMode.ContainsCaseSensitive &&
                mode != AutoCompleteSearchMode.ContainsOrdinal &&
                mode != AutoCompleteSearchMode.ContainsOrdinalCaseSensitive &&
                mode != AutoCompleteSearchMode.Custom &&
                mode != AutoCompleteSearchMode.Equals &&
                mode != AutoCompleteSearchMode.EqualsCaseSensitive &&
                mode != AutoCompleteSearchMode.EqualsOrdinal &&
                mode != AutoCompleteSearchMode.EqualsOrdinalCaseSensitive &&
                mode != AutoCompleteSearchMode.None &&
                mode != AutoCompleteSearchMode.StartsWith &&
                mode != AutoCompleteSearchMode.StartsWithCaseSensitive &&
                mode != AutoCompleteSearchMode.StartsWithOrdinal &&
                mode != AutoCompleteSearchMode.StartsWithOrdinalCaseSensitive)
            {
                source.SetValue(e.Property, e.OldValue);

                throw new ArgumentException("", "value");
            }

            // Sets the filter predicate for the new value
            AutoCompleteSearchMode newValue = (AutoCompleteSearchMode)e.NewValue;
            source.TextFilter = AutoCompleteSearch.GetFilter(newValue);
        }
        #endregion public AutoCompleteSearchMode SearchMode

        #region public AutoCompleteSearchPredicate ItemFilter
        /// <summary>
        /// Gets or sets a search filter that determines whether an item object 
        /// is a valid suggestion given the search string.
        /// </summary>
        public Func<string, object, bool> ItemFilter
        {
            get { return GetValue(ItemFilterProperty) as Func<string, object, bool>; }
            set { SetValue(ItemFilterProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemFilter dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemFilterProperty =
            DependencyProperty.Register(
                "ItemFilter",
                typeof(Func<string, object, bool>),
                typeof(AutoCompleteTextBox),
                new PropertyMetadata(OnItemFilterPropertyChanged));

        /// <summary>
        /// ItemFilterProperty property changed handler.
        /// </summary>
        /// <param name="d">AutoCompleteBox that changed its ItemFilter.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnItemFilterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteTextBox source = d as AutoCompleteTextBox;
            Func<string, object, bool> value = e.NewValue as Func<string, object, bool>;

            // If null, revert to the "None" predicate
            if (value == null)
            {
                source.SearchMode = AutoCompleteSearchMode.None;
            }
            else
            {
                source.SearchMode = AutoCompleteSearchMode.Custom;
                source.TextFilter = null;
            }
        }
        #endregion public AutoCompleteSearchPredicate ItemFilter

        #region public AutoCompleteStringFilterPredicate TextFilter
        /// <summary>
        /// Gets or sets a search filter that determines whether a string is a 
        /// valid suggestion given the search text.
        /// </summary>
        public AutoCompleteSearchPredicate<string> TextFilter
        {
            get { return GetValue(TextFilterProperty) as AutoCompleteSearchPredicate<string>; }
            set { SetValue(TextFilterProperty, value); }
        }

        /// <summary>
        /// Identifies the TextFilter dependency property.
        /// </summary>
        public static readonly DependencyProperty TextFilterProperty =
            DependencyProperty.Register(
                "TextFilter",
                typeof(AutoCompleteSearchPredicate<string>),
                typeof(AutoCompleteTextBox),
                new PropertyMetadata(AutoCompleteSearch.GetFilter(AutoCompleteSearchMode.StartsWith)));
        #endregion public AutoCompleteStringFilterPredicate TextFilter

        #region public IValueConverter Converter
        /// <summary>
        /// Gets or sets the value converter used to convert item instances to 
        /// string values.
        /// </summary>
        /// <remarks>
        /// This enables high performance lookups. The conversion is from object 
        /// to type of string. 
        /// </remarks>
        public IValueConverter Converter
        {
            get { return GetValue(ConverterProperty) as IValueConverter; }
            set { SetValue(ConverterProperty, value); }
        }

        /// <summary>
        /// Identifies the Converter dependency property.
        /// </summary>
        public static readonly DependencyProperty ConverterProperty =
            DependencyProperty.Register(
                "Converter",
                typeof(IValueConverter),
                typeof(AutoCompleteTextBox),
                new PropertyMetadata(null, null));

        #endregion public IValueConverter Converter

        #region public object ConverterParameter
        /// <summary>
        /// Gets or sets the parameter used with the converter property.
        /// </summary>
        public object ConverterParameter
        {
            get { return GetValue(ConverterParameterProperty); }
            set { SetValue(ConverterParameterProperty, value); }
        }

        /// <summary>
        /// Identifies the ConverterParameter dependency property.
        /// </summary>
        public static readonly DependencyProperty ConverterParameterProperty =
            DependencyProperty.Register(
                "ConverterParameter",
                typeof(object),
                typeof(AutoCompleteTextBox),
                new PropertyMetadata(null, OnConverterParameterPropertyChanged));

        /// <summary>
        /// ConverterParameterProperty property changed handler.
        /// </summary>
        /// <param name="d">
        /// AutoCompleteBox that changed its ConverterParameter.
        /// </param>
        /// <param name="e">Event arguments.</param>
        private static void OnConverterParameterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteTextBox source = d as AutoCompleteTextBox;
            if (source.View != null && source.View.Count > 0)
            {
                source.ToggleDropDown(d, new RoutedEventArgs());
            }
        }
        #endregion public object ConverterParameter

        #region public CultureInfo ConverterCulture
        /// <summary>
        /// Gets or sets the culture used in with the converter property.
        /// </summary>
        public CultureInfo ConverterCulture
        {
            get { return GetValue(ConverterCultureProperty) as CultureInfo; }
            set { SetValue(ConverterCultureProperty, value); }
        }

        /// <summary>
        /// Identifies the ConverterCulture dependency property.
        /// </summary>
        public static readonly DependencyProperty ConverterCultureProperty =
            DependencyProperty.Register(
                "ConverterCulture",
                typeof(CultureInfo),
                typeof(AutoCompleteTextBox),
                new PropertyMetadata(CultureInfo.CurrentUICulture, OnConverterCulturePropertyChanged));

        /// <summary>
        /// ConverterCultureProperty property changed handler.
        /// </summary>
        /// <param name="d">
        /// AutoCompleteBox that changed its ConverterCulture.
        /// </param>
        /// <param name="e">Event arguments.</param>
        private static void OnConverterCulturePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteTextBox source = d as AutoCompleteTextBox;
            if (source.View != null && source.View.Count > 0)
            {
                source.ToggleDropDown(d, new RoutedEventArgs());
            }
        }
        #endregion public CultureInfo ConverterCulture

        #region Template parts and Popup elements

        /// <summary>
        /// Gets or sets the template's ToggleButton part.
        /// </summary>
        private ToggleButton DropDownToggleButton
        {
            get { return _toggleButton; }
            set
            {
                if (_toggleButton != null)
                {
                    DropDownToggleButton.Click -= ToggleDropDown;
                }

                _toggleButton = value;

                if (_toggleButton != null)
                {
                    DropDownToggleButton.Click += ToggleDropDown;
                }
            }
        }

        /// <summary>
        /// Gets or sets the drop down popup control.
        /// </summary>
        private Popup DropDownPopup { get; set; }

        /// <summary>
        /// The toggle button template part.
        /// </summary>
        private ToggleButton _toggleButton;



        /// <summary>
        /// The SelectionAdapter.
        /// </summary>
        private ISelectionAdapter _adapter;

        /// <summary>
        /// Gets or sets the adapter that represents a selection control. Now 
        /// internal or protected so that automation peers and derived classes 
        /// can access it.
        /// </summary>
        protected internal ISelectionAdapter SelectionAdapter
        {
            get { return _adapter; }
            set
            {
                if (_adapter != null)
                {
                    _adapter.SelectionChanged -= OnAdapterSelectionChanged;
                    _adapter.Commit -= OnAdapterSelectionComplete;
                    _adapter.Cancel -= OnAdapterSelectionCanceled;
                    _adapter.Cancel -= OnAdapterSelectionComplete;
                    _adapter.ItemsSource = null;
                }

                _adapter = value;

                if (_adapter != null)
                {
                    _adapter.SelectionChanged += OnAdapterSelectionChanged;
                    _adapter.Commit += OnAdapterSelectionComplete;
                    _adapter.Cancel += OnAdapterSelectionCanceled;
                    _adapter.Cancel += OnAdapterSelectionComplete;
                    _adapter.ItemsSource = View;
                }
            }
        }

        /// <summary>
        /// Gets or sets the popup child content.
        /// </summary>
        private FrameworkElement PopupChild { get; set; }

        /// <summary>
        /// Gets or sets the expansive area outside of the popup.
        /// </summary>
        private Canvas OutsidePopupCanvas { get; set; }

        /// <summary>
        /// Gets or sets the canvas for the popup child.
        /// </summary>
        private Canvas PopupChildCanvas { get; set; }

        #endregion
    }
}
