using System;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Luna.Common;
using Luna.Common.Extensions;
using Luna.Common.Interfaces;
using Luna.Core.Extensions;
using Luna.WPF.ApplicationFramework.Extensions;
using Luna.WPF.ApplicationFramework.Graphics;
using System.Collections.Generic;

namespace Luna.WPF.ApplicationFramework.Controls.Cell
{
    public class RangeSelectedDraw : ElementDraw<RangeSelected>
    {
        private readonly Pen _pen;
        private Rect[] _ranges;
        private int _topRowIndex;

        public RangeSelectedDraw()
        {
            _pen = new Pen(Brushes.Gray, 2);
        }

        public string RangeText
        {
            get { return (string)GetValue(RangeTextProperty); }
            set { SetValue(RangeTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeTextProperty =
            DependencyProperty.Register("RangeText", typeof(string), typeof(RangeSelectedDraw), new UIPropertyMetadata("0 x 0"));


        protected override void InternalDraw(DrawingContext dc)
        {
            var dataRowRange = Element.DataRowRange;

            object firstDrawBlock = default(ITerm);

            _ranges = new Rect[2];

            _topRowIndex = Element.AxisPanel.GetScreenTopRowIndex();

            if (_topRowIndex < 0) return;

            if (dataRowRange[0] <= dataRowRange[1] &&  0 <= _topRowIndex)
            {
                var timeRange = new TimeRange(Element.TimeRange.Start.Date, Element.TimeRange.End.Date);

                var x = Element.AxisXConverter.DataToScreen(timeRange.Start);
                var y = (dataRowRange[0] - _topRowIndex) * Element.Interval;

                var width = Element.AxisXConverter.DataToScreen(timeRange.End) - x;
                var height = ((dataRowRange[1] + 1 - _topRowIndex) * Element.Interval) - y;

                var rect = new Rect(x, y, width, height);


                dc.DrawRectangle(Element.Fill, null, rect.SetMargin(new Thickness(0, 0, 0, 1)));




                _ranges = new[] { new Rect(x, y, 72, Element.Interval), new Rect(x + height, y + height, 72, Element.Interval) };

                Point point;

                if (Element.IsInViewRange(out point))
                {
                    //dc.DrawGuidelineRect(null, _pen, new Rect(point, new Size(72, Element.Interval-1)).SetMargin(new Thickness(0.5,1, 0, 0)));
                    dc.DrawRectangle(Brushes.LightSkyBlue, null, new Rect(point, new Size(72, Element.Interval)).SetMargin(new Thickness(1, 0, 0, 0.5)));
                }
                else
                {
                    Element.NotInViewRange();
                    return;
                }

            }
            // 必须读取实体类
            firstDrawBlock = GetItemsSourceCell(Element.InitalDrawLocation); // new TimeRange(timeRange.Start, timeRange.Start.AddDays(1));
            Element.DrawComplete(_ranges, firstDrawBlock);
        }

        private object GetItemsSourceCell(Rect? relativeInitalDrawLocation)
        {
            object found = null;

            if (relativeInitalDrawLocation != null)
            {
                var rowIndex = (int)(relativeInitalDrawLocation.Value.Y / Element.Interval);
                var clickDate = Element.AxisXConverter.ToData(relativeInitalDrawLocation.Value.X);

                if (rowIndex < Element.ItemsSource.Count)
                {
                    Element.ItemsSource[rowIndex].SaftyInvoke<IIntIndexer>(o =>
                    {
                        found = o.GetItem(clickDate.IndexOf(Element.AxisPanel.DataRangeX.Min));
                    });    
                }
            }

            //foreach (var block in Element.GetItems(rowIndex, new TimeRange(clickDate,clickDate)))
            //{
            //    var dateKey = Element.BlockConverter.GetStart(block);
            //    if (dateKey == clickDate) // 一天一班判断
            //    {
            //        found = block;
            //        break;
            //    }
            //}
            return found;
        }
    }
}