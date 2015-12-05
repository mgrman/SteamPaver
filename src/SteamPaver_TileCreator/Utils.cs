using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SteamPaver.TileCreator
{
    internal static class Utils
    {
        public static System.Drawing.Bitmap ToBitmap(this BitmapSource bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new System.Drawing.Bitmap(bitmap);
            }
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
