using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Paver.TileCreator
{
    internal static class Utils
    {
        //public static System.Drawing.Bitmap ToBitmap(this BitmapSource bitmapImage)
        //{
        //    using (MemoryStream outStream = new MemoryStream())
        //    {
        //        BitmapEncoder enc = new BmpBitmapEncoder();
        //        enc.Frames.Add(BitmapFrame.Create(bitmapImage));
        //        enc.Save(outStream);
        //        System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

        //        return new System.Drawing.Bitmap(bitmap);
        //    }
        //}
        public static BitmapSource CreateResizedImage(this BitmapSource srcImage, int width, int height, bool stretch)
        {

            double srcRatio = srcImage.Width / srcImage.Height;
            double targetRatio = (double)width / height;

            double newHeight;
            double newWidth;
            if (!stretch && targetRatio > srcRatio)
            {
                newHeight = height;
                newWidth = height * srcRatio;
            }
            else if (!stretch && targetRatio < srcRatio)
            {

                newHeight = width / srcRatio;
                newWidth = width;
            }
            else
            {
                newWidth = width;
                newHeight = height;
            }


            var rect = new Rect((width - newWidth) / 2, (height - newHeight) / 2, newWidth, newHeight);


            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                SolidColorBrush visualBrush = new SolidColorBrush(Colors.Black);
                drawingContext.DrawRectangle(visualBrush, null,
                  new Rect(new Point(), new Size(width, height)));

                var group = new DrawingGroup();
                RenderOptions.SetBitmapScalingMode(group, BitmapScalingMode.HighQuality);
                group.Children.Add(new ImageDrawing(srcImage, rect));
                drawingContext.DrawDrawing(group);
            }

            var resizedImage = new RenderTargetBitmap(
                width, height,         // Resized dimensions
                96, 96,                // Default DPI values
                PixelFormats.Default); // Default pixel format
            resizedImage.Render(drawingVisual);

            return resizedImage;
        }

        public static void SaveAsPng(this BitmapFrame frame, string path)
        {

            PngBitmapEncoder encoder = new PngBitmapEncoder();

            encoder.Frames.Add(frame);

            using (var filestream = new FileStream(path, FileMode.Create))
                encoder.Save(filestream);

        }
        public static void SaveAsPng(this BitmapSource newImage,string path)
        {
            SaveAsPng(BitmapFrame.Create(newImage), path);
        }

        public static string ToHex(this Color col,bool includeAlpha)
        {


            if (includeAlpha)
                return $"#{col.A.ToString("X2")}{col.R.ToString("X2")}{col.G.ToString("X2")}{col.B.ToString("X2")}";
            else
                return $"#{col.R.ToString("X2")}{col.G.ToString("X2")}{col.B.ToString("X2")}";

        }
    }
}
