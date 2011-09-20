using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Luna.Common;
using Luna.WPF.ApplicationFramework.Controls;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using FlowDirection = System.Windows.FlowDirection;
using FontFamily = System.Windows.Media.FontFamily;
using Point = System.Windows.Point;

namespace Luna.WPF.ApplicationFramework.Extensions
{
    public static class TextDrawingExt
    {
        private const float FontSize = 12;

        private static Dictionary<string, FormattedText> _formatedTextCaches;

        static TextDrawingExt()
        {
            _formatedTextCaches = new Dictionary<string, FormattedText>(100);
        }

        public static FormattedText ToFormattedText(this string text, double emSize, FontWeight weight, Brush foreground)
        {
            var typeface = new Typeface(new FontFamily(), FontStyles.Normal, weight, FontStretches.Normal);

            return new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, emSize, foreground);
        }

        public static System.Drawing.Size GetTtextSize(this string text)
        {
            var font = new Font(Luna.Common.Constants.Global.FamilyFonts, FontSize, System.Drawing.FontStyle.Regular);
            var size = TextRenderer.MeasureText(text, font);
            return size;
        }

        public static FormattedText ToFormattedText(this string text, Brush foreground)
        {
            var typeface = new Typeface(new FontFamily(), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

            return new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, FontSize, foreground);
        }

        public static void DrawCenterText(this DrawingContext dc, FormattedText formattedText, Rect rect)
        {
            var text = formattedText.Text;

            var charWidth = formattedText.Width / text.Length;

            if (formattedText.Width > rect.Width)
            {
                var maxChar = (int)(rect.Width / charWidth);
                formattedText = text.Substring(0, maxChar).ToFormattedText(Brushes.Black);
            }

            var horizontailMargin = (rect.Width - formattedText.Width) / 2; //margin left & right

            dc.DrawText(formattedText, new Point(rect.Left + horizontailMargin, rect.Top + ((rect.Height - formattedText.Baseline) / 2)));
        }

        public static void DrawCenterText(this DrawingContext dc, string text, Rect rect, Brush foreground)
        {
            if (string.IsNullOrEmpty(text)) return;

            FormattedText formattedText;

            if (!_formatedTextCaches.ContainsKey(text))
            {
                formattedText = text.ToFormattedText(foreground);

                var charWidth = formattedText.Width / text.Length;

                if (formattedText.Width > rect.Width)
                {
                    var maxChar = (int)(rect.Width / charWidth);
                    formattedText = text.Substring(0, maxChar).ToFormattedText(foreground);
                }
                _formatedTextCaches[text] = formattedText;
            }
            else
                formattedText = _formatedTextCaches[text];


            var horizontailMargin = (rect.Width - formattedText.Width) / 2; //margin left & right


            dc.DrawText(formattedText, new Point(rect.Left + horizontailMargin, rect.Top + ((rect.Height - formattedText.Baseline) / 2)));
        }

        public static void DrawText(this DrawingContext dc, string text, SolidColorBrush foreground, Thickness margin, Rect rect)
        {
            if (string.IsNullOrEmpty(text)) return;

            FormattedText formattedText;

            formattedText = text.ToFormattedText(foreground);

            var charWidth = formattedText.Width / text.Length;

            var maxWidth = Math.Max(0, rect.Width - margin.Left - margin.Right);

            if (maxWidth < formattedText.Width)
            {
                var maxChar = (int)(maxWidth / charWidth);
                if (maxChar <= 0)
                    return;
                formattedText = text.Substring(0, maxChar).ToFormattedText(Brushes.Black);
            }

            _formatedTextCaches[text] = formattedText;


            var y = margin.Top == 0 ? rect.Top + ((rect.Height - formattedText.Baseline) / 2) : margin.Top;
            dc.DrawText(formattedText, new Point(rect.Left + margin.Left, y));
        }



        public static Rect ToRect(this ITerm term, IHorizontalControl axisConverter, double top, double height)
        {
            var left = axisConverter.DataToScreen(term.Start);

            //if(!axisConverter.IsInViewRagne(left))
            //{

            //}

            var right = axisConverter.DataToScreen(term.End);
            return new Rect(left, top, right - left, height);
        }

        public static Rect ToRect(this DateTime date, IHorizontalControl axisConverter, double top, double height)
        {
            var left = axisConverter.DataToScreen(date);
            var right = axisConverter.DataToScreen(date.AddDays(1));
            return new Rect(left, top, right - left, height);
        }

        public static Rect SetMargin(this Rect rect, Thickness margin)
        {
            return new Rect(rect.X + margin.Left, rect.Y + margin.Top, rect.Width - margin.Right - margin.Left, rect.Height - margin.Bottom - margin.Top);
        }
    }
}
