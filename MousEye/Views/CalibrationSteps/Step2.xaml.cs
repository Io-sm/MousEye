using MousEye.ViewModels;

namespace MousEye.Views.CalibrationSteps
{
    public partial class Step2
    {
        public Step2(CameraViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}