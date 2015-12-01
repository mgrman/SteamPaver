using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using PropertyChanged;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Newtonsoft.Json;
using System.Windows;
using System.Text.RegularExpressions;

namespace SteamPaver_Main
{
    [ImplementPropertyChanged]
    public class GameData
    {

        public string Name
        {
            get; set;
        }
        public int GameID
        {
            get; set;
        }

        private BitmapSource _squareDraft;
        public BitmapSource SquareDraft
        {
            get
            {
                return _squareDraft;
            }
            set
            {
                if (value == _squareDraft)
                    return;
                _squareDraft = value;

                if (value == null)
                {
                    CroppingRectangle = new Rect(0,0,100,100);
                    SquareFinal = null;
                }
                else
                {
                    if (_squareDraft.IsDownloading)
                    {
                        EventHandler handler=null; 
                        handler= (o, e) =>
                        {
                            _squareDraft.DownloadCompleted -= handler;

                            double minSize = Math.Min(value.Width, value.Height);
                            CroppingRectangle = new Rect(0, 0, minSize, minSize);
                        };
                        _squareDraft.DownloadCompleted += handler;
                    }
                    else
                    {
                        double minSize = Math.Min(value.Width, value.Height);
                        CroppingRectangle = new Rect(0, 0, minSize, minSize);
                    }

                }
            }
        }

        private Rect _croppingRectangle;
        public Rect CroppingRectangle
        {
            get
            {
                return _croppingRectangle;
            }
            set
            {
                if (_croppingRectangle == value) return;
                _croppingRectangle = value;

                CropDraft(_croppingRectangle);
            }
        }


        public BitmapSource SquareFinal
        {
            get; set;
        }
        public Color Color
        {
            get; set;
        }


        public bool Installed { get; private set; }


        public GameData(int gameID)
        {
            GameID = gameID;
        }

        public static GameData Create(int gameID)
        {
            var data = new GameData(gameID);
            data.ResetName();
            data.UpdateInstalled();
            data.SetWideImageFromSteam();

            return data;
        }

        public void UpdateInstalled()
        {
            Installed = Steam.AllInstaledGames.GetGames().Contains(GameID);
        }

        public void ResetName()
        {
            Name = Steam.IdToName.GetName(GameID) ?? $"<Game not found - {GameID}>";
        }



        public void SetWideImageFromSteam()
        {
            string url = String.Format("http://cdn.akamai.steamstatic.com/steam/apps/{0}/header.jpg", GameID);

            App.Current.Dispatcher.InvokeAsync(() =>
            {
                SquareDraft = new BitmapImage(new Uri(url));
            });
        }


        public void CropDraft(Rect rectangle)
        {
            CroppingRectangle = rectangle;

            double scaleX = SquareDraft.DpiX / 96;
            double scaleY = SquareDraft.DpiY / 96;

            var imgRect = new Rect(0, 0, SquareDraft.Width, SquareDraft.Height);

            rectangle.Intersect(imgRect);
            rectangle.Scale(scaleX, scaleY);


            var intRect = new Int32Rect((int)rectangle.X, (int)rectangle.Y, (int)Math.Round(rectangle.Width), (int)Math.Round(rectangle.Height));

            var croppedBmp = new CroppedBitmap(SquareDraft, intRect);


            double scale = 1;
            if (intRect.Width != intRect.Height)
            {
                scale = (double)intRect.Height / intRect.Width;
            }

            SquareFinal = new TransformedBitmap(croppedBmp, new ScaleTransform(scale, 1));

        }


        public void SetFinalAsTile()
        {
            if (SquareFinal == null)
                throw new InvalidOperationException("SquareFinal cannot be Null when calling this method");

            SteamPaver_TileCreator.TileCreator.CreateTile(Name, GameID, SquareFinal);
        }






        //#endregion

        //#region ColorFromSquare

        //public void SetColorFromSquare()
        //{
        //    if (Square == null)
        //        Color = Color.White;
        //    else
        //        Color = Utils.GetAverageColorUsingHSL(Square);
        //}

        //#endregion

        //#region ColorFromWide

        //public void SetColorFromWide()
        //{
        //    if (Wide == null)
        //        Color = Color.White;
        //    else
        //        Color = Utils.GetAverageColorUsingHSL(Wide);
        //}

        //#endregion

        //#region IDisposable

        //public void Dispose()
        //{
        //    foreach (var obj in _resources)
        //    {
        //        obj.Dispose();
        //    }
        //}

        //#endregion


    }
}
