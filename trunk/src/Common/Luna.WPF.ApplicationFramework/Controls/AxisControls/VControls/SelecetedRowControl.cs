namespace Luna.WPF.ApplicationFramework.Controls
{
    using System.Windows;
    using System.Windows.Media;

    public class SelectedRowControl : AxisControl
    {
        static SelectedRowControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SelectedRowControl), new FrameworkPropertyMetadata(typeof(SelectedRowControl)));
        }

        #region Properties

        public override void Initialize()
        {
            ElementDraws.Add(new SelectedRowControlDraw());
            base.Initialize();
        }

        protected override void AddToPanel(IAxisPanel axisPanel)
        {
            axisPanel.Add(this);
        }

        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            return new RectangleGeometry(new Rect(RenderSize));
        }

    
      
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush),typeof(SelectedRowControl), 
            new UIPropertyMetadata(Brushes.LightYellow));

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.Register("RowHeight", typeof(double),typeof(SelectedRowControl), 
            new UIPropertyMetadata(0d));

        public double RowHeight
        {
            get { return (double)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }

        public int SelectedRowIndex
        {
            get { return (int)GetValue(SelectedRowIndexProperty); }
            set { SetValue(SelectedRowIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedRowIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedRowIndexProperty =
            DependencyProperty.Register("SelectedRowIndex", typeof(int), typeof(SelectedRowControl), 
            new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        #endregion Properties
    }
}
