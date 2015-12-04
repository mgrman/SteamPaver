using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SteamPaver.TileCreator
{
    public interface ITileCreator
    {
        void CreateTile(string name, string pathOrUrl, BitmapSource image,Color backgroundColor,bool showLabel,bool useDarkLabel);
    }
}
