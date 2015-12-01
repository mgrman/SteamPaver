using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SteamPaver_Main
{
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : UserControl
    {
        public GameData Model { get { return DataContext as GameData; } }

        public GameView()
        {
            InitializeComponent();
        }

        //public ICommand CropDraftCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(()=> Model.CropDraft(Cropper.Selection));
        //    }
        //}

        public ICommand SetAsTileCommand
        {
            get
            {
                return new RelayCommand(()=> Model.SetFinalAsTile(),()=>Model!=null && Model.SquareFinal!=null);
            }
        }
    }
}
