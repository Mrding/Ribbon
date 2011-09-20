using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using Luna.Common.Extensions;

namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    //继承于DependencyObject是为了在xaml中有智能提示
    public class ScheduleGridDropBehavior:DependencyObject
    {
        private static readonly DataFormat Format = DataFormats.GetDataFormat("DragAndDropControl");
        private PostionAdorner _dragPositonAdorner;
        private UIElement _verticalControl;
        private UIElement _horizontalControl;
        //private IVerticalControl<int> _verticalInterface;
        //private IHorizontalControl<DateTime> _horizontalInterface;
        private readonly ScheduleGrid _scheduleGrid;
        public ScheduleGridDropBehavior(ScheduleGrid panel)
        {
            _scheduleGrid = panel;            
        }

        public static readonly RoutedEvent ItemDropedEvent = 
            EventManager.RegisterRoutedEvent("ItemDroped", RoutingStrategy.Bubble, typeof(ItemDropEventHandler), typeof(ScheduleGridDropBehavior));
        public static void AddItemDropedHandler(DependencyObject d, ItemDropEventHandler handler)
        {
            var uie = d as UIElement;
            if (uie != null)
            {
                uie.AddHandler(ItemDropedEvent, handler);
            }
        }
        public static void RemoveItemDropedHandler(DependencyObject d, ItemDropEventHandler handler)
        {
            var uie = d as UIElement;
            if (uie != null)
            {
                uie.RemoveHandler(ItemDropedEvent, handler);
            }
        }

        public static DataTemplate GetDragItemTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(DragItemTemplateProperty);
        }

        public static void SetDragItemTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(DragItemTemplateProperty, value);
        }
      
        public static readonly DependencyProperty DragItemTemplateProperty =
            DependencyProperty.RegisterAttached("DragItemTemplate", typeof(DataTemplate), typeof(ScheduleGridDropBehavior), 
            new UIPropertyMetadata());



        private static ScheduleGridDropBehavior GetDropBehavior(DependencyObject obj)
        {
            return (ScheduleGridDropBehavior)obj.GetValue(DropBehaviorProperty);
        }

        private static void SetDropBehavior(DependencyObject obj, ScheduleGridDropBehavior value)
        {
            obj.SetValue(DropBehaviorProperty, value);
        }

        private static readonly DependencyProperty DropBehaviorProperty =
            DependencyProperty.RegisterAttached("DropBehavior", typeof(ScheduleGridDropBehavior), typeof(ScheduleGridDropBehavior), new UIPropertyMetadata());

        public static double GetDragItemXoffset(DependencyObject obj)
        {
            return (double)obj.GetValue(DragItemXoffsetProperty);
        }

        public static void SetDragItemXoffset(DependencyObject obj, double value)
        {
            obj.SetValue(DragItemXoffsetProperty, value);
        }
     
        public static readonly DependencyProperty DragItemXoffsetProperty =
            DependencyProperty.RegisterAttached("DragItemXoffset", typeof(double), typeof(ScheduleGridDropBehavior), new UIPropertyMetadata(10.0));


        public static double GetDragItemYoffset(DependencyObject obj)
        {
            return (double)obj.GetValue(DragItemYoffsetProperty);
        }

        public static void SetDragItemYoffset(DependencyObject obj, double value)
        {
            obj.SetValue(DragItemYoffsetProperty, value);
        }
      
        public static readonly DependencyProperty DragItemYoffsetProperty =
            DependencyProperty.RegisterAttached("DragItemYoffset", typeof(double), typeof(ScheduleGridDropBehavior), new UIPropertyMetadata(-20.0));

        public static bool GetCanDrop(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanDropProperty);
        }

        public static void SetCanDrop(DependencyObject obj, bool value)
        {
            obj.SetValue(CanDropProperty, value);
        }

        public static readonly DependencyProperty CanDropProperty =
            DependencyProperty.RegisterAttached("CanDrop", typeof(bool), typeof(ScheduleGridDropBehavior),
            new UIPropertyMetadata((d, e) =>
                {
                    var scheduleGrid = d as ScheduleGrid;
                    if (scheduleGrid == null)
                        return;

                    var behavior = GetDropBehavior(scheduleGrid);
                    if (behavior == null)
                        SetDropBehavior(scheduleGrid, behavior = new ScheduleGridDropBehavior(scheduleGrid));
                    var canDrop = (bool)e.NewValue;
                    behavior.CanDrop = canDrop;
                    if (!canDrop)
                    {
                        SetDropBehavior(scheduleGrid, null);
                    }                  
                }));

        private bool _canDrop;
        public bool CanDrop
        {
            get
            {
                return _canDrop;
            }
            set
            {
                if (_canDrop != value)
                {
                    _canDrop = value;
                    if (_canDrop)
                    {
                        SetDrop();
                    }
                    else
                    {
                        UnDrop();
                    }
                }
            }
        }

        private void SetDrop()
        {
            _scheduleGrid.AllowDrop = true;
            _scheduleGrid.Drop += PanelDrop;
            _scheduleGrid.DragEnter += PanelDragEnter;
            _scheduleGrid.DragLeave += PanelDragLeave;
            _scheduleGrid.DragOver += PanelDragOver;
        }

        private void UnDrop()
        {
            _scheduleGrid.AllowDrop = false;
            _scheduleGrid.Drop -= PanelDrop;
            _scheduleGrid.DragEnter -= PanelDragEnter;
            _scheduleGrid.DragLeave -= PanelDragLeave;
            _scheduleGrid.DragOver -= PanelDragOver;
        }

        void PanelDragOver(object sender, DragEventArgs e)
        {         
            if (_dragPositonAdorner != null )
            {
                Point hPoint;
                Point vPoint;
                var isInBound = InBound(e, out hPoint, out vPoint);
                if (isInBound)
                {
                    var date = hPoint.X;// _horizontalInterface.ScreenToDataX(hPoint.X);
                    var point = e.GetPosition(_scheduleGrid);
                 //   _dragPositonAdorner.Content = date.TurnToMultiplesOf5();
                    _dragPositonAdorner.Positon = point;
                    return;
                }
                _dragPositonAdorner.Content = null;
            }

            e.Effects = DragDropEffects.None;
            e.Handled = true;

        }

        private void PanelDragLeave(object sender, DragEventArgs e)
        {
            if (_dragPositonAdorner != null)
            {
                _dragPositonAdorner.Detach();
                _dragPositonAdorner = null;
            }
        }

        private void PanelDragEnter(object sender, DragEventArgs e)
        {
            //由于ScheduleGrid的元素加载过慢，所以在这里预存方便调用。
            _verticalControl = _scheduleGrid.VerticalMainElement;
            _horizontalControl = _scheduleGrid.HorizontalMainElement;

            //_horizontalInterface = (IHorizontalControl<DateTime>)_scheduleGrid.HorizontalMainElement;
            //_verticalInterface = (IVerticalControl<int>)_scheduleGrid.VerticalMainElement;

            try
            {
                var data = e.Data.GetData(Format.Name);
                var canDrag = data != null;
                if (!canDrag)
                    return;

                if (_dragPositonAdorner == null)
                {
                    var template = GetDragItemTemplate(_scheduleGrid);
                    var xoffset = GetDragItemXoffset(_scheduleGrid);
                    var yoffset = GetDragItemYoffset(_scheduleGrid);
                    var adornerLayer = AdornerLayer.GetAdornerLayer(_scheduleGrid);
                    _dragPositonAdorner = new PostionAdorner(_scheduleGrid, adornerLayer, template)
                                              {
                                                  Xoffset = xoffset,
                                                  Yoffset = yoffset
                                              };
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

            }
        }

        private void PanelDrop(object sender, DragEventArgs e)
        {
            if (_dragPositonAdorner != null)
            {
                Point hPoint;
                Point vPoint;
                var isInBound = InBound(e, out hPoint,out vPoint);
                if (isInBound)
                {
                    var date = hPoint.X;//_horizontalInterface.ScreenToDataX(hPoint.X);
                    var data = e.Data.GetData(Format.Name);
                    
                    //var panel = _scheduleGrid as IAxisPanel<DateTime, int>;
                    //var index = panel.ScreenToDataY(vPoint.Y);

                    //if (index >= 0 && index < _scheduleGrid.RowCount
                    //    && date >= _scheduleGrid.StartTime
                    //    && date <= _scheduleGrid.EndTime)
                    //{
                    //    var args = new ItemDropEventArgs(index, date.TurnToMultiplesOf5())
                    //                   {
                    //                       RoutedEvent = ItemDropedEvent,
                    //                       Source = this,
                    //                       DropData = data
                    //                   };
                    //    _scheduleGrid.RaiseEvent(args);
                    //    PanelDragLeave(null, null);
                    //    return;
                    //}
                }
            }

            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private bool InBound(DragEventArgs e, out Point hPoint,out Point vPoint)
        {
            hPoint = e.GetPosition(_horizontalControl);
            var inXDragBound = hPoint.X >= 0 && hPoint.X <= _horizontalControl.DesiredSize.Width;
            vPoint = e.GetPosition(_verticalControl);
            var inYDragBound = vPoint.Y >= 0 && vPoint.Y <= _verticalControl.DesiredSize.Height;

            return inXDragBound || inYDragBound;
        }
    }
}
