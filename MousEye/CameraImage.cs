using System;
using System.Windows;
using System.Windows.Controls;

namespace MousEye
{
    public class CameraImage : Image, IDisposable
    {
        public CameraDevice CameraDevice { get; private set; }

        public CameraImage()
        {
            CameraDevice = new CameraDevice();
            CameraDevice.BitmapReady += OnBitmapReady;
        }

        ~CameraImage()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        private void OnBitmapReady(object sender, EventArgs e)
        {
            Source = CameraDevice.BitmapSource;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (CameraDevice != null)
                {
                    CameraDevice.Stop();
                    CameraDevice.Dispose();
                    CameraDevice = null;
                }
            }
            // free native resources if there are any.
        }

        #region [ Dependency Properties ]

        public float Framerate
        {
            get { return (float)GetValue(FramerateProperty); }
            set { SetValue(FramerateProperty, value); }
        }

        public static readonly DependencyProperty FramerateProperty =
            DependencyProperty.Register("Framerate", typeof(float), typeof(CameraImage),
            new UIPropertyMetadata((float)15, (PropertyChangedCallback)delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
                CameraImage typedSender = sender as CameraImage;
                typedSender.CameraDevice.Framerate = (float)e.NewValue;
            }));

        public CLEyeCameraColorMode ColorMode
        {
            get { return (CLEyeCameraColorMode)GetValue(ColorModeProperty); }
            set { SetValue(ColorModeProperty, value); }
        }

        public static readonly DependencyProperty ColorModeProperty =
            DependencyProperty.Register("ColorMode", typeof(CLEyeCameraColorMode), typeof(CameraImage),
            new UIPropertyMetadata(default(CLEyeCameraColorMode), (PropertyChangedCallback)delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
                CameraImage typedSender = sender as CameraImage;
                typedSender.CameraDevice.ColorMode = (CLEyeCameraColorMode)e.NewValue;
            }));

        public CLEyeCameraResolution Resolution
        {
            get { return (CLEyeCameraResolution)GetValue(ResolutionProperty); }
            set { SetValue(ResolutionProperty, value); }
        }

        public static readonly DependencyProperty ResolutionProperty =
            DependencyProperty.Register("Resolution", typeof(CLEyeCameraResolution), typeof(CameraImage),
            new UIPropertyMetadata(default(CLEyeCameraResolution), (PropertyChangedCallback)delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
                CameraImage typedSender = sender as CameraImage;
                typedSender.CameraDevice.Resolution = (CLEyeCameraResolution)e.NewValue;
            }));

        #endregion [ Dependency Properties ]
    }
}