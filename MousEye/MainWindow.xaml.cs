using System.Windows;

namespace MousEye
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int numCameras = 0;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (numCameras >= 1)
            {
                cameraImage1.CameraDevice.Stop();
                cameraImage1.CameraDevice.Destroy();
            }
            if (numCameras == 2)
            {
                cameraImage2.CameraDevice.Stop();
                cameraImage2.CameraDevice.Destroy();
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Query for number of connected cameras
            numCameras = CameraDevice.CameraCount;
            if (numCameras == 0)
            {
                MessageBox.Show("Could not find any PS3Eye cameras!");
                return;
            }
            output.Items.Add(string.Format("Found {0} CLEyeCamera devices", numCameras));
            // Show camera's UUIDs
            for (int i = 0; i < numCameras; i++)
            {
                output.Items.Add(string.Format("CLEyeCamera #{0} UUID: {1}", i + 1, CameraDevice.CameraUUID(i)));
            }
            // Create cameras, set some parameters and start capture
            if (numCameras >= 1)
            {
                cameraImage1.CameraDevice.Create(CameraDevice.CameraUUID(0));
                cameraImage1.CameraDevice.Zoom = -50;
                cameraImage1.CameraDevice.Start();
            }
            if (numCameras == 2)
            {
                cameraImage2.CameraDevice.Create(CameraDevice.CameraUUID(1));
                cameraImage2.CameraDevice.Rotation = 200;
                cameraImage2.CameraDevice.Start();
            }
        }
    }
}