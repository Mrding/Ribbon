using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using Luna.WPF.ApplicationFramework.Graphics;

namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    public class DragItemLayerDraw : ElementDraw<MoveDargItemsLayer>
    {
        public override bool CanDraw()
        {
            if (Element.IsDragLeave() || !Element.IsMouseDragging() || Element.Visibility != Visibility.Visible)
            {
                return false;
            }
                
            return base.CanDraw();
        }

        protected override void InternalDraw(DrawingContext dc)
        {
            if (Element.PointOutBlock == null) return;


            var axisXConverter = Element.AxisXConverter;
            var blockConverter = Element.BlockConverter;

            

            var height = blockConverter.GetHeight(Element.PointOutBlock) - 0.5; // 0.5 = 1 border thickness / 2 ;

            var blockLeft = axisXConverter.DataToScreen(Element.DropedPlacement.Start);
            var blockRight = axisXConverter.DataToScreen(Element.DropedPlacement.End);

            var rect = new Rect(blockLeft, Element.GetPointOutY(), blockRight - blockLeft, height);

            //Debug.Print(rect.ToString());
            dc.DrawRectangle(Element.BlockConverter.GetBackground(Element.PointOutBlock), null, rect);

        }
    }
}