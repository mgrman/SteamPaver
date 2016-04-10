using Paver.Common.ViewModels;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paver.Common
{
    public class TileDataSelectedEvent: Prism.Events.PubSubEvent<TileDataViewModel>
    {
    }
}
