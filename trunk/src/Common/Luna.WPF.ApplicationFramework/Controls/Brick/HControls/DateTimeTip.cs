namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using Luna.WPF.ApplicationFramework.Extensions;

    [Localizability(LocalizationCategory.None),
    DefaultProperty("Content"),
    ContentProperty("Content")]
    public class DateTimeTip : AxisControl
    {
        #region Fields

        private Popup _popup;

        #endregion Fields

        #region Constructors

        public DateTimeTip()
        {
            _popup = new Popup
                         {
                             AllowsTransparency = true,
                             PopupAnimation = PopupAnimation.Fade,
                             Placement = PlacementMode.Bottom,
                             PlacementTarget = this,
                             DataContext = this
                         };

            //this.Loaded += new RoutedEventHandler(DateTimeTip_Loaded);
            SizeChanged += OnSizeChanged;
            //Dispatcher.BeginInvoke(() =>
            //{
            //    var tabContainer = this.FindAncestor<TabbedMdiContainer>();
            //    if (tabContainer != null)
            //        tabContainer.SelectedWindow.Unloaded += SelectedWindow_Unloaded;
            //}, DispatcherPriority.Loaded);
        }

        private void OnSizeChanged(object s, SizeChangedEventArgs e)
        {
            _popup.PlacementRectangle = new Rect(e.NewSize);
        }

        public override void Initialize()
        {
            this.InvalidateVisual();
            base.Initialize();
        }

        protected override void AddToPanel(IAxisPanel axisPanel)
        {
            
        }

        //void DateTimeTip_Loaded(object sender, RoutedEventArgs e)
        //{
        //    
        //    this.Loaded -= DateTimeTip_Loaded;
        //}

        #endregion Constructors

        #region Properties

        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(DateTimeTip),
            new UIPropertyMetadata(0.0));

        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(DateTimeTip),
            new UIPropertyMetadata(15.0));

        public DateTime SelectedTime
        {
            get { return (DateTime)GetValue(SelectedTimeProperty); }
            set { SetValue(SelectedTimeProperty, value); }
        }

        public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.Register("SelectedTime", typeof(DateTime), typeof(DateTimeTip),
            new UIPropertyMetadata(default(DateTime)));

        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(DateTimeTip),
            new UIPropertyMetadata((d, e) =>
                {
                    var control = (DateTimeTip)d;
                    var content = e.NewValue as UIElement;
                    if (content != null)
                        control._popup.Child = content;
                }));

        public DateTime WatchTime
        {
            get { return (DateTime)GetValue(WatchTimeProperty); }
            private set { SetValue(WatchTimePropertyKey, value); }
        }

        public static readonly DependencyPropertyKey WatchTimePropertyKey =
            DependencyProperty.RegisterReadOnly("WatchTime", typeof(DateTime), typeof(DateTimeTip),
            new UIPropertyMetadata(default(DateTime)));

        public static readonly DependencyProperty WatchTimeProperty = WatchTimePropertyKey.DependencyProperty;

        #endregion Properties

        #region Methods

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if(_popup == null) return;

            _popup.IsOpen = true;
            var point = e.GetPosition(this);
            _popup.HorizontalOffset = point.X + HorizontalOffset;
            _popup.VerticalOffset = VerticalOffset;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (_popup == null) return;

            _popup.IsOpen = false;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (IsEnabled)
                SelectedTime = WatchTime;
            else
                e.Handled = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if(AxisPanel == null) return;
         

            var point = e.GetPosition(this);
            WatchTime = AxisXConverter.ScreenToData(point.X);

            if (_popup == null) return;
            _popup.HorizontalOffset = point.X + HorizontalOffset;
            _popup.VerticalOffset = point.Y - RenderSize.Height + VerticalOffset;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (!Caliburn.PresentationFramework.PresentationFrameworkModule.IsInDesignMode && IsLoaded)
                drawingContext.DrawRectangle(Brushes.Transparent, null, new Rect(this.DesiredSize));
        }

        //void SelectedWindow_Unloaded(object sender, RoutedEventArgs e)
        //{
        //    //var tabContainer = sender as FrameworkElement;
        //    //tabContainer.Unloaded -= SelectedWindow_Unloaded;


        //}

        protected override void OnDispose()
        {
            SizeChanged -= OnSizeChanged;
            _popup.ClearAllLocalValue();
            this.ClearValue(ContentProperty);
            _popup = null;
            base.OnDispose();
        }

        #endregion Methods
    }
}