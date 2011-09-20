using System;
using System.Windows.Media;

namespace Luna.WPF.ApplicationFramework.Extensions
{
    public static class ColorExt
    {
        public static Color ToColor(this string colorName)
        {
            if (colorName.StartsWith("#"))
                colorName = colorName.Replace("#", string.Empty);
            var v = int.Parse(colorName, System.Globalization.NumberStyles.HexNumber);
            return new Color
                       {
                           A = Convert.ToByte((v >> 24) & 255),
                           R = Convert.ToByte((v >> 16) & 255),
                           G = Convert.ToByte((v >> 8) & 255),
                           B = Convert.ToByte((v >> 0) & 255)
                       };
        }
    }
}