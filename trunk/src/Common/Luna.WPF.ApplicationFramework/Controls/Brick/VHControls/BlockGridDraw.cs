using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Luna.Common;
using Luna.Core.Extensions;
using Luna.WPF.ApplicationFramework.Extensions;

namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    public class BlockGridDraw : BaseBlockGridDraw
    {
        private static IDictionary<string, SolidColorBrush> _foregroundCards = new Dictionary<string, SolidColorBrush>
        {
            
            {"BurlyWood",Brushes.White},
            {"CadetBlue",Brushes.White},
            {"Chocolate",Brushes.White},
            {"CornflowerBlue",Brushes.White},
            {"DarkGray",Brushes.White},
            {"DarkKhaki",Brushes.Black},
            {"DarkOrange",Brushes.White},
            {"DarkSalmon",Brushes.White},
            {"DarkSeaGreen",Brushes.White},
            {"LightPink",Brushes.Black},
            {"LightSalmon",Brushes.Black},
            {"LightSkyBlue",Brushes.White},
            {"MediumPurple",Brushes.White},
            {"Orchid",Brushes.FloralWhite},
            {"YellowGreen",Brushes.White}
        };

        protected override void RowRender(int index, double top, DrawingContext drawingContext)
        {
            foreach (ITerm block in Element.GetItems(index, GetViewRange()))
            {
                var height = Element.BlockConverter.GetHeight(block);

                if (height <= 0 || OutOfVisualRange(block)) continue;

                var rect = block.ToRect(Element.AxisXConverter, Element.BlockConverter.GetTop(block) + top, height);


                var bg = block.SaftyGetProperty<string, IStyledTerm>(o => o.Background);
                var foreground = string.IsNullOrEmpty(bg) || !_foregroundCards.ContainsKey(bg) ? Brushes.Black : _foregroundCards[bg];

                drawingContext.DrawRectangle(Element.BlockConverter.GetBackground(block), null, rect);

                drawingContext.DrawText(Element.BlockConverter.GetContentText(block), foreground, new Thickness(3, 0, 3, 0), rect);

                //锁图

                //DrawPicture(blockConverter.GetImage(block), rect, drawingContext);
            }
        }

        internal static FormattedText CreatText(string text, double fontSize)
        {
            return new FormattedText(
                text, CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(Application.Current.Resources["BrickFontName"].ToString()),
                fontSize,
                Brushes.Black);
        }


        internal static void DrawPicture(ImageSource imageSource, Rect rect, DrawingContext context)
        {
            if (imageSource != null)
            {
                double imageWidth = imageSource.Width;
                double imageHeight = imageSource.Height;
                context.DrawImage(imageSource, new Rect(rect.X + (rect.Width - imageWidth) / 2,
                    rect.Y + (rect.Height - imageHeight) / 2, imageWidth, imageHeight));
            }
        }
    }
}
