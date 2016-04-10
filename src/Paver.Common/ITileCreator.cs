using Paver.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Paver.Common
{
    public interface ITileCreator
    {
        void CreateTile(TileSettings tileSettings);

        IEnumerable<LinkTypes> GetSupportedLinkTypes(TileSettings tileSettings);
    }

    public enum LinkTypes
    {
        DirectLink =1,
        ProxyLink=2
    }
}
