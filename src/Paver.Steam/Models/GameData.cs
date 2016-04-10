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
using Paver.Common;
using Paver.Common.Models;
using Paver.Common.Utils;

namespace Paver.Steam
{

    [ImplementPropertyChanged]
    [JsonObject()]
    public class GameData : TileData
    {
        public int GameID { get; }

        public override string AdditionalInfo
        {
            get
            {
                return $"GameID : {GameID}";
            }
        }

        [JsonProperty]
        public bool IsInstalled { get; set; }


        public override string InstanceID
        {
            get
            {
                return $"GameData_{GameID}";
            }
        }

        public override Uri Uri
        {
            get
            {
                return new Uri( $"steam://rungameid/{GameID}");
            }
        }
        

        [JsonConstructor()]
        public GameData(int GameID)
        {
            this.GameID = GameID;

            Color = Colors.Black;

        }

        

        
    }
}
