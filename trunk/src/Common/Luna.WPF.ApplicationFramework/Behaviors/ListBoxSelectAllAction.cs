namespace Luna.WPF.ApplicationFramework.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Interactivity;
    using System.Windows.Controls;
    using System.Windows;
    using System.Collections;
    using System.ComponentModel;

    using Luna.WPF.ApplicationFramework.Threads;

    public class ListBoxSelectAllAction : TargetedTriggerAction<FrameworkElement>
    {


        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool),
            typeof(ListBoxSelectAllAction), new UIPropertyMetadata());

        public ListBox TargetElement
        {
            get { return (ListBox)GetValue(TargetElementProperty); }
            set { SetValue(TargetElementProperty, value); }
        }

        public static readonly DependencyProperty TargetElementProperty =
            DependencyProperty.Register("TargetElement", typeof(ListBox), typeof(ListBoxSelectAllAction), 
            new UIPropertyMetadata(new PropertyChangedCallback((e,a)=>{
                var element = e as ListBoxSelectAllAction;
                if (a.NewValue != null) element.Register();
                (a.NewValue as ListBox).SetValue(VirtualizingStackPanel.IsVirtualizingProperty, false);
            })));

        private DependencyPropertyDescriptor dpd;

        private void Register()
        {
            if (TargetElement == null) return;
            if (EnableListenSource)
            {
                dpd = DependencyPropertyDescriptor.FromProperty(ListBox.ItemsSourceProperty, TargetElement.GetType());
                dpd.RemoveValueChanged(TargetElement, dpdValueChanged);
            }
            TargetElement.Loaded += new RoutedEventHandler(TargetElement_Loaded);
            TargetElement.Unloaded += new RoutedEventHandler(TargetElement_Unloaded);
          
        }

        void TargetElement_Loaded(object sender, RoutedEventArgs e)
        {
            //dpd.AddValueChanged(TargetElement, dpdValueChanged);
            TargetElement.SelectionChanged -= TargetElement_SelectionChanged;
            TargetElement.SelectionChanged += TargetElement_SelectionChanged;


            if (EnableSelectedCache)
            {
                TargetElement.Items.CurrentChanged -= Items_CurrentChanged;
                TargetElement.Items.CurrentChanged += Items_CurrentChanged;
                TargetElement.Items.CurrentChanging += Items_CurrentChanging;
            }
        }

        void TargetElement_Unloaded(object sender, RoutedEventArgs e)
        {
            if (EnableListenSource)
            {
                dpd.RemoveValueChanged(TargetElement, dpdValueChanged);
            }
            TargetElement.SelectionChanged -= TargetElement_SelectionChanged;
           
            if (EnableSelectedCache)
            {
                TargetElement.Items.CurrentChanged -= Items_CurrentChanged;
              
                TargetElement.Items.CurrentChanging -=Items_CurrentChanging;
            }
        }



        public bool EnableListenSource
        {
            get { return (bool)GetValue(EnableListenSourceProperty); }
            set { SetValue(EnableListenSourceProperty, value); }
        }

        public static readonly DependencyProperty EnableListenSourceProperty =
            DependencyProperty.Register("EnableListenSource", typeof(bool), typeof(ListBoxSelectAllAction), 
            new UIPropertyMetadata(true));

        

        private bool stopListAction = false;

        void Items_CurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            stopListAction = true;
        }

        public static IList GetCacheSelectedItems(DependencyObject obj)
        {
            return (IList)obj.GetValue(CacheSelectedItemsProperty);
        }

        public static void SetCacheSelectedItems(DependencyObject obj, IList value)
        {
            obj.SetValue(CacheSelectedItemsProperty, value);
        }

        public static readonly DependencyProperty CacheSelectedItemsProperty =
            DependencyProperty.RegisterAttached("CacheSelectedItems", typeof(IList), typeof(ListBoxSelectAllAction),
            new UIPropertyMetadata(null));

        void Items_CurrentChanged(object sender, EventArgs e)
        {
            MarkCheckBox();
            if (EnableSelectedCache)
            {
                UIThread.BeginInvoke(() =>
                {
                    ItemsSelected(_cache, true);
                });
                stopListAction = false;
            }
        }

        private void ItemsSelected(IEnumerable items,bool selected)
        {
            foreach (var item in items)
            {
                var entity = item as Luna.Common.ISelectable;
                if (entity != null)
                    entity.IsSelected = true;
                else
                {
                    var element = TargetElement.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                    if (element != null)
                        element.IsSelected = selected;
                }
            }
        }

  

        private void TargetElement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (!stopListAction && EnableSelectedCache)
            {
                foreach (var item in e.RemovedItems)
                {
                    _cache.Remove(item);
                }

                foreach (var item in e.AddedItems)
                {
                    _cache.Add(item);
                }
                TargetElement.SetValue(CacheSelectedItemsProperty, _cache.ToList());
                AssociatedObject.SetValue(SelectedItemsCountProperty, _cache.Count);
                //Console.WriteLine(_cache.Count);
            }
            MarkCheckBox();
        }

        public bool EnableSelectedCache
        {
            get { return (bool)GetValue(EnableSelectedCacheProperty); }
            set { SetValue(EnableSelectedCacheProperty, value); }
        }

        public static readonly DependencyProperty EnableSelectedCacheProperty =
            DependencyProperty.Register("EnableSelectedCache", typeof(bool), typeof(ListBoxSelectAllAction), 
            new UIPropertyMetadata(false));

        private void dpdValueChanged(object sender, EventArgs arg)
        {
            MarkCheckBox();
        }

        private HashSet<object> _oldcache = new HashSet<object>();
        private HashSet<object> _cache = new HashSet<object>();

        private void MarkCheckBox()
        {
            var box = AssociatedObject as CheckBox;
            if (!box.IsThreeState)
                box.IsChecked = (TargetElement.Items.Count == TargetElement.SelectedItems.Count) && TargetElement.SelectedItems.Count > 0;
            else
            {
                if (TargetElement.SelectedItems.Count == 0)
                {
                    box.IsChecked = false;
                }
                if (TargetElement.SelectedItems.Count > 0 && TargetElement.SelectedItems.Count < TargetElement.Items.Count)
                {
                    box.IsChecked = null;
                }
                else if (TargetElement.Items.Count == TargetElement.SelectedItems.Count && TargetElement.SelectedItems.Count > 0)
                {
                    box.IsChecked = true;
                }
            }

            
        }

        protected override void Invoke(object parameter)
        {
            if (IsChecked)
            {
                if (EnableSelectedCache)
                {
                    ItemsSelected(TargetElement.Items, true);
                    foreach (var item in TargetElement.SelectedItems)
                    {
                        _cache.Add(item);
                    }
                    TargetElement.SetValue(CacheSelectedItemsProperty, _cache.ToList());
                }
                else
                    TargetElement.SelectAll();
            }
            else
            {
                TargetElement.UnselectAll();
                if (EnableSelectedCache)
                {
                    foreach (var item in TargetElement.Items)
                    {
                        _cache.Remove(item);
                    }
                    ItemsSelected(TargetElement.Items, false);
                    TargetElement.SetValue(CacheSelectedItemsProperty, _cache.ToList());
                }
                else
                    TargetElement.UnselectAll();
            }
            AssociatedObject.SetValue(SelectedItemsCountProperty,_cache.Count);
        }

        public static int GetSelectedItemsCount(DependencyObject obj)
        {
            return (int)obj.GetValue(SelectedItemsCountProperty);
        }

        public static void SetSelectedItemsCount(DependencyObject obj, int value)
        {
            obj.SetValue(SelectedItemsCountProperty, value);
        }

        
        public static readonly DependencyProperty SelectedItemsCountProperty =
            DependencyProperty.RegisterAttached("SelectedItemsCount", typeof(int), typeof(ListBoxSelectAllAction), 
            new UIPropertyMetadata(0));

    }
}
