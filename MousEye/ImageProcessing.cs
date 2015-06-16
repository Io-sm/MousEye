using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Point = System.Windows.Point;

namespace MousEye
{
    public static class ImageProcessing
    {
        #region PRIVATES

        private static byte[] _temporaryByteOrder;

        private static Bitmap _temporaryBitmap;

        private static bool _isFirstBitmap = true;

        private static readonly int[] RectSize = new int[2];

        private static Point _topLeft;

        private static Point _topRight;

        private static Point _bottomRight;

        private static Point _bottomLeft;

        private static int _miniX;

        private static int _miniY;

        private static int _maxiX;

        private static int _maxiY;

        #endregion PRIVATES

        #region IMAGE PROPERTIES

        public static InteropBitmap InvertedBitmap { get; private set; }

        public static InteropBitmap BinaryBitmap { get; private set; }

        public static InteropBitmap FinalImage { get; private set; }

        public static InteropBitmap CalibrationBitmap { get; private set; }

        #endregion IMAGE PROPERTIES

        #region STATIC METHODS

        private static Bitmap ConvertToBitmap(InteropBitmap sourceBitmap, out byte[] rgbVal, out int numBytes)
        {
            using (var memoryStream = new MemoryStream())
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(sourceBitmap));
                encoder.Save(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                _temporaryBitmap = new Bitmap(memoryStream);
            }

            var rect = new Rectangle(0, 0, _temporaryBitmap.Width, _temporaryBitmap.Height);

            var data = _temporaryBitmap.LockBits(rect, ImageLockMode.ReadWrite, _temporaryBitmap.PixelFormat);
            var bytesNum = data.Stride * _temporaryBitmap.Height;
            var rgbValues = new byte[bytesNum];
            Marshal.Copy(data.Scan0, rgbValues, 0, bytesNum);
            _temporaryBitmap.UnlockBits(data);

            numBytes = bytesNum;
            rgbVal = rgbValues;

            return _temporaryBitmap;
        }

        private static Bitmap InvertImage(InteropBitmap originalBitmap)
        {
            int numBytes;
            byte[] rgbValues;
            var bitmap = ConvertToBitmap(originalBitmap, out rgbValues, out numBytes);

            for (var i = 0; i < numBytes; i += 4)
            {
                rgbValues[i] = (byte)(255 - rgbValues[i]);
                rgbValues[i + 1] = (byte)(255 - rgbValues[i + 1]);
                rgbValues[i + 2] = (byte)(255 - rgbValues[i + 2]);
            }

            var bitmapWrite = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly,
                bitmap.PixelFormat);
            Marshal.Copy(rgbValues, 0, bitmapWrite.Scan0, numBytes);
            bitmap.UnlockBits(bitmapWrite);

