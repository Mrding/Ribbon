using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Luna.Common;
using Luna.Common.Domain;
using Luna.Core.Extensions;
using Luna.WPF.ApplicationFramework.Extensions;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public abstract class BlockGridLayerBase : AxisControl, IBlockGridLayer
    {
        static BlockGridLayerBase()
        {
            IsHitTestVisibleProperty.OverrideMetadata(typeof(BlockGridLayerBase), new FrameworkPropertyMetadata(false));
        }

        public override void Initialize()
        {
            LayerContainer.AddAsChild(this);
            AddToPanel(AxisPanel);
            InvalidateVisual();
            OnLoaded();
            
        }

     


        private BlockGridLayerContainer _layerContainer;
        protected BlockGridLayerContainer LayerContainer
        {
            get
            {
                if (_layerContainer == null)
                    _layerContainer = this.FindAncestorByLogicalTree<BlockGridLayerContainer>();

                if (_layerContainer == null)
                    _layerContainer = this.FindAncestor<BlockGridLayerContainer>();

                return _layerContainer;
            }
        }

        //protected new ScheduleGrid AxisPanel { get { return base.AxisPanel as ScheduleGrid; } }

        protected override void OnDispose()
        {
            //if (SelectedItemsAndIndex != null)
            //    SelectedItemsAndIndex.CollectionChanged -= SelectedItemsAndIndexChanged;

            //x_layerContainer.Children.Remove(this);
            _layerContainer = null;
            _mouseTrackingTimer = null;
        }

        //protected override Size MeasureOverride(Size availableSize)
        //{
        //    if (double.IsInfinity(availableSize.Height) || double.IsInfinity(availableSize.Width))
        //    {
        //        return new Size(200, Interval);
        //    }
        //    return availableSize;
        //}

        #region IBlockGridLayer

        protected virtual bool ParentMouseMoveMustbeDone
        {
            get { return false; }
        }

        protected virtual bool ParentMouseDownMustbeDone
        {
            get { return false; }
        }

        protected virtual bool ParentMouseUpMustbeDone
        {
            get { return false; }
        }

        void IBlockGridLayer.OnParentHitOutOfRangePrivate()
        {
            OnParentHitOutOfRange();
        }

        void IBlockGridLayer.OnParentMouseMovePrivate(object sender, MouseEventArgs e)
        {
            if (!e.Handled || ParentMouseMoveMustbeDone)
                OnParentMouseMove(sender, e);
        }

        void IBlockGridLayer.OnParentKeyDownPrivate(object sender, KeyEventArgs e)
        {
            if (!e.Handled)
                OnParentKeyDown(sender, e);
        }

        void IBlockGridLayer.OnParentKeyEscPressPrivate(object sender, KeyEventArgs e)
        {
            if (!e.Handled)
                OnParentKeyEscPress(sender, e);
        }

        void IBlockGridLayer.OnParentKeyUpPrivate(object sender, KeyEventArgs e)
        {
            if (!e.Handled)
                OnParentKeyUp(sender, e);
        }

        void IBlockGridLayer.OnParentMouseDownPrivate(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled || ParentMouseDownMustbeDone)
                OnParentMouseDown(sender, e);
        }

        void IBlockGridLayer.OnParentMouseUpPrivate(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled || ParentMouseUpMustbeDone)
                OnParentMouseUp(sender, e);
        }

        void IBlockGridLayer.OnParentMouseEnterPrivate(object sender, MouseEventArgs e)
        {
            if (!e.Handled)
                OnParentMouseEnter(sender, e);
        }

        void IBlockGridLayer.OnParentMouseLeavePrivate(object sender, MouseEventArgs e)
        {
            if (!e.Handled)
                OnParentMouseLeave(sender, e);
        }

        void IBlockGridLayer.OnParentDragOverPrivate(DragEventArgs e)
        {
            if (!e.Handled)
                OnParentDragOver(e);
        }

        void IBlockGridLayer.OnParentDropPrivate(DragEventArgs e)
        {
            if (!e.Handled)
                OnParentDrop(e);
        }

        void IBlockGridLayer.OnParentDragLeavePrivate(DragEventArgs e)
        {
            if (!e.Handled)
                OnParentDragLeave(e);
        }

        void IBlockGridLayer.OnParentTextInputPrivate(TextCompositionEventArgs e)
        {
            if (!e.Handled)
                OnParentTextInput(e);
        }

        void IBlockGridLayer.OnParentMouseDoubleClickPrivate(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled)
                OnParentMouseDoubleClick(e);
        }

        protected virtual void OnParentHitOutOfRange() { }

        protected virtual void OnParentDrop(DragEventArgs e) { }

        protected virtual void OnParentDragOver(DragEventArgs e) { }

        protected virtual void OnParentDragEnter(DragEventArgs e) { }

        protected virtual void OnParentDragLeave(DragEventArgs e) { }

        protected virtual void OnParentMouseMove(object sender, MouseEventArgs e) { }

        protected virtual void OnParentMouseDown(object sender, MouseButtonEventArgs e) { }

        protected virtual void OnParentMouseUp(object sender, MouseButtonEventArgs e) { }

        protected virtual void OnParentMouseEnter(object sender, MouseEventArgs e) { }

        protected virtual void OnParentMouseLeave(object sender, MouseEventArgs e) { }

        protected virtual void OnParentKeyDown(object sender, KeyEventArgs e) { }

        protected virtual void OnParentKeyEscPress(object sender, KeyEventArgs e) { }

        protected virtual void OnParentKeyUp(object sender, KeyEventArgs e) { }

        protected virtual void OnParentTextInput(TextCompositionEventArgs e) { }

        protected virtual void OnParentMouseDoubleClick(MouseButtonEventArgs e) { }

        #endregion

        public ITerm PointOutBlock
        {
            get { return (ITerm)GetValue(PointOutBlockProperty); }
            set { SetValue(PointOutBlockProperty, value); }
        }

        public static readonly DependencyProperty PointOutBlockProperty =
                BlockGridLayerContainer.PointOutBlockProperty.
            AddOwner(typeof(BlockGridLayerBase), new FrameworkPropertyMetadata(BlockGridLayerContainer.PointOutBlockProperty.DefaultMetadata.DefaultValue,
        FrameworkPropertyMetadataOptions.Inherits));

        public int PointOutDataRowIndex
        {
            get { return (int)GetValue(PointOutDataRowIndexProperty); }
            set { SetValue(PointOutDataRowIndexProperty, value); }
        }

        public static readonly DependencyProperty PointOutDataRowIndexProperty =
                BlockGridLayerContainer.PointOutDataRowIndexProperty.
            AddOwner(typeof(BlockGridLayerBase), new FrameworkPropertyMetadata(BlockGridLayerContainer.PointOutDataRowIndexProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits));


        //internal double PointOutDataRowTop { get; set; }

        internal virtual double GetPointOutY()
        {
            return _layerContainer.PointOutDataRowTop;
        }

        public double LayoutClipY
        {
            get { return (double)GetValue(LayoutClipYProperty); }
            set { SetValue(LayoutClipYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LayoutClipY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LayoutClipYProperty =
            DependencyProperty.Register("LayoutClipY", typeof(double), typeof(BlockGridLayerBase), new UIPropertyMetadata(0d));

        public ITerm DropedPlacement
        {
            get { return (ITerm)GetValue(DropedPlacementProperty); }
            set { SetValue(DropedPlacementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DropedPlacement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DropedPlacementProperty =
            DependencyProperty.Register("DropedPlacement", typeof(ITerm), typeof(BlockGridLayerBase), new FrameworkPropertyMetadata(null));

        public double Interval
        {
            get { return (double)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        public static readonly DependencyProperty IntervalProperty =
                BlockGridLayerContainer.IntervalProperty.AddOwner(typeof(BlockGridLayerBase),
                new FrameworkPropertyMetadata(BlockGridLayerContainer.IntervalProperty.DefaultMetadata.DefaultValue,
                    FrameworkPropertyMetadataOptions.Inherits));

        public IBlockConverter BlockConverter
        {
            get { return (IBlockConverter)GetValue(BlockConverterProperty); }
            set { SetValue(BlockConverterProperty, value); }
        }

        public static readonly DependencyProperty BlockConverterProperty =
                BlockGridLayerContainer.BlockConverterProperty.
            AddOwner(typeof(BlockGridLayerBase), new FrameworkPropertyMetadata(BlockGridLayerContainer.BlockConverterProperty.DefaultMetadata.DefaultValue,
        FrameworkPropertyMetadataOptions.Inherits));

        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            BlockGridLayerContainer.ItemsSourceProperty.AddOwner(typeof(BlockGridLayerBase), new FrameworkPropertyMetadata(BlockGridLayerContainer.ItemsSourceProperty.DefaultMetadata.DefaultValue,
               (sender, e) => { }, (d, baseValue) =>
               {
                   if (baseValue != null)
                   {
                       d.SaftyInvoke<BlockGridLayerBase>(o =>
                                                             {
                                                                 o.ItemsSourceChanging();
                                                                 o.InvalidateVisual();
                                                             });
                   }
                   return baseValue;
               }));

        //public LayerRegion Region { get; set; }

        internal IEnumerable GetItems(int index, ITerm fetchRange)
        {
            if (ItemsSource.IsNot<IList<IEnumerable>>())
                return ItemsSource;

            return ItemsSource[index].SaftyGetProperty<IEnumerable, ITermContainer>(o =>
            {
                return o.Fetch(fetchRange.Start, fetchRange.End);
            }, () => ItemsSource[index] as IEnumerable);
        }

        public int GetItemsSourceCount()
        {
            if (ItemsSource == null) return 0;
            return ItemsSource.IsNot<IList<IEnumerable>>() ? 1 : ItemsSource.Count;
        }

        public virtual bool IsMouseDragging()
        {
            return false;
        }

        protected bool IsHeader(object source)
        {
            // LayerContainer.
            return source.If<FrameworkElement>(el => el.FindAncestor<FrameworkElement>(p => p == LayerContainer.Header) != null);
        }

        internal virtual void ItemsSourceChanging() { }

        protected override Size MeasureOverride(Size availableSize)
        {
            // 如发现所需高度为无穷大, 务必要先给个最小高度(不可为0), 不然无法呈现
            var height = double.IsPositiveInfinity(availableSize.Height) ? Interval : availableSize.Height;
            var width = double.IsInfinity(availableSize.Width) ? 0 : availableSize.Width;

            var itemsSourceCount = GetItemsSourceCount();
            if (0 < itemsSourceCount)
            {
                height = Math.Min(Interval * itemsSourceCount, availableSize.Height);
            }
            return new Size(width, height);
        }

        internal double VerticalOffset
        {
            get
            {
                //if (AxisPanel.ViewportRangeY.ViewMax < AxisPanel.VerticalOffset + Interval)
                //    return AxisPanel.ViewportRangeY.ViewMin + Interval;
                return AxisPanel.ViewportRangeY.ViewMin;
            }
        }
        internal double HorizontalOffset { get { return AxisPanel.ViewportRangeX.ViewMin; } }
    }
}
