using MousEye.ViewModels;
using MousEye.Views;
using System;

namespace MousEye
{
    public static class SettingsManager
    {
        #region PRIVATES

        private static bool _cameraSettingsActive;

        #endregion PRIVATES

        #region CONSTRUCTORS

        public static void CameraSettings(CameraViewModel vm)
        {
            if (_cameraSettingsActive) return;
            _cameraSettingsActive = true;
            var cameraSettingsWindow = new CameraSettings(vm);
            cameraSettingsWindow.Closed += SettingsOnClosed;
            cameraSettingsWindow.Show();
        }

        #endregion CONSTRUCTORS

        #region EVENT HANDLERS

        private static void SettingsOnClosed(object sender, EventArgs eventArgs)
        {
            _cameraSettingsActive = false;
        }

        #endregion EVENT HANDLERS
    }
}