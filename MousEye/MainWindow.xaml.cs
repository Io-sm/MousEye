using System.Windows;

namespace MousEye
{
    public partial class MainWindow
    {
        private bool _isVisible;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TaskbarIcon_OnTrayLeftMouseDown(object sender, RoutedEventArgs e)
        {
            if (_isVisible)
            {
                Visibility = Visibility.Hidden;
                _isVisible = false;
                return;
            }

            Visibility = Visibility.Visible;
            _isVisible = true;
        }
    }
}