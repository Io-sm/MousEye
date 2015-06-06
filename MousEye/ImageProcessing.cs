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
        private static InteropBitmap _originalBitmap;

        private static byte[] tt;

        public static InteropBitmap OriginalBitmap
        {
            get { return _originalBitmap; }

            set
            {
                if (value == null) return;
                _originalBitmap = value;
            }
        }

        public static InteropBitmap InvertedBitmap { get; private set; }

        public static InteropBitmap BinaryBitmap { get; private set; }

        public static InteropBitmap TestBitmap { get; private set; }

        private static Bitmap Invert(InteropBitmap bmp)
        {
            var temp = bmp;

            var ms = new MemoryStream();
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(temp));
            encoder.Save(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var bitmap = new Bitmap(ms);

            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var data = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var numBytes = data.Stride * bitmap.Height;
            var rgbValues = new byte[numBytes];
            Marshal.Copy(data.Scan0, rgbValues, 0, numBytes);
            bitmap.UnlockBits(data);

            for (var i = 0; i < numBytes; i += 4)
            {
                rgbValues[i] = (byte)(255 - rgbValues[i]);
                rgbValues[i + 1] = (byte)(255 - rgbValues[i + 1]);
                rgbValues[i + 2] = (byte)(255 - rgbValues[i + 2]);
            }

            var bitmapWrite = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
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

            tt = rgbValues;

            var bitmapWrite = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
            Marshal.Copy(rgbValues, 0, bitmapWrite.Scan0, numBytes);
            bmp.UnlockBits(bitmapWrite);

            return bmp;
        }

        private static Bitmap PerformTest(InteropBitmap source)
        {
            var temp = source;

            var ms = new MemoryStream();
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(temp));
            encoder.Save(ms);
            ms.Seek(0, SeekOrigin.Begin);

            var bitmap = new Bitmap(ms);

            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var data = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var numBytes = data.Stride * bitmap.Height;
            var rgbValues = new byte[numBytes];
            Marshal.Copy(data.Scan0, rgbValues, 0, numBytes);
            bitmap.UnlockBits(data);

            for (var i = 0; i < numBytes; i += 4)
            {
                if (tt[i] != 255 || tt[i + 1] != 255 || tt[i + 2] != 255) continue;
                rgbValues[i] = 255;
                rgbValues[i + 1] = 192;
                rgbValues[i + 2] = 203;
            }

            var bitmapWrite = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
            Marshal.Copy(rgbValues, 0, bitmapWrite.Scan0, numBytes);
            bitmap.UnlockBits(bitmapWrite);

            return bitmap;
        }

        public static void Proc(InteropBitmap bmp, double threshold)
        {
            var bitmap = Invert(bmp);

            InvertedBitmap = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero,
                Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()) as InteropBitmap;

            bitmap = ApplyThreshold(bitmap, threshold);

            BinaryBitmap = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero,
                Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()) as InteropBitmap;

            var temp = PerformTest(bmp);

            TestBitmap = Imaging.CreateBitmapSourceFromHBitmap(temp.GetHbitmap(), IntPtr.Zero,
                Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()) as InteropBitmap;
        }
    }
}