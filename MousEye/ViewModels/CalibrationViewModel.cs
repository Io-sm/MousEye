using Microsoft.Practices.Prism.Commands;
using MousEye.Utility;
using MousEye.Views;
using MousEye.Views.CalibrationSteps;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace MousEye.ViewModels
{
    public class CalibrationViewModel : INotifyPropertyChanged
    {
        #region EVENTS

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion EVENTS

        #region PRIVATES

        private int _stepCount;

        private Step4 _temp;

        private int[] _savedSize;

        private int _counter;

        private int _tick;

        private readonly DispatcherTimer _timer;

        private readonly CameraViewModel _viewModel;

        #endregion PRIVATES

        #region PROPERTIES

        private HorizontalAlignment _horizontalAlignment;

        public HorizontalAlignment HorizontalAlignment
        {
            get { return _horizontalAlignment; }
            set
            {
                _horizontalAlignment = value;
                NotifyPropertyChanged("HorizontalAlignment");
            }
        }

        private VerticalAlignment _verticalAlignment;

        public VerticalAlignment VerticalAlignment
        {
            get { return _verticalAlignment; }
            set
            {
                _verticalAlignment = value;
                NotifyPropertyChanged("VerticalAlignment");
            }
        }

        private bool _isElipseVisible;

        public bool IsElipseVisible
        {
            get { return _isElipseVisible; }
            set
            {
                _isElipseVisible = value;
                NotifyPropertyChanged("IsElipseVisible");
            }
        }

        private bool _isNextEnabled;

        public bool IsNextEnabled
        {
            get { return _isNextEnabled; }
            set
            {
                _isNextEnabled = value;
                NotifyPropertyChanged("IsNextEnabled");
            }
        }

        private bool _isContentVisible;

        public bool IsContentVisible
        {
            get { return _isContentVisible; }
            set
            {
                _isContentVisible = value;
                NotifyPropertyChanged("IsContentVisible");
            }
        }

        private bool _isFinishVisible;

        public bool IsFinishVisible
        {
            get { return _isFinishVisible; }
            set
            {
                _isFinishVisible = value;
                NotifyPropertyChanged("IsFinishVisible");
            }
        }

        public ObservableCollection<CalibrationTabItem> TabCategory { get; set; }

        private CalibrationTabItem _selectedCategory;

        public CalibrationTabItem SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                NotifyPropertyChanged("SelectedCategory");
            }
        }

        #endregion PROPERTIES

        #region COMMANDS

        private readonly DelegateCommand<string> _nextCommand;

        public DelegateCommand<string> NextCommand
        {
            get { return _nextCommand; }
        }

        private readonly DelegateCommand _captureCommand;

        public DelegateCommand CaptureCommand
        {
            get { return _captureCommand; }
        }

        private readonly DelegateCommand _calibrationStartCommand;

        public DelegateCommand CalibrationStartCommand
        {
            get { return _calibrationStartCommand; }
        }

        private readonly DelegateCommand _calibrationFinishCommand;

        public DelegateCommand CalibrationFinishCommand
        {
            get { return _calibrationFinishCommand; }
        }

        #endregion COMMANDS

        #region CONSTRUCTORS

        public CalibrationViewModel(CameraViewModel vm)
        {
            _viewModel = vm;

            _stepCount = 1;
            _counter = 0;
            _tick = 0;
            IsNextEnabled = false;
            IsElipseVisible = false;
            IsFinishVisible = false;
            IsContentVisible = true;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;

            _timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0) };
            _timer.Tick += TimerOnTick;

            TabCategory = new ObservableCollection<CalibrationTabItem>
            {
                new CalibrationTabItem("Welcome!", new Step1(_viewModel))
            };

            _nextCommand = new DelegateCommand<string>(NextStep);
            _captureCommand = new DelegateCommand(Capture);
            _calibrationStartCommand = new DelegateCommand(CalibrationStart);
            _calibrationFinishCommand = new DelegateCommand(CalibrationFinish);
        }

        #endregion CONSTRUCTORS

        #region METHODS

        private void CalibrationFinish()
        {
            new MainteanceView(_viewModel);
            _viewModel.OnClosingRequest();
            _viewModel.IsRunning = true;
        }

        private void CalibrationStart()
        {
            _timer.Start();
            IsContentVisible = false;
        }

        private void NextStep(string step)
        {
            switch (step)
            {
                case "Step1View":

                    if (_stepCount == 1)
                    {
                        TabCategory.Add(new CalibrationTabItem("Camera position", new Step2(_viewModel)));
                        SelectedCategory = TabCategory.Last();
                        _stepCount++;

                        if (_viewModel.CameraDevice == null)
                        {
                            _viewModel.Start();
                        }
                    }

                    break;

                case "Step2View":

                    if (_stepCount == 2)
                    {
                        TabCategory.Add(new CalibrationTabItem("Pupil detection", new Step3(_viewModel)));
                        SelectedCategory = TabCategory.Last();
                        _viewModel.OriginalImage.Changed += OnOriginalImageChanged;
                        _stepCount++;
                    }

                    break;

                case "Step3View":

                    if (_stepCount == 3)
                    {
                        _temp = new Step4(_viewModel);
                        _temp.Show();
                        _stepCount++;
                        break;
                    }

                    if (_stepCount == 4)
                    {
                        if (_temp != null) _temp.Close();
                        TabCategory.Add(new CalibrationTabItem("That's it!", new Step5(_viewModel)));
                        SelectedCategory = TabCategory.Last();
                    }

                    break;
            }
        }

        private void Capture()
        {
            IsNextEnabled = true;
            _savedSize = ImageProcessing.GetRectSize();
        }

        #endregion METHODS

        #region EVENT HANDLERS

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            switch (_tick)
            {
                case 0:
                    _tick++;
                    IsElipseVisible = true;
                    HorizontalAlignment = HorizontalAlignment.Left;
                    VerticalAlignment = VerticalAlignment.Top;
                    _timer.Interval = new TimeSpan(0, 0, 2);
                    break;

                case 1:
                    ImageProcessing.SaveCoords(0);
                    HorizontalAlignment = HorizontalAlignment.Right;
                    _tick++;
                    break;

                case 2:
                    ImageProcessing.SaveCoords(1);
                    VerticalAlignment = VerticalAlignment.Bottom;
                    _tick++;
                    break;

                case 3:
                    ImageProcessing.SaveCoords(2);
                    HorizontalAlignment = HorizontalAlignment.Left;
                    _tick++;
                    break;

                case 4:
                    ImageProcessing.SaveCoords(3);
                    _tick = 0;
                    _timer.Stop();
                    IsElipseVisible = false;
                    IsFinishVisible = true;
                    NextStep("Step3View");
                    break;
            }
        }

        private void OnOriginalImageChanged(object sender, EventArgs eventArgs)
        {
            if (_savedSize == null)
            {
                return;
            }

            if (_counter < _viewModel.Framerate / 8)
            {
                _counter++;
            }
            else
            {
                var temp = ImageProcessing.GetRectSize();

                var pole1 = _savedSize[0] * _savedSize[1];
                var pole2 = temp[0] * temp[1];

                if (pole1 < 2.5 * pole2)
                {
                    if (_viewModel.Threshold < _viewModel.Threshold + 5)
                    {
                        _viewModel.Threshold += 0.0005;
                    }
                    else
                    {
                        _viewModel.Threshold = 0.86;
                    }
                }

                if (2.5 * pole1 > pole2)
                {
                    if (_viewModel.Threshold > _viewModel.Threshold - 5)
                    {
                        _viewModel.Threshold -= 0.0005;
                    }
                    else
                    {
                        _viewModel.Threshold = 0.80;
                    }
                }

                _counter = 0;
            }
        }

        #endregion EVENT HANDLERS

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