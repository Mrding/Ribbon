using System.Windows.Media;
using Luna.WPF.ApplicationFramework.Graphics;
using System.Windows;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public class SelectedRowControlDraw : ElementDraw<SelectedRowControl>
    {
        protected override void InternalDraw(DrawingContext drawingContext)
        {
            // var axisConverter = Element.AxisConverter;
            //画一个透明矩形，这样可以让HeadListBox空白区域可以点击
            //drawingContext.DrawRectangle(Brushes.Transparent, null, new Rect(Element.RenderSize));

            //double halfPenWidth = pen.Thickness / 2;

            
            //int endIndex = axisConverter.ScreenToData(Element.RenderSize.Height);
            //var displayCount = RenderSize.Height / RowHeight;
            // if (Element.SelectedIndex < startIndex || endIndex < Element.SelectedIndex)
            // return;

            var screenTopRowIndex = Element.AxisPanel.GetScreenTopRowIndex();

            if(screenTopRowIndex < 0 ) 
                return;

            var selectedRowIndex = Element.SelectedRowIndex - screenTopRowIndex;

            if (Element.AxisYConverter.IsInViewRagne(selectedRowIndex))
                drawingContext.DrawRectangle(Element.Fill, null, new Rect(0, (selectedRowIndex) * Element.RowHeight, Element.RenderSize.Width, Element.RowHeight));
            
            
        }
    }
}
