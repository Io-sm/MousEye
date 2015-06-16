using MousEye.ViewModels;

namespace MousEye.Views
{
    public partial class CameraCalibration
    {
        public CameraCalibration()
        {
            InitializeComponent();
            var vm = new CameraViewModel();
            DataContext = vm;
            vm.ClosingRequest += (sender, e) => Close();
        }
    }
}