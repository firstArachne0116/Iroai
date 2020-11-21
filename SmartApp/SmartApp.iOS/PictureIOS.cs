using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;


using System.Threading.Tasks;
using SmartApp.iOS;
using SmartApp.Interface;
using Xamarin.Forms;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(PictureIOS))]
namespace SmartApp.iOS
{
    public class PictureIOS : IPicture
    {
        public async Task<bool> SaveImage(string directory, string filename, ImageSource img)
        {
            bool success = false;

            NSData imgData = null;
            var renderer = new Xamarin.Forms.Platform.iOS.StreamImagesourceHandler();
            UIKit.UIImage photo = await renderer.LoadImageAsync(img);            

            var savedImageFilename = System.IO.Path.Combine(directory, filename);
            NSFileManager.DefaultManager.CreateDirectory(directory, true, null);

            if (System.IO.Path.GetExtension(filename).ToLower() == ".png")
                imgData = photo.AsPNG();
            else
                imgData = photo.AsJPEG(100);

            NSError err = null;
            success = imgData.Save(savedImageFilename, NSDataWritingOptions.Atomic, out err);

            return success;
        }
    }


}