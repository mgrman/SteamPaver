using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamPaver.Common;

namespace SteamPaver
{
    [ImplementPropertyChanged]
    [JsonObject()]
    public class UserData:ICacheable,INotifyPropertyChanged
    {
        
        public string SteamCommunityId { get; set; }

        string ICacheable.InstanceID
        {
            get
            {
                return "UserData";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
