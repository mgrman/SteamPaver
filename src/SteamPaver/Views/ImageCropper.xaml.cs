using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace SteamPaver
{
    /// <summary>
    /// Interaction logic for ImageCropper.xaml
    /// </summary>
    public partial class ImageCropper : UserControl
    {


        public Rect CroppingRectangle
        {
            get { return (Rect)GetValue(CroppingRectangleProperty); }
            set { SetValue(CroppingRectangleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CroppingRectangle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CroppingRectangleProperty =
            DependencyProperty.Register("CroppingRectangle", typeof(Rect), typeof(ImageCropper), new FrameworkPropertyMetadata(new Rect(0, 0, 100, 100), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (o, e) =>
            {
                var control = o as ImageCropper;

                var value = (e.NewValue as Rect?) ?? new Rect(0, 0, 100, 100);



                control.CropGrid.Margin = new Thickness(value.X, value.Y, 0, 0);
                control.CropGrid.Width = value.Width;
                control.CropGrid.Height = value.Height;

            }));



        public BitmapSource Image
        {
            get { return (BitmapSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(BitmapSource), typeof(ImageCropper), new PropertyMetadata(null,(o,e)=>
            {
                var data = (e.NewValue as BitmapSource);
                if (data == null)
                    return;

                var control = o as ImageCropper;



                double size = Math.Min(data.Width, data.Height);
                double maxSize = Math.Max(data.Width, data.Height);


                control.negativeMarginX = Math.Min(data.Width - maxSize, 0);
                control.negativeMarginY = Math.Min(data.Height - maxSize, 0);
                
                control.CropGrid.MaxWidth = maxSize;
                control.CropGrid.MaxHeight = maxSize;

            }));







        //public Rect Selection
        //{
        //    get
        //    {
        //        return new Rect(CropGrid.Margin.Left, CropGrid.Margin.Top, CropGrid.Width, CropGrid.Height);
        //    }
        //    set
        //    {
        //        CropGrid.Margin = new Thickness(value.X, value.Y, 0, 0);
        //        CropGrid.Width = value.Width;
        //        CropGrid.Height = value.Height;
        //    }
        //}



        double negativeMarginX = 0;
        double negativeMarginY = 0;


        public ImageCropper()
        {
            InitializeComponent();


            Point startPoint = new Point();
            Rect originalRect = CroppingRectangle;

            MouseDown += (o, e) =>
            {
                if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
                {
                    startPoint = e.GetPosition(this);
                    originalRect = CroppingRectangle;
                    this.CaptureMouse();
                }
            };

            MouseUp += (o, e) =>
            {
                if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Released)
                {
                    this.ReleaseMouseCapture();
                }
            };


            MouseMove += (o, e) =>
            {
                if (e.LeftButton.HasFlag(MouseButtonState.Pressed))
                {
                    var curPos = e.GetPosition(this);

                    var viewBoxScale = GetScaleFactor(MainViewBox);

                    var offsetX = (curPos.X - startPoint.X) / viewBoxScale;
                    var offsetY = (curPos.Y - startPoint.Y) / viewBoxScale;

                    var newRect = originalRect;

                    if (Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        var offsetMag = offsetX + offsetY;

                        var scale = Math.Exp(offsetMag / 100);

                        newRect.Width *= scale;
                        newRect.Height *= scale;

                        double maxSize;
                        if (Keyboard.IsKeyDown(Key.LeftShift))
                            maxSize = Math.Max(Image.Width, Image.Height);
                        else
                            maxSize = Math.Min(Image.Width, Image.Height);

                        newRect.Width = Math.Min(Math.Max(newRect.Width, 1), maxSize);
                        newRect.Height = Math.Min(Math.Max(newRect.Height, 1), maxSize);
                    }
                    else
                    {

                        newRect.X += offsetX;
                        newRect.Y += offsetY;

                        Rect validRect;
                        if (Keyboard.IsKeyDown(Key.LeftShift))
                            validRect = new Rect(negativeMarginX, negativeMarginY, Math.Max(CropGrid.MaxWidth - CropGrid.Width, 0) - negativeMarginX, Math.Max(CropGrid.MaxHeight - CropGrid.Height, 0) - negativeMarginY);
                        else
                            validRect = new Rect(0, 0, Math.Max(Image.Width - CropGrid.Width, 0), Math.Max(Image.Height - CropGrid.Height, 0));

                        newRect.X = Math.Min(Math.Max(newRect.X, validRect.Left), validRect.Right);
                        newRect.Y = Math.Min(Math.Max(newRect.Y, validRect.Top), validRect.Bottom);
                    }



                    CroppingRectangle = newRect;
                }
            };
        }
        public static double GetScaleFactor(Viewbox viewbox)
        {
            if (viewbox.Child == null ||
                (viewbox.Child is FrameworkElement) == false)
            {
                return double.NaN;
            }
            FrameworkElement child = viewbox.Child as FrameworkElement;
            return viewbox.ActualWidth / child.ActualWidth;
        }
    }
}
