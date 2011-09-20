using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Luna.Common;
using Luna.Core.Extensions;
using Luna.WPF.ApplicationFramework.Commands;
using System.Linq;

namespace Luna.GUI.UserControls
{
    /// <summary>
    /// Interaction logic for OrderingItemsButtons.xaml
    /// </summary>
    public partial class OrderingItemsButtons : UserControl, IDisposable
    {
        public OrderingItemsButtons()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(OrderingItemsButtons_Loaded);
        }

        void OrderingItemsButtons_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                if (!isReady)
                {
                    MoveTopButton.SetBinding(Button.CommandProperty, new Binding("TopCommand") { Source = this });
                    MoveUpButton.SetBinding(Button.CommandProperty, new Binding("UpCommand") { Source = this });
                    MoveDownButton.SetBinding(Button.CommandProperty, new Binding("DownCommand") { Source = this });
                    MoveToLastButton.SetBinding(Button.CommandProperty, new Binding("BottomCommand") { Source = this });
                    RegisterCommand();
                    this.Unloaded += new RoutedEventHandler(OrderingItemsButtons_Unloaded);
                    isReady = true;
                }
            }
        }

        private bool isReady;



        public bool EditInexValueOnly
        {
            get { return (bool)GetValue(EditInexValueOnlyProperty); }
            set { SetValue(EditInexValueOnlyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EditInexValueOnly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditInexValueOnlyProperty =
            DependencyProperty.Register("EditInexValueOnly", typeof(bool), typeof(OrderingItemsButtons), new UIPropertyMetadata(false));



        /// <summary>
        /// Gets or sets a value indicating 是否开启的是inf:EnableIndexing = true
        /// </summary>
        /// <value>
        /// 	<c>true</c> 代表使用ListBox类的EnableIndexing; otherwise, <c>false</c>.
        /// </value>
        public bool IsIndexEnable
        {
            get { return (bool)GetValue(IsIndexEnableProperty); }
            set { SetValue(IsIndexEnableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsIndexEnable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsIndexEnableProperty =
            DependencyProperty.Register("IsIndexEnable", typeof(bool), typeof(OrderingItemsButtons), new UIPropertyMetadata(false));

        public Selector TargetElement
        {
            get { return (ListBox)GetValue(TargetElementProperty); }
            set { SetValue(TargetElementProperty, value); }
        }

        public static readonly DependencyProperty TargetElementProperty =
            DependencyProperty.Register("TargetElement", typeof(Selector), typeof(OrderingItemsButtons),
            new UIPropertyMetadata(default(Selector), new PropertyChangedCallback(SelectorPropertyChangedCallback)));

        public static void SelectorPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OrderingItemsButtons control = d as OrderingItemsButtons;

            if (control.TargetElement != null)
            {
                control.RegisterCommand();
            }
        }

        #region ignore

        private void MoveTopButton_Click(object sender, RoutedEventArgs e)
        {
            Move(SelectedIndex, 0);
        }

        private void MoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            Move(SelectedIndex, SelectedIndex - 1);
        }

        private void MoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            Move(SelectedIndex, SelectedIndex + 1);
        }

        private void MoveToLastButton_Click(object sender, RoutedEventArgs e)
        {
            Move(SelectedIndex, ItemsCount - 1);
        }

        private void Move(int oldIndex, int newIndex)
        {
            if (oldIndex == -1 || newIndex == -1 || newIndex >= ItemsCount || oldIndex == newIndex)
                return;

            var methodInfo = TargetElement.ItemsSource.GetType().GetMethod("Move");
            if (methodInfo == null) return;
            methodInfo.FastInvoke(TargetElement.ItemsSource, oldIndex, newIndex);
        }

        #endregion

        #region add by Terry


        //private IList _renewedList;

        private IList InnerList
        {
            get
            {
                //if (_renewedList != null)
                //    return _renewedList;
                if (TargetElement != null)
                {
                    if (TargetElement.ItemsSource != null)
                    {

                        return TargetElement.ItemsSource as IList ??
                                     TargetElement.ItemsSource.OfType<IIndexable>()
                                                                               .Where(i => i.Index != -1)
                                                                               .OrderBy(i=> i.Index)
                                                                               .ToList();
                    }

                    return TargetElement.Items;
                }
                return ItemsSource;
            }
        }

        private object InnerSelectedItem
        {
            get
            {
                if (TargetElement != null)
                    return TargetElement.SelectedItem;
                return SelectedItem;
            }
        }

        public virtual bool SelectedItemCanMove()
        {
            return InnerSelectedItem != null && InnerList.Count > 1;
        }

        #region Add New

        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IList), typeof(OrderingItemsButtons),
            new UIPropertyMetadata(null));

        public IIndexable SelectedItem
        {
            get { return (IIndexable)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(IIndexable), typeof(OrderingItemsButtons),
            new UIPropertyMetadata(null));

        private int SelectedIndex
        {
            get
            {
                if (TargetElement != null)
                {
                    if (ReferenceEquals(TargetElement.ItemsSource, InnerList))
                        return TargetElement.SelectedIndex;
                    var index = InnerList.IndexOf(TargetElement.SelectedItem);
                    return index;
                }
               
                return ItemsSource.IndexOf(InnerSelectedItem);
            }
            set
            {
                if (TargetElement != null)
                {
                    if (ReferenceEquals(TargetElement.ItemsSource, InnerList))
                        TargetElement.SelectedIndex = value;
                    else
                    {
                        if (value < InnerList.Count)
                            TargetElement.SelectedItem = InnerList[value];
                    }
                }

                else
                    SelectedItem = InnerList[value] as IIndexable;
            }
        }

        private int ItemsCount
        {
            get
            {
                if (TargetElement != null)
                {
                    if (ReferenceEquals(TargetElement.ItemsSource, InnerList))
                        return TargetElement.Items.Count;
                    else
                    {
                        return InnerList.Count;
                    }
                }
                    
                return ItemsSource.Count;
            }
        }

        #endregion

        private bool IndexIsInValid()
        {
            return InnerSelectedItem.SaftyGetProperty<int, IIndexable>(o => o.Index) == -1;
        }

        public virtual bool CanMoveup()
        {
            if (IndexIsInValid()) return false;

            if (IsIndexEnable)
                return SelectedIndex != 0;
            return SelectedItemCanMove() && !Equals(InnerSelectedItem, InnerList[0]);
        }

        public virtual bool CanMovedown()
        {
            if (IndexIsInValid()) return false;

            if (IsIndexEnable)
                return SelectedIndex != ItemsCount - 1;
            return SelectedItemCanMove() && !Equals(InnerSelectedItem, InnerList[InnerList.Count - 1]);
        }

        private void RegisterCommand()
        {
            if (UpCommand == null)
            {
                UpCommand = new OperationCommand(param => Up(), param => CanMoveup());

                DownCommand = new OperationCommand(param => Down(), param => CanMovedown());
                BottomCommand = new OperationCommand(param => Bottom(), param => CanMovedown());
                TopCommand = new OperationCommand(param => Top(), param => CanMoveup());
            }
        }

        private bool CheckIndexableItem()
        {
            IIndexable indexItem = InnerSelectedItem as IIndexable;
            return indexItem == null || IsIndexEnable;
        }

        public virtual void Top()
        {
            if (CheckIndexableItem())
                Move(SelectedIndex, 0);
            else
            {
                object item = InnerSelectedItem;
                var index = InnerList.IndexOf(item);
                ChangeIndex(MoveAction.Top, index, item);
                InnerList.RemoveAt(index);
                InnerList.Insert(0, item);
                OnChanged(0);
            }
        }

        public virtual void Bottom()
        {
            if (CheckIndexableItem())
                Move(SelectedIndex, ItemsCount - 1);
            else
            {
                object item = InnerSelectedItem;
                var index = InnerList.IndexOf(item);
                ChangeIndex(MoveAction.Bottom, index, item);
                InnerList.RemoveAt(index);
                InnerList.Add(item);
                OnChanged(InnerList.Count - 1);
            }
        }

        public virtual void Up()
        {

            if (CheckIndexableItem())
                Move(SelectedIndex, SelectedIndex - 1);
            else
            {
                object item = InnerSelectedItem;
                var index = InnerList.IndexOf(item);
                ChangeIndex(MoveAction.Up, index, item);
                var upIndex = index > 1 ? index - 1 : 0;
                InnerList.RemoveAt(index);
                InnerList.Insert(upIndex, item);
                OnChanged(upIndex);
            }

        }

        public virtual void Down()
        {
            if (CheckIndexableItem())
                Move(SelectedIndex, SelectedIndex + 1);
            else
            {
                object item = InnerSelectedItem;
                var index = InnerList.IndexOf(item);
                ChangeIndex(MoveAction.Down, index, item);

                InnerList.RemoveAt(index);
                var upIndex = index >= InnerList.Count ? InnerList.Count : index + 1;
                InnerList.Insert(upIndex, item);
                OnChanged(upIndex);
            }
        }

        protected void OnChanged(int index)
        {
            if (TargetElement == null) return;
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                                        (Action)delegate()
                                                     {
                                                         (TargetElement.ItemContainerGenerator.ContainerFromIndex(index)
                                                          as FrameworkElement).Focus();
                                                         (TargetElement as ListBox).ScrollIntoView(
                                                             TargetElement.SelectedItem);
                                                     });
            if (TargetElement.ItemsSource is ICollectionView)
            {
                ((ICollectionView)TargetElement.ItemsSource).Refresh();
            }
            SelectedIndex = index;
        }

        public enum MoveAction
        {
            Top,
            Up,
            Down,
            Bottom
        }

        private IIndexable GetItem(int index)
        {
            return InnerList[index] as IIndexable;
        }

        protected void ChangeIndex(MoveAction moveAction, int Index, object item)
        {
            var currentItem = item as IIndexable;
            if (currentItem == null) return;
            switch (moveAction)
            {
                case MoveAction.Top:
                    IIndexable topItem = GetItem(0);
                    int topIndex = topItem.Index;
                    List<int> tempArray = new List<int>();
                    foreach (var tempItem in InnerList)
                    {
                        tempArray.Add((tempItem as IIndexable).Index);
                    }
                    for (int i = 0; i < Index; i++)
                    {
                        IIndexable moveitem = InnerList[i] as IIndexable;
                        moveitem.Index = tempArray[i + 1];
                    }
                    currentItem.Index = topIndex;
                    break;
                case MoveAction.Up:
                    if (Index == 0) return;
                    IIndexable prevItem = GetItem(Index - 1);
                    int prevIndex = prevItem.Index;
                    prevItem.Index = currentItem.Index;
                    currentItem.Index = prevIndex;
                    break;
                case MoveAction.Down:
                    IIndexable nextItem = GetItem(Index + 1);
                    int nextIndex = nextItem.Index;
                    nextItem.Index = currentItem.Index;
                    currentItem.Index = nextIndex;
                    break;
                case MoveAction.Bottom:
                    IIndexable bottomItem = GetItem(InnerList.Count - 1);
                    int bottomIndex = bottomItem.Index;

                    for (int i = InnerList.Count - 1; i > Index; i--)
                    {
                        IIndexable moveitem = InnerList[i] as IIndexable;
                        moveitem.Index = (InnerList[i - 1] as IIndexable).Index;
                    }
                    currentItem.Index = bottomIndex;
                    break;
                default:
                    break;
            }
            RefreshDefaultView();
            if (TargetElement != null)
                TargetElement.SelectedItem = null;
        }

        private void RefreshDefaultView()
        {
            object source = TargetElement != null ? TargetElement.ItemsSource : ItemsSource;
            CollectionViewSource.GetDefaultView(source).Refresh();
        }

        #region Command

        public ICommand UpCommand
        {
            get { return (ICommand)GetValue(UpCommandProperty); }
            set { SetValue(UpCommandProperty, value); }
        }

        public static readonly DependencyProperty UpCommandProperty =
            DependencyProperty.Register("UpCommand", typeof(ICommand), typeof(OrderingItemsButtons),
            new UIPropertyMetadata(null));

        public ICommand DownCommand
        {
            get { return (ICommand)GetValue(DownCommandProperty); }
            set { SetValue(DownCommandProperty, value); }
        }

        public static readonly DependencyProperty DownCommandProperty =
            DependencyProperty.Register("DownCommand", typeof(ICommand), typeof(OrderingItemsButtons),
            new UIPropertyMetadata(null));

        public ICommand BottomCommand
        {
            get { return (ICommand)GetValue(BottomCommandProperty); }
            set { SetValue(BottomCommandProperty, value); }
        }

        public static readonly DependencyProperty BottomCommandProperty =
            DependencyProperty.Register("BottomCommand", typeof(ICommand), typeof(OrderingItemsButtons),
            new UIPropertyMetadata(null));


        public ICommand TopCommand
        {
            get { return (ICommand)GetValue(TopCommandProperty); }
            set { SetValue(TopCommandProperty, value); }
        }

        public static readonly DependencyProperty TopCommandProperty =
            DependencyProperty.Register("TopCommand", typeof(ICommand), typeof(OrderingItemsButtons),
            new UIPropertyMetadata(null));

        #endregion

        #endregion

        void OrderingItemsButtons_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Dispose();
            this.Unloaded -= OrderingItemsButtons_Unloaded;
        }

        #region IDisposable Members

        public void Dispose()
        {

            BindingOperations.ClearBinding(MoveTopButton, Button.CommandProperty);
            BindingOperations.ClearBinding(MoveUpButton, Button.CommandProperty);
            BindingOperations.ClearBinding(MoveDownButton, Button.CommandProperty);
            BindingOperations.ClearBinding(MoveToLastButton, Button.CommandProperty);
            TopCommand = null;
            UpCommand = null;
            DownCommand = null;
            BottomCommand = null;
            //this.ClearValue(OrderingItemsButtons.TargetElementProperty);
            GC.SuppressFinalize(this);
            //_list.Clear();

            isReady = false;

        }

        #endregion
    }


}
