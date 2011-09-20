using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Luna.Common.Domain;
using Luna.Core;
using Luna.WPF.ApplicationFramework.Controls.Cell;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public class BlockGrid : BlockGridLayerBase
    {
        protected override void AddToPanel(IAxisPanel axisPanel)
        {
            axisPanel.Add(this);
        }

        //internal int GetScreenTopRowIndex()
        //{
        //    int index = AxisYConverter.ScreenToData(0);
        //    // 当 Container 有不同的 ItemsSource 会发生 Scrren(x,0)Index 超出问题
        //    // Statistics2(header) & AssignmentTypes(content) both in headerContentPresenter (in ShiftComposerView)
        //    if (GetItemsSourceCount() <= index)
        //        return 0;
        //    return index;
        //}

        public IList<Tuple<int, DateTerm>> Items
        {
            get { return (IList<Tuple<int, DateTerm>>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DirtyItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(IList<Tuple<int, DateTerm>>), typeof(BlockGrid), new UIPropertyMetadata(null));
        

        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            return
                new RectangleGeometry(new Rect(new Point(0, -Interval),
                                               new Size(RenderSize.Width, RenderSize.Height + Interval)));
        }
    }
}