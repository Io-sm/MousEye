﻿using MousEye.ViewModels;

namespace MousEye.Views
{
    public partial class CameraSettings
    {
        public CameraSettings(CameraViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}