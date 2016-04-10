using System.Windows;

namespace Paver.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();

            SelectorsTabControl.SelectedIndex = 0;
        }
    }
}
