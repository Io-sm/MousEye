using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

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

        public static BitmapSource InvertedBitmap { get; private set; }

        public static InteropBitmap GrayScaleBitmap { get; private set; }

        public static InteropBitmap BinaryBitmap { get; private set; }

        private static Bitmap Invert(InteropBitmap bmp)
        {
            var temp = bmp;
            Color c;

            var ms = new MemoryStream();
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(temp));
            encoder.Save(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var bitmap = new Bitmap(ms);

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    c = bitmap.GetPixel(i, j);
                    bitmap.SetPixel(i, j, Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B));
                }
            }

            return bitmap;
        }

        public static BitmapSource Proc(InteropBitmap bmp)
        {
            var bitmap = Invert(bmp);

            return InvertedBitmap = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero,
                Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
    }
}