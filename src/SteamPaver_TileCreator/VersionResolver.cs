using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SteamPaver.TileCreator
{
    public static class VersionResolver
    {
        static VersionResolver()
        {
            if (Environment.OSVersion.Version.Build > 10586)
                Creator = new Win10TP2TileCreator();
        }

        public static ITileCreator Creator { get; }
    }
}
