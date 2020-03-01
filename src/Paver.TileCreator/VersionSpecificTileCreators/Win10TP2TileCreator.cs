using Paver.Common;
using Paver.Common.Models;
using Paver.TileCreator.Properties;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Paver.TileCreator
{
    internal class Win10TP2TileCreator : ITileCreator
    {
        private static readonly Regex _replaceRegex = new Regex(@"({.*?})|(')");
        private readonly string _proxyFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SteamPaver", "Win10TP2Tiles");
        private readonly string _proxyStartMenuPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft", "Windows", "Start Menu", "Programs", "SteamPaverLinks");

        private readonly ILoggerFacade _logger;

        public Win10TP2TileCreator(ILoggerFacade logger)
        {
            _logger = logger;
            //TODO
            //Add logging!!

            if (!Directory.Exists(_proxyFolderPath))
                Directory.CreateDirectory(_proxyFolderPath);
            if (!Directory.Exists(_proxyStartMenuPath))
                Directory.CreateDirectory(_proxyStartMenuPath);
        }

        public IEnumerable<LinkTypes> GetSupportedLinkTypes(TileSettings tileSettings)
        {
            if (tileSettings.Uri.IsFile)
            {
                yield return LinkTypes.DirectLink;
                yield return LinkTypes.ProxyLink;
            }
            else
            {
                yield return LinkTypes.ProxyLink;
            }
        }

        public void CreateDirectLink(TileSettings tileSettings)
        {
            if (!tileSettings.Uri.IsFile)
                throw new ArgumentException("Path must lead to a local file.", nameof(tileSettings.Uri));

            string localPath = tileSettings.Uri.ToString();

            var baseName = Path.GetFileNameWithoutExtension(localPath);
            var folderPath = Path.GetDirectoryName(localPath);

            string imageName_150 = CreateImage(tileSettings, baseName, folderPath, 150);
            string imageName_75 = CreateImage(tileSettings, baseName, folderPath, 70);

            string manifestPath = CreateManifest(tileSettings.Name, tileSettings, baseName, folderPath, imageName_150, imageName_75);
        }

        public void CreateProxyLink(TileSettings tileSettings)
        {
            var baseName = String.Join("", new Regex(@"[A-Za-z\d]").Matches(tileSettings.Name).Cast<Match>().Select(o => o.Value));
            var folderPath = _proxyFolderPath;
            var startMenuPath = _proxyStartMenuPath;

            string executablePath = CreateProxyExecutable(tileSettings.Uri, baseName, folderPath);

            string imageName_150 = CreateImage(tileSettings, baseName, folderPath, 150);
            string imageName_75 = CreateImage(tileSettings, baseName, folderPath, 70);

            string iconPath = CreateIcon(tileSettings, baseName, folderPath);

            string manifestPath = CreateManifest(tileSettings.Name, tileSettings, baseName, folderPath, imageName_150, imageName_75);

            string linkName = CreateLink(tileSettings.Name, baseName, startMenuPath, executablePath, iconPath);
        }

        void ITileCreator.CreateTile(TileSettings tileSettings)
        {
            var supportedLinkTypes = GetSupportedLinkTypes(tileSettings);
            if (!supportedLinkTypes.Contains(tileSettings.LinkType))
                throw new ArgumentException($"LinkType:{tileSettings.LinkType} is not supported for this instance! (Supported LinkTypes:{supportedLinkTypes})", nameof(tileSettings));

            switch (tileSettings.LinkType)
            {
                case LinkTypes.DirectLink:
                    CreateDirectLink(tileSettings);
                    break;

                case LinkTypes.ProxyLink:
                default:
                    CreateProxyLink(tileSettings);
                    break;
            }
        }

        private static void TouchFile(string path)
        {
            File.SetCreationTime(path, DateTime.Now);
            File.SetLastAccessTime(path, DateTime.Now);
            File.SetLastWriteTime(path, DateTime.Now);
        }

        private static void CreateShortcut(string shortcutPath, string shortcutTarget, string shortcutIcon, string shortcutDescription)
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

        private static void SaveIcon(BitmapSource bitmapImage, string path)
        {
            var scale = 256.0 / bitmapImage.PixelWidth;
            var scaledImage = new TransformedBitmap(bitmapImage, new ScaleTransform(scale, scale));

            var icon = new Inedo.Iconator.IconFile();
            icon.Images.Add(scaledImage);

            icon.Save(path);
        }

        private string CreateProxyExecutable(Uri uri, string baseName, string folderPath)
        {
            var execName = $"{baseName}.vbs";
            var execPath = Path.Combine(folderPath, execName);
            File.WriteAllText(execPath, $"CreateObject(\"Wscript.Shell\").Run \"\"\"\" & \"{uri}\" & \"\"\"\", 0, False");
            TouchFile(execPath);
            return execPath;
        }

        private string CreateLink(string name, string baseName, string startMenuPath, string batPath, string iconPath)
        {
            var linkName = $"{baseName} (SteamPaver).lnk";
            var linkPath = Path.Combine(startMenuPath, linkName);
            CreateShortcut(linkPath, batPath, iconPath, name);
            TouchFile(linkPath);
            return linkName;
        }

        private string CreateManifest(string name, TileSettings tileSettings, string baseName, string folderPath, string imageName_150, string imageName_75)
        {
            var manifestName = $"{baseName}.VisualElementsManifest.xml";
            var manifestPath = Path.Combine(folderPath, manifestName);

            var backgroundColorString = tileSettings.BackgroundColor == Colors.Transparent ? "Transparent" : tileSettings.BackgroundColor.ToHex(false);

            Dictionary<string, string> replacementsDict = new Dictionary<string, string>();
            replacementsDict["{imagePath_150}"] = imageName_150;
            replacementsDict["{imagePath_75}"] = imageName_75;
            replacementsDict["{color}"] = backgroundColorString;
            replacementsDict["{showName}"] = tileSettings.ShowLabel ? "on" : "off";
            replacementsDict["{labelType}"] = tileSettings.UseDarkLabel ? "dark" : "light";
            replacementsDict["{name}"] = name;
            replacementsDict["'"] = "\"";

            string win10TP2_TemplateManifest = @"
    <Application Id='App'>
        <VisualElements
            DisplayName = '{name}'
            Description = '{name}'
            BackgroundColor = '{color}'
            ShowNameOnSquare150x150Logo='{showName}'
            ForegroundText='{labelType}'
            Square150x150Logo='{imagePath_150}'
            Square70x70Logo='{imagePath_75}'
            Logo='{imagePath_150}'
            SmallLogo='{imagePath_75}'
            ToastCapable='false'>
            <DefaultTile ShowName='allLogos'/>
            <SplashScreen BackgroundColor='white' Image='images\splash-sdk.png'/>
        </VisualElements>
    </Application>";

            var manifest = _replaceRegex.Replace(win10TP2_TemplateManifest, (m) => replacementsDict[m.Value]).Trim();

            File.WriteAllText(manifestPath, manifest);
            TouchFile(manifestPath);
            return manifestPath;
        }

        private string CreateIcon(TileSettings tileSettings, string baseName, string folderPath)
        {
            var iconName = $"{baseName}.ico";
            var iconPath = Path.Combine(folderPath, iconName);
            SaveIcon(tileSettings.Image, iconPath);
            TouchFile(iconPath);
            return iconPath;
        }

        private string CreateImage(TileSettings tileSettings, string baseName, string folderPath, int resolution)
        {
            var resizedImage = tileSettings.Image.CreateResizedImage(resolution, resolution, false);

            var imageName = $"{baseName}_{resolution}x{resolution}.png";
            var imagePath = Path.Combine(folderPath, imageName);
            resizedImage.SaveAsPng(imagePath);

            //using (var fileStream = new FileStream(imagePath, FileMode.Create))
            //{
            //    BitmapEncoder encoder = new PngBitmapEncoder();
            //    var frame = BitmapFrame.Create(tileSettings.Image);
            //    encoder.Frames.Add(frame);

            //    encoder.Save(fileStream);
            //}
            TouchFile(imagePath);
            return imageName;
        }
    }
}