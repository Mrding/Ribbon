using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using Luna.Common;
using Luna.Common.Domain;
using Luna.Common.Interfaces;
using Luna.Core.Extensions;
using Luna.WPF.ApplicationFramework.Behaviors;
using Luna.WPF.ApplicationFramework.Extensions;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public class BlockGridLayerContainer : HeaderedContentControl, IInitialize
    {
        static BlockGridLayerContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BlockGridLayerContainer), new FrameworkPropertyMetadata(typeof(BlockGridLayerContainer)));
        }

        public static readonly RoutedEvent AfterMouseUpEvent = EventManager.RegisterRoutedEvent("AfterMouseUp", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BlockGridLayerContainer));

        public static readonly DependencyProperty PointOutBlockProperty = DependencyProperty.RegisterAttached("PointOutBlock", typeof(ITerm), typeof(BlockGridLayerContainer), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty PointOutDataRowIndexProperty = DependencyProperty.Register("PointOutDataRowIndex", typeof(int), typeof(BlockGridLayerContainer), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty IntervalProperty = DependencyProperty.RegisterAttached("Interval", typeof(double), typeof(BlockGridLayerContainer), new FrameworkPropertyMetadata(50.0, FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty BlockConverterProperty = DependencyProperty.RegisterAttached("BlockConverter", typeof(IBlockConverter),
            typeof(BlockGridLayerContainer), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.RegisterAttached("ItemsSource", typeof(IList), typeof(BlockGridLayerContainer),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, (sender, e) => { }, (d, baseValue) =>
            {
                if (baseValue != null)
                {
                    d.SaftyInvoke<BlockGridLayerContainer>(o => o.Children.ForEach<FrameworkElement>(c => c.InvalidateVisual()));
                }

                return baseValue;
            }));

        //public static readonly DependencyProperty ClickPointProperty = DependencyProperty.Register("ClickPoint", typeof(Point), typeof(BlockGridLayerContainer), new UIPropertyMetadata(new Point(0, 0)));

        private ScheduleGrid _axisPanel;
        private List<IBlockGridLayer> _children;
        protected IBlockGridLayer _preventMouseLayer;

        public BlockGridLayerContainer()
        {
            Interaction.GetBehaviors(this).Add(new InitializeBehavior());
        }

        public void Initialize()
        {
            Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Children.Clear();
            Unloaded -= OnUnloaded;
        }

        internal void AddAsChild(BlockGridLayerBase layer)
        {
            layer.AxisPanel = AxisPanel;
            Children.Add(layer);
        }

        protected ScheduleGrid AxisPanel
        {
            get
            {
                while (_axisPanel == null)
                    _axisPanel = this.FindAncestor<ScheduleGrid>();
                return _axisPanel;
            }
        }

        internal void PreventMouseEvent(IBlockGridLayer layer)
        {
            _preventMouseLayer = layer;
        }

        private void TryPassThroughMousePrevent<TArg>(IBlockGridLayer layer, Action<object, TArg> handel, TArg e)
        {
            if (_preventMouseLayer == null || _preventMouseLayer == layer)
                handel(this, e);
        }

        #region HandleEvent

        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            HandleEvent(item =>
            {
                item.OnParentTextInputPrivate(e);
                return e.Handled;
            }, e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            //Mouse.Capture(this, CaptureMode.SubTree);

            HandleEvent(item =>
                            {
                                TryPassThroughMousePrevent(item, item.OnParentMouseDownPrivate, e);
                                return e.Handled;
                            }, e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            //if (e.LeftButton == MouseButtonState.Released)
            //    TraceHoverBlock();

          
            HandleEvent(item =>
            {
                TryPassThroughMousePrevent(item, item.OnParentMouseUpPrivate, e);
                return e.Handled;
            }, e);
            //xFocus();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Source == Header) return;

            var mousePoint = GetMousePosition(false);
            if (e.LeftButton == MouseButtonState.Released) // mouse moving for tooltip using
            {
                PointOutBlock = PointoutBlock(mousePoint) as ITerm;
            }
                
            ResetCursor();
            HandleEvent(item =>
                            {
                                TryPassThroughMousePrevent(item, item.OnParentMouseMovePrivate, e);
                                return e.Handled;
                            }, mousePoint, e);

            if (e.LeftButton != MouseButtonState.Pressed)
                SetCursor();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            //xthis.Focus(); // 会造成问题需想其他办法
            HandleEvent(item =>
                            {
                                TryPassThroughMousePrevent(item, item.OnParentMouseEnterPrivate, e);
                                return e.Handled;
                            }, e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            HandleEvent(item =>
                            {
                                TryPassThroughMousePrevent(item, item.OnParentMouseLeavePrivate, e);
                                return e.Handled;
                            }, e);
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            var handle = new Func<IBlockGridLayer, bool>(item =>
            {
                item.OnParentDragLeavePrivate(e);
                return e.Handled;
            });
            foreach (var blockGridLayerBase in Children)
            {
                if (handle(blockGridLayerBase)) break;
            }
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            var handle = new Func<IBlockGridLayer, bool>(item =>
            {
                item.OnParentDragOverPrivate(e);
                return e.Handled;
            });
            foreach (var blockGridLayerBase in Children)
            {
                if (handle(blockGridLayerBase)) break;
            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            var handle = new Func<IBlockGridLayer, bool>(item =>
            {
                item.OnParentDropPrivate(e);
                return e.Handled;
            });

            foreach (var blockGridLayerBase in Children)
            {
                if (handle(blockGridLayerBase)) break;
            }
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            HandleEvent(item =>
            {
                TryPassThroughMousePrevent(item, item.OnParentMouseDoubleClickPrivate, e);
                return e.Handled;
            }, e);
        }


        protected override void OnKeyDown(KeyEventArgs e)
        {
            var operationCanceled = false;
            if (e.Key == Key.Escape)
            {
                HandleEvent(item =>
                   {
                       item.OnParentKeyEscPressPrivate(this, e);
                       if (e.Handled)
                           operationCanceled = true;
                       return e.Handled;
                   }, e);
            }

            if (operationCanceled) return;

            HandleEvent(item =>
                            {
                                item.OnParentKeyDownPrivate(this, e);
                                return e.Handled;
                            }, e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            HandleEvent(item =>
            {
                item.OnParentKeyUpPrivate(this, e);
                return e.Handled;
            }, e);
        }

        private void HandleEvent(Func<IBlockGridLayer, bool> handle, Point point, RoutedEventArgs e)
        {
            if ((AxisYConverter.IsInViewRagne(point.Y) && AxisXConverter.IsInViewRagne(point.X)) || HeaderIsHitted() || e.OriginalSource != e.Source)
            {
                foreach (BlockGridLayerBase blockGridLayerBase in Children)
                {
                    if (blockGridLayerBase.Visibility == Visibility.Visible)
                        if (handle(blockGridLayerBase)) break;
                }
            }
            else // out of range
            {
                if (e.RoutedEvent == Mouse.MouseUpEvent)
                    foreach (var blockGridLayerBase in Children)
                        blockGridLayerBase.OnParentHitOutOfRangePrivate();
            }

            //var head = Header as FrameworkElement;

            //IEnumerable<BlockGridLayerBase> layers;

            //if (head != null && point.Y <= head.ActualHeight)
            //    layers =
            //        Children.OfType<BlockGridLayerBase>().Where(c => (c.Region & LayerRegion.Head) == LayerRegion.Head);
            //else
            //    layers =
            //        Children.OfType<BlockGridLayerBase>().Where(c => (c.Region & LayerRegion.Body) == LayerRegion.Body);

            //BMK Refine Children When HandleEvent

        }

        private void HandleEvent(Func<IBlockGridLayer, bool> handle, RoutedEventArgs e)
        {
            HandleEvent(handle, GetMousePosition(false), e);
        }

        #endregion

        #region Cursor

        private bool _changeCursor;
        private Cursor _mouseCursor;

        private void ResetCursor()
        {
            _changeCursor = false;
            _mouseCursor = null;
        }

        internal void ChangeCursor(Cursor cursor)
        {
            _mouseCursor = cursor;
            _changeCursor = true;
        }

        private void SetCursor()
        {
            if (_mouseCursor == null && Mouse.OverrideCursor == null) return;
            Mouse.OverrideCursor = _changeCursor ? _mouseCursor : null;
        }

        #endregion

        protected IHorizontalControl AxisXConverter
        {
            get { return AxisPanel; }
        }

        protected IVerticalControl AxisYConverter
        {
            get { return AxisPanel; }
        }

        public double PointOutDataRowTop { get; set; }

        public ITerm PointOutBlock
        {
            get { return (ITerm)GetValue(PointOutBlockProperty); }
            set { SetValue(PointOutBlockProperty, value); }
        }

        public int PointOutDataRowIndex
        {
            get { return (int)GetValue(PointOutDataRowIndexProperty); }
            set { SetValue(PointOutDataRowIndexProperty, value); }
        }

        public double Interval
        {
            get { return (double)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        public bool RecursivelyPointout
        {
            get { return (bool)GetValue(RecursivelyPointoutProperty); }
            set { SetValue(RecursivelyPointoutProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecursivelyPointout.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RecursivelyPointoutProperty =
            DependencyProperty.Register("RecursivelyPointout", typeof(bool), typeof(BlockGridLayerContainer), new UIPropertyMetadata(false));



        public bool CellMode
        {
            get { return (bool)GetValue(CellModeProperty); }
            set { SetValue(CellModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellModeProperty =
            DependencyProperty.Register("CellMode", typeof(bool), typeof(BlockGridLayerContainer), new FrameworkPropertyMetadata(false, (d, e) =>
            {
                //d.SaftyInvoke<BlockGridLayerContainer>();
            }));


        public IBlockConverter BlockConverter
        {
            get { return (IBlockConverter)GetValue(BlockConverterProperty); }
            set { SetValue(BlockConverterProperty, value); }
        }

        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        protected List<IBlockGridLayer> Children
        {
            get { return _children ?? (_children = new List<IBlockGridLayer>(10)); }
        }

        public static readonly RoutedEvent PreventEvent = EventManager.RegisterRoutedEvent("Prevent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BlockGridLayerBase));

        public event RoutedEventHandler Prevent
        {
            add { AddHandler(PreventEvent, value); }
            remove { RemoveHandler(PreventEvent, value); }
        }

        internal bool TryPrevent(ITerm block)
        {
            var e = new RoutedEventArgs(PreventEvent, block);
            RaiseEvent(e);
            return e.Handled;
        }

        public event RoutedEventHandler AfterMouseUp
        {
            add { AddHandler(AfterMouseUpEvent, value); }
            remove { RemoveHandler(AfterMouseUpEvent, value); }
        }

        public void OnRaiseAfterMouseUpEvent(BlockGridLayerBase layerBase)
        {
            var args = new RoutedEventArgs(AfterMouseUpEvent, layerBase);
            RaiseEvent(args);

            //Henry modified: after block changed
            // TODO:BlockGrid可利用args来决定是否要刷新
            AxisPanel.Refresh();
        }

        

        internal Point GetMousePosition(bool absolutPosition)
        {
            var point = Mouse.GetPosition(AxisPanel.HorizontalMainElement);
            if (absolutPosition)
            {
                point.Y += AxisPanel.ViewportRangeY.ViewMin;
                point.X += AxisPanel.ViewportRangeX.ViewMin;
            }

            return point;
        }

        private object PointoutBlock(Point e)
        {
            var y = e.Y; //- GetHeaderHeight();

            //var y1 = AxisYConverter.ScreenToData(y);
            //var y2 = (int)(y / Interval) + AxisYConverter.ScreenToData(0);

            //if (y1 != y2)
            //    Debug.Print(string.Format("-------- ViewportToDataY called : y1 = {0}  y2 = {1}", y1, y2));

            // BMK Ambiguous getRowIndex
            PointOutDataRowIndex = AxisYConverter.ScreenToData(y);

            object fountBlock = null;
            if (ItemsSource != null && 0 <= PointOutDataRowIndex && PointOutDataRowIndex < ItemsSource.Count)
            {
                var dateTime = AxisXConverter.ScreenToData(e.X); // current mouse moved postion
                var positionY = y % Interval; // for Top useage


                if (CellMode)
                {
                    var date = dateTime.Date;
                    ItemsSource[PointOutDataRowIndex].SaftyInvoke<IEnumerable>(
                     o =>
                     {
                         fountBlock = o.OfType<ITerm>().FirstOrDefault(t =>
                                        {
                                            //using HrDate
                                            var blockStart = BlockConverter.GetStart(t);
                                            return (date == blockStart) && !TryPrevent(t);
                                        });
                     });

                    if (RecursivelyPointout)
                        ItemsSource[PointOutDataRowIndex].SaftyInvoke<IDateIndexer<DateTerm>>(o => o[date].SaftyInvoke(b => fountBlock = b));
                }
                else
                {
                    ItemsSource[PointOutDataRowIndex].SaftyInvoke<IEnumerable>(
                     o =>
                     {
                         fountBlock = o.OfType<ITerm>().LastOrDefault(block =>
                                           {
                                               var blockStart = BlockConverter.GetStart(block);
                                               var blockTop = BlockConverter.GetTop(block);
                                               return (blockStart <= dateTime && dateTime < block.End &&
                                                       blockTop <= positionY && (BlockConverter.GetHeight(block) + blockTop) >= positionY) && !TryPrevent(block);
                                           });
                     });
                }
            }

            if (fountBlock != null)
                PointOutDataRowTop = (e.Y - e.Y % Interval) + BlockConverter.GetTop(fountBlock);

            return fountBlock;
        }

        internal bool HeaderIsHitted()
        {
            var y = Mouse.GetPosition(this).Y;
            return 0 < y && y < (RenderSize.Height - AxisPanel.HorizontalMainElement.RenderSize.Height);
        }

        internal bool DataRowIndexIsOutOfScreen(int dataRowIndex, out double viewportTop)
        {
            var y0 = AxisPanel.GetScreenTopRowIndex();
            var yMax = AxisYConverter.ScreenToData(AxisPanel.ViewportHeight); // renderSize.Height ? 高度尚未经完整测试

            int dataRowScreenIndex;
            var outOfRange = (AxisControlExt.AxisOutOfRange(dataRowIndex, y0, yMax, out dataRowScreenIndex));

            viewportTop = dataRowScreenIndex * Interval;

            return outOfRange;
        }

        //protected override Size ArrangeOverride(Size arrangeBounds)
        //{
        //    var width = arrangeBounds.Width;
        //    if (EndTime != DateTime.MaxValue && _axisXConverter != null)
        //        width = EndTime.Subtract(_axisXConverter.ScreenToData(0)).TotalDays * 72;
        //    //if (ItemsSource != null && ItemsSource.Count > 0)
        //        return new Size(Math.Min(width, arrangeBounds.Width), arrangeBounds.Height);
        //    //return base.MeasureOverride(arrangeBounds);
        //}

        protected override Size MeasureOverride(Size constraint)
        {
            if (double.IsInfinity(constraint.Height) && ItemsSource != null && ItemsSource.Count > 0)
                return constraint; //return new Size(constraint.Width, ItemsSource.Count * Interval);
            return base.MeasureOverride(constraint);
        }


    }
}