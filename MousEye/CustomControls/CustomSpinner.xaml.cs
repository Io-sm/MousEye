using System.Collections.Generic;
using System.Windows;

namespace MousEye.CustomControls
{
    /// <summary>
    /// Interaction logic for CustomSpinner.xaml
    /// </summary>
    public partial class CustomSpinner
    {
        private readonly List<int> _list;
        private int _index;

        public CustomSpinner()
        {
            InitializeComponent();
            _list = new List<int> { 15, 30, 60, 75, 100, 125 };
            CurrentValue = _list[_index];
        }

        public static readonly DependencyProperty TitleProperty =
    DependencyProperty.Register("Title", typeof(string),
    typeof(CustomSpinner), new PropertyMetadata(string.Empty));

        public string Title
        {
            get { return GetValue(TitleProperty).ToString(); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty CurrentValueProperty =
    DependencyProperty.Register("CurrentValue", typeof(int),
    typeof(CustomSpinner));

        public int CurrentValue
        {
            get { return (int)GetValue(CurrentValueProperty); }
            set { SetValue(CurrentValueProperty, value); }
        }

        private void OnPreviousClick(object sender, RoutedEventArgs e)
        {
            if (_index != 0)
            {
                CurrentValue = _list[--_index];
            }
        }

        private void OnNextClick(object sender, RoutedEventArgs e)
        {
            if (_index < 5)
            {
                CurrentValue = _list[++_index];
            }
        }
    }
}