using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Paver.Common;
using System.Windows.Media.Imaging;
using Gameloop.Vdf;
using Gameloop.Vdf.Linq;
using System.IO;

namespace Paver.Steam
{
    public class SteamAPI
    {
        public string CommunityId { get; }

        public SteamAPI(string communityId,bool refresh=false)
        {
            CommunityId = communityId;
            
            if (refresh)
                Refresh();


        }
      
        private IReadOnlyCollection<int> GetOwnedGameIds()
        {
            HashSet<int> games = new HashSet<int>();

            string html;
            using (var webClient = new WebClient())
            {
                html = webClient.DownloadString($"http://steamcommunity.com/id/{CommunityId}/games/?tab=all");
            }


            var regex = new Regex("\"appid\":(.*?),");

            var matches = regex.Matches(html);
            foreach (Match match in matches)
            {
                int parsInt;
                if (Int32.TryParse(match.Groups[1].Value, out parsInt))
                {
                    games.Add(parsInt);
                }
            }

            return games;
        }

        private IReadOnlyCollection<int> GetInstalledGameIds()
        {
            try
            {
                var libraryFoldersConfigPath = @"C:\Program Files (x86)\Steam\steamapps\libraryfolders.vdf";

            var libraryFoldersConfig= VdfConvert.Deserialize(File.ReadAllText(libraryFoldersConfigPath));
            var appIds = new List<int>();
            foreach (VProperty folder in libraryFoldersConfig.Value)
            {
                try
                {
                    if (Regex.IsMatch(folder.Key, @"\d+"))
                    {
                        var libraryFolder = folder.Value.ToString();

                        var manifestFilePaths = Directory.GetFiles(libraryFolder, "appmanifest_*.acf", SearchOption.AllDirectories);

                        foreach (var manifestFilePath in manifestFilePaths)
                        {
                            try
                            {
                                dynamic manifest = VdfConvert.Deserialize(File.ReadAllText(manifestFilePath));

                                var appId = int.Parse(manifest.Value.appid.ToString());

                                appIds.Add(appId);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return appIds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Array.Empty<int>();
            }
        }

        private bool GetIsInstalled(int gameId)
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
                    if (values.Length > 2 && Int32.TryParse(values[2], out parsInt) && parsInt== gameId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static IReadOnlyDictionary<int,string> GetIdsToNames()
        {

            string json;
            using (var webClient = new WebClient())
            {
                json = webClient.DownloadString("http://api.steampowered.com/ISteamApps/GetAppList/v2");
            }
            SteamApiContainer account = JsonConvert.DeserializeObject<SteamApiContainer>(json);

            return account.applist.apps.ToDictionary(o => o.appid, o => o.name);
            
        }

        private static Lazy<BitmapSource> GetBannerlazy(int gameId)
        {
            return new Lazy<BitmapSource>(() =>
            {
                string url = $"http://cdn.akamai.steamstatic.com/steam/apps/{gameId}/header.jpg";
                return new BitmapImage(new Uri(url));
            });
        }

        private Dictionary<int, SteamGameData> _games = new Dictionary<int,SteamGameData>();

        public IReadOnlyDictionary<int,SteamGameData> Games => _games;

        public void Refresh()
        {
            _games.Clear();

            var ownedGames = GetOwnedGameIds();
            var installedGames = GetInstalledGameIds();
            var toName = GetIdsToNames();

            foreach(var ownedGame in ownedGames)
            {
                string name;
                if (!toName.TryGetValue(ownedGame, out name))
                    name = $"Unknown GameID : {ownedGame.ToString()}";

                bool isInstalled = installedGames.Contains(ownedGame);

                var banner = GetBannerlazy(ownedGame);

                _games[ownedGame]=new SteamGameData(ownedGame,isInstalled, name,banner);
            }
        }

        public Lazy<BitmapSource> RefreshBanner(int gameId)
        {
            Lazy<BitmapSource> banner = GetBannerlazy(gameId);
            if (_games.ContainsKey(gameId))
            {

                var game = _games[gameId];

                var newGame = new SteamGameData(game.Id, game.IsInstalled, game.Name, GetBannerlazy(game.Id));

                _games[gameId] = newGame;
            }
            return banner;
        }

        public bool RefreshIsInstalled(int gameId)
        {
            bool isInstalled = GetIsInstalled(gameId);
            if (_games.ContainsKey(gameId))
            {
                var game = _games[gameId];

                var newGame = new SteamGameData(game.Id, isInstalled, game.Name, game.Banner);

                _games[gameId] = newGame;
            }
            return isInstalled;
        }

        public class SteamGameData
        {
            public int Id { get; }
            public bool IsInstalled { get; }
            public string Name { get; }
            public Lazy<BitmapSource> Banner { get; }
            

            internal SteamGameData(int id,bool isInstalled,string name,Lazy<BitmapSource> banner)
            {
                Id = id;
                IsInstalled = isInstalled;
                Name = name;
                Banner = banner;
            }

            public override bool Equals(object obj)
            {
                return (obj as SteamGameData)?.Id == Id;
            }

            public override int GetHashCode()
            {
                return Id;
            }
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
