using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using App1.Droid;
using Xamarin.Forms;
using Bitmap = Android.Graphics.Bitmap;
using Size = Android.Util.Size;

[assembly: Dependency(typeof(ImageServiceDroid))]
namespace App1.Droid
{
    public class ImageServiceDroid : IImageService
    {
        public void ResizeImage(string sourceFile, string targetFile, float maxWidth, float maxHeight)
        {
            if (!File.Exists(targetFile) && File.Exists(sourceFile))
            {
                // First decode with inJustDecodeBounds=true to check dimensions
                var options = new BitmapFactory.Options()
                {
                    InJustDecodeBounds = false,
                    InPurgeable = true,
                };

                using (var image = BitmapFactory.DecodeFile(sourceFile, options))
                {
                    if (image != null)
                    {
                        var sourceSize = new Size((int)image.GetBitmapInfo().Height, (int)image.GetBitmapInfo().Width);

                        var maxResizeFactor = Math.Min(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);

                        string targetDir = System.IO.Path.GetDirectoryName(targetFile);
                        if (!Directory.Exists(targetDir))
                            Directory.CreateDirectory(targetDir);

                        if (maxResizeFactor > 0.9)
                        {
                            File.Copy(sourceFile, targetFile);
                        }
                        else
                        {
                            var width = (int)(maxResizeFactor * sourceSize.Width);
                            var height = (int)(maxResizeFactor * sourceSize.Height);

                            using (var bitmapScaled = Bitmap.CreateScaledBitmap(image, height, width, true))
                            {
                                using (Stream outStream = File.Create(targetFile))
                                {
                                    if (targetFile.ToLower().EndsWith("png"))
                                        bitmapScaled.Compress(Bitmap.CompressFormat.Png, 100, outStream);
                                    else
                                        bitmapScaled.Compress(Bitmap.CompressFormat.Jpeg, 95, outStream);
                                }
                                bitmapScaled.Recycle();
                            }
                        }

                        image.Recycle();
                    }
                    else
                        Log.Error("Error", "String", "Image scaling failed: " + sourceFile);
                }
            }
        }
    }
}