            return bitmap;
        }

        private static Bitmap ApplyThreshold(Bitmap bmp, double threshold)
        {
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

            var data = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
            var numBytes = data.Stride * bmp.Height;
            var rgbValues = new byte[numBytes];
            Marshal.Copy(data.Scan0, rgbValues, 0, numBytes);
            bmp.UnlockBits(data);

            for (var i = 0; i < numBytes; i += 4)
            {
                if (rgbValues[i] > threshold * 255)
                {
                    rgbValues[i] = 255;
                }
                else
                {
                    rgbValues[i] = 0;
                }

                if (rgbValues[i + 1] > threshold * 255)
                {
                    rgbValues[i + 1] = 255;
                }
                else
                {
                    rgbValues[i + 1] = 0;
                }

                if (rgbValues[i + 2] > threshold * 255)
                {
                    rgbValues[i + 2] = 255;
                }
                else
                {
                    rgbValues[i + 2] = 0;
                }
            }

            _temporaryByteOrder = rgbValues;

            var bitmapWrite = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly,
                bmp.PixelFormat);
            Marshal.Copy(rgbValues, 0, bitmapWrite.Scan0, numBytes);
            bmp.UnlockBits(bitmapWrite);

            return bmp;
        }

        private static Bitmap FindPupil(InteropBitmap originalBitmap)
        {
            int numBytes;
            byte[] rgbValues;
            var bitmap = ConvertToBitmap(originalBitmap, out rgbValues, out numBytes);

            var _maxY = 0;
            var _minY = 240;
            var _maxX = 0;
            var _minX = 320;

            for (var i = 0; i < numBytes; i += 4)
            {
                if (_temporaryByteOrder[i] != 255 || _temporaryByteOrder[i + 1] != 255 ||
                    _temporaryByteOrder[i + 2] != 255) continue;

                rgbValues[i] = 255;
                rgbValues[i + 1] = 192;
                rgbValues[i + 2] = 203;

                if ((i - ((i / 1280)) * 1280) / 4 < _minX)
                {
                    _minX = ((i - ((i / 1280)) * 1280) / 4);
                }

                if ((i - ((i / 1280)) * 1280) / 4 > _maxX)
                {
                    _maxX = ((i - ((i / 1280)) * 1280) / 4);
                }

                if (i / 1280 < _minY)
                {
                    _minY = (i / 1280);
                }

                if (i / 1280 > _maxY)
                {
                    _maxY = (i / 1280);
                }
            }

            var y1 = (_minY * 1280) - 1280;
            var y2 = (_maxY * 1280) + 1280;

            for (var i = y1; i < y2; i += 4)
            {
                if ((i - ((i / 1280)) * 1280) / 4 <= _minX - 1 || (i - ((i / 1280)) * 1280) / 4 >= _maxX + 1) continue;
                if ((i - (i / 1280) * 1280) / 4 > _minX && (i - ((i / 1280)) * 1280) / 4 < _maxX && i / 1280 > _minY - 1 && i / 1280 < _maxY)
                {
                    continue;
                }
                rgbValues[i] = 0;
                rgbValues[i + 1] = 0;
                rgbValues[i + 2] = 0;
            }

            var bitmapWrite = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly,
                bitmap.PixelFormat);
            Marshal.Copy(rgbValues, 0, bitmapWrite.Scan0, numBytes);
            bitmap.UnlockBits(bitmapWrite);

            RectSize[0] = _maxX - _minX;
            RectSize[1] = _maxY - _minY;

            _maxiX = _maxX;
            _miniX = _minX;
            _maxiY = _maxY;
            _miniY = _minY;

            return bitmap;
        }

        private static Bitmap CalibrateBitmap(InteropBitmap originalBitmap)
        {
            int numBytes;
            byte[] rgbValues;
            var bitmap = ConvertToBitmap(originalBitmap, out rgbValues, out numBytes);

            for (var i = bitmap.Width * 2; i < numBytes; i += bitmap.Width * 4)
            {
                rgbValues[i] = 0;
                rgbValues[i + 1] = 255;
                rgbValues[i + 2] = 255;
            }

            for (var i = bitmap.Height / 2 * 1280; i < bitmap.Height / 2 * 1280 + 1280; i += 4)
            {
                rgbValues[i] = 0;
                rgbValues[i + 1] = 255;
                rgbValues[i + 2] = 255;
            }

            var bitmapWrite = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly,
            bitmap.PixelFormat);
            Marshal.Copy(rgbValues, 0, bitmapWrite.Scan0, numBytes);
            bitmap.UnlockBits(bitmapWrite);

            return bitmap;
        }

        public static void ProcessImage(InteropBitmap bmp, double threshold)
        {
            if (_isFirstBitmap)
            {
                _isFirstBitmap = !_isFirstBitmap;
                return;
            }

            var calibrationBitmap = CalibrateBitmap(bmp);
            var hbitmap = calibrationBitmap.GetHbitmap();

            CalibrationBitmap = Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero,
                Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()) as InteropBitmap;

            CameraDevice.DeleteObject(hbitmap);
            calibrationBitmap.Dispose();

            var bitmap = InvertImage(bmp);
            hbitmap = bitmap.GetHbitmap();

            InvertedBitmap = Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero,
                Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()) as InteropBitmap;

            CameraDevice.DeleteObject(hbitmap);

            bitmap = ApplyThreshold(bitmap, threshold);
            hbitmap = bitmap.GetHbitmap();

            BinaryBitmap = Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero,
                Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()) as InteropBitmap;

            CameraDevice.DeleteObject(hbitmap);
            bitmap.Dispose();

            var temp = FindPupil(bmp);
            hbitmap = temp.GetHbitmap();

            FinalImage = Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero,
                Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()) as InteropBitmap;

            CameraDevice.DeleteObject(hbitmap);
            temp.Dispose();
        }

        public static int[] GetRectSize()
        {
            var temp = new int[2];

            temp[0] = RectSize[0];
            temp[1] = RectSize[1];

            return temp;
        }

        public static void SaveCoords(int value)
        {
            switch (value)
            {
                case 0:
                    _topLeft = new Point(_miniX, _miniY);
                    break;

                case 1:
                    _topRight = new Point(_maxiX, _miniY);
                    break;

                case 2:
                    _bottomRight = new Point(_maxiX, _maxiY);
                    break;

                case 3:
                    _bottomLeft = new Point(_miniX, _maxiY);
                    break;
            }
        }

        #endregion STATIC METHODS
    }
}