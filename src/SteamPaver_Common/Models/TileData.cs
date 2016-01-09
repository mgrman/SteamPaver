using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using PropertyChanged;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Newtonsoft.Json;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Interop;
using SteamPaver.Common;

namespace SteamPaver.Common
{
    public enum LabelTypes
    {
        NoLabel,
        LightLabel,
        DarkLabel
    }


    [ImplementPropertyChanged]
    [JsonObject()]
    public abstract class TileData : ICacheable
    {
        public string Name { get; set; }
        
        private bool _deserializingSquareDraft;

        private BitmapSource _squareDraft;
        [JsonIgnore]
        public BitmapSource SquareDraft
        {
            get
            {
                return _squareDraft;
            }
            set
            {
                if (value == _squareDraft)
                    return;
                _squareDraft = value;

                if (_squareDraft == null)
                {
                    CroppingRectangle = new Rect(0, 0, 100, 100);
                    SquareFinal = null;
                }
                else
                {
                    if (_deserializingSquareDraft)
                    {
                        if (SquareDraft.IsDownloading)
                        {
                            SquareDraft.DownloadCompleted += (o, e) =>
                            {
                                CropDraft();
                            };
                        }
                        else
                        {
                            CropDraft();
                        }
                    }
                    else
                    {
                        if (SquareDraft.IsDownloading)
                        {
                            SquareDraft.DownloadCompleted += (o, e) =>
                            {
                                SetCroppingRectBaseOnImage();
                            };
                        }
                        else
                        {
                            SetCroppingRectBaseOnImage();
                        }
                    }
                }
            }
        }

        private void SetCroppingRectBaseOnImage()
        {

            double minSize = Math.Min(_squareDraft.Width, _squareDraft.Height);
            CroppingRectangle = new Rect(0, 0, minSize, minSize);

        }

        [JsonProperty]
        protected string SquareDraftBytes
        {
            get
            {
                if (SquareDraft == null)
                    return null;
                StringBuilder sb = new StringBuilder();
                if (SquareDraft is InteropBitmap || ((SquareDraft as BitmapImage)?.UriSource == null))
                {
                    sb.Append("PixelBitmap ");

                    byte[] bit = new byte[0];

                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(SquareDraft));
                    using (MemoryStream stream = new MemoryStream())
                    {
                        encoder.Frames.Add(BitmapFrame.Create(SquareDraft));
                        encoder.Save(stream);
                        bit = stream.ToArray();
                        stream.Close();
                    }

                    sb.Append(Convert.ToBase64String(bit));
                }
                else if (SquareDraft is BitmapImage)
                {
                    var im = SquareDraft as BitmapImage;


                    sb.Append("UriBitmap ");

                    sb.Append(im.UriSource);
                }


                return sb.ToString();
            }
            set
            {
                _deserializingSquareDraft = true;
                try
                {
                    if (value == null)
                    {
                        SquareDraft = null;
                        return;
                    }
                    var pixelBitmapTag = "PixelBitmap ";
                    var uriBitmapTag = "UriBitmap ";
                    if (value.StartsWith(pixelBitmapTag))
                    {

                        var bytes = Convert.FromBase64String(value.Substring(pixelBitmapTag.Length));

                        using (var ms = new System.IO.MemoryStream(bytes))
                        {
                            var image = new BitmapImage();
                            image.BeginInit();
                            image.CacheOption = BitmapCacheOption.OnLoad; // here
                            image.StreamSource = ms;
                            image.EndInit();
                            SquareDraft = image;
                        }
                    }
                    else if (value.StartsWith(uriBitmapTag))
                    {
                        var uri = value.Substring(uriBitmapTag.Length);
                        try
                        {
                            SquareDraft = new BitmapImage(new Uri(uri));
                        }
                        catch { }
                    }
                }
                finally
                {
                    _deserializingSquareDraft = false;
                }
            }
        }

        private Rect _croppingRectangle;
        public Rect CroppingRectangle
        {
            get
            {
                return _croppingRectangle;
            }
            set
            {
                if (_croppingRectangle == value) return;
                _croppingRectangle = value;

                CropDraft();
            }
        }

        [JsonIgnore]
        public BitmapSource SquareFinal { get; private set; }

        private Color _color;
        public Color Color
        {
            get { return _color; }
            set
            {
                value.A = 255;
                if (_color == value) return;
                _color = value;
            }
        }

        public LabelTypes LabelType { get; set; }

        public bool ShowLabel { get { return LabelType != LabelTypes.NoLabel; } }

        public Color LabelColor
        {
            get
            {
                switch (LabelType)
                {
                    case LabelTypes.LightLabel:
                        return Colors.White;
                    case LabelTypes.NoLabel:
                    default:
                    case LabelTypes.DarkLabel:
                        return Colors.Black;

                }
            }
        }
        


        [JsonIgnore]
        public abstract string InstanceID { get; }

        [JsonConstructor()]
        public TileData()
        {
            Color = Colors.Black;
        }

        public abstract string Shortcut{get;}
        
        public string SetFinalAsTile()
        {
            if (SquareFinal == null)
                throw new InvalidOperationException("SquareFinal cannot be Null when calling this method");

            var tileCreator = SteamPaver.TileCreator.VersionResolver.Creator;
            if (tileCreator == null)
                throw new InvalidOperationException("No Tile creator available for this Windows version!");

            return tileCreator.CreateTile(Name, Shortcut, SquareFinal, Color, LabelType != LabelTypes.NoLabel, LabelType == LabelTypes.DarkLabel);
        }
        
        private void CropDraft()
        {
            if (SquareDraft == null || SquareDraft.IsDownloading)
                return;

            var rectangle = CroppingRectangle;

            double scaleX = SquareDraft.DpiX / 96;
            double scaleY = SquareDraft.DpiY / 96;

            var imgRect = new Rect(0, 0, SquareDraft.Width, SquareDraft.Height);

            rectangle.Intersect(imgRect);
            rectangle.Scale(scaleX, scaleY);

            if (rectangle == Rect.Empty)
                rectangle = new Rect(0, 0, 1, 1);

            var intRect = new Int32Rect((int)rectangle.X, (int)rectangle.Y, (int)Math.Round(rectangle.Width), (int)Math.Round(rectangle.Height));

            try
            {
                var croppedBmp = new CroppedBitmap(SquareDraft, intRect);


                double scale = 1;
                if (intRect.Width != intRect.Height)
                {
                    scale = (double)intRect.Height / intRect.Width;
                }

                SquareFinal = new TransformedBitmap(croppedBmp, new ScaleTransform(scale, 1));
            }
            catch { }
        }
        
    }
}
