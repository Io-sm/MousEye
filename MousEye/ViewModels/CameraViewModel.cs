using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace MousEye.ViewModels
{
    public class CameraViewModel : INotifyPropertyChanged
    {
        #region EVENTY

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion EVENTY

        #region ZMIENNE PRYWATNE

        private readonly int _cameraNum;

        #endregion ZMIENNE PRYWATNE

        #region WŁAŚCIWOŚCI

        private string _message;

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                NotifyPropertyChanged("Message");
            }
        }

        private InteropBitmap _test;

        public InteropBitmap Test
        {
            get { return _test; }
            set
            {
                _test = value;
                NotifyPropertyChanged("Test");
            }
        }

        private BitmapSource _test2;

        public BitmapSource Test2
        {
            get { return _test2; }
            set
            {
                _test2 = value;
                NotifyPropertyChanged("Test2");
            }
        }

        public CameraDevice CameraDevice { get; set; }

        #endregion WŁAŚCIWOŚCI

        #region KONSTRUKTOR

        public CameraViewModel()
        {
            Application.Current.Exit += CurrentOnExit;

            CameraDevice = new CameraDevice();
            CameraDevice.BitmapReady += OnBitmapReady;

            _cameraNum = CameraDevice.CameraCount;

            if (_cameraNum < 1)
            {
                MessageBox.Show("Could not find any PS3Eye cameras!");
                return;
            }

            Message = string.Format("Found {0} CLEyeCamera devices\r\n" +
                                    "Camera ID: {1}", _cameraNum, CameraDevice.CameraUuid(0));

            CameraDevice.Create(CameraDevice.CameraUuid(0));
            CameraDevice.Zoom = 0;
            CameraDevice.Start();
        }

        #endregion KONSTRUKTOR

        #region METODY

        private void CurrentOnExit(object sender, ExitEventArgs exitEventArgs)
        {
            if (_cameraNum < 1) return;
            CameraDevice.Stop();
            CameraDevice.Destroy();
        }

        protected void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private void OnBitmapReady(object sender, EventArgs e)
        {
            Test = CameraDevice.BitmapSource;
            ImageProcessing.OriginalBitmap = Test;
            Test2 = ImageProcessing.InvertedBitmap; //CameraDevice.BitmapSource;
        }

        #endregion METODY
    }
}