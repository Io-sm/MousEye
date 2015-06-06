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

        public static InteropBitmap GrayScaleBitmap { get; private set; }

        public static InteropBitmap BinaryBitmap { get; private set; }

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

        public static InteropBitmap Proc(InteropBitmap bmp)
        {
            var bitmap = Invert(bmp);

            return InvertedBitmap = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero,
                Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()) as InteropBitmap;
        }
    }
}