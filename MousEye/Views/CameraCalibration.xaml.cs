using MousEye.ViewModels;

namespace MousEye.Views
{
    public partial class CameraCalibration
    {
        public CameraCalibration(CameraViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}