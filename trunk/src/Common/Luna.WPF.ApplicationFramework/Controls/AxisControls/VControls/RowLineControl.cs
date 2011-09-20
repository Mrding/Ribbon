namespace Luna.WPF.ApplicationFramework.Controls
{
    using System.Windows;
    using System.Windows.Media;
    using System;

    /// <summary>
    /// 画网格的控件
    /// </summary>
    public class RowLineControl : AxisControl
    {
        static RowLineControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RowLineControl), new FrameworkPropertyMetadata(typeof(RowLineControl)));
        }

        public override void Initialize()
        {
            ElementDraws.Add(new RowLinesControlDraw());
            base.Initialize();
        }

        protected override void AddToPanel(IAxisPanel axisPanel)
        {
             axisPanel.Add(this);
        }

        public static readonly DependencyProperty LinePenProperty =
         DependencyProperty.Register("LinePen", typeof(Pen),
         typeof(RowLineControl), new UIPropertyMetadata(new Pen(Brushes.LightGray, 1)));

        public Pen LinePen
        {
            get { return (Pen)GetValue(LinePenProperty); }
            set { SetValue(LinePenProperty, value); }
        }

        public double RowHeight
        {
            get { return (double)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }

        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.Register("RowHeight", typeof(double),
            typeof(RowLineControl), new FrameworkPropertyMetadata(25.0));

        public int RowCount
        {
            get { return (int)GetValue(RowCountProperty); }
            set { SetValue(RowCountProperty, value); }
        }

        public static readonly DependencyProperty RowCountProperty =
           DependencyProperty.Register("RowCount", typeof(int), typeof(RowLineControl), new FrameworkPropertyMetadata(0,
               FrameworkPropertyMetadataOptions.AffectsRender));
    }
}
