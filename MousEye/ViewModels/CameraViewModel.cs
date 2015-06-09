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
        #region EVENTS

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion EVENTS

        #region PRIVATE

        private readonly int _cameraIndex;

        #endregion PRIVATE

        #region COMMANDS

        private readonly DelegateCommand _startCommand;

        public DelegateCommand StartCommand
        {
            get { return _startCommand; }
        }

        private readonly DelegateCommand<string> _settingsManagerCommand;

        public DelegateCommand<string> SettingsManagerCommand
        {
            get { return _settingsManagerCommand; }
        }

        private readonly DelegateCommand _restoreDefaultsCommand;

        public DelegateCommand RestoreDefaultsCommand
        {
            get { return _restoreDefaultsCommand; }
        }

        #endregion COMMANDS

        #region CAMERA PROPERTIES

        private bool _horizontalFlip;

        public bool HorizontalFlip
        {
            get { return _horizontalFlip; }
            set
            {
                _horizontalFlip = value;
                CameraDevice.HorizontalFlip = value;
                NotifyPropertyChanged("HorizontalFlip");
            }
        }

        private bool _verticalFlip;

        public bool VerticalFlip
        {
            get { return _verticalFlip; }
            set
            {
                _verticalFlip = value;
                CameraDevice.VerticalFlip = value;
                NotifyPropertyChanged("VerticalFlip");
            }
        }

        private int _gain;

        public int Gain
        {
            get { return _gain; }
            set
            {
                _gain = value;
                CameraDevice.Gain = value;
                NotifyPropertyChanged("Gain");
            }
        }

        private int _exposure;

        public int Exposure
        {
            get
            {
                return _exposure;
            }
            set
            {
                _exposure = value;
                CameraDevice.Exposure = value;
                NotifyPropertyChanged("Exposure");
            }
        }

        private int _redBalance;

        public int RedBalance
        {
            get { return _redBalance; }
            set
            {
                _redBalance = value;
                CameraDevice.WhiteBalanceRed = value;
                NotifyPropertyChanged("RedBalance");
            }
        }

        private int _greenBalance;

        public int GreenBalance
        {
            get { return _greenBalance; }
            set
            {
                _greenBalance = value;
                CameraDevice.WhiteBalanceGreen = value;
                NotifyPropertyChanged("GreenBalance");
            }
        }

        private int _blueBalance;

        public int BlueBalance
        {
            get { return _blueBalance; }
            set
            {
                _blueBalance = value;
                CameraDevice.WhiteBalanceBlue = value;
                NotifyPropertyChanged("BlueBalance");
            }
        }

        private int _zoom;

        public int Zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = value;
                CameraDevice.Zoom = value;
                NotifyPropertyChanged("Zoom");
            }
        }

        private int _framerate;

        public int Framerate
        {
            get { return _framerate; }
            set
            {
                _framerate = value;
                CameraDevice.Framerate = value;
                NotifyPropertyChanged("Framerate");
            }
        }

        #endregion CAMERA PROPERTIES

        #region CALIBRATION PROPERTIES

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

        #endregion CALIBRATION PROPERTIES

        #region IMAGE PROPERTIES

        private InteropBitmap _originalImage;

        public InteropBitmap OriginalImage
        {
            get { return _originalImage; }
            set
            {
                _originalImage = value;
                NotifyPropertyChanged("OriginalImage");
            }
        }

        private BitmapSource _invertedImage;

        public BitmapSource InvertedImage
        {
            get { return _invertedImage; }
            set
            {
                _invertedImage = value;
                NotifyPropertyChanged("InvertedImage");
            }
        }

        private BitmapSource _binaryImage;

        public BitmapSource BinaryImage
        {
            get { return _binaryImage; }
            set
            {
                _binaryImage = value;
                NotifyPropertyChanged("BinaryImage");
            }
        }

        private BitmapSource _finalImage;

        public BitmapSource FinalImage
        {
            get { return _finalImage; }
            set
            {
                _finalImage = value;
                NotifyPropertyChanged("FinalImage");
            }
        }

        #endregion IMAGE PROPERTIES

        #region PROPERTIES

        public CameraDevice CameraDevice { get; set; }

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

        #endregion PROPERTIES

        #region CONSTRUCTORS

        public CameraViewModel()
        {
            IsStartEnabled = true;
            _cameraIndex = CameraDevice.CameraCount;
            _startCommand = new DelegateCommand(Start);
            _settingsManagerCommand = new DelegateCommand<string>(OpenSettingsManager);
            _restoreDefaultsCommand = new DelegateCommand(DefaultValues);

            Application.Current.Exit += OnCurrentExit;

            if (_cameraIndex < 1)
            {
                MessageBox.Show("Could not find any devices!");
                return;
            }

            Message = string.Format("Found {0} CLEyeCamera devices\r\n" +
                                    "Camera ID: {1}", _cameraIndex, CameraDevice.CameraUuid(0));
        }

        #endregion CONSTRUCTORS

        #region EVENT HANDLERS

        private void OnCurrentExit(object sender, ExitEventArgs exitEventArgs)
        {
            if (_cameraIndex < 1 || CameraDevice == null) return;
            CameraDevice.Stop();
            CameraDevice.Destroy();
        }

        private void OnBitmapReady(object sender, EventArgs e)
        {
            OriginalImage = CameraDevice.BitmapSource;
            OriginalImage.Changed += OnImageChanged;
        }

        private void OnImageChanged(object sender, EventArgs eventArgs)
        {
            ImageProcessing.ProcessImage(CameraDevice.BitmapSource, Threshold);
            InvertedImage = ImageProcessing.InvertedBitmap;
            BinaryImage = ImageProcessing.BinaryBitmap;
            FinalImage = ImageProcessing.FinalImage;
        }

        #endregion EVENT HANDLERS

        #region METHODS

        private void Start()
        {
            IsStartEnabled = false;

            CameraDevice = new CameraDevice();

            CameraDevice.BitmapReady += OnBitmapReady;

            CameraDevice.Create(CameraDevice.CameraUuid(0));
            DefaultValues();
            CameraDevice.Start();
        }

        private void DefaultValues()
        {
            HorizontalFlip = true;
            VerticalFlip = false;
            Framerate = 15;
            Zoom = 0;
            Exposure = 511;
            Gain = 0;
            RedBalance = 0;
            GreenBalance = 0;
            BlueBalance = 0;
        }

        private void OpenSettingsManager(string mode)
        {
            if (CameraDevice == null)
            {
                MessageBox.Show("No cameras detected!");
                return;
            }

            switch (mode)
            {
                case "camera":

                    SettingsManager.CameraSettings(this);

                    break;

                case "calibration":

                    break;
            }
        }

        #endregion METHODS

        #region UTILITIES

        protected void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion UTILITIES
    }
}