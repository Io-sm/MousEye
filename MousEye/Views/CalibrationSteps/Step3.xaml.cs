using MousEye.ViewModels;

namespace MousEye.Views.CalibrationSteps
{
    /// <summary>
    /// Interaction logic for Step3.xaml
    /// </summary>
    public partial class Step3
    {
        public Step3(CameraViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}