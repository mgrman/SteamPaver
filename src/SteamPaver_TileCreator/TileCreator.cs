
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SteamPaver_TileCreator
{
    public static class TileCreator
    {

        public static void CreateTile(string name, int id,BitmapSource image)
        {

            var baseName = String.Join("", new Regex(@"[A-Za-z\d]").Matches(name).Cast<Match>().Select(o => o.Value));


            var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SteamPaver");
            var startMenuPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft", "Windows", "Start Menu", "Programs", "SteamPaverLinks");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var batName = baseName + ".vbs";
            var batPath = Path.Combine(folderPath, batName);
            File.WriteAllText(batPath, $"CreateObject(\"Wscript.Shell\").Run \"\"\"\" & \"steam://rungameid/{id}\" & \"\"\"\", 0, False");
            System.IO.File.SetCreationTime(batPath, DateTime.Now);
            System.IO.File.SetLastAccessTime(batPath, DateTime.Now);
            System.IO.File.SetLastWriteTime(batPath, DateTime.Now);

            var imageName = baseName + ".png";
            var imagePath = Path.Combine(folderPath, imageName);

            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(fileStream);
            }
            System.IO.File.SetCreationTime(imagePath, DateTime.Now);
            System.IO.File.SetLastAccessTime(imagePath, DateTime.Now);
            System.IO.File.SetLastWriteTime(imagePath, DateTime.Now);

            var iconName = baseName + ".ico";
            var iconPath = Path.Combine(folderPath, iconName);
            SaveIcon(image, iconPath);


            var manifestName = baseName + ".VisualElementsManifest.xml";
            var manifestPath = Path.Combine(folderPath, manifestName);

            var manifest = string.Format(TemplateManifest, imageName);
            File.WriteAllText(manifestPath, manifest);
            System.IO.File.SetCreationTime(manifestPath, DateTime.Now);
            System.IO.File.SetLastAccessTime(manifestPath, DateTime.Now);
            System.IO.File.SetLastWriteTime(manifestPath, DateTime.Now);

            var linkName = "SteamPaver_"+ baseName + ".lnk";
            var linkPath = Path.Combine(startMenuPath, linkName);
            CreateShortcut(linkPath, batPath, iconPath,name);
            System.IO.File.SetCreationTime(linkPath, DateTime.Now);
            System.IO.File.SetLastAccessTime(linkPath, DateTime.Now);
            System.IO.File.SetLastWriteTime(linkPath, DateTime.Now);
        }

        private static void CreateShortcut(string shortcutPath, string shortcutTarget,string shortcutIcon,string shortcutDescription)
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

        private static void SaveIcon(BitmapSource bitmapImage,string path)
        {
            var icon = new Inedo.Iconator.IconFile();
            icon.Images.Add(bitmapImage);

            icon.Save(path);
        }

        private static System.Drawing.Bitmap BitmapSource2Bitmap(BitmapSource bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new System.Drawing.Bitmap(bitmap);
            }
        }

        private static string TemplateManifest =
@"<Application xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>    <VisualElements        BackgroundColor = '#FFFFFF'        ShowNameOnSquare150x150Logo='off'        ForegroundText='dark'        Square150x150Logo='{0}'        Square70x70Logo='{0}'        /></Application>".Replace('\'', '"');
    }
}
