using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SmartApp.Droid;
using SmartApp.Interface;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(PictureAndroid))]
namespace SmartApp.Droid
{
    public class PictureAndroid: IPicture
    {
        public async Task<bool> SaveImage(string directory, string filename, ImageSource img)
        {
            System.IO.Stream outputStream = null;

            var renderer = new Xamarin.Forms.Platform.Android.StreamImagesourceHandler();
            Bitmap photo = await renderer.LoadImageAsync(img, Forms.Context);
            var savedImageFilename = System.IO.Path.Combine(directory, filename);

            System.IO.Directory.CreateDirectory(directory);

            bool success = false;
            using (outputStream = new System.IO.FileStream(savedImageFilename, System.IO.FileMode.Create))
            {
                if (System.IO.Path.GetExtension(filename).ToLower() == ".png")
                    success = await photo.CompressAsync(Bitmap.CompressFormat.Png, 100, outputStream);
                else
                    success = await photo.CompressAsync(Bitmap.CompressFormat.Jpeg, 100, outputStream);
            }
            return success;
        }
    }
}