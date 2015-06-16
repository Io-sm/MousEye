using Microsoft.Practices.Prism.Commands;
using MousEye.Utility;
using MousEye.Views.CalibrationSteps;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace MousEye.ViewModels
{
    public class CalibrationViewModel : INotifyPropertyChanged
    {
        private int _stepCount;

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly CameraViewModel _viewModel;

        private bool _isNextEnabled;

        private int[] _savedSize;

        private int _counter;

        private int _windowWidth;

        public int WindowWidth
        {
            get { return _windowWidth; }
            set
            {
                _windowWidth = value;
                NotifyPropertyChanged("WindowWidth");
            }
        }

        private int _windowHeight;

        public int WindowHeight
        {
            get { return _windowHeight; }
            set
            {
                _windowHeight = value;
                NotifyPropertyChanged("WindowHeight");
            }
        }

        public bool IsNextEnabled
        {
            get { return _isNextEnabled; }
            set
            {
                _isNextEnabled = value;
                NotifyPropertyChanged("IsNextEnabled");
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

        public CalibrationViewModel(CameraViewModel vm)
        {
            _viewModel = vm;

            _stepCount = 1;
            IsNextEnabled = false;
            _counter = 0;

            TabCategory = new ObservableCollection<CalibrationTabItem>
            {
                new CalibrationTabItem("Welcome!", new Step1(_viewModel))
            };

            _nextCommand = new DelegateCommand<string>(NextStep);
            _captureCommand = new DelegateCommand(Capture);
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
                        var calWindow = new Step4(_viewModel);
                        calWindow.Show();
                    }

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

        private void Capture()
        {
            IsNextEnabled = true;
            _savedSize = ImageProcessing.GetRectSize();
        }

        protected void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}