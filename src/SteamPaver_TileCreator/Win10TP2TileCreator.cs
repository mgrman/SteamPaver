
using SteamPaver.TileCreator.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SteamPaver.TileCreator
{
    public class Win10TP2TileCreator: ITileCreator
    {
       private readonly string _folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SteamPaver", "Win10TP2Tiles");
        private readonly string _startMenuPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft", "Windows", "Start Menu", "Programs", "SteamPaverLinks");

        public Win10TP2TileCreator()
        {
            if (!Directory.Exists(_folderPath))
                Directory.CreateDirectory(_folderPath);
            if (!Directory.Exists(_startMenuPath))
                Directory.CreateDirectory(_startMenuPath);
        }


        public string CreateTile(string name, string pathOrUrl,BitmapSource image, Color backgroundColor, bool showLabel, bool useDarkLabel)
        {
            var baseName = String.Join("", new Regex(@"[A-Za-z\d]").Matches(name).Cast<Match>().Select(o => o.Value));
            

            var batName = $"{baseName}.vbs";
            var batPath = Path.Combine(_folderPath, batName);
            File.WriteAllText(batPath, $"CreateObject(\"Wscript.Shell\").Run \"\"\"\" & \"{pathOrUrl}\" & \"\"\"\", 0, False");
            TouchFile(batPath);


            var imageName = $"{baseName}.png";
            var imagePath = Path.Combine(_folderPath, imageName);
            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                
                var frame = BitmapFrame.Create(image);
                encoder.Frames.Add(frame);
                
                encoder.Save(fileStream);
            }
            TouchFile(imagePath);


            var iconName = $"{baseName}.ico";
            var iconPath = Path.Combine(_folderPath, iconName);
            SaveIcon(image, iconPath);
            TouchFile(iconPath);


            var manifestName = $"{baseName}.VisualElementsManifest.xml";
            var manifestPath = Path.Combine(_folderPath, manifestName);

            var backgroundColorString = backgroundColor == Colors.Transparent ? "Transparent" : backgroundColor.ToHex(false);

            var manifest = string.Format(Resources.Win10TP2_TemplateManifest, imageName, backgroundColorString,showLabel ?"on":"off", useDarkLabel?"dark":"light",name);
            File.WriteAllText(manifestPath, manifest);
            TouchFile(manifestPath);


            var linkName = $"{baseName} (SteamPaver).lnk";
            var linkPath = Path.Combine(_startMenuPath, linkName);
            CreateShortcut(linkPath, batPath, iconPath,name);
            TouchFile(linkPath);

            return Path.Combine("SteamPaverLinks", linkName);
        }

        private void TouchFile(string path)
        {
            File.SetCreationTime(path, DateTime.Now);
            File.SetLastAccessTime(path, DateTime.Now);
            File.SetLastWriteTime(path, DateTime.Now);
        }

        private void CreateShortcut(string shortcutPath, string shortcutTarget,string shortcutIcon,string shortcutDescription)
        {
            var wsh = new IWshRuntimeLibrary.WshShellClass();
            IWshRuntimeLibrary.IWshShortcut shortcut = wsh.CreateShortcut(shortcutPath) as IWshRuntimeLibrary.IWshShortcut;
            //shortcut.Arguments = "c:\\app\\settings1.xml";
            shortcut.TargetPath = shortcutTarget;
            // not sure about what this is for
            shortcut.WindowStyle = 1;
            shortcut.Description = shortcutDescription;
            shortcut.WorkingDirectory = Path.GetDirectoryName(shortcutTarget);
            shortcut.IconLocation = shortcutIcon;
            shortcut.Save();
        }

        private void SaveIcon(BitmapSource bitmapImage,string path)
        {
            var scale = 256.0 / bitmapImage.Width;
            var scaledImage = new TransformedBitmap(bitmapImage, new ScaleTransform(scale, scale));
            
            var icon = new Inedo.Iconator.IconFile();
            icon.Images.Add(scaledImage);

            icon.Save(path);
        }

        
    }
}
