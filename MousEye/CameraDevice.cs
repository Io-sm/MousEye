using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace MousEye
{
    #region CAMERA PARAMETERS

    public enum CameraColorMode                                 //Tryb obrazu
    {
        MONO_RAW,
        MONO_PROCESSED
    };

    public enum CameraResolution                                //Rozdzielczość kamery
    {
        QVGA,                                                   //320x240
        VGA                                                     //640x480
    };

    public enum CameraParameter                                 //Parametry kamery
    {
        // parametry sensora
        AUTO_GAIN,			                                    // [false, true]

        GAIN,					                                // [0, 79]
        AUTO_EXPOSURE,		                                    // [false, true]
        EXPOSURE,				                                // [0, 511]
        AUTO_WHITEBALANCE,	                                    // [false, true]
        WHITEBALANCE_RED,		                                // [0, 255]
        WHITEBALANCE_GREEN,	                                    // [0, 255]
        WHITEBALANCE_BLUE,	                                    // [0, 255]

        // parametry transformacji kamery
        HFLIP,				                                    // [false, true]

        VFLIP,				                                    // [false, true]
        HKEYSTONE,			                                    // [-500, 500]
        VKEYSTONE,			                                    // [-500, 500]
        XOFFSET,				                                // [-500, 500]
        YOFFSET,				                                // [-500, 500]
        ROTATION,				                                // [-500, 500]
        ZOOM,					                                // [-500, 500]

        // inne parametry kamery
        LENSCORRECTION1,		                                // [-500, 500]

        LENSCORRECTION2,		                                // [-500, 500]
        LENSCORRECTION3,		                                // [-500, 500]
        LENSBRIGHTNESS		                                    // [-500, 500]
    };

    #endregion CAMERA PARAMETERS

    public class CameraDevice : DependencyObject, IDisposable
    {
        #region DLLs

        [DllImport("CLEyeMulticam.dll")]
        public static extern int CLEyeGetCameraCount();

        [DllImport("CLEyeMulticam.dll")]
        public static extern Guid CLEyeGetCameraUUID(int camId);

        [DllImport("CLEyeMulticam.dll")]
        public static extern IntPtr CLEyeCreateCamera(Guid camUuid, CameraColorMode mode, CameraResolution res, float frameRate);

        [DllImport("CLEyeMulticam.dll")]
        public static extern bool CLEyeDestroyCamera(IntPtr camera);

        [DllImport("CLEyeMulticam.dll")]
        public static extern bool CLEyeCameraStart(IntPtr camera);

        [DllImport("CLEyeMulticam.dll")]
        public static extern bool CLEyeCameraStop(IntPtr camera);

        [DllImport("CLEyeMulticam.dll")]
        public static extern bool CLEyeCameraLED(IntPtr camera, bool on);

        [DllImport("CLEyeMulticam.dll")]
        public static extern bool CLEyeSetCameraParameter(IntPtr camera, CameraParameter param, int value);

        [DllImport("CLEyeMulticam.dll")]
        public static extern int CLEyeGetCameraParameter(IntPtr camera, CameraParameter param);

        [DllImport("CLEyeMulticam.dll")]
        public static extern bool CLEyeCameraGetFrameDimensions(IntPtr camera, ref int width, ref int height);

        [DllImport("CLEyeMulticam.dll")]
        public static extern bool CLEyeCameraGetFrame(IntPtr camera, IntPtr pData, int waitTimeout);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpFileMappingAttributes, uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool UnmapViewOfFile(IntPtr hMap);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hHandle);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        #endregion DLLs

        #region PRIVATE

        private IntPtr _map = IntPtr.Zero;
        private IntPtr _section = IntPtr.Zero;
        private IntPtr _camera = IntPtr.Zero;
        private bool _running;
        private Thread _workerThread;

        #endregion PRIVATE

        #region EVENTS

        public event EventHandler BitmapReady;

        #endregion EVENTS

        #region CAMERA PROPERTIES

        public float Framerate { get; set; }

        public CameraColorMode ColorMode { get; set; }

        public CameraResolution Resolution { get; set; }

        public bool AutoGain
        {
            get
            {
                return CLEyeGetCameraParameter(_camera, CameraParameter.AUTO_GAIN) != 0;
            }
            set
            {
                CLEyeSetCameraParameter(_camera, CameraParameter.AUTO_GAIN, value ? 1 : 0);
            }
        }

        public int Gain
        {
            get
            {
                return CLEyeGetCameraParameter(_camera, CameraParameter.GAIN);
            }
            set
            {
                CLEyeSetCameraParameter(_camera, CameraParameter.GAIN, value);
            }
        }

        public bool AutoExposure
        {
            get
            {
                return CLEyeGetCameraParameter(_camera, CameraParameter.AUTO_EXPOSURE) != 0;
            }
            set
            {
                CLEyeSetCameraParameter(_camera, CameraParameter.AUTO_EXPOSURE, value ? 1 : 0);
            }
        }

        public int Exposure
        {
            get
            {
                return CLEyeGetCameraParameter(_camera, CameraParameter.EXPOSURE);
            }
            set
            {
                CLEyeSetCameraParameter(_camera, CameraParameter.EXPOSURE, value);
            }
        }

        public bool AutoWhiteBalance
        {
            get
            {
                return CLEyeGetCameraParameter(_camera, CameraParameter.AUTO_WHITEBALANCE) != 0;
            }
            set
            {
                CLEyeSetCameraParameter(_camera, CameraParameter.AUTO_WHITEBALANCE, value ? 1 : 0);
            }
        }

        public int WhiteBalanceRed
        {
            get
            {
                return CLEyeGetCameraParameter(_camera, CameraParameter.WHITEBALANCE_RED);
            }
            set
            {
                CLEyeSetCameraParameter(_camera, CameraParameter.WHITEBALANCE_RED, value);
            }
        }

        public int WhiteBalanceGreen
        {
            get
            {
                return CLEyeGetCameraParameter(_camera, CameraParameter.WHITEBALANCE_GREEN);
            }
            set
            {
                CLEyeSetCameraParameter(_camera, CameraParameter.WHITEBALANCE_GREEN, value);
            }
        }

        public int WhiteBalanceBlue
        {
            get
            {
                return CLEyeGetCameraParameter(_camera, CameraParameter.WHITEBALANCE_BLUE);
            }
            set
            {
                CLEyeSetCameraParameter(_camera, CameraParameter.WHITEBALANCE_BLUE, value);
            }
        }

        public bool HorizontalFlip
        {
            get
            {
                return CLEyeGetCameraParameter(_camera, CameraParameter.HFLIP) != 0;
            }
            set
            {
                CLEyeSetCameraParameter(_camera, CameraParameter.HFLIP, value ? 1 : 0);
            }
        }

        public bool VerticalFlip
        {
            get
            {
                return CLEyeGetCameraParameter(_camera, CameraParameter.VFLIP) != 0;
            }
            set
            {
                CLEyeSetCameraParameter(_camera, CameraParameter.VFLIP, value ? 1 : 0);
            }
        }

        public int HorizontalKeystone
        {
            get
            {
                return CLEyeGetCameraParameter(_camera, CameraParameter.HKEYSTONE);
            }
            set
            {
                CLEyeSetCameraParameter(_camera, CameraParameter.HKEYSTONE, value);
            }
        }

        public int VerticalKeystone
        {
            get
            {
                return CLEyeGetCameraParameter(_camera, CameraParameter.VKEYSTONE);
            }
            set
            {
                CLEyeSetCameraParameter(_camera, CameraParameter.VKEYSTONE, value);
            }
        }

        public int XOffset
        {
            get
            {
                return CLEyeGetCameraParameter(_camera, CameraParameter.XOFFSET);
            }
            set
            {
                CLEyeSetCameraParameter(_camera, CameraParameter.XOFFSET, value);
            }
        }

        public int YOffset
        {
            get
            {
                return CLEyeGetCameraParameter(_camera, CameraParameter.YOFFSET);
            }
            set
            {
                CLEyeSetCameraParameter(_camera, CameraParameter.YOFFSET, value);
            }
        }

        public int Rotation
        {
            get
            {
                return CLEyeGetCameraParameter(_camera, CameraParameter.ROTATION);
            }
            set
            {
                CLEyeSetCameraParameter(_camera, CameraParameter.ROTATION, value);
            }
        }

        public int Zoom
        {
            get
            {
                return CLEyeGetCameraParameter(_camera, CameraParameter.ZOOM);
            }
            set
            {
                CLEyeSetCameraParameter(_camera, CameraParameter.ZOOM, value);
            }
        }

        public int LensCorrection1
        {
            get
            {
                return CLEyeGetCameraParameter(_camera, CameraParameter.LENSCORRECTION1);
            }
            set
            {
                CLEyeSetCameraParameter(_camera, CameraParameter.LENSCORRECTION1, value);
            }
        }

        public int LensCorrection2
        {
            get
            {
                return CLEyeGetCameraParameter(_camera, CameraParameter.LENSCORRECTION2);
            }
            set
            {
                CLEyeSetCameraParameter(_camera, CameraParameter.LENSCORRECTION2, value);
            }
        }

        public int LensCorrection3
        {
            get
            {
                return CLEyeGetCameraParameter(_camera, CameraParameter.LENSCORRECTION3);
            }
            set
            {
                CLEyeSetCameraParameter(_camera, CameraParameter.LENSCORRECTION3, value);
            }
        }

        public int LensBrightness
        {
            get
            {
                return CLEyeGetCameraParameter(_camera, CameraParameter.LENSBRIGHTNESS);
            }
            set
            {
                CLEyeSetCameraParameter(_camera, CameraParameter.LENSBRIGHTNESS, value);
            }
        }

        #endregion CAMERA PROPERTIES

        #region STATIC METHODS

        public static int CameraCount { get { return CLEyeGetCameraCount(); } }

        public static Guid CameraUuid(int idx)
        {
            return CLEyeGetCameraUUID(idx);
        }

        #endregion STATIC METHODS

        #region DEPENDENCY PROPERTIES

        public InteropBitmap BitmapSource
        {
            get { return (InteropBitmap)GetValue(BitmapSourceProperty); }
            private set { SetValue(BitmapSourcePropertyKey, value); }
        }

        private static readonly DependencyPropertyKey BitmapSourcePropertyKey =
            DependencyProperty.RegisterReadOnly("BitmapSource", typeof(InteropBitmap), typeof(CameraDevice), new UIPropertyMetadata(default(InteropBitmap)));

        public static readonly DependencyProperty BitmapSourceProperty = BitmapSourcePropertyKey.DependencyProperty;

        #endregion DEPENDENCY PROPERTIES

        #region METHODS

        public CameraDevice()
        {
            Framerate = 15;
            ColorMode = default(CameraColorMode);
            Resolution = default(CameraResolution);
        }

        ~CameraDevice()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();
            }
            Destroy();
        }

        public void Create(Guid cameraGuid)
        {
            int w = 0, h = 0;
            _camera = CLEyeCreateCamera(cameraGuid, ColorMode, Resolution, Framerate);
            if (_camera == IntPtr.Zero) return;
            CLEyeCameraGetFrameDimensions(_camera, ref w, ref h);

            var imageSize = (uint)w * (uint)h;
            _section = CreateFileMapping(new IntPtr(-1), IntPtr.Zero, 0x04, 0, imageSize, null);
            _map = MapViewOfFile(_section, 0xF001F, 0, 0, imageSize);
            BitmapSource = Imaging.CreateBitmapSourceFromMemorySection(_section, w, h, PixelFormats.Gray8, w, 0) as InteropBitmap;

            if (BitmapReady != null) BitmapReady(this, null);
            if (BitmapSource != null) BitmapSource.Invalidate();
        }

        public void Destroy()
        {
            if (_map != IntPtr.Zero)
            {
                UnmapViewOfFile(_map);
                _map = IntPtr.Zero;
            }
            if (_section == IntPtr.Zero) return;
            CloseHandle(_section);
            _section = IntPtr.Zero;
        }

        public void Start()
        {
            _running = true;
            _workerThread = new Thread(CaptureThread);
            _workerThread.Start();
        }

        public void Stop()
        {
            if (!_running) return;
            _running = false;
            _workerThread.Join(1000);
        }

        private void CaptureThread()
        {
            CLEyeCameraStart(_camera);

            while (_running)
            {
                if (!CLEyeCameraGetFrame(_camera, _map, 500))
                    continue;
                if (!_running)
                    break;
                Dispatcher.BeginInvoke(DispatcherPriority.Render, (SendOrPostCallback)delegate
                {
                    BitmapSource.Invalidate();
                }, null);
            }

            CLEyeCameraStop(_camera);
            CLEyeDestroyCamera(_camera);
        }

        #endregion METHODS
    }
}