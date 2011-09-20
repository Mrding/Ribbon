using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Luna.WPF.ApplicationFramework.Graphics;
using System.Windows.Media;
using System.Windows;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public class AxisControlDesignDraw : DesignElementDraw<AxisControl>
    {
        protected override void InternalDraw(DrawingContext dc)
        {
            var text = string.Format("{0}:{1}", Element.GetType().Name, Element.Name);
            var textFormat = new FormattedText(
            text, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
            new Typeface("Courier New"),12,Brushes.Black) {TextAlignment = TextAlignment.Center};

            textFormat.SetFontWeight(FontWeights.Bold);

            dc.DrawText(textFormat,
                new Point(Element.Width / 2,
                    Element.Height / 2 - textFormat.Height / 2));
        }
    }
}
