using System;
using System.Windows.Controls;

namespace MousEye
{
    public class CameraImage : Image, IDisposable
    {
        public CameraDevice CameraDevice { get; set; }

        public CameraImage()
        {
            CameraDevice = new CameraDevice();
            CameraDevice.BitmapReady += OnBitmapReady;
        }

        ~CameraImage()
        {
            Dispose(false);
        }

        private void OnBitmapReady(object sender, EventArgs e)
        {
            Source = CameraDevice.BitmapSource;
            ImageProcessing.Proc(CameraDevice.BitmapSource);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (CameraDevice == null) return;
            CameraDevice.Stop();
            CameraDevice.Dispose();
            CameraDevice = null;
        }

        //#region [ Dependency Properties ]

        //public float Framerate
        //{
        //    get { return (float)GetValue(FramerateProperty); }
        //    set { SetValue(FramerateProperty, value); }
        //}

        //public static readonly DependencyProperty FramerateProperty =
        //    DependencyProperty.Register("Framerate", typeof(float), typeof(CameraImage),
        //    new UIPropertyMetadata((float)15, (PropertyChangedCallback)delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        //    {
        //        CameraImage typedSender = sender as CameraImage;
        //        typedSender.CameraDevice.Framerate = (float)e.NewValue;
        //    }));

        //public CameraColorMode ColorMode
        //{
        //    get { return (CameraColorMode)GetValue(ColorModeProperty); }
        //    set { SetValue(ColorModeProperty, value); }
        //}

        //public static readonly DependencyProperty ColorModeProperty =
        //    DependencyProperty.Register("ColorMode", typeof(CameraColorMode), typeof(CameraImage),
        //    new UIPropertyMetadata(default(CameraColorMode), (PropertyChangedCallback)delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        //    {
        //        CameraImage typedSender = sender as CameraImage;
        //        typedSender.CameraDevice.ColorMode = (CameraColorMode)e.NewValue;
        //    }));

        //public CameraResolution Resolution
        //{
        //    get { return (CameraResolution)GetValue(ResolutionProperty); }
        //    set { SetValue(ResolutionProperty, value); }
        //}

        //public static readonly DependencyProperty ResolutionProperty =
        //    DependencyProperty.Register("Resolution", typeof(CameraResolution), typeof(CameraImage),
        //    new UIPropertyMetadata(default(CameraResolution), (PropertyChangedCallback)delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        //    {
        //        CameraImage typedSender = sender as CameraImage;
        //        typedSender.CameraDevice.Resolution = (CameraResolution)e.NewValue;
        //    }));

        //#endregion [ Dependency Properties ]
    }
}