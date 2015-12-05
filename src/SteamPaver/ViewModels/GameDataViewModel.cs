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
        public GameData Model { get; }

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
                return new RelayCommand(() =>
                {
                    string path ;
                    try
                    {
                        path=Model.SetFinalAsTile();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show($"{ex.GetType().Name} - {ex.Message}", "Problem setting Tile", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    MessageBox.Show($"Tile set successfully.\r\nTile can be found in \"{path}\".\r\nIt can be up to a minute before the new shortcuts become visible in Start menu.", "Problem setting Tile", MessageBoxButton.OK, MessageBoxImage.None);
                }
                , () => Model != null && Model.SquareFinal != null);
            }
        }

        public ICommand UpdateImageFromSteamCommand
        {
            get
            {
                return new RelayCommand(() => Model.UpdateImageFromSteam(), () => Model != null);
            }
        }

        public ICommand LoadImageCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var newImg = ImageUtils.LoadLocalImage();
                    if (newImg != null)
                    {
                        Model.SquareDraft = newImg;
                    }
                }, () => Model != null);
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
