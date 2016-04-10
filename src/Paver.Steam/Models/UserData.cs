using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paver.Common;
using Paver.Common.Utils;

namespace Paver.Steam
{
    [ImplementPropertyChanged]
    [JsonObject()]
    public class UserData:ICacheable
    {
        public UserData()
        {
            (this as INotifyPropertyChanged).PropertyChanged +=(o,e)=> this.Fire(SteamCommunityIdChanged);
        }
        
        public string SteamCommunityId { get; set; }

        string ICacheable.InstanceID
        {
            get
            {
                return "UserData";
            }
        }

        public event EventHandler SteamCommunityIdChanged;
    }
}
