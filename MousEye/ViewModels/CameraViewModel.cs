using Microsoft.Practices.Prism.Commands;
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

        #region KOMENDY

        private readonly DelegateCommand _startCommand;

        public DelegateCommand StartCommand
        {
            get { return _startCommand; }
        }

        #endregion KOMENDY

        #region WŁAŚCIWOŚCI

        private bool _isStartEnabled;

        public bool IsStartEnabled
        {
            get { return _isStartEnabled; }
            set
            {
                _isStartEnabled = value;
                NotifyPropertyChanged("IsStartEnabled");
            }
        }

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

        private BitmapSource _test3;

        public BitmapSource Test3
        {
            get { return _test3; }
            set
            {
                _test3 = value;
                NotifyPropertyChanged("Test3");
            }
        }

        private BitmapSource _test4;

        public BitmapSource Test4
        {
            get { return _test4; }
            set
            {
                _test4 = value;
                NotifyPropertyChanged("Test4");
            }
        }

        public CameraDevice CameraDevice { get; set; }

        private double _threshold;

        public double Threshold
        {
            get { return _threshold; }
            set
            {
                _threshold = value;
                NotifyPropertyChanged("Threshold");
            }
        }

        #endregion WŁAŚCIWOŚCI

        #region KONSTRUKTOR

        public CameraViewModel()
        {
            IsStartEnabled = true;
            Application.Current.Exit += CurrentOnExit;

            _startCommand = new DelegateCommand(OnStart);

            _cameraNum = CameraDevice.CameraCount;

            if (_cameraNum < 1)
            {
                MessageBox.Show("Could not find any PS3Eye cameras!");
                return;
            }

            Message = string.Format("Found {0} CLEyeCamera devices\r\n" +
                                    "Camera ID: {1}", _cameraNum, CameraDevice.CameraUuid(0));
        }

        private void TestOnChanged(object sender, EventArgs eventArgs)
        {
            ImageProcessing.Proc(CameraDevice.BitmapSource, Threshold);
            Test2 = ImageProcessing.InvertedBitmap;
            Test3 = ImageProcessing.BinaryBitmap;
            Test4 = ImageProcessing.TestBitmap;
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
            Test.Changed += TestOnChanged;
        }

        private void OnStart()
        {
            IsStartEnabled = false;

            CameraDevice = new CameraDevice();
            CameraDevice.BitmapReady += OnBitmapReady;

            CameraDevice.Create(CameraDevice.CameraUuid(0));
            CameraDevice.Zoom = 0;
            CameraDevice.Framerate = 60;
            CameraDevice.Start();
        }

        #endregion METODY
    }
}