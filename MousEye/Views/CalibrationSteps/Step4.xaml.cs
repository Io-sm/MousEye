﻿using MousEye.ViewModels;

namespace MousEye.Views.CalibrationSteps
{
    public partial class Step4
    {
        public Step4(CameraViewModel vm)
        {
            InitializeComponent();

            DataContext = vm;
        }
    }
}