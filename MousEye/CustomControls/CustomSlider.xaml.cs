using System.Windows;

namespace MousEye.CustomControls
{
    /// <summary>
    /// Interaction logic for CustomSlider.xaml
    /// </summary>
    public partial class CustomSlider
    {
        public CustomSlider()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string),
            typeof(CustomSlider), new PropertyMetadata(string.Empty));

        public string Title
        {
            get { return GetValue(TitleProperty).ToString(); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty MinimalValueProperty =
            DependencyProperty.Register("MinimalValue", typeof(double),
            typeof(CustomSlider), new PropertyMetadata(0.0));

        public double MinimalValue
        {
            get { return (double)GetValue(MinimalValueProperty); }
            set { SetValue(MinimalValueProperty, value); }
        }

        public static readonly DependencyProperty MaximalValueProperty =
            DependencyProperty.Register("MaximalValue", typeof(double),
            typeof(CustomSlider), new PropertyMetadata(10.0));

        public double MaximalValue
        {
            get { return (double)GetValue(MaximalValueProperty); }
            set { SetValue(MaximalValueProperty, value); }
        }

        public static readonly DependencyProperty CurrentValueProperty =
            DependencyProperty.Register("CurrentValue", typeof(double),
            typeof(CustomSlider), new PropertyMetadata(0.0));

        public double CurrentValue
        {
            get { return (double)GetValue(CurrentValueProperty); }
            set { SetValue(CurrentValueProperty, value); }
        }
    }
}