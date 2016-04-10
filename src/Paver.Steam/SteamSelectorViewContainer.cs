using Paver.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paver.Steam.Views;

namespace Paver.Steam
{
    public class SteamSelectorViewContainer : BaseSelectorViewContainer<Steam.Views.SteamSelectorView>
    {
        public SteamSelectorViewContainer() 
            : base("Steam Games", new SteamSelectorView())
        {
        }
    }
}
