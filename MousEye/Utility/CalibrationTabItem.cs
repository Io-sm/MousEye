using System.Windows;

namespace MousEye.Utility
{
    public class CalibrationTabItem
    {
        public string TabHeader { get; set; }

        public UIElement TabContent { get; set; }

        public CalibrationTabItem(string tabHeader, UIElement tabContent)
        {
            TabHeader = tabHeader;
            TabContent = tabContent;
        }
    }
}