using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Paver.Common.Models
{
    public class TileSettings
    {
        public string Name { get;  }
        public Uri Uri { get; }
        public BitmapSource Image { get;  }
        public Color BackgroundColor { get; }
        public bool ShowLabel { get; }
        public bool UseDarkLabel { get; }
        public LinkTypes LinkType { get; }


        public TileSettings(string name, Uri uri,BitmapSource image, Color backgroundColor, bool showLabel, bool useDarkLabel, LinkTypes linkType)
        {
            Name = name;
            Uri = uri;
            Image = image;
            BackgroundColor = backgroundColor;
            ShowLabel = showLabel;
            UseDarkLabel = useDarkLabel;
            LinkType = linkType;
        }

        public TileSettings(TileData tileData)
        {
            Name = tileData.Name;
            Uri = tileData.Uri;
            Image = tileData.SquareFinal;
            BackgroundColor = tileData.Color;
            ShowLabel = tileData.LabelType != LabelTypes.NoLabel;
            UseDarkLabel = tileData.LabelType == LabelTypes.DarkLabel;
            LinkType = tileData.LinkType;
        }
    }
}
