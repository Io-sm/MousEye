using System.ComponentModel;
using System.Windows;

namespace MousEye.ViewModels
{
    public class CameraViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly int _cameraNum;

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

        private CameraImage _cameraImage;

        public CameraImage CameraImage
        {
            get
            {
                return _cameraImage;
            }
            set
            {
                _cameraImage = value;
                NotifyPropertyChanged("CameraImage");
            }
        }

        public CameraViewModel()
        {
            Application.Current.Exit += CurrentOnExit;

            CameraImage = new CameraImage();

            _cameraNum = CameraDevice.CameraCount;

            if (_cameraNum < 1)
            {
                MessageBox.Show("Could not find any PS3Eye cameras!");
                return;
            }

            Message = string.Format("Found {0} CLEyeCamera devices\r\n" +
                                    "Camera ID: {1}", _cameraNum, CameraDevice.CameraUuid(0));

            CameraImage.CameraDevice.Create(CameraDevice.CameraUuid(0));
            CameraImage.CameraDevice.Zoom = -50;
            CameraImage.CameraDevice.Start();
        }

        private void CurrentOnExit(object sender, ExitEventArgs exitEventArgs)
        {
            if (_cameraNum < 1) return;
            CameraImage.CameraDevice.Stop();
            CameraImage.CameraDevice.Destroy();
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