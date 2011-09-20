using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Luna.Common;
namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    public class SelectedBlocksLayer : BlockGridLayerBase
    {
        private int _selectedBlockDataRowIndex;

        public Brush SelectedBrush
        {
            get { return (Brush)GetValue(SelectedBrushProperty); }
            set { SetValue(SelectedBrushProperty, value); }
        }

        public static readonly DependencyProperty SelectedBrushProperty =
            DependencyProperty.Register("SelectedBrush", typeof(Brush), typeof(SelectedBlocksLayer),
            new UIPropertyMetadata());

        public Pen SelectedBorder
        {
            get { return (Pen)GetValue(SelectedBorderProperty); }
            set { SetValue(SelectedBorderProperty, value); }
        }

        public static readonly DependencyProperty SelectedBorderProperty =
            DependencyProperty.Register("SelectedBorder", typeof(Pen), typeof(SelectedBlocksLayer),
            new UIPropertyMetadata(new Pen(Brushes.Black, 1)));

        public ITerm SelectedBlock
        {
            get { return (ITerm)GetValue(SelectedBlockProperty); }
            set { SetValue(SelectedBlockProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedBlock.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedBlockProperty =
            DependencyProperty.Register("SelectedBlock", typeof(ITerm), typeof(SelectedBlocksLayer),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d,e)=>
            {
                (d as SelectedBlocksLayer).InvalidateVisual();                                                                                          
            }));

        //internal override void ItemsSourceChanging()
        //{
        //    SelectedBlock = null;
        //}

        protected override void OnParentMouseDown(object sender, MouseButtonEventArgs e)
        {
            _selectedBlockDataRowIndex = PointOutDataRowIndex;
            SelectedBlock = PointOutBlock;
        }

        protected override void OnParentMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Released)
                InvalidateVisual();
        }

        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            return new RectangleGeometry(new Rect(RenderSize));
        }

        public bool SelectedBlockInvisible(out double viewportTop)
        {
            return LayerContainer.DataRowIndexIsOutOfScreen(_selectedBlockDataRowIndex, out viewportTop);
        }

        public override void Initialize()
        {
            ElementDraws.Add(new SelectedBlocksLayerDraw());
            base.Initialize();
        }

        protected override void AddToPanel(IAxisPanel axisPanel)
        {
            axisPanel.Add(this);
        }
    }
}
