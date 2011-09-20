using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Luna.WPF.ApplicationFramework.Extensions
{
    public static class ColorMixerExt
    {
        private static Dictionary<Color, Brush> _alphaLinearGradientBrushDic = new Dictionary<Color, Brush>();

        public static Brush GetAlphaLinearGradientBrushDic(Color color)
        {
            if (!_alphaLinearGradientBrushDic.ContainsKey(color))
            {
                lock (_alphaLinearGradientBrushDic)
                {
                    _alphaLinearGradientBrushDic.Add(color, GetAlphaLinearGradientBrush1(color));
                }
            }
            return _alphaLinearGradientBrushDic[color];
        }

        public static Color GetAlphaColor(Color color, float alpha)
        {
            float r = color.ScR * alpha + (1 - alpha);
            float g = color.ScG * alpha + (1 - alpha);
            float b = color.ScB * alpha + (1 - alpha);

            return Color.FromScRgb(1, r, g, b);
        }

        public static LinearGradientBrush GetAlphaLinearGradientBrush1(Color color)
        {
            //Color.FromScRgb(0.44f, color.ScR, color.ScG, color.ScB)
            //GetAlphaColor(color, 0.44f)
            LinearGradientBrush brush = new LinearGradientBrush();
            brush.StartPoint = new System.Windows.Point(0, 0);
            brush.EndPoint = new System.Windows.Point(0, 1.5);
            GradientStopCollection gradientStopCollection = new GradientStopCollection();
            GradientStop stop1 = new GradientStop(GetAlphaColor(color, 0.44f), 0);
            GradientStop stop2 = new GradientStop(GetAlphaColor(color, 0.44f), 0.30);
            GradientStop stop3 = new GradientStop(color, 0.31);
            GradientStop stop4 = new GradientStop(Colors.White, 1);
            gradientStopCollection.Add(stop1);
            gradientStopCollection.Add(stop2);
            gradientStopCollection.Add(stop3);
            gradientStopCollection.Add(stop4);
            brush.GradientStops = gradientStopCollection;

            //brush.Freeze();

            return brush;
        }

        public static Brush ToBrush(this string color, double opacity)
        {
            if (!string.IsNullOrEmpty(color))
            {
                var colorObj = ColorConverter.ConvertFromString(color);
                if (colorObj != null && colorObj is Color)
                {
                    var brush =  new SolidColorBrush((Color) colorObj) {Opacity = opacity};
                    brush.Freeze();
                    return brush;
                }
                    
            }
            return Brushes.Black;
        }
    }
}