using MousEye.ViewModels;
using System.ComponentModel;
using System.Windows;

namespace MousEye.Views
{
    public partial class MainteanceView
    {
        public MainteanceView(CameraViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }
    }
}