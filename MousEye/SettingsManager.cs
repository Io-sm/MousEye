using MousEye.ViewModels;
using MousEye.Views;
using System;

namespace MousEye
{
    public static class SettingsManager
    {
        private static bool _cameraSettingsActive;

        public static void CameraSettings(CameraViewModel vm)
        {
            if (_cameraSettingsActive) return;
            _cameraSettingsActive = true;
            var cameraSettingsWindow = new CameraSettings(vm);
            cameraSettingsWindow.Closed += SettingsOnClosed;
            cameraSettingsWindow.Show();
        }

        private static void SettingsOnClosed(object sender, EventArgs eventArgs)
        {
            _cameraSettingsActive = false;
        }
    }
}