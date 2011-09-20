using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Luna.WPF.ApplicationFramework.Extensions
{
    public static class BitmapImageExtension
    {
        public static BitmapImage ByteToImage(this byte[] buffByte)
        {
            if (buffByte == null || buffByte.Length == 0)
                throw new ArgumentNullException("buffByte");

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
    }
}
