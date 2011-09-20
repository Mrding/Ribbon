namespace Luna.WPF.ApplicationFramework.Controls
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    public class GIFImage : System.Windows.Controls.Image
    {
        #region Fields

        private System.Drawing.Bitmap _bitmap;
        private BitmapSource _bitmapSource;

        #endregion Fields

        #region Delegates

        delegate void OnFrameChangedDelegate();

        #endregion Delegates

        #region Properties

        public bool IsAnimation
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public void StartAnimation()
        {
            ImageAnimator.Animate(_bitmap, OnFrameChanged);
            IsAnimation = true;
        }

        public void StopAnimation()
        {
            ImageAnimator.StopAnimate(_bitmap, OnFrameChanged);
            IsAnimation = false;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            Animation();
        }

        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);

        private void Animation()
        {
            if (Source != null)
            {
                var source = Source.ToString();
                //来源于路径
                if (source.StartsWith("file"))
                {
                    source = Source.ToString().Remove(0, 8);
                    if (source.Contains("gif"))
                        AnimationImage(source);
                }
                //内部资源Uri
                else if (!source.StartsWith("System"))
                {
                    AnimationImage(new Uri(source));
                }

                StartAnimation();
            }
        }

        private void AnimationImage(string path)
        {
            _bitmap = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(path);
            SetImageSource(_bitmap);
        }

        private void AnimationImage(Uri uri)
        {
            var streamInfo = Application.GetContentStream(uri);
            if (streamInfo == null)
                streamInfo = Application.GetResourceStream(uri);
            if (streamInfo != null)
            {
                _bitmap = (System.Drawing.Bitmap)System.Drawing.Image.FromStream(streamInfo.Stream);
                SetImageSource(_bitmap);
            }
        }

        private BitmapSource GetBitmapSource()
        {
            IntPtr inptr = _bitmap.GetHbitmap();
            _bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                inptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(inptr);
            if (!_bitmapSource.IsFrozen && _bitmapSource.CanFreeze)
                _bitmapSource.Freeze();
            return _bitmapSource;
        }

        private void OnFrameChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new OnFrameChangedDelegate(OnFrameChangedInMainThread));
        }

        private void OnFrameChangedInMainThread()
        {
            ImageAnimator.UpdateFrames();
            _bitmapSource = GetBitmapSource();
            Source = _bitmapSource;
            InvalidateVisual();
            System.Diagnostics.Debug.Print("Frame");
        }

        private void SetImageSource(System.Drawing.Bitmap bitmap)
        {
            Width = bitmap.Width;
            Height = bitmap.Height;
            ImageAnimator.Animate(bitmap, OnFrameChanged);
            _bitmapSource = GetBitmapSource();
            Source = _bitmapSource;
        }

        #endregion Methods
    }
}