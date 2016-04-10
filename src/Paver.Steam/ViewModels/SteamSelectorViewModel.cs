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
using Paver.Common;
using System.Windows;
using Prism.Events;
using Paver.Common.Utils;
using Paver.Common.ViewModels;

namespace Paver.Steam.ViewModels
{

    [ImplementPropertyChanged]
    public class SteamSelectorViewModel
    {
        private readonly TileDataSelectedEvent _tileDataSelectedEvent;
        private readonly ITileCreator _tileCreator;

        public SteamSelectorViewModel(IEventAggregator eventAggregator, ITileCreator tileCreator)
        {
            _tileDataSelectedEvent = eventAggregator.GetEvent<TileDataSelectedEvent>();
            _tileDataSelectedEvent.Subscribe(o =>
            {
                try
                {
                    _reactingToTileDataSelectedEvent = true;
                    SelectedGameData = o as GameDataViewModel;
                }
                finally
                {
                    _reactingToTileDataSelectedEvent = false;
                }
            });
            _tileCreator = tileCreator;

            RefreshCommand = new AsyncRelayCommand(() => Refresh(), null, true);


            UserData = CacheManager.LoadFromCache<UserData>().FirstOrDefault() ?? new UserData();
            SteamAPI = new SteamAPI(UserData.SteamCommunityId);


            UserData.SteamCommunityIdChanged += async (o, e) =>
             {
                 await UserData.SaveToCacheAsync();
                 SteamAPI = new SteamAPI(UserData.SteamCommunityId);
             };



            var cachedGameDatas = CacheManager.LoadFromCache<GameData>()
                    .OrderBy(o => o.IsInstalled ? 0 : 1)
                    .ThenBy(o => o.Name)
                    .Select(o => new GameDataViewModel(o, _tileCreator, SteamAPI));

            GameDatas.AddRange(cachedGameDatas);

            GameDatas.CollectionChanged += async (o, e) =>
             {
                 switch (e.Action)
                 {
                     case NotifyCollectionChangedAction.Add:

                         foreach (GameData item in e.NewItems)
                             await item.SaveToCacheAsync();
                         break;
                     case NotifyCollectionChangedAction.Remove:
                         foreach (GameData item in e.OldItems)
                             await item.RemoveFromCacheAsync();
                         break;
                     default:
                         break;
                 }
             };


            Application.Current.Exit += (o, e) =>
            {
                UserData.SaveToCache();
                if (SelectedGameData != null)
                    SelectedGameData.GameModel.SaveToCache();

            };


        }

        private SteamAPI _steamAPI;
        public SteamAPI SteamAPI
        {
            get { return _steamAPI; }
            private set
            {
                if (_steamAPI == value) return;
                _steamAPI = value;

                GameDatas.Clear();
            }
        }

        private bool _reactingToTileDataSelectedEvent;
        private void FireTileDataSelectedEvent()
        {
            if (_reactingToTileDataSelectedEvent)
                return;

            _tileDataSelectedEvent.Publish(SelectedGameData);
        }


        public ObservableCollectionEx<GameDataViewModel> GameDatas { get; } = new ObservableCollectionEx<GameDataViewModel>();



        private GameDataViewModel _selectedGameData;
        public GameDataViewModel SelectedGameData
        {
            get { return _selectedGameData; }
            set
            {
                if (_selectedGameData == value) return;

                var oldGameData = _selectedGameData;
                if (oldGameData != null)
                    oldGameData.GameModel.SaveToCache();

                _selectedGameData = value;
                FireTileDataSelectedEvent();
            }
        }

        public ICommand RefreshCommand { get; }

        public UserData UserData { get; }

        public async Task Refresh()
        {
            await Task.Run(() => SteamAPI.Refresh());

            var games = SteamAPI.Games.Values
                .Select(o => new GameData(o.Id) { IsInstalled = o.IsInstalled, Name = o.Name, SquareDraft = o.Banner.Value })
                .OrderBy(o => o.IsInstalled ? 0 : 1)
                .ThenBy(o => o.Name)
                .Select(o => new GameDataViewModel(o, _tileCreator, SteamAPI))
                .ToList();

            var oldSelection = SelectedGameData?.GameModel?.GameID;

            GameDatas.ReplaceAndResetCollection(games);

            SelectedGameData = GameDatas.Where(o => o.GameModel.GameID == oldSelection).FirstOrDefault();
        }


    }
}
