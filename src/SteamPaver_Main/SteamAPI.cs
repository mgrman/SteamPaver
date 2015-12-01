using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SteamPaver_Main.Steam
{
    public class AllInstaledGames
    {
        private List<int> _games = new List<int>();

        private AllInstaledGames()
        {
            var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");
            var names = key.GetSubKeyNames();

            var gameTasks = new List<GameData>();

            foreach (var name in names)
            {
                if (name.Contains("Steam"))
                {
                    int parsInt;
                    var values = name.Split(' ');
                    if (values.Length > 2 && Int32.TryParse(values[2], out parsInt))
                    {
                        _games.Add(parsInt);
                    }
                }
            }
        }

        private static AllInstaledGames _instance;
        public static IEnumerable<int> GetGames()
        {
            if (_instance == null)
            {
                _instance = new AllInstaledGames();
            }

            return _instance._games;
        }


    }
    public class AllOwnedGames
    {
        private List<int> _games = new List<int>();

        private string _communityId;

        private AllOwnedGames(string comunityId)
        {
            _communityId = comunityId;
            string html;
            using (var webClient = new WebClient())
            {
                html = webClient.DownloadString($"http://steamcommunity.com/id/{comunityId}/games/?tab=all");
            }


            var regex = new Regex("\"appid\":(.*?),");

            var matches = regex.Matches(html);
            foreach (Match match in matches)
            {
                int parsInt;
                if ( Int32.TryParse(match.Groups[1].Value, out parsInt))
                {
                    _games.Add(parsInt);
                }
            }
        }

        private static AllOwnedGames _instance;
        public static IEnumerable<int> GetGames(string comunityId)
        {
            if (_instance == null || _instance._communityId!=comunityId)
            {
                _instance = new AllOwnedGames(comunityId);
            }
            
            return _instance._games;
        }

     
    }

    public class IdToName
    {
        private static Dictionary<int, string> _idToName;
        
        private IdToName()
        {
            string json;
            using (var webClient = new WebClient())
            {
                json = webClient.DownloadString("http://api.steampowered.com/ISteamApps/GetAppList/v2");
            }
            SteamApiContainer account = JsonConvert.DeserializeObject<SteamApiContainer>(json);

            _idToName = account.applist.apps.ToDictionary(o => o.appid, o => o.name);
        }

        private static IdToName _instance;
        public static string GetName(int id)
        {
            if (_instance == null)
            {
                _instance = new IdToName();
            }

            string newName;
            if (!_idToName.TryGetValue(id, out newName))
                return null;

            return newName;
        }

        #region Private Classes

        private class SteamApiContainer
        {
            public Applist applist { get; set; }
        }
        private class Applist
        {
            public List<Game> apps { get; set; }
        }

        private class Game
        {
            public int appid { get; set; }
            public string name { get; set; }
        }

        #endregion
    }

}
