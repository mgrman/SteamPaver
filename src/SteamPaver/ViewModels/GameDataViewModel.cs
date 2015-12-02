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

namespace SteamPaver
{
    [ImplementPropertyChanged]
    public class GameDataViewModel
    {
        public GameData Model { get;  }

        public GameDataViewModel()
        {
            Model = new GameData();
        }

        public GameDataViewModel(GameData data)
        {
            Model = data;
        }

        public ICommand SetAsTileCommand
        {
            get
            {
                return new RelayCommand(() => Model.SetFinalAsTile(), () => Model != null && Model.SquareFinal != null);
            }
        }

        public ICommand PasteImageCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Model.SquareDraft = Clipboard.GetImage();
                }, () =>
                {
                    return Model != null && Clipboard.ContainsImage();

                });
            }
        }
    }
}
