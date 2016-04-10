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
using System.Windows;
using Paver.Common;
using Paver.Common.ViewModels;

namespace Paver.Steam.ViewModels
{
    [ImplementPropertyChanged]
    public class GameDataViewModel: TileDataViewModel
    {
        private SteamAPI _steamAPI;


        public GameDataViewModel(GameData data, ITileCreator creator,SteamAPI steamAPI)
            :base(data, creator)
        {
            _steamAPI = steamAPI;


            UpdateImageFromSteamCommand= new LabeledRelayCommand("Update image from Steam", () => UpdateImageFromSteam(), () => Model != null);
            UpdateIsInstalledCommand = new LabeledRelayCommand("Update Installation status", () => UpdateIsInstalled(), () => Model != null);
        }

        public GameData GameModel { get { return Model as GameData; } }

        public override IEnumerable<ILabeledCommand> CustomCommands { get
            {
                yield return UpdateImageFromSteamCommand;
            }
        }


        public ILabeledCommand UpdateImageFromSteamCommand { get; }
        public void UpdateImageFromSteam()
        {
            
            _steamAPI.RefreshBanner(GameModel.GameID);
        }


        public ILabeledCommand UpdateIsInstalledCommand { get; }
        public void UpdateIsInstalled()
        {
            
            GameModel.IsInstalled = _steamAPI.RefreshIsInstalled(GameModel.GameID);
        }
    }
}
