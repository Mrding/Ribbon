using System;
using System.Collections;
using System.Windows.Media;
using Luna.Common;
using Luna.Common.Domain;
using Luna.Core.Extensions;
using Luna.WPF.ApplicationFramework.Graphics;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public abstract class BaseBlockGridDraw : ElementDraw<BlockGrid>
    {

        private DateRange _fetchRange;
        
        private int _itemsSourceCount;


        public override bool CanDraw()
        {
            _itemsSourceCount = Element.GetItemsSourceCount();
            return 0 < _itemsSourceCount;
        }

        protected virtual int GetTopRowIndex()
        {
            return Math.Min(_itemsSourceCount - 1, Element.AxisPanel.GetScreenTopRowIndex());
        }

        protected abstract void RowRender(int index, double top , DrawingContext drawingContext);

        protected override void InternalDraw(DrawingContext dc)
        {
            Element.AxisPanel.DataRangeX.ViewMax.Self(d =>
            {
                _fetchRange = new DateRange(Element.AxisPanel.DataRangeX.ViewMin, 0 < d.TimeOfDay.TotalHours ? d.Date.AddDays(1) : d);
            });

            if (_fetchRange.InValid)
                return;

            //UI可以显示的笔数

            var desiredDisplayCount = (int)(Element.RenderSize.Height / Element.Interval + 0.9); // 当最后一笔在画面不足一个 interval 也需要画, 采用无条件进入

            var topRowIndex = GetTopRowIndex();

            //循环出当前UI可视笔数, 需要当前UI第一位置的dataRowIndex
            //i只是做循环次数计数

            for (var i = 0; i < desiredDisplayCount  && 0 <= topRowIndex ; i++, topRowIndex++)
            {
                if (_itemsSourceCount <= topRowIndex)
                    break;

                var y = i * Element.Interval;
                //dc.PushTransform(new TranslateTransform(0, y));
                RowRender(topRowIndex, y , dc);
                //dc.Pop();
            }
        }

        protected ITerm GetViewRange()
        {
            return _fetchRange;
        }

        protected bool OutOfVisualRange(ITerm term)
        {
            return _fetchRange.End <= term.Start || term.End <= _fetchRange.Start;
        }
    }
}
