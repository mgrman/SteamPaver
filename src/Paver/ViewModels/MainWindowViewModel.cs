using Prism.Mvvm;
using PropertyChanged;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Paver.ViewModels
{
    [ImplementPropertyChanged]
    public class MainWindowViewModel : BindableBase
    {
        public string Title { get; } = "Steam Paver";
        public ImageSource Icon { get; } = new BitmapImage(new System.Uri(@"pack://application:,,,/Resources/Paver_dark.png"));

        public MainWindowViewModel(Prism.Events.IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<Paver.Common.TileDataSelectedEvent>()
                .Subscribe((data) => SelectedTileData = data,Prism.Events.ThreadOption.UIThread);

        }

        public Paver.Common.ViewModels.TileDataViewModel SelectedTileData { get; private set; }
    }
}
