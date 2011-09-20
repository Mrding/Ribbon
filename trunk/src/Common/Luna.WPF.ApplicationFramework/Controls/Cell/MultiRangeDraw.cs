using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Luna.Common;
using Luna.Common.Domain;
using Luna.Common.Interfaces;
using Luna.Core;
using Luna.Core.Extensions;
using Luna.WPF.ApplicationFramework.Extensions;
using Luna.WPF.ApplicationFramework.Graphics;

namespace Luna.WPF.ApplicationFramework.Controls.Cell
{
    public class MultiRangeDraw : BaseBlockGridDraw
    {
        //private Brush Background;

        //public MultiRangeDraw()
        //{
        //    _brush = new SolidColorBrush("#FFFFC0CB".ToColor()) { Opacity = 0.4 };
        //    _brush.Freeze();
        //}

        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Background.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(MultiRangeDraw), new UIPropertyMetadata(Brushes.Transparent));



        //public override bool CanDraw()
        //{
        //    return Element.Items != null && 0 < Element.Items.Count;
        //}

        /*protected override void InternalDraw(DrawingContext dc)
        {
            var topRowIndex = Element.AxisPanel.GetScreenTopRowIndex();

            foreach (var tuple in Element.Items)
            {
                var rect = tuple.Item2.Date.ToRect(Element.AxisXConverter,
                                                         0 + (tuple.Item1 - topRowIndex) * Element.Interval,
                                                         Element.Interval);
                dc.DrawRectangle(Background, null, rect.SetMargin(new Thickness(0, 0, 0, 1)));
                
                dc.DrawCenterText(tuple.Item2.Text, rect, Brushes.Red);
            }

            #region one range area
            //var dataRowRange = Element.DataRowRange;
            //var timeRange = Element.TimeRange;
            //if (timeRange == null || dataRowRange[0] > dataRowRange[1])
            //    return;

            //var topRowIndex = Element.AxisPanel.GetScreenTopRowIndex();

            //var top = (dataRowRange[0] - topRowIndex) * Element.Interval;
            //var height = (dataRowRange[1] - dataRowRange[0] + 1) * Element.Interval;
            //var left = Element.AxisXConverter.DataToScreen(timeRange.Start.Date);
            //var width = Element.AxisXConverter.DataToScreen(timeRange.End.Date) - left;

            //var yOffset = Element.AxisPanel.VerticalOffset;
            //var xOffset = Element.AxisPanel.HorizontalOffset;

            //var rect = new Rect(left + xOffset, top + yOffset, width, height);

            //IEnumerable<Rect> rects;
            //Element.MarkIsDirty(rect, out rects);

            //foreach (var r in rects)
            //{
            //    var relativeRect = new Rect(r.X - xOffset, r.Y - yOffset, r.Width, r.Height);
            //    dc.DrawRectangle(_brush, null, relativeRect.SetMargin(new Thickness(0, 0, 0, 1)));
            //}
            #endregion

            base.InternalDraw(dc);
        }*/

        protected override void RowRender(int index, DrawingContext dc)
        {
           

            //foreach (var item in Element.ItemsSource[index])
            //{
            //    var block = item as ITerm;

            //    if (block == null)
            //        continue;

            //    if (OutOfVisualRange(block)) continue;
            //    //注意 HrDate

            //    var rect = Element.BlockConverter.GetStart(block).ToRect(Element.AxisXConverter, 0, Element.BlockConverter.GetHeight(block));

            //    dc.DrawCenterText(Element.BlockConverter.GetContentText(block), rect, Element.BlockConverter.GetForeground(block));
            //}
        }
    }
}