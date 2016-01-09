using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Drawing;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace SteamPaver.Common
{
    public static class ImageUtils
    {

        public static BitmapImage LoadLocalImage()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Png images|*.png|Jpg images|*.jpg|Bitmap images|*.bmp";
            if (dialog.ShowDialog() == true)
            {
                var path = dialog.FileName;
                return new BitmapImage(new Uri(path));
            }
            return null;
        }

        //public static BitmapImage SetAspectToWide(BitmapImage bmp)
        //{
        //    if (bmp == null) return null;

        //    var newBmp = new Bitmap(310, 150);
        //    using (Graphics g = Graphics.FromImage(newBmp))
        //    {
        //        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        //        g.DrawImage(bmp, new Rectangle(0, 0, newBmp.Width, newBmp.Height));
        //    }

        //    return newBmp;
        //}

        //public static Bitmap SetAspectToSquare(Bitmap bmp)
        //{
        //    if (bmp == null) return null;

        //    var newBmp = new Bitmap(310, 310);
        //    using(Graphics g = Graphics.FromImage(newBmp))
        //    {
        //        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        //        g.DrawImage(bmp, new Rectangle(0, 0, newBmp.Width, newBmp.Height));
        //    }

        //    return newBmp;
        //}



        //public static Color GetAverageColorUsingHSL(Bitmap bmp)
        //{
        //    int[] hue = new int[360];
        //    float saturation = 0f;
        //    float brightness = 0f;
        //    for (int i = 0; i < bmp.Height; i += (int)bmp.Height / 100)
        //    {
        //        for (int j = 0; j < bmp.Width; j += (int)bmp.Width / 100)
        //        {
        //            Color c = bmp.GetPixel(j, i);
        //            if (c.GetSaturation() > 0.1f && c.GetBrightness() > 0.05f) hue[(int)c.GetHue()]++;
        //            saturation += c.GetSaturation();
        //            brightness += c.GetBrightness();
        //        }
        //    }
        //    brightness /= 10000;
        //    brightness = (float)Math.Sqrt(brightness) / 2;
        //    //brightness = brightness * 2 - brightness * brightness;

        //    saturation /= 10000;
        //    saturation = saturation * 2 - saturation * saturation;
        //    brightness += saturation / 4;
        //    int[] newhue = new int[360];
        //    for (int i = 1; i < 359; i++)
        //    {
        //        newhue[i] = (hue[i] + hue[i - 1] + hue[i + 1]) / 3;
        //    }
        //    int x = 0;
        //    int huemax = 0;
        //    for (int i = 0; i < 360; i++)
        //    {
        //        if (x < newhue[i])
        //        {
        //            x = newhue[i];
        //            huemax = i;
        //        }
        //    }
        //    //Console.WriteLine("hue: {0} sat: {1} bri: {2}", huemax, saturation, brightness);
        //    return ColorFromHSB(huemax, saturation, brightness);
        //}


        //public static Color ColorFromHSB(float hu, float sa, float br)
        //{

        //    float chroma = sa * br;
        //    float hue = hu / 60f;
        //    float x = chroma * (1 - Math.Abs(hue % 2 - 1));
        //    float r = 0f, g = 0f, b = 0f;
        //    if (0f <= hue && hue < 1f)
        //    {
        //        r = chroma;
        //        g = x;
        //        b = 0;
        //    }
        //    if (1f <= hue && hue < 2f)
        //    {
        //        r = x;
        //        g = chroma;
        //        b = 0;
        //    }
        //    if (2f <= hue && hue < 3f)
        //    {
        //        r = 0;
        //        g = chroma;
        //        b = x;
        //    }
        //    if (3f <= hue && hue < 4f)
        //    {
        //        r = 0;
        //        g = x;
        //        b = chroma;
        //    }
        //    if (4f <= hue && hue < 5f)
        //    {
        //        r = x;
        //        g = 0;
        //        b = chroma;
        //    }
        //    if (5f <= hue && hue < 6f)
        //    {
        //        r = chroma;
        //        g = 0;
        //        b = x;
        //    }
        //    //listBox1.Items.Add(x.ToString()+" "+chroma.ToString()+" "+hue.ToString()+" "+r.ToString()+" "+g.ToString()+" "+b.ToString());
        //    float m = br - chroma;
        //    r += m;
        //    g += m;
        //    b += m;
        //    return Color.FromArgb(Convert.ToInt32(r * 255f), Convert.ToInt32(g * 255f), Convert.ToInt32(b * 255f));
        //}


        //public static Bitmap CropToSquare(Bitmap bmp)
        //{
        //    var size=Math.Min(bmp.Width,bmp.Height*2);
        //    var newBmp = new Bitmap(size, size/2);

        //    using (Graphics g = Graphics.FromImage(newBmp))
        //    {
        //        g.DrawImage(bmp,-(bmp.Width-size)/2,-(bmp.Height-size/2)/2);
        //    }
        //    return newBmp;
        //}

        //public static Bitmap CropToWide(Bitmap bmp)
        //{
        //    var size = Math.Min(bmp.Width, bmp.Height);
        //    var newBmp = new Bitmap(size, size);

        //    using (Graphics g = Graphics.FromImage(newBmp))
        //    {
        //        g.DrawImage(bmp, -(bmp.Width - size) / 2, -(bmp.Height - size) / 2);
        //    }
        //    return newBmp;
        //}
    }
}
