using MousEye.ViewModels;

namespace MousEye.Views.CalibrationSteps
{
    public partial class Step1
    {
        public Step1(CameraViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}