using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Luna.WPF.ApplicationFramework.Extensions;

namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    //相关事件参考
    //http://msdn.microsoft.com/zh-cn/library/system.windows.forms.control.givefeedback.aspx
    public class DragAndDropControl : DependencyObject
    {

        #region dp property

        public static void SetCanDrag(DependencyObject control, bool value)
        {
            control.SetValue(CanDragProperty, value);
        }

        public static object GetCanDrag(DependencyObject obj)
        {
            return (object)obj.GetValue(CanDragProperty);
        }

        public static readonly DependencyProperty CanDragProperty =
            DependencyProperty.RegisterAttached("CanDrag", typeof(bool), typeof(DragAndDropControl), 
            new FrameworkPropertyMetadata(false, (d, e) =>
            {
                if (LunaControlHelper.IsInDesignMode) return;
                var itemsControl = d as ItemsControl;
                if (itemsControl == null) return;
                var behavior = itemsControl.GetValue(DragBehavior.BehaviorProperty) as DragBehavior;
                if (object.Equals(e.NewValue, true))
                {
                    new DragBehavior(itemsControl);
                }
                else if (!object.Equals(e.NewValue, true) && behavior != null)
                {
                    behavior.UnRegisterEvent();
                }
            }));

        public static void SetCanDrop(DependencyObject control, DataTemplate value)
        {
            control.SetValue(CanDropProperty, value);
        }

        public static object GetCanDrop(DependencyObject obj)
        {
            return (object)obj.GetValue(CanDropProperty);
        }

        public static readonly DependencyProperty CanDropProperty = 
            DependencyProperty.RegisterAttached("CanDrop", typeof(bool),typeof(DragAndDropControl), 
            new UIPropertyMetadata((d, e) =>
                {
                    if (LunaControlHelper.IsInDesignMode) return;
                    var itemsControl = d as ItemsControl;
                    if (itemsControl == null)
                        return;

                    itemsControl.AllowDrop = (bool)e.NewValue;
                    var behavior = itemsControl.GetValue(DropBehavior.BehaviorProperty) as DropBehavior;
                    if (itemsControl.AllowDrop && behavior==null)
                    {
                        new DropBehavior(itemsControl);
                    }
                    else if (!itemsControl.AllowDrop && behavior != null)
                    {
                        behavior.UnRegisterEvent();
                    }
                }));

        public static object GetDragItem(DependencyObject obj)
        {
            return (object)obj.GetValue(DragItemProperty);
        }

        public static void SetDragItem(DependencyObject obj, object value)
        {
            obj.SetValue(DragItemProperty, value);
        }

        public static readonly DependencyProperty DragItemProperty =
            DependencyProperty.RegisterAttached("DragItem", typeof(object), typeof(DragAndDropControl), 
            new UIPropertyMetadata());

        #endregion

        private static readonly DataFormat _format = DataFormats.GetDataFormat("DragAndDropControl");

        private static object _data;
        private static Point _dragStartPosition;
        public static bool _canDrag;

        class DragBehavior
        {

           public static readonly DependencyProperty BehaviorProperty =
                DependencyProperty.RegisterAttached("Behavior", typeof(DragBehavior), typeof(DragBehavior), 
                new UIPropertyMetadata());

            private readonly ItemsControl _element;
            public DragBehavior(ItemsControl element)
            {
                _element = element;
                _element.SetValue(BehaviorProperty,this);
                RegisterEvent();
            }

            private void RegisterEvent()
            {
                _element.PreviewMouseLeftButtonDown += itemsControl_MouseLeftButtonDown;
                _element.GiveFeedback += itemsControl_GiveFeedback;
                _element.QueryContinueDrag += itemsControl_QueryContinueDrag;
                _element.PreviewMouseLeftButtonUp += itemsControl_PreviewMouseLeftButtonUp;
            }

            public void UnRegisterEvent()
            {
                _element.PreviewMouseLeftButtonDown -= itemsControl_MouseLeftButtonDown;
                _element.GiveFeedback -= itemsControl_GiveFeedback;
                _element.QueryContinueDrag -= itemsControl_QueryContinueDrag;
                _element.PreviewMouseLeftButtonUp -= itemsControl_PreviewMouseLeftButtonUp;
            }


            void itemsControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                var itemsControl = (ItemsControl)sender;

                Point point = e.GetPosition(itemsControl);
                _data = itemsControl.GetDataObject(point);
                SetDragItem(itemsControl, _data);
                _canDrag = _data != null;
                if (!_canDrag)
                    return;

                //var itemControl = e.OriginalSource as UIElement;//itemsControl.ItemContainerGenerator.ContainerFromItem(_data);

                itemsControl.MouseMove += itemsControl_MouseMove;
            }

            void itemsControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
            {
                ItemsControl itemsControl = sender as ItemsControl;
                itemsControl.MouseMove -= itemsControl_MouseMove;
            }

            void itemsControl_MouseMove(object sender, MouseEventArgs e)
            {
                if (!_canDrag)
                    return;
                if (e.LeftButton != MouseButtonState.Pressed)
                    return;

                ItemsControl itemsControl = (ItemsControl)sender;
                Point currentPosition = e.GetPosition(itemsControl);

                //在一定返回内无效
                if ((Math.Abs(currentPosition.X - _dragStartPosition.X) <= SystemParameters.MinimumHorizontalDragDistance) ||
                        (Math.Abs(currentPosition.Y - _dragStartPosition.Y) <= SystemParameters.MinimumVerticalDragDistance))
                {
                    return;
                }

                DataObject dObject = new DataObject(_format.Name, _data);
                itemsControl.AllowDrop = false;

                //把数据传过去
                DragDropEffects effects = DragDrop.DoDragDrop(itemsControl, dObject, DragDropEffects.Move | DragDropEffects.None);

                //直到鼠标放开，完成拖拽动作才会继续下面的代码
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                itemsControl.MouseMove -= itemsControl_MouseMove;
                itemsControl.AllowDrop = (bool)itemsControl.GetValue(CanDragProperty);
            }

            //此函数在QueryContinueDrag中设置DragAction.Continue后引发和DragOver同时发生，用来设置鼠标外观
            void itemsControl_GiveFeedback(object sender, GiveFeedbackEventArgs e)
            {
                //鼠标状态改变
                if (e.Effects == DragDropEffects.None)
                {
                    //e.Handled = true;
                }
                e.UseDefaultCursors = false;

                Mouse.SetCursor(Cursors.Hand);
            }

            //此函数只在键盘鼠标发生变化时发生，可以在e.Action中设置是否继续，取消和拖拽成立返回
            void itemsControl_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
            {
                //e.Action默认为Continue
                if (e.EscapePressed)
                {
                    e.Action = DragAction.Cancel;
                    e.Handled = true;
                }
            }

        }

        class DropBehavior
        {

            public static readonly DependencyProperty BehaviorProperty =
                DependencyProperty.RegisterAttached("Behavior", typeof(DropBehavior), typeof(DropBehavior), 
                new UIPropertyMetadata());

            private readonly ItemsControl _element;

            internal DropBehavior(ItemsControl element)
            {
                _element = element;
                _element.SetValue(BehaviorProperty,this);
                RegisterEvent();
            }

            public void RegisterEvent()
            {
                _element.DragEnter += itemsControl_DragEnter;
                _element.DragOver += itemsControl_DragOver;
                _element.DragLeave += itemsControl_DragLeave;
                _element.Drop += itemsControl_Drop;
            }

            public void UnRegisterEvent()
            {
                _element.DragEnter -= itemsControl_DragEnter;
                _element.DragOver -= itemsControl_DragOver;
                _element.DragLeave -= itemsControl_DragLeave;
                _element.Drop -= itemsControl_Drop;
            }

            //当鼠标放开或QueryContinueDrag.Action = DragAction.Drop时引发
            void itemsControl_Drop(object sender, DragEventArgs e)
            {
                ItemsControl itemsControl = (ItemsControl)sender;
                object itemToAdd = e.Data.GetData(_format.Name);

                if (itemToAdd != null)
                {
                    e.Effects = ((e.KeyStates & DragDropKeyStates.ControlKey) != 0) ? DragDropEffects.Copy : DragDropEffects.Move;

                    if (itemsControl.ItemsSource != null)
                    {
                        IList list = itemsControl.ItemsSource as IList;
                        if (list != null && !list.Contains(itemToAdd))
                        {
                            list.Add(itemToAdd);
                        }
                    }
                    else
                    {
                        if (!itemsControl.Items.Contains(itemToAdd))
                        {
                            //如果是可视元素要先移除
                            Visual visual = itemToAdd as Visual;
                            if (visual != null)
                            {
                                ItemsControl container = visual.FindAncestor<ItemsControl>();
                                container.Items.Remove(itemToAdd);
                                e.Effects = DragDropEffects.None;
                            }
                            itemsControl.Items.Add(itemToAdd);
                        }
                        else
                        {
                            e.Effects = DragDropEffects.None;
                        }
                    }
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
                e.Handled = true;
            }

            //当鼠标移开控件或QueryContinueDrag中e.Action = DragAction.Cancel时引发
            void itemsControl_DragLeave(object sender, DragEventArgs e)
            {
                _canDrag = true;
            }

            //决定是否能拖放，当QueryContinueDrag中e.Action = DragAction.Continue时引发
            void itemsControl_DragOver(object sender, DragEventArgs e)
            {
                //只有下面两项均设置才能使拖拽为不可用状态
                if (!_canDrag)//根据DragEnter中的标志位来决定是否能放
                {
                    e.Effects = DragDropEffects.None;
                    e.Handled = true;
                }
            }

            void itemsControl_DragEnter(object sender, DragEventArgs e)
            {
                ItemsControl itemsControl = sender as ItemsControl;

                object draggedItem = e.Data.GetData(_format.Name);
                _canDrag = true;//在DragEnter中无法使鼠标状态变为不可选，需设置标志位

                //判断拖进的数据和原数据是否有重复
                if (draggedItem == null)
                {
                    _canDrag = false;
                }
                else
                {
                    if (itemsControl.ItemsSource != null)
                    {
                        IList list = itemsControl.ItemsSource as IList;
                        if (list == null || list.Contains(draggedItem))
                        {
                            _canDrag = false;
                        }
                    }
                    else
                    {
                        if (itemsControl.Items.Contains(draggedItem))
                        {
                            _canDrag = false;
                        }
                    }
                }
                e.Handled = true;
            }
        }
    }
}