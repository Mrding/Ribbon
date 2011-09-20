using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Luna.Core.Extensions;
using Luna.WPF.ApplicationFramework.Extensions;
using Luna.WPF.ApplicationFramework.Graphics;
using Luna.WPF.ApplicationFramework.Threads;
using System.Diagnostics;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public class ElementItemsControl : ElementPanel
    {
        protected int _childrenArrangeCount;
        protected int _childrenMeasureCount;

        private ScheduleGrid _scrollViewer;

        protected ScheduleGrid ScrollViewer
        {
            get
            {
                while (_scrollViewer == null)
                {
                    var scheduleGrid = this.FindAncestor<ScheduleGrid>();

                    if (scheduleGrid == null) continue;

                    _scrollViewer = scheduleGrid;
                    _scrollViewer.ScrollChanged += ViewerScrollChanged;
                }
                return _scrollViewer;
            }
        }

        protected virtual int GetScreenTopRowIndex()
        {
            if (_scrollViewer == null || ItemsSource == null) return 0;

            return _scrollViewer.DataRangeY.ViewMin;
        }

        protected virtual int GetDesiredDisplayCount()
        {
            if (_scrollViewer == null || _scrollViewer.DataRangeY.ViewMin < 0) return 0;

            if (_scrollViewer.DataRangeY.ViewMax + _scrollViewer.DataRangeY.ViewMin == 0) return 0;

            return (_scrollViewer.DataRangeY.ViewMax - _scrollViewer.DataRangeY.ViewMin) + 1;// 因为 viewMax & viewMin 是 zerobased row Index 故此需要用1校正个数
        }

        public static readonly RoutedEvent SelectionChangedEvent = Selector.SelectionChangedEvent.AddOwner(typeof(ElementItemsControl));

        public event SelectionChangedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        private void ViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (Orientation == Orientation.Horizontal && e.HorizontalChange != 0 || Orientation == Orientation.Vertical && e.VerticalChange != 0)
                Refresh(false);
        }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ElementItemsControl),
            new UIPropertyMetadata(Orientation.Vertical));

        public double RowHeight
        {
            get { return (double)GetValue(RowHeightlProperty); }
            set { SetValue(RowHeightlProperty, value); }
        }

        public static readonly DependencyProperty RowHeightlProperty =
            DependencyProperty.Register("RowHeight", typeof(double), typeof(ElementItemsControl),
            new UIPropertyMetadata(25.0));

        protected void SetItemsContent(int desiredDisplayCount, int screenTopRowIndex)
        {
            foreach (ElementListBoxItem child in Children)
            {
                if (ItemsSource.Count <= screenTopRowIndex)
                    break;
                child.Content = ItemsSource[screenTopRowIndex];
                
                screenTopRowIndex++;
            }
            _childrenMeasureCount = 0;
            _childrenArrangeCount = 0;

            //UpdateLayout();
        }

        protected bool AddVisibleChildren(int desiredDisplayCount)
        {
            var screenTopRowIndex = GetScreenTopRowIndex();

            if (desiredDisplayCount <= Children.Count) return false;

            Debug.Print("BuildVisibleChildren ({0})",desiredDisplayCount - Children.Count);

            for (var i = Children.Count; i < desiredDisplayCount; i++)
            {
                var listBoxItem = new ElementListBoxItem { Height = RowHeight, ContentTemplate = ItemTemplate };
                //x listBoxItem.MouseLeftButtonDown += ContentMouseLeftButtonDown;
                var dataRowIndex = Children.Add(listBoxItem) + screenTopRowIndex;
                listBoxItem.Content = ItemsSource[dataRowIndex];
            }
            return true;
        }

        protected void RemoveVisiableChildren(int desiredDisplayCount)
        {
            for (var i = Children.Count - 1; desiredDisplayCount <= i; i--)
            {
                if (i == -1) break;
                Children.RemoveAt(i);
            }
        }

        private int _lastTopRowIndex = -1;

        protected void Refresh(bool reloadDataContent)
        {
           
            if (ItemsSource == null || ItemsSource.Count == 0) return;

            var screenTopRowIndex = GetScreenTopRowIndex(); // 第一比在Screen上的 DataRowIndex
            var desiredDisplayCount = GetDesiredDisplayCount(); // Screen 可以顯示的筆数
        
            AddVisibleChildren(desiredDisplayCount);

            /* 以下方法保留 RemoveVisiableChildren, ArrangeOverride 已经跑过此逻辑
            if (desiredDisplayCount < Children.Count)
                RemoveVisiableChildren(desiredDisplayCount);            
            */

            if (screenTopRowIndex != _lastTopRowIndex || reloadDataContent)
            {
                SetItemsContent(desiredDisplayCount, screenTopRowIndex);
                _lastTopRowIndex = screenTopRowIndex;
            }
        }

        //protected override Geometry GetLayoutClip(Size layoutSlotSize)
        //{
        //    return new RectangleGeometry(new Rect(RenderSize));
        //}
       

        private void ContentMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ElementListBoxItem;
            this.InternalSeletedItem = item;
        }

        private void InvokeSelectionChanged(object unselectedItem, object selectedItem)
        {
            var unselectedItems = new List<object> { unselectedItem };
            var selectedItems = new List<object> { selectedItem };
            var e = new SelectionChangedEventArgs(SelectionChangedEvent, unselectedItems, selectedItems) { Source = this };
            this.OnSelectionChanged(e);
        }

        protected virtual void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(ElementItemsControl),
            new UIPropertyMetadata(null));

        public ElementListBoxItem InternalSeletedItem
        {
            get { return (ElementListBoxItem)GetValue(InternalSeletedItemProperty); }
            set { SetValue(InternalSeletedItemProperty, value); }
        }

        public static readonly DependencyProperty InternalSeletedItemProperty =
            DependencyProperty.Register("InternalSeletedItem", typeof(ElementListBoxItem), typeof(ElementItemsControl),
            new UIPropertyMetadata(null, (o, a) =>
            {
                var element = (ElementItemsControl)o;
                if (a.NewValue != null)
                {
                    element.SelectedItem = element.InternalSeletedItem.Content;
                    element.InternalSeletedItem.IsSelected = true;
                }

                a.OldValue.SaftyInvoke<ElementListBoxItem>(item => item.IsSelected = false);
                element.InvokeSelectionChanged(a.OldValue, a.NewValue);
            }));

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(ElementItemsControl),
            new UIPropertyMetadata(null));

        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IList), typeof(ElementItemsControl),
            new FrameworkPropertyMetadata(default(IList), (d, a) =>
            {
                //var element = (ElementItemsControl)d;
             var element = (ElementItemsControl)d;
                    element.Refresh(true);

            }, (d, baseValue) =>
            {
                return baseValue;
            }));

        protected override void OnDispose()
        {
            _scrollViewer.ScrollChanged -= ViewerScrollChanged;
            _scrollViewer = null;
        }

        protected override void AddToPanel(IAxisPanel axisPanel)
        {
            axisPanel.Add(this);
        }
    }


}
