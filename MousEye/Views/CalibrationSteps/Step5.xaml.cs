using MousEye.ViewModels;

namespace MousEye.Views.CalibrationSteps
{
    /// <summary>
    /// Interaction logic for Step5.xaml
    /// </summary>
    public partial class Step5
    {
        public Step5(CameraViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}