using MousEye.ViewModels;

namespace MousEye.Views.CalibrationSteps
{
    /// <summary>
    /// Interaction logic for Step4.xaml
    /// </summary>
    public partial class Step4
    {
        public Step4(CameraViewModel vm)
        {
            InitializeComponent();

            DataContext = vm;
        }
    }
}