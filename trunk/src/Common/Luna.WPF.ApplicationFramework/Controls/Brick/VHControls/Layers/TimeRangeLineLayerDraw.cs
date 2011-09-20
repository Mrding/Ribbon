using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Luna.WPF.ApplicationFramework.Graphics;
namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    public class TimeRangeLineLayerDraw : ElementDraw<BlockGridLayerBase>
    {
        private Pen _pen;

        public override bool CanDraw()
        {
            if (!Element.IsMouseDragging() || Element.Visibility != Visibility.Visible)
                return false;

            return base.CanDraw();
        }

        protected override void InternalDraw(DrawingContext dc)
        {
            if (_pen == null)
                _pen = Element.GetDashPen(Brushes.Gray);

            var axisXConverter = Element.AxisXConverter;

            var blockLeft = axisXConverter.DataToScreen(Element.DropedPlacement.Start);
            var blockRight = axisXConverter.DataToScreen(Element.DropedPlacement.End);

            //if(0 < blockLeft)
                dc.DrawGuideLineLine(_pen, new Point(blockLeft, 0), new Point(blockLeft, Element.RenderSize.Height));
            dc.DrawGuideLineLine(_pen, new Point(blockRight, 0), new Point(blockRight, Element.RenderSize.Height));

            base.InternalDraw(dc);
        }
    }
}
