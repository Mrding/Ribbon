using Caliburn.PresentationFramework;

namespace Luna.WPF.ApplicationFramework.Behaviors
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Threading;

    using ActiproSoftware.Windows.Controls.Docking;

    using Luna.Common;
    using Luna.Core.Extensions;
    using Luna.WPF.ApplicationFramework.Extensions;

    using System.Collections.Generic;

    using Luna.WPF.ApplicationFramework.Threads;

    public static class ItemsElementBehavior
    {
        #region Fields

        public static readonly DependencyProperty BindableSelectedItemsProperty;

        public static readonly DependencyProperty DefaultSelectedIndexProperty;

        public static readonly DependencyProperty ForceSingleSelectionModeItemIndexProperty;

        public static readonly DependencyProperty HasBindableSelectedItemsProperty;

        public static readonly DependencyProperty SelectionChangedHandlerProperty;

        public static readonly DependencyProperty MultiUpdatePropertyNamesProperty;

        public static readonly DependencyProperty EnableSortingProperty;

        internal static readonly DependencyPropertyKey SortInfoPropertyKey;

        #endregion Fields

        #region Constructors

        static ItemsElementBehavior()
        {
            BindableSelectedItemsProperty = DependencyProperty.Register(
                "BindableSelectedItems", typeof(IList), typeof(System.Windows.Controls.ListBox));

            HasBindableSelectedItemsProperty = DependencyProperty.RegisterAttached(
                "HasBindableSelectedItems", typeof(bool), typeof(System.Windows.Controls.ListBox), new PropertyMetadata(false));

            SelectionChangedHandlerProperty = DependencyProperty.RegisterAttached(
                "SelectionChangedHandler", typeof(SelectionChangedHandler), typeof(System.Windows.Controls.ListBox));

            DefaultSelectedIndexProperty = DependencyProperty.RegisterAttached(
                "DefaultSelectedIndex", typeof(int), typeof(System.Windows.Controls.ListBox), new UIPropertyMetadata(0));

            ForceSingleSelectionModeItemIndexProperty = DependencyProperty.RegisterAttached(
                "ForceSingleSelectionModeItemIndex", typeof(int), typeof(System.Windows.Controls.ListBox), new UIPropertyMetadata(0));

            SortInfoPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
                "SortInfo", typeof(SortInfo), typeof(ItemsElementBehavior), new PropertyMetadata());

            MultiUpdatePropertyNamesProperty = DependencyProperty.RegisterAttached(
                "MultiUpdatePropertyNames", typeof(IEnumerable), typeof(ItemsElementBehavior), new UIPropertyMetadata(null, (obj, args) =>
                {
                    var listBox = (System.Windows.Controls.ListBox)obj;
                    if (args.NewValue != null)
                        new ListBoxMultiUpdatePropertyNamesBehavior(listBox);
                }));

            EnableSortingProperty = DependencyProperty.RegisterAttached(
                "EnableSorting", typeof(bool), typeof(ListView), new UIPropertyMetadata(false, (obj, args) =>
                {
                    if (args.NewValue != null && args.NewValue is bool && ((bool)args.NewValue) == true)
                    {
                        var listView = (ListView)obj;

                        listView.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler((sender, e) =>
                        {
                            var header = e.OriginalSource as GridViewColumnHeader;
                            if (header != null && header.Column != null && header.Column.DisplayMemberBinding != null)
                            {
                                string bindingProperty = (header.Column.DisplayMemberBinding as Binding).Path.Path;
                                ListSortDirection sortDirection = ListSortDirection.Ascending;
                                if (listView.Items.SortDescriptions.Count > 0)
                                {
                                    SortDescription sd = listView.Items.SortDescriptions[0];
                                    sortDirection = (ListSortDirection)(1 - (int)sd.Direction);
                                    listView.Items.SortDescriptions.Clear();
                                }

                                SortInfo sortInfo = listView.GetValue(SortInfoPropertyKey.DependencyProperty) as SortInfo;
                                if (sortInfo == null)
                                {
                                    sortInfo = new SortInfo() { LastSortColumn = header, CurrentAdorner = new ArrowAdorner(header, new ListSortDecorator { SortDirection = sortDirection }) };
                                    AdornerLayer.GetAdornerLayer(header).Add(sortInfo.CurrentAdorner);
                                    listView.SetValue(SortInfoPropertyKey, sortInfo);
                                }
                                else
                                {
                                    if (sortInfo.LastSortColumn != header)
                                    {
                                        AdornerLayer.GetAdornerLayer(sortInfo.LastSortColumn).Remove(sortInfo.CurrentAdorner);
                                        sortInfo.LastSortColumn = header;
                                        sortInfo.CurrentAdorner = new ArrowAdorner(header, new ListSortDecorator { SortDirection = sortDirection });
                                        AdornerLayer.GetAdornerLayer(header).Add(sortInfo.CurrentAdorner);
                                    }
                                    sortInfo.CurrentAdorner.Child.SortDirection = sortDirection;
                                }
                                try
                                {
                                    listView.Items.SortDescriptions.Add(new SortDescription(bindingProperty, sortDirection));
                                }
                                catch (InvalidOperationException)
                                {
#if DEBUG
                                    throw;
#endif
                                }
                            }
                        }));
                    }
                }));
        }

        public class ListBoxMultiUpdatePropertyNamesBehavior
        {
            private System.Windows.Controls.ListBox _listBox;
            private IList _multiUpdateProperties;
            private object _previousSelectedItem;
            private IList<INotifyPropertyChanged> _previousSubItems = new List<INotifyPropertyChanged>();

            public ListBoxMultiUpdatePropertyNamesBehavior(System.Windows.Controls.ListBox listBox)
            {
                this._listBox = listBox;


                //存储原先的SelectedItem
                _previousSelectedItem = listBox.SelectedItem;

                listBox.SelectionChanged += listBox_SelectionChanged;

                //UnLoad时候，释放存储的资源，否则内存会泄露
                UIThread.BeginInvoke(new Action(() =>
                {
                    var container = _listBox.FindAncestor<TabbedMdiContainer>();
                    if (container != null)
                        container.SelectedWindow.Unloaded += SelectedWindow_Unloaded;
                }), DispatcherPriority.Render);
            }

            void SelectedWindow_Unloaded(object sender, RoutedEventArgs e)
            {
                var window = sender as FrameworkElement;
                window.Unloaded -= SelectedWindow_Unloaded;
                _listBox.SelectionChanged -= listBox_SelectionChanged;
                //由于可能不触发PropertyChanged方法，无法移除事件监听，所以这里循环移除事件监听
                foreach (var item in _listBox.Items)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged -= Item_PropertyChanged;
                }
                foreach (var item in _previousSubItems)
                {
                    item.PropertyChanged -= SubItem_PropertyChanged;
                }
                _listBox = null;
                _multiUpdateProperties = null;
                _previousSelectedItem = null;
                _previousSubItems = null;
            }

            void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                var listBox = sender as System.Windows.Controls.ListBox;
                //listBox.IsSynchronizedWithCurrentItem = true;
                if (listBox.SelectedItems.Count == 0)
                {
                    listBox.SelectedIndex = 0;
                }
                //消除PropertyChanged
                if (_previousSelectedItem != null)
                {
                    var notify = (INotifyPropertyChanged)_previousSelectedItem;
                    notify.PropertyChanged -= Item_PropertyChanged;
                    foreach (var item in _previousSubItems)
                    {
                        item.PropertyChanged -= SubItem_PropertyChanged;
                    }
                    _previousSubItems.Clear();
                }

                if (listBox.SelectedItems.Count > 1)    //>1时，开启多选监听
                {
                    var item = (INotifyPropertyChanged)listBox.SelectedItem;
                    _previousSelectedItem = item;
                    item.PropertyChanged += Item_PropertyChanged;
                    //子属性
                    _multiUpdateProperties = _listBox.GetValue(ItemsElementBehavior.MultiUpdatePropertyNamesProperty) as IList;
                    var deepProperties = _multiUpdateProperties.Cast<string>().Where(s => s.Contains('.'));
                    if (deepProperties.Count() > 0)
                    {
                        foreach (var property in deepProperties)
                        {
                            var array = property.Split('.');
                            var index = array.Length - 1;
                            var subItem = item.GetValueByIndex(property, index);
                            if (subItem is INotifyPropertyChanged)
                            {
                                var notify = subItem as INotifyPropertyChanged;
                                notify.PropertyChanged += SubItem_PropertyChanged;
                                _previousSubItems.Add(notify);
                            }
                        }
                    }
                }
                else
                {
                    _previousSelectedItem = null;
                }
            }

            void SubItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                PropertyInfo propertyInfo = null;
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    //For vista and Win7
                    propertyInfo = sender.GetType().GetProperty(e.PropertyName);
                }
                else
                {
                    //For XP or Win2003
                    propertyInfo = sender.GetType().GetProperties().First(p => p.Name.Equals(e.PropertyName, StringComparison.Ordinal));
                }

                var newValue = propertyInfo.GetValue(sender, null);
                var property = _multiUpdateProperties.Cast<string>().FirstOrDefault(s => s.Contains("." + e.PropertyName));
                if (property.IsNullOrEmpty())
                    return;

                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    foreach (var item in _listBox.SelectedItems)
                    {
                        var source = item.GetValueByIndex(property, property.Split('.').Length - 1);
                        //除去Binding的Item，因为已经Binding，所有不需要用反射赋值
                        if (sender != source)
                        {
                            item.SetValue(property, newValue);
                        }
                    }
                });
            }

            private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                PropertyInfo propertyInfo = null;
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    //For vista and Win7
                    propertyInfo = sender.GetType().GetProperty(e.PropertyName);
                }
                else
                {
                    //For XP or Win2003
                    propertyInfo = sender.GetType().GetProperties().First(p => p.Name.Equals(e.PropertyName, StringComparison.Ordinal));
                }

                var newValue = propertyInfo.GetValue(sender, null);

                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    foreach (var item in _listBox.SelectedItems)
                    {
                        _multiUpdateProperties = _listBox.GetValue(ItemsElementBehavior.MultiUpdatePropertyNamesProperty) as IList;
                        //除去Binding的Item，因为已经Binding，所有不需要用反射赋值
                        if (item != sender)
                        {
                            if (_multiUpdateProperties.Contains(e.PropertyName))
                                propertyInfo.SetValue(item, newValue, null);
                        }
                    }
                });

            }
        }

        #endregion Constructors

        #region Methods

        public static void SetDefaultSelectedIndex(Selector source, int value)
        {
            if (source != null)
            {
                new SelectorDefaultSelectedBeavaior(source, value);
            }
        }
        internal class SelectorDefaultSelectedBeavaior
        {
            private Selector _selector;
            private int defaultValue;
            private DependencyPropertyDescriptor _itemsSourceDescriptor;
            public SelectorDefaultSelectedBeavaior(Selector selector, int value)
            {
                _selector = selector;
                defaultValue = value;
                _itemsSourceDescriptor = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, selector.GetType());
                RegisterEvent();
                _selector.SetValue(DefaultSelectedIndexProperty, value);
            }

            private void selector_Unloaded(object sender, RoutedEventArgs e)
            {
                _selector.Unloaded -= selector_Unloaded;
                _itemsSourceDescriptor.RemoveValueChanged(_selector, ItemsSourceChanged);
                _selector.Loaded += new RoutedEventHandler(selector_Loaded);
            }

            private void RegisterEvent()
            {
                _itemsSourceDescriptor.AddValueChanged(_selector, ItemsSourceChanged);
                _selector.Unloaded += new RoutedEventHandler(selector_Unloaded);
            }

            private void selector_Loaded(object sender, RoutedEventArgs e)
            {
                _selector.Loaded -= selector_Loaded;
                RegisterEvent();
            }

            private void ItemsSourceChanged(object sender, EventArgs args)
            {
                _selector.Dispatcher.BeginInvoke((Action)delegate()
                {
                    if (_selector.Items.Count > defaultValue && _selector.SelectedIndex == -1)
                        _selector.SelectedIndex = defaultValue;
                }, DispatcherPriority.Render);
            }
        }

        public static void SetForceSingleSelectionModeItemIndex(System.Windows.Controls.ListBox source, int value)
        {
            if (source != null)
            {
                source.SelectionChanged += (sender, e) =>
                {
                    if (e.AddedItems.Count == 0) return;
                    var selectedItem = e.AddedItems[0];
                    var lb = sender as System.Windows.Controls.ListBox;
                    var selectedIndex = lb.Items.IndexOf(selectedItem);

                    var itemIndexValue = (int)source.GetValue(ForceSingleSelectionModeItemIndexProperty);
                    var selectedItems = (IList)source.GetValue(BindableSelectedItemsProperty);

                    if (selectedIndex == itemIndexValue)
                    {
                        foreach (var item in lb.Items)
                        {
                            if (item != selectedItem)
                            {
                                if (selectedItems.Contains(item))
                                    selectedItems.Remove(item);
                            }
                        }
                    }
                    else
                        selectedItems.Remove(lb.Items[itemIndexValue]);

                };

                source.SetValue(ForceSingleSelectionModeItemIndexProperty, value);
            }
        }

        public static void SetHasBindableSelectedItems(System.Windows.Controls.ListBox source, bool value)
        {
            var handler = (SelectionChangedHandler)source.GetValue(SelectionChangedHandlerProperty);

            if (value && handler == null)
            {
                handler = new SelectionChangedHandler(source);
                source.SetValue(SelectionChangedHandlerProperty, handler);

            }
            else if (!value && handler != null)
                source.ClearValue(SelectionChangedHandlerProperty);
        }

        public static bool GetEnableSorting(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableSortingProperty);
        }

        public static void SetEnableSorting(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableSortingProperty, value);
        }

        public static IEnumerable GetMultiUpdatePropertyNames(DependencyObject obj)
        {
            return (IEnumerable)obj.GetValue(MultiUpdatePropertyNamesProperty);
        }

        public static void SetMultiUpdatePropertyNames(DependencyObject obj, IEnumerable value)
        {
            obj.SetValue(MultiUpdatePropertyNamesProperty, value);
        }

        internal static SortInfo GetSortInfo(DependencyObject obj)
        {
            return (SortInfo)obj.GetValue(SortInfoPropertyKey.DependencyProperty);
        }

        internal static void SetSortInfo(DependencyObject obj, SortInfo value)
        {
            obj.SetValue(SortInfoPropertyKey.DependencyProperty, value);
        }

        #endregion Methods

        #region IsScrollIntoViewWhenSelected

        public static bool GetIsScrollIntoViewWhenSelected(System.Windows.Controls.ListBox obj)
        {
            return (bool)obj.GetValue(IsScrollIntoViewWhenSelectedProperty);
        }

        public static void SetIsScrollIntoViewWhenSelected(System.Windows.Controls.ListBox obj, bool value)
        {
            obj.SetValue(IsScrollIntoViewWhenSelectedProperty, value);
        }


        public static readonly DependencyProperty IsScrollIntoViewWhenSelectedProperty =
            DependencyProperty.RegisterAttached("IsScrollIntoViewWhenSelected", typeof(bool), typeof(ItemsElementBehavior),
            new UIPropertyMetadata(false, (sender, arg) =>
                {
                    var element = sender as System.Windows.Controls.ListBox;

                    var isLoaded = false;
                    element.Loaded += (s, args) =>
                                            {
                                                if (!isLoaded)
                                                {
                                                    element.SelectionChanged += (s1, args1) =>
                                                    {
                                                        element.ScrollIntoView(element.SelectedItem);
                                                    };

                                                    element.Dispatcher.BeginInvoke(new Action(() =>
                                                    {
                                                        element.ScrollIntoView(element.SelectedItem);
                                                    }), DispatcherPriority.SystemIdle);
                                                }

                                                isLoaded = true;
                                            };

                }));

        #endregion

        #region IsBroughtIntoViewWhenSelected

        public static bool GetIsBroughtIntoViewWhenSelected(ListBoxItem item)
        {
            return (bool)item.GetValue(IsBroughtIntoViewWhenSelectedProperty);
        }

        public static void SetIsBroughtIntoViewWhenSelected(ListBoxItem item, bool value)
        {
            item.SetValue(IsBroughtIntoViewWhenSelectedProperty, value);
        }

        public static readonly DependencyProperty IsBroughtIntoViewWhenSelectedProperty =
            DependencyProperty.RegisterAttached(
                "IsBroughtIntoViewWhenSelected",
                typeof(bool),
                typeof(ItemsElementBehavior),
                new UIPropertyMetadata(false, OnIsBroughtIntoViewWhenSelectedChanged));

        static void OnIsBroughtIntoViewWhenSelectedChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            var item = depObj as ListBoxItem;
            if (item == null)
                return;
            if (PresentationFrameworkModule.IsInDesignMode) return;
            if (e.NewValue is bool == false)
                return;

            if ((bool)e.NewValue)
            {
                item.Selected += (sender, arg) =>
                {
                    if (!ReferenceEquals(sender, arg.OriginalSource))
                        return;
                    
                    item.BringIntoView();
                };
            }
        }

        #endregion // IsBroughtIntoViewWhenSelected


        #region EnableIndexing

        public static IList GetEnableIndexing(Selector selector)
        {
            return (IList)selector.GetValue(EnableIndexingProperty);
        }

        public static void SetEnableIndexing(Selector selector, IList value)
        {
            selector.SetValue(EnableIndexingProperty, value);
        }

        public static readonly DependencyProperty EnableIndexingProperty =
            DependencyProperty.RegisterAttached(
                "EnableIndexing",
                typeof(IList),
                typeof(ItemsElementBehavior),
                new UIPropertyMetadata(null, OnEnableIndexingChanged));

        private static void OnEnableIndexingChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue is INotifyCollectionChanged)
            {
                ItemsControl control = depObj as ItemsControl;
                if (control != null)
                {
                    ((INotifyCollectionChanged)e.NewValue).CollectionChanged += (sender, args) =>
                    {
                        var list = e.NewValue as IList;

                        if (args.Action == NotifyCollectionChangedAction.Move)
                        {
                            IndexManager.Move(list, args.OldStartingIndex, args.NewStartingIndex);
                        }
                        else if (args.Action == NotifyCollectionChangedAction.Add)
                        {
                            IndexManager.Add(list);
                        }
                        else if (args.Action == NotifyCollectionChangedAction.Remove)
                        {
                            IndexManager.Remove(list, args.OldStartingIndex);
                        }
                    };
                }
            }
        }

        #endregion // EnableIndexing

        public static object GetOneWayBindingSelectedValue(DependencyObject obj)
        {
            return obj.GetValue(OneWayBindingSelectedValueProperty);
        }

        public static void SetOneWayBindingSelectedValue(DependencyObject obj, object value)
        {
            obj.SetValue(OneWayBindingSelectedValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for OneWayBindingSelectedValue. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OneWayBindingSelectedValueProperty =
        DependencyProperty.RegisterAttached("OneWayBindingSelectedValue", typeof(object), typeof(ItemsElementBehavior),
        new UIPropertyMetadata(new PropertyChangedCallback((d, e) =>
                                                               {
                                                                   var itemsControl = d as System.Windows.Controls.ListBox;
                                                                   if (itemsControl == null || e.NewValue == null) return;
                                                                   itemsControl.SelectedValue = e.NewValue;
                                                                   itemsControl.ScrollIntoView(itemsControl.SelectedItem);
                                                               })));



    }


    internal class SelectionChangedHandler
    {
        #region Fields

        private readonly Binding _binding;

        #endregion Fields

        #region Constructors

        internal SelectionChangedHandler(System.Windows.Controls.ListBox owner)
        {
            _binding = new Binding("SelectedItems") { Source = owner };
            owner.SetBinding(ItemsElementBehavior.BindableSelectedItemsProperty, _binding);
            owner.SelectionChanged += Owner_SelectionChanged;
        }

        #endregion Constructors

        #region Methods

        void Owner_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var owner = (System.Windows.Controls.ListBox)sender;

            BindingOperations.ClearBinding(owner, ItemsElementBehavior.BindableSelectedItemsProperty);
            owner.SetBinding(ItemsElementBehavior.BindableSelectedItemsProperty, _binding);
        }

        #endregion Methods
    }


    public class ArrowAdorner : Adorner
    {
        private ListSortDecorator _child;

        public ArrowAdorner(UIElement element, ListSortDecorator child)
            : base(element)
        {
            _child = child;
            AddVisualChild(_child);
            AddLogicalChild(_child);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _child.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _child.Measure(constraint);
            return AdornedElement.RenderSize;
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        protected override IEnumerator LogicalChildren
        {
            get
            {
                return new UIElement[] { _child }.GetEnumerator();
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return _child;
        }

        public ListSortDecorator Child
        {
            get { return _child; }
        }
    }

    public class SortInfo
    {
        public GridViewColumnHeader LastSortColumn { get; set; }

        public ArrowAdorner CurrentAdorner { get; set; }
    }

    public class ListSortDecorator : Control
    {
        // Using a DependencyProperty as the backing store for SortDirectionProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortDirectionProperty =
            DependencyProperty.Register("SortDirection", typeof(ListSortDirection), typeof(ListSortDecorator));

        static ListSortDecorator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ListSortDecorator), new FrameworkPropertyMetadata(typeof(ListSortDecorator)));
        }

        public ListSortDirection SortDirection
        {
            get { return (ListSortDirection)GetValue(SortDirectionProperty); }
            set { SetValue(SortDirectionProperty, value); }
        }
    }
}
