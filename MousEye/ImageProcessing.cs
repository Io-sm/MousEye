using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

namespace MousEye
{
    public static class ImageProcessing
    {
        private static InteropBitmap _originalBitmap;

        private static Canvas can;

        public static void GetCan(Canvas _can)
        {
            can = _can;
        }

        public static InteropBitmap OriginalBitmap
        {
            get { return _originalBitmap; }

            set
            {
                if (value == null) return;
                _originalBitmap = value;
                //Proc();
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

        public static void Proc(InteropBitmap bmp)
        {
            var bitmap = Invert(bmp);

            InvertedBitmap = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero,
                Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            var temp = new ImageBrush
            {
                ImageSource = InvertedBitmap
            };

            //can.Background = temp;
        }
    }
}