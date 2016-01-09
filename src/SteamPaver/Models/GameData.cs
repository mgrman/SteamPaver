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
using SteamPaver.Common;

namespace SteamPaver
{

    [ImplementPropertyChanged]
    [JsonObject()]
    public class GameData : TileData
    {
    

        public int GameID { get; set; }
        

        [JsonProperty]
        public bool Installed { get; private set; }
        
        
        public override string InstanceID
        {
            get
            {
                return $"GameData_{GameID}";
            }
        }

        public override string Shortcut { get
            {
                return $"steam://rungameid/{GameID}";
            }
            }

        [JsonConstructor()]
        public GameData()
        {
            Color = Colors.Black;
        }

        public GameData(int gameID)
            : this()
        {
            GameID = gameID;
            UpdateInstalled();
            UpdateNameFromSteam();
            UpdateImageFromSteam();
        }


        private void UpdateNameFromSteam()
        {
            Name = IdToName.GetName(GameID) ?? $"<Game not found - {GameID}>";
        }

        public void UpdateImageFromSteam()
        {
            string url = String.Format("http://cdn.akamai.steamstatic.com/steam/apps/{0}/header.jpg", GameID);

            App.Current.Dispatcher.InvokeAsync(() =>
            {
                SquareDraft = new BitmapImage(new Uri(url));
                this.SaveToCache();
            });
        }
        

        private void UpdateInstalled()
        {
            Installed = AllInstaledGames.GetGames().Contains(GameID);
        }
    }
}
