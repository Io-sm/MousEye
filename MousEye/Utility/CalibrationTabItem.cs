using System.Windows;

namespace MousEye.Utility
{
    public class CalibrationTabItem
    {
        #region PROPERTIES

        public string TabHeader { get; set; }

        public UIElement TabContent { get; set; }

        #endregion PROPERTIES

        #region CONSTRUCTORS

        public CalibrationTabItem(string tabHeader, UIElement tabContent)
        {
            TabHeader = tabHeader;
            TabContent = tabContent;
        }

        #endregion CONSTRUCTORS
    }
}