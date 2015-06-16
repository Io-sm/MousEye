using MousEye.ViewModels;
using MousEye.Views;
using System;

namespace MousEye
{
    public static class SettingsManager
    {
        private static bool _cameraSettingsActive;
        private static bool _calibrationSettingsActive;

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

        public static void CalibrationSettings(CameraViewModel vm)
        {
            if (_calibrationSettingsActive) return;
            _calibrationSettingsActive = true;
            var calibrationSettingsWindow = new CameraCalibration(vm);
            calibrationSettingsWindow.Closed += CalibrationOnClosed;
            calibrationSettingsWindow.Show();
        }

        private static void CalibrationOnClosed(object sender, EventArgs e)
        {
            _calibrationSettingsActive = false;
        }
    }
}