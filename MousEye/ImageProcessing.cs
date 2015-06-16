using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace MousEye
{
    public static class ImageProcessing
    {
        #region PRIVATES

        private static byte[] _temporaryByteOrder;

        private static Bitmap _temporaryBitmap;

        private static bool _isFirstBitmap = true;

        private static int[] _rectSize = new int[2];

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

            var maxY = 0;
            var minY = 240;
            var maxX = 0;
            var minX = 320;

            for (var i = 0; i < numBytes; i += 4)
            {
                if (_temporaryByteOrder[i] != 255 || _temporaryByteOrder[i + 1] != 255 ||
                    _temporaryByteOrder[i + 2] != 255) continue;

                rgbValues[i] = 255;
                rgbValues[i + 1] = 192;
                rgbValues[i + 2] = 203;

                if ((i - ((i / 1280)) * 1280) / 4 < minX)
                {
                    minX = ((i - ((i / 1280)) * 1280) / 4);
                }

                if ((i - ((i / 1280)) * 1280) / 4 > maxX)
                {
                    maxX = ((i - ((i / 1280)) * 1280) / 4);
                }

                if (i / 1280 < minY)
                {
                    minY = (i / 1280);
                }

                if (i / 1280 > maxY)
                {
                    maxY = (i / 1280);
                }
            }

            var y1 = (minY * 1280) - 1280;
            var y2 = (maxY * 1280) + 1280;

            for (var i = y1; i < y2; i += 4)
            {
                if ((i - ((i / 1280)) * 1280) / 4 <= minX - 1 || (i - ((i / 1280)) * 1280) / 4 >= maxX + 1) continue;
                if ((i - (i / 1280) * 1280) / 4 > minX && (i - ((i / 1280)) * 1280) / 4 < maxX && i / 1280 > minY - 1 && i / 1280 < maxY)
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

            _rectSize[0] = maxX - minX;
            _rectSize[1] = maxY - minY;

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

            temp[0] = _rectSize[0];
            temp[1] = _rectSize[1];

            return temp;
        }

        #endregion STATIC METHODS
    }
}