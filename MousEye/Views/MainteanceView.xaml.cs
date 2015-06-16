using MousEye.ViewModels;

namespace MousEye.Views
{
    public partial class MainteanceView
    {
        public MainteanceView(CameraViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}