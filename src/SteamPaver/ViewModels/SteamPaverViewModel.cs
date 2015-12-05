using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using Microsoft.Win32;
using System.Collections.Specialized;
using System.Windows.Input;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace SteamPaver
{
    [ImplementPropertyChanged]
    public class SteamPaverViewModel
    {
        public SteamPaverViewModel()
        {
            _refreshCommand = AsyncRelayCommand.Lazy(() => Refresh(), null, true);
            GameDatas = new ObservableCollectionEx<GameData>();

            var cachedGameDatas = CacheManager.LoadFromCache<GameData>()
                    .OrderBy(o => o.Installed ? 0 : 1)
                    .ThenBy(o => o.Name);

            //var cachedGameDatas = new GameData[] { CacheManager.LoadFromCache<GameData>($"GameData_{219740}") };


            foreach (var cachedGameData in cachedGameDatas)
            {
                GameDatas.Add(cachedGameData);
            }

            GameDatas.CollectionChanged += (o, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:

                        foreach(GameData item in e.NewItems)
                            item.SaveToCache();
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (GameData item in e.OldItems)
                            item.RemoveFromCache();
                        break;
                    default:
                        break;
                }
            };

            UserData = CacheManager.LoadFromCache<UserData>().FirstOrDefault();
            if (UserData == null)
            {
                UserData = new UserData();
                UserData.SaveToCache();
            }


            UserData.PropertyChanged += (o, e) =>
            {
                UserData.SaveToCache();
            };
            App.Current.Exit += (o, e) =>
            {
                UserData.SaveToCache();
                if (SelectedGameData != null)
                    SelectedGameData.SaveToCache();

            };

        }

        public ObservableCollectionEx<GameData> GameDatas { get; private set; }

        private GameData _selectedGameData;
        public GameData SelectedGameData
        {
            get { return _selectedGameData; }
            set
            {
                if (_selectedGameData == value) return;

                var oldGameData = _selectedGameData;
                if(oldGameData!= null)
                    oldGameData.SaveToCache();

                _selectedGameData = value;
            }
        }

        private Lazy<AsyncRelayCommand> _refreshCommand;
        public ICommand RefreshCommand { get { return _refreshCommand.Value; } }

        public UserData UserData { get; private set; }

        public async Task Refresh()
        {
            GameDatas.SuspendCollectionChanged = true;
            var games = Task.Run<IEnumerable<GameData>>(() =>
            {
                return Steam.AllOwnedGames.GetGames(UserData.SteamCommunityId)
                    .Select(o => new GameData(o))
                    .OrderBy(o => o.Installed ? 0 : 1)
                    .ThenBy(o => o.Name)
                    .ToList();
            });

            GameDatas.Clear();

            foreach (var game in (await games))
            {
                GameDatas.Add(game);
                game.SaveToCache();
            }
            GameDatas.SuspendCollectionChanged = false;
            GameDatas.RefreshBinding();
        }


        public ICommand CreateTileForSelfCommand { get
            {
                return new RelayCommand(() =>
                {
                    var creator = TileCreator.VersionResolver.Creator;

                    var path = System.Reflection.Assembly.GetExecutingAssembly().Location;

                    var bitmapPath = Path.Combine(Path.GetDirectoryName(path), "Resources", "SteamPaver_light.png");
                    var bitmap = new BitmapImage(new Uri(bitmapPath));
                    creator.CreateTile("SteamPaver", path, bitmap, Colors.Transparent, false, true);
                });
            } }
    }

    public class ObservableCollectionEx<T> : ObservableCollection<T>
    {
        public bool SuspendCollectionChanged { get; set; }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (SuspendCollectionChanged)
                return;

            base.OnCollectionChanged(e);
        }


        public void RefreshBinding()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

        }
    }
}
