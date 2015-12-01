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

namespace SteamPaver_Main
{
    [ImplementPropertyChanged]
    public class SteamPaverViewModel
    {
        public SteamPaverViewModel()
        {
            _refreshCommand = AsyncRelayCommand.Lazy(() => Refresh(),null,true);
            GameDatas = new ObservableCollectionEx<GameData>();
        }

        public ObservableCollectionEx<GameData> GameDatas { get; private set; }

        private Lazy<AsyncRelayCommand> _refreshCommand;
        public ICommand RefreshCommand { get { return _refreshCommand.Value; } }

        public string SteamCommunityName { get; set; }

        public async Task Refresh()
        {


            GameDatas.SuspendCollectionChanged = true;
            var games = Task.Run<IEnumerable<GameData>>(() =>
            {
                return Steam.AllOwnedGames.GetGames(SteamCommunityName)
                    .Select(o => GameData.Create(o))
                    .OrderBy(o => o.Installed ? 0 : 1)
                    .ThenBy(o => o.Name)
                    .ToList();
            });

            GameDatas.Clear();

            foreach (var game in (await games))
            {
                GameDatas.Add(game);
            }
            GameDatas.SuspendCollectionChanged = false;
            GameDatas.RefreshBinding();
        }
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
