using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using Luna.Common;
using Luna.Core;
using Luna.Core.Extensions;
using Luna.WPF.ApplicationFramework.Extensions;

namespace Luna.WPF.ApplicationFramework.Controls
{
    [DefaultProperty("CellTemplate"), ContentProperty("CellTemplate"), Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
    public class EditControlLayer : RangeSelected
    {
        #region Attached Event 'EndEdit'
        static EditControlLayer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EditControlLayer), new FrameworkPropertyMetadata(typeof(EditControlLayer)));
            VisibilityProperty.OverrideMetadata(typeof(EditControlLayer), new FrameworkPropertyMetadata(VisibilityProperty.DefaultMetadata.DefaultValue,
                (d, e) =>
                {
                    d.SaftyInvoke<EditControlLayer>(o => o.TryEndEdit(Key.Escape));
                }));
        }

        public static readonly RoutedEvent EndEditEvent = EventManager.RegisterRoutedEvent("EndEdit", RoutingStrategy.Tunnel, typeof(CellEditRoutedEventHandler), typeof(EditControlLayer));

        public static void AddEndEditHandler(DependencyObject d, CellEditRoutedEventHandler handler)
        {
            d.SaftyInvoke<UIElement>(uie => uie.AddHandler(EndEditEvent, handler));
        }

        public static void RemoveEndEditHandler(DependencyObject d, CellEditRoutedEventHandler handler)
        {
            d.SaftyInvoke<UIElement>(uie => uie.RemoveHandler(EndEditEvent, handler));
        }
        #endregion

        private FrameworkElement _control;

        private bool OnRaiseEndEditEvent(IInputElement uiElement)
        {
            if (uiElement == null) return false;
            var arg = new CellEditRoutedEventArgs(EndEditEvent, this);
            uiElement.RaiseEvent(arg);

            if (arg.HasChanged)
            {
                //arg.ChangedSet.ForEach(x => DirtyItems.Add(x));
                //_isDirty = true;
                LayerContainer.OnRaiseAfterMouseUpEvent(this); // 刷新控制, 重绘
            }
            return arg.HasChanged;
        }

        public DataTemplate CellTemplate { get; set; }



        public bool Popup
        {
            get { return (bool)GetValue(PopupProperty); }
            set { SetValue(PopupProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Popup.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PopupProperty =
            DependencyProperty.Register("Popup", typeof(bool), typeof(EditControlLayer), new UIPropertyMetadata(false));



        public Type InputControlType { get; set; }

        private FrameworkElement GetInnerControl()
        {
            if (_control == null) return null;

            var popup = _control as Popup;
            if (popup != null)
            {
                return popup.Child.FindVisualChild(InputControlType);
            }

            return _control.FindVisualChild(InputControlType);
        }

        private void LocateControl(Rect? rect, bool absolutPosition)
        {
            if (_control == null || rect == null) return;

            //if (rect.Value.Y < 0 || rect.Value.X < 0 || IsDraging || rect.Value == default(Rect))
            //    RemoveControl();
            //else
            //{
            var x = rect.Value.X - (absolutPosition ? HorizontalOffset : 0);
            var y = rect.Value.Y - (absolutPosition ? VerticalOffset : 0);


            if (_control is Popup)
            {
                var popup = _control as Popup;
                popup.Placement = PlacementMode.RelativePoint;
                popup.HorizontalOffset = x;
                popup.VerticalOffset = y + Interval; //  1 for border
            }
            else
                _control.Margin = new Thickness(x, y, 0, 0);
            //}
        }

        private void RenderControl(Panel panel)
        {
            if (panel == null || InitalDrawLocation == null || (_control != null && panel.Children.Contains(_control)))
                return;

            var content = new ContentPresenter
                           {
                               Tag = this,
                               //关键数据
                               ContentTemplate = CellTemplate,
                               Content = InitalDrawBlock,
                               //关键数据 当前选中的Cell
                               HorizontalAlignment = HorizontalAlignment.Left,
                               VerticalAlignment = VerticalAlignment.Top
                           };
            if (Popup)
            {
                _control = new Popup { Child = content, PlacementTarget = this, AllowsTransparency = true };
            }
            else
                _control = content;

            panel.Children.Add(_control); // 被加到BlockGridLayerContainer下的容器

        }

        private void ScrollToInitalDrawLocation()
        {
            #region 自动滚动到编辑格
            Point point;
            if (!IsInViewRange(out point))
            {
                //Scroll to 当 InitalDrawLocation 不再可视范围内, 自动定位
                if (!InitalDrawLocation.HasValue) return;

                if (!AxisYConverter.IsInViewRagne(point.Y))
                    AxisPanel.SetVerticalOffset(InitalDrawLocation.Value.Y);
                if (!AxisXConverter.IsInViewRagne(point.X))
                    AxisPanel.SetHorizontalOffset(InitalDrawLocation.Value.X);
            }
            #endregion
        }

        private void BeginEdit(RoutedEventArgs e)
        {
            _editCell = InitalDrawLocation;

            ScrollToInitalDrawLocation();

            var panel = this.FindAncestor<Panel>();
            if (panel.IsMouseCaptured) // 当编辑格控件已打开,自动将焦点移至编辑控件
            {
                GetInnerControl().SaftyInvoke(c => Keyboard.Focus(c));
                return;
            }
            RenderControl(panel); // panel might be the Grid
            LocateControl(InitalDrawLocation, true);

            if (Popup)
            {
                _control.SaftyInvoke<Popup>(c =>
                {
                    if (!c.IsLoaded)
                        c.Opened += delegate
                        {
                            OnInnerChildLoaed(e);
                            LayerContainer.PreventMouseEvent(this);
                        };
                    c.IsOpen = true;
                });
            }
            else
            {
                _control.SaftyInvoke(c =>
                {
                    c.Loaded += delegate
                    {
                        OnInnerChildLoaed(e);
                    };
                });
            }
            Mouse.Capture(panel, CaptureMode.SubTree);
        }

        private void OnInnerChildLoaed(RoutedEventArgs e)
        {
            var innerControl = GetInnerControl();

            if (innerControl == null)
                return;

            Keyboard.Focus(innerControl);

            if (!Popup && e.IsNot<MouseButtonEventArgs>())
                innerControl.Loaded += (s, arg) => innerControl.RaiseEvent(e);

            innerControl.PreviewKeyDown += (s, keyEventArgs) =>
            {
                Dispatcher.BeginInvoke(() => TryEndEdit(keyEventArgs.Key), DispatcherPriority.ContextIdle);
                e.Handled = true;
            };

        }

        private Rect? _editCell;

        protected override void OnParentTextInput(TextCompositionEventArgs e)
        {
            var text = e.Text;
            if (e.OriginalSource != LayerContainer)
                return;

            //Popup专用
            if (text == "\r" || !Popup) return;

            ScrollToInitalDrawLocation();

            _editCell = InitalDrawLocation;

            if (_control != null)
            {
                _control.SaftyInvoke<Popup>(o =>
                                                {
                                                    //xLocateControl(InitalDrawLocation, true);
                                                    o.IsOpen = true;
                                                    RaiseTextInput(text);
                                                    LayerContainer.PreventMouseEvent(this);
                                                });
                return;
            }

            var panel = this.FindAncestor<Panel>();
            RenderControl(panel); // panel might be the Grid
            LocateControl(InitalDrawLocation, true);

            _control.SaftyInvoke<Popup>(c =>
            {
                EventHandler openedEventHandler = delegate
                                                      {
                                                          RaiseTextInput(text);
                                                          LayerContainer.PreventMouseEvent(this);
                                                      };
                c.Opened += openedEventHandler;
                c.Closed += delegate
                                {
                                    c.Opened -= openedEventHandler;
                                };
                c.IsOpen = true;
            });
            Mouse.Capture(panel, CaptureMode.SubTree);
        }

        private void RaiseTextInput(string text)
        {
            var innerControl = GetInnerControl();
            Keyboard.Focus(innerControl);

            innerControl.RaiseEvent(new TextCompositionEventArgs(InputManager.Current.PrimaryKeyboardDevice,
                                         new TextComposition(InputManager.Current, innerControl, text)) { RoutedEvent = TextCompositionManager.TextInputEvent });
        }

        // BMK Keyboard operation
        protected override void OnParentKeyDown(object sender, KeyEventArgs e)
        {
            //当Popoup在用键盘选择ListBox的Item时必须阻止让其事件传递下续,因为传递下去位导致选中Cell发生移动
            var isNavigateKey = e.Key.IsNavigateKey();
            if (isNavigateKey && !EditControlIsClosed())
            {
                e.Handled = true;
                return;
            }

            TryEndEdit(e.Key);

            //int32EditBox使用, Popup不需要利用OnParentKeyDown事件而是使用OnParentTextInput
            if (_control == null && !Popup && !isNavigateKey && e.IsNumberKey())
            {
                BeginEdit(e);
            }

            if (!e.Handled)
                base.OnParentKeyDown(sender, e);
        }

        protected override void OnParentMouseUp(object sender, MouseButtonEventArgs e)
        {
            //当Popup点中ListBox选项代表结束编辑,保存
            if (Popup && !EditControlIsClosed() && e.Source == _control.SaftyGetProperty<UIElement, Popup>(o => o.Child, () => _control))
            {
                e.Handled = TryEndEdit(Key.Enter); //为了不让其选中其他Cell必须在此跳出
                return;
            }

            if (!Popup && _control != null && _editCell != InitalDrawLocation)
            {
                //int32EditBox进入编辑状态, 鼠标点其他Cell , 必须执行修改动作
                TryEndEdit(Key.Enter);
            }

            base.OnParentMouseUp(sender, e);

            if (Popup && _editCell != InitalDrawLocation && _editCell != null)
            {
                //因为双击事件会导致重新进入OnParentMouseUp, 所以需要识别InitalDrawLocation是不是点中一样的Cell, 如果点到其他Cell, 必须将Popup关闭
                RemoveControl();
                _editCell = null;
            }

            if (_editCell != InitalDrawLocation && EditControlIsClosed()) // 为了让键盘事件能够捕捉,必须LayerContainer设为焦点
                LayerContainer.Focus();
        }

        protected override void OnParentMouseDoubleClick(MouseButtonEventArgs e)
        {
            BeginEdit(e);
            e.Handled = true;
        }

        protected override void OnParentHitOutOfRange()
        {
            RemoveControl();
        }

        internal bool TryEndEdit(Key key)
        {
            var controlRemoved = false;

            if (key == Key.Delete)
                OnRaiseEndEditEvent(this); // 作用於此 EditControlLayer 上使用 EndEdit 事件
            else if (_control != null && key == Key.Enter)
            {
                var popup = _control as Popup;

                (popup == null ? _control : popup.Child).LoopVisualChild(d =>
                                              {
                                                  var found = false;
                                                  d.SaftyInvoke<Control>(uie =>
                                                                             {
                                                                                 if (uie.Name != "PART_Value") return;

                                                                                 found = true;
                                                                                 controlRemoved = OnRaiseEndEditEvent(uie);
                                                                                 //uie.RaiseEvent(new RoutedEventArgs(EndEditEvent));
                                                                                 RemoveControl();
                                                                                 //LayerContainer.OnAfterMouseUp(this);
                                                                             });
                                                  return found;
                                              });
            }
            else if (key == Key.Escape)
            {
                RemoveControl();
                controlRemoved = true;
            }

            return controlRemoved;
        }

        private bool EditControlIsClosed()
        {
            return _control.SaftyGetProperty<bool, Popup>(p => !p.IsOpen, () => _control == null);
        }

        private void RemoveControl()
        {
            LayerContainer.Focus();
            LayerContainer.PreventMouseEvent(null);

            if (Popup)
                _control.SaftyInvoke<Popup>(p => p.IsOpen = false);
            else
            {
                if (_control == null) return;
                var panel = this.FindAncestor<Panel>();
                if (panel == null || !panel.Children.Contains(_control)) return;

                panel.Children.Remove(_control);

                _control = null;
            }

            this.FindAncestor<Panel>().ReleaseMouseCapture();

        }

        internal override void DrawComplete(Rect[] range, object block)
        {
            base.DrawComplete(range, block);

            var y = range[0].Y + VerticalOffset; // 转换成相对定位
            var x = range[0].X + HorizontalOffset;  // 转换成相对定位
            if (y < 0)
                y = 0;
            if (x < 0)
                x = 0;
            if (range[0] == range[1] && (!RectIsZero(range[0]) && !RectIsZero(range[1])) || InitalDrawLocation == null)
                InitalDrawLocation = new Rect(x, y, range[0].Width, range[1].Height);


            LocateControl(InitalDrawLocation, true);

        }

        internal override void NotInViewRange()
        {
            RemoveControl();
        }

        private bool RectIsZero(Rect rect)
        {
            return rect.X == 0 && rect.Y == 0 && rect.Height == 0 && rect.Width == 0;
        }

        protected override void OnDispose()
        {
            RemoveControl();
            base.OnDispose();
        }
    }
}