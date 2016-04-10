using Paver.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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

namespace Paver.Views
{
    /// <summary>
    /// Interaction logic for TileView.xaml
    /// </summary>
    public partial class TileView : UserControl
    {
        public TileDataViewModel ViewModel { get { return DataContext as TileDataViewModel; } }

        public TileView()
        {
            InitializeComponent();

            InitializeFormats();
        }

        private void FocusOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            (sender as FrameworkElement).Focusable = true;
            var res = (sender as FrameworkElement).Focus();
        }

        private IDictionary<string, Action<IDataObject, string>> _validFormats = new Dictionary<string, Action<IDataObject, string>>();

        private void InitializeFormats()
        {
            _validFormats["FileNameW"] = ProcessFileNames;
            _validFormats["FileName"] = ProcessFileNames;
            _validFormats["FileDrop"] = ProcessFileNames;
            _validFormats["UsingDefaultDragImage"] = ProcesStream;
        }

        private void ProcessFileNames(IDataObject obj, string format)
        {
            try
            {
                var fileNames = obj.GetData(format, true) as string[];

                var fileName = fileNames.FirstOrDefault();

                if (fileName != null)
                {
                    var file = File.ReadAllBytes(fileName);



                    using (var ms = new System.IO.MemoryStream(file))
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad; // here
                        image.StreamSource = ms;
                        image.EndInit();
                        ViewModel.Model.SquareDraft = image;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProcesStream(IDataObject obj, string format)
        {
            try
            {
                var stream = obj.GetData(format, true) as Stream;

                using (var ms = stream)
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad; // here
                    image.StreamSource = ms;
                    image.EndInit();
                    ViewModel.Model.SquareDraft = image;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Control_DragEnter(object sender, DragEventArgs e)
        {
        }

        private void Control_DragLeave(object sender, DragEventArgs e)
        {

        }

        private void Control_DragOver(object sender, DragEventArgs e)
        {

            var formats = e.Data.GetFormats();


            if (_validFormats.Keys.Any(o => formats.Contains(o)))
            {
                e.Effects = DragDropEffects.Copy;
            }
        }

        private void Control_Drop(object sender, DragEventArgs e)
        {
            var formats = e.Data.GetFormats();


            var possiblePair = _validFormats.Where(o => formats.Contains(o.Key)).Select(o => o as KeyValuePair<string, Action<IDataObject, string>>?).FirstOrDefault();

            if (possiblePair != null)
            {
                var actionPair = possiblePair.Value;

                actionPair.Value(e.Data, actionPair.Key);
            }


        }
    }
}
