using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamPaver
{
    [ImplementPropertyChanged]
    public class UserData
    {
        public string SteamCommunityId { get; set; }
    }
}
