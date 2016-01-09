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
using SteamPaver.Common;

namespace SteamPaver
{
    [ImplementPropertyChanged]
    public class GameDataViewModel: TileDataViewModel
    {
        public GameDataViewModel()
            :base(new GameData())
        {
        }

        public GameDataViewModel(GameData data)
            :base(data)
        {
        }

        public GameData GameModel { get { return Model as GameData; } }

        public override IEnumerable<ILabeledCommand> CustomCommands { get
            {
                yield return new LabeledRelayCommand("Update image from Steam",() => GameModel.UpdateImageFromSteam(), () => Model != null);
            }
        }
    }
}
