using System;
using System.Windows;
using System.Windows.Media;
using Luna.WPF.ApplicationFramework.Extensions;
using Luna.WPF.ApplicationFramework.Graphics;

namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    public class ScaleSelectedItemLayerDraw : ElementDraw<ScaleSelectedItemLayer>
    {
        private Pen _pen;

        public override bool CanDraw()
        {
            //if (!Element.FlagExist(MouseState.MouseDown) || Element.PointOutBlock == null) return false;

            //if (!Element.FlagExist(MouseState.LeftDirection) && !Element.FlagExist(MouseState.RightDirection))
            //    return false;

            //if (Element.Visibility != Visibility.Visible) return false;
            return Element.IsMouseDragging();
        }

        protected override void InternalDraw(DrawingContext dc)
        {
            if (Element.InvalidPlacement) return;

            if (_pen == null)
                _pen = Element.GetDashPen(Brushes.Gray);

            var axisXConverter = Element.AxisXConverter;
            var blockConverter = Element.BlockConverter;
            var selectedBlock = Element.PointOutBlock;

            var top = Element.GetPointOutY();
            var blockHeight = blockConverter.GetHeight(selectedBlock) - 0.5; // 1 = border thickness;
            var left = axisXConverter.DataToScreen(Element.DropedPlacement.Start);
            var right = axisXConverter.DataToScreen(Element.DropedPlacement.End);


            var rect = new Rect(left < right ? left : right, top, Math.Abs(right - left), blockHeight);


            dc.DrawGuidelineRect(null, _pen, rect);
        }
    }
}