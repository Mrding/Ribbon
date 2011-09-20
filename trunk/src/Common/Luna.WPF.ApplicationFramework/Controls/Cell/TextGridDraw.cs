using System.Windows;
using System.Windows.Media;
using Luna.Common;
using Luna.WPF.ApplicationFramework.Extensions;

namespace Luna.WPF.ApplicationFramework.Controls.Cell
{
    public class TextGridDraw : CellBlockGridDraw
    {
        protected override void RowRender(int index, double top, DrawingContext dc)
        {
            foreach (var item in Element.GetItems(index, GetViewRange()))
            {
                var block = item as ITerm;

                if (block == null)
                    continue;

                if (OutOfVisualRange(block))
                    continue;

                var rect = block.Start.Date.ToRect(Element.AxisXConverter, top, Element.BlockConverter.GetHeight(block));

                if (Element.BlockConverter.IsDirty(block))
                    dc.DrawRectangle(Background, null, rect.SetMargin(new Thickness(0, 0, 0, 1)));

                dc.DrawCenterText(Element.BlockConverter.GetContentText(block), rect, Element.BlockConverter.GetForeground(block));
            }

            //var itemsSource = Element.ItemsSource;
            //var axisXConverter = Element.AxisXConverter;
            //var blockConverter = Element.BlockConverter;
            //var screenStart = axisXConverter.ScreenToData(0);
            //var screenEnd = axisXConverter.ScreenToData(Element.RenderSize.Width);

            //var terms = itemsSource[index];
            //if (terms == null) return;

            //foreach (var item in terms)
            //{
            //    var block = item as ITerm;
            //    if (block == null)
            //        continue;
            //    var blockStart = block.Start;
            //    var blockEnd = block.End;

            //    if (blockStart > screenEnd || blockEnd < screenStart)
            //        continue;

            //    var blockLeft = axisXConverter.DataToScreen(blockStart);
            //    var blockRight = axisXConverter.DataToScreen(blockEnd);
            //    var blockHeight = Element.Interval;
            //    var blockTop = 0;

            //    //new Pen(Brushes.Black,1)
            //    var rect = new Rect(blockLeft, blockTop, blockRight - blockLeft, blockHeight);
            //    context.DrawRectangle(blockConverter.GetBackground(block), null, rect);
            //    //班名
            //    var text = blockConverter.GetContentText(block);
            //DrawCenterText(text, "/", blockConverter.FontSize, rect, Brushes.Black, context);
            //}
        }

        private void DrawText(string text, string seperator, Rect rect, Brush foreground, DrawingContext dc)
        {
            //var texts = text.Split(new[] { seperator[0] });

            //    var leftText = CreatText(texts[0], fontSize, foreground);

            //    var formatText = CreatText(text, fontSize, foreground);
            //    var x = rect.Width / 2 + rect.X - leftText.Width;
            //    var y = rect.Y + (rect.Height - fontSize) / 2;
            //    //rect = new Rect(x, rect.Y, rect.Width, rect.Height);
            //    dc.DrawText(formatText, new Point(x, y));

        }
    }
}
