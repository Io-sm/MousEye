using System.Windows;
using System.Windows.Controls;

namespace MousEye
{
    public partial class MainWindow
    {
        private int _numCameras;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            CheckBox1.Checked += CheckBox1OnChecked;
            CheckBox1.Unchecked += CheckBox1OnUnchecked;
            Siki.ValueChanged += SikiOnValueChanged;
        }

        private void SikiOnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> routedPropertyChangedEventArgs)
        {
            var slider = sender as Slider;
            if (slider != null) CameraImage1.CameraDevice.Gain = (int)slider.Value;
        }

        private void CheckBox1OnUnchecked(object sender, RoutedEventArgs routedEventArgs)
        {
            CameraImage1.CameraDevice.HorizontalFlip = false;
        }

        private void CheckBox1OnChecked(object sender, RoutedEventArgs routedEventArgs)
        {
            CameraImage1.CameraDevice.HorizontalFlip = true;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_numCameras < 1) return;
            CameraImage1.CameraDevice.Stop();
            CameraImage1.CameraDevice.Destroy();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _numCameras = CameraDevice.CameraCount;
            if (_numCameras == 0)
            {
                MessageBox.Show("Could not find any PS3Eye cameras!");
                return;
            }
            Output.Items.Add(string.Format("Found {0} CLEyeCamera devices", _numCameras));
            for (var i = 0; i < _numCameras; i++)
            {
                Output.Items.Add(string.Format("CLEyeCamera #{0} UUID: {1}", i + 1, CameraDevice.CameraUuid(i)));
            }

            if (_numCameras < 1) return;

            CameraImage1.CameraDevice.Create(CameraDevice.CameraUuid(0));
            CameraImage1.CameraDevice.Zoom = -50;
            CameraImage1.CameraDevice.Start();
        }
    }
}