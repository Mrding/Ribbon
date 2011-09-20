using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Luna.WPF.ApplicationFramework.Converters
{
    public class ByteToBitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var buffByte = value as byte[];
            if (buffByte == null || buffByte.Length == 0)
                return default(BitmapImage);

            var ms = new MemoryStream(buffByte);
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnDemand;
            bi.CreateOptions = BitmapCreateOptions.DelayCreation;
            bi.StreamSource = ms;
            bi.EndInit();
            if (bi.CanFreeze)
                bi.Freeze();
            return bi;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
