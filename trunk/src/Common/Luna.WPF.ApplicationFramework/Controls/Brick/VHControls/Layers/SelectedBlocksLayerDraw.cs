using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Luna.WPF.ApplicationFramework.Graphics;
using Luna.WPF.ApplicationFramework.Extensions;
namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    public class SelectedBlocksLayerDraw : ElementDraw<SelectedBlocksLayer>
    {
        public override bool CanDraw()
        {
            //stop drawing when alter block operation..
            //verify moveoffset
            if (Element.SelectedBlock == null || Element.Visibility != Visibility.Visible)
                return false;
            return base.CanDraw();
        }

        protected override void InternalDraw(DrawingContext dc)
        {
            // y
            double viewportTop;
            if (Element.SelectedBlockInvisible(out viewportTop))
                return;

            // size 
            var blockConverter = Element.BlockConverter;
            var thickness = Element.SelectedBorder.Thickness / 2;
            var height = blockConverter.GetHeight(Element.SelectedBlock) - thickness;

             // x
            var top = blockConverter.GetTop(Element.SelectedBlock);

            var rect = Element.SelectedBlock.ToRect(Element.AxisXConverter, top, height);

            dc.PushTransform(new TranslateTransform(0, viewportTop));

            dc.DrawGuidelineRect(Element.SelectedBrush, Element.SelectedBorder,rect);

            dc.Pop();

            base.InternalDraw(dc);
        }
    }
}
