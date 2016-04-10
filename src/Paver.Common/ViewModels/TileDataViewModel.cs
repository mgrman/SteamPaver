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
using Paver.Common.Models;
using Paver.Common.Utils;
using System.ComponentModel;

namespace Paver.Common.ViewModels
{
    [ImplementPropertyChanged]
    public abstract class TileDataViewModel
    {
        public TileData Model { get; }
        
        public ITileCreator TileCreator { get; }

        public TileDataViewModel(TileData data,ITileCreator creator)
        {
            Model = data;
            TileCreator = creator;

            UpdateAvailableLinkTypes();
            var dataChanged = (data as INotifyPropertyChanged);
            if(dataChanged!= null)
            {
                dataChanged.PropertyChanged += (o, e) =>
                {
                    UpdateAvailableLinkTypes();
                };
            }
        }

        private void UpdateAvailableLinkTypes()
        {
            if (Model != null)
            {

                AvailableLinkTypes.DiffCollection(TileCreator.GetSupportedLinkTypes(Model.ToTileSettings()));
                CanSelectAvailableLinkTypes = AvailableLinkTypes.Count > 1;
            }
            else
            {
                AvailableLinkTypes.Clear();
                CanSelectAvailableLinkTypes = false;
            }

        }

        public ICommand SetAsTileCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    try
                    {
                        TileCreator.CreateTile(Model.ToTileSettings());
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show($"{ex.GetType().Name} - {ex.Message}", "Problem setting Tile", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    MessageBox.Show($"Tile created successfully.\r\n.\r\nIt can be up to a minute before the new shortcuts become visible in Start menu.", "Problem setting Tile", MessageBoxButton.OK, MessageBoxImage.None);
                }
                , () => Model != null && Model.SquareFinal != null);
            }
        }

        public virtual IEnumerable<ILabeledCommand> CustomCommands { get; }
        public IEnumerable<ILabeledCommand> AllCommands { get
            {
                if (CustomCommands != null)
                {
                    foreach (var cmd in CustomCommands)
                        yield return cmd;
                }

                yield return LoadImageCommand;
            }
        }


        public ObservableCollectionEx<LinkTypes> AvailableLinkTypes { get; } = new ObservableCollectionEx<LinkTypes>();

        public bool CanSelectAvailableLinkTypes { get; private set; } 


        //public ICommand UpdateImageFromSteamCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(() => Model.UpdateImageFromSteam(), () => Model != null);
        //    }
        //}

        public ILabeledCommand LoadImageCommand
        {
            get
            {
                return new LabeledRelayCommand("Load image ...",() =>
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
