using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace MousEye.ViewModels
{
    public class CameraViewModel : INotifyPropertyChanged
    {
        private int w_x;
        private int w_y;

        #region EVENTS

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler ClosingRequest;

        #endregion EVENTS

        #region PRIVATE

        private readonly int _cameraIndex;

        private int _mCounter;

        #endregion PRIVATE

        #region COMMANDS

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

        private readonly DelegateCommand _mainteanceViewCommand;

        public DelegateCommand MainteanceViewCommand
        {
            get { return _mainteanceViewCommand; }
        }

        private readonly DelegateCommand _closeApplicationCommand;

        public DelegateCommand CloseApplicationCommand
        {
            get { return _closeApplicationCommand; }
        }

        private readonly DelegateCommand _suspendExecutionCommand;

        public DelegateCommand SuspendExecutionCommand
        {
            get { return _suspendExecutionCommand; }
        }

        private readonly DelegateCommand _startExecutionCommand;

        public DelegateCommand StartExecutionCommand
        {
            get { return _startExecutionCommand; }
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

        private BitmapSource _calibrationImage;

        public BitmapSource CalibrationImage
        {
            get { return _calibrationImage; }
            set
            {
                _calibrationImage = value;
                NotifyPropertyChanged("CalibrationImage");
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

        public CalibrationViewModel CalibrationViewModel { get; private set; }

        public CameraDevice CameraDevice { get; private set; }

        private bool _isMainteanceViewVisible;

        public bool IsMainteanceViewVisible
        {
            get { return _isMainteanceViewVisible; }
            set
            {
                MainteanceContextMenuMessage = value ? "Close mainteance window" : "Open mainteance window";
                _isMainteanceViewVisible = value;
                NotifyPropertyChanged("IsMainteanceViewVisible");
            }
        }

        private string _mainteanceContextMenuMessage = "Open mainteance window";

        public string MainteanceContextMenuMessage
        {
            get { return _mainteanceContextMenuMessage; }
            set
            {
                _mainteanceContextMenuMessage = value;
                NotifyPropertyChanged("MainteanceContextMenuMessage");
            }
        }

        private string _iconFileSource = "../../Icons/Run.ico";

        public string IconFileSource
        {
            get { return _iconFileSource; }
            set
            {
                _iconFileSource = value;
                NotifyPropertyChanged("IconFileSource");
            }
        }

        private string _toolTipText = "MousEye is running.";

        public string ToolTipText
        {
            get { return _toolTipText; }
            set
            {
                _toolTipText = value;
                NotifyPropertyChanged("ToolTipText");
            }
        }

        public bool IsRunning;

        #endregion PROPERTIES

        #region CONSTRUCTORS

        public CameraViewModel()
        {
            _cameraIndex = CameraDevice.CameraCount;
            _settingsManagerCommand = new DelegateCommand<string>(OpenSettingsManager);
            _restoreDefaultsCommand = new DelegateCommand(DefaultValues);
            _mainteanceViewCommand = new DelegateCommand(MainteanceViewVisibility);
            _closeApplicationCommand = new DelegateCommand(Application.Current.Shutdown);
            _suspendExecutionCommand = new DelegateCommand(SuspendExecution);
            _startExecutionCommand = new DelegateCommand(StartExecution);

            CalibrationViewModel = new CalibrationViewModel(this);

            Application.Current.Exit += OnCurrentExit;

            IsMainteanceViewVisible = false;

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
            try
            {
                ImageProcessing.ProcessImage(CameraDevice.BitmapSource, Threshold);
                InvertedImage = ImageProcessing.InvertedBitmap;
                BinaryImage = ImageProcessing.BinaryBitmap;
                CalibrationImage = ImageProcessing.CalibrationBitmap;
                FinalImage = ImageProcessing.FinalImage;
            }
            catch (IndexOutOfRangeException)
            {
                Threshold += 0.08;
            }

            if (IsRunning && _mCounter == Framerate / 4)
            {
                MoveMouse();
                _mCounter = 0;
            }

            if (IsRunning)
            {
                _mCounter++;
            }
        }

        #endregion EVENT HANDLERS

        #region METHODS

        public void Start()
        {
            CameraDevice = new CameraDevice();

            CameraDevice.BitmapReady += OnBitmapReady;

            CameraDevice.Create(CameraDevice.CameraUuid(0));
            DefaultValues();
            CameraDevice.Start();
        }

        private void DefaultValues()
        {
            HorizontalFlip = false;
            VerticalFlip = true;
            Framerate = 15;
            Zoom = 145;
            Exposure = 511;
            Gain = 0;
            RedBalance = 0;
            GreenBalance = 0;
            BlueBalance = 0;

            Threshold = 0.8;
        }

        private void OpenSettingsManager(string mode)
        {
            if (CameraDevice == null)
            {
                MessageBox.Show("No cameras detected!");
                return;
            }

            SettingsManager.CameraSettings(this);
        }

        public void OnClosingRequest()
        {
            if (ClosingRequest != null)
            {
                ClosingRequest(this, EventArgs.Empty);
            }
        }

        private void MainteanceViewVisibility()
        {
            if (!IsMainteanceViewVisible)
            {
                IsMainteanceViewVisible = true;
                return;
            }

            IsMainteanceViewVisible = true;
        }

        private void StartExecution()
        {
            IconFileSource = "../../Icons/Run.ico";
            ToolTipText = "MousEye is running.";
            IsRunning = true;
        }

        private void SuspendExecution()
        {
            IconFileSource = "../../Icons/Stop.ico";
            ToolTipText = "MousEye is stopped.";
            IsRunning = false;
        }

        private void MoveMouse()
        {
            var list = ImageProcessing.GetCoords();
            var point = ImageProcessing.GetRectMiddle();

            var c_w = list[1].X - list[0].X;
            var c_h = list[3].Y - list[0].Y;
            var xx = point.X - list[0].X;
            var yy = point.Y - list[0].Y;
            var px = (xx/c_w);
            var py = (yy/ c_h);

            MouseManipulation.SetCursorPos((int)(Screen.PrimaryScreen.Bounds.Width*px),
                (int)(Screen.PrimaryScreen.Bounds.Height * py));
        }

        private List<Point> CalculatePosition(List<Point> points, out Point point)
        {
            points.Insert(1, new Point(points[1].X, points[0].Y));
            points.RemoveAt(2);

            points.Insert(2, new Point(points[1].X, points[2].Y));
            points.RemoveAt(3);

            points.Insert(3, new Point(points[0].X, points[2].Y));
            points.RemoveAt(4);

            var list = new List<Point>();

            var middlePoint = ImageProcessing.GetRectMiddle();

            foreach (var variable in points)
            {
                list.Add(variable);
            }

            for (var i = 0; i < list.Count; i++)
            {
                list.Insert(i, new Point(points[i].X - points[0].X, points[i].Y - points[0].Y));
                list.RemoveAt(i + 1);
            }

            var tempx = middlePoint.X - points[0].X;
            if (tempx < 0)
            {
                tempx = 0;
            }

            var tempy = middlePoint.Y - points[0].Y;
            if (tempy < 0)
            {
                tempy = 0;
            }

            point = new Point(tempx, tempy);

            return list;
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