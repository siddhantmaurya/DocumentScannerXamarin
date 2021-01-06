using Android.Graphics;
using Imaging.Library;
using Imaging.Library.Entities;
using Imaging.Library.Enums;
using Imaging.Library.Filters.BasicFilters;
using Imaging.Library.Filters.ComplexFilters;
using Imaging.Library.Maths;
using Java.Nio;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocumentScanner : ContentPage
    {
        public DocumentScanner()
        {
            InitializeComponent();
        }

        public static PixelMap GetPixelMap(Stream stream)
        {
            var decoder = BitmapRegionDecoder.NewInstance(stream, false);
            var bitmap = decoder.DecodeRegion(new Rect(0, 0, decoder.Width, decoder.Height), null);

            var width = bitmap.Width;
            var height = bitmap.Height;

            var source = new PixelMap(width, height);

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var pixel = bitmap.GetPixel(x, y);

                    source[x, y] = new Pixel
                    {
                        B = (byte)(pixel & 0x000000FF),
                        G = (byte)((pixel & 0x0000FF00) >> 8),
                        R = (byte)((pixel & 0x00FF0000) >> 16),
                        A = (byte)((pixel & 0xFF000000) >> 24)
                    };
                }
            }

            decoder.Recycle();

            return source;
        }

        public static ImageSource LoadImageFromPixelMap(PixelMap pixelMap)
        {
            var buffer = ByteBuffer.Wrap(pixelMap.ToByteArray());
            buffer.Rewind();

            var bitmap = Bitmap.CreateBitmap(pixelMap.Width, pixelMap.Height, Bitmap.Config.Argb8888);
            bitmap.CopyPixelsFromBuffer(buffer);

            var ms = new MemoryStream();
            bitmap.Compress(Bitmap.CompressFormat.Png, 100, ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ImageSource.FromStream(() => ms);
        }

        private async void StartScan(object sender, EventArgs e)
        {
            var scale = 0.4;
            var stream = await TakePhoto();

            Loader.IsRunning = true;
            Loader.IsVisible = true;
            LoaderGrid.IsVisible = true;
            await Task.Delay(10);
            var source = GetPixelMap(stream);
            var imaging = new ImagingManager(source);       //source is PixelMap, you can find samples how to convert image to PixelMap
            imaging.AddFilter(new BicubicFilter(scale));    //Downscaling
            imaging.AddFilter(new CannyEdgeDetector());     //This filter contains Grayscale and Gaussian filter in it
            imaging.Render();                               //Renders the image to use it further use

            var blobCounter = new BlobCounter()
            {
                ObjectsOrder = ObjectsOrder.Size
            };
            imaging.AddFilter(blobCounter);

            imaging.Render();

            //Following code finds largest quadratical blob
            List<Imaging.Library.Entities.Point> corners = null;
            var blobs = blobCounter.GetObjectsInformation();
            foreach (var blob in blobs)
            {
                var points = blobCounter.GetBlobsEdgePoints(blob);

                var shapeChecker = new SimpleShapeChecker();

                if (shapeChecker.IsQuadrilateral(points, out corners))
                    break;
            }
            
            imaging.UndoAll();                              //Undo every filters applied

            var edgePoints = new EdgePoints();
            edgePoints.SetPoints(corners.ToArray());
            edgePoints = edgePoints.ZoomIn(scale);          //Corrects points that found on downscaled image to original
            imaging.AddFilter(new QuadrilateralTransformation(edgePoints, true));

            imaging.Render();
            CovertedImage.Source = LoadImageFromPixelMap(imaging.Output);

            //imaging.Output gives that extracted rectangle shape from photo. Check out WPF sample how to save it.

            Loader.IsRunning = false;
            Loader.IsVisible = false;
            LoaderGrid.IsVisible = false;
            await Task.Delay(10);
        }

        private async Task<Stream> TakePhoto()
        {
            MediaFile _mediaFile;

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await App.Current.MainPage.DisplayAlert("No Camera", ":( No camera available.", "OK");
                return null;
            }

            _mediaFile = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Pictures",
                Name = "test.jpg",
                PhotoSize = PhotoSize.Medium
            });
            //var targetFile = new MediaFile(_mediaFile);
            //DependencyService.Get<IImageService>().ResizeImage(_mediaFile.Path,"" );
            if (_mediaFile == null)
                return null;

            //ViewModel.StoreImageUrl(file.Path);

            Stream stream = _mediaFile.GetStream();
            _mediaFile.Dispose();

            return stream; 
        }

        private void CancelScan(object sender, EventArgs e)
        {
            //Loader.IsRunning = false;
            //Loader.IsVisible = false;
            //LoaderGrid.IsVisible = false;
        }
    }
}