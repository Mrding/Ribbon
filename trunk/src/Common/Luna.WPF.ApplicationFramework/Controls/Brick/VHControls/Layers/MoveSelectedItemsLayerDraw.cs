using System.Windows;
using System.Windows.Media;
using Luna.WPF.ApplicationFramework.Extensions;
using Luna.WPF.ApplicationFramework.Graphics;

namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    public class MoveSelectedItemsLayerDraw : ElementDraw<MoveSelectedItemsLayer>
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
            var axisXConverter = Element.AxisXConverter;
            var blockConverter = Element.BlockConverter;

            if (_pen == null)
                _pen = Element.GetDashPen(Brushes.Gray);

            if (!Element.AxisYConverter.IsInViewRagne(Element.PointOutDataRowIndex))
                return;

            var height = blockConverter.GetHeight(Element.PointOutBlock) - 0.5; // 0.5 = 1 border thickness / 2 ;

            var blockLeft = axisXConverter.DataToScreen(Element.DropedPlacement.Start);
            var blockRight = axisXConverter.DataToScreen(Element.DropedPlacement.End);

            dc.DrawGuidelineRect(null, _pen, new Rect(blockLeft, Element.GetPointOutY(), blockRight - blockLeft, height));

            base.InternalDraw(dc);
        }
    }
}
