using System;
using System.Collections.Generic;
using System.IO;
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
using Java.IO;
using SmartApp.Droid;
using SmartApp.Interface;
using Xamarin.Forms;
using File = Java.IO.File;

[assembly: Xamarin.Forms.Dependency(typeof(ScreenshotService))]

namespace SmartApp.Droid
{
    public class ScreenshotService : IScreenshotService
    {
        private static Activity _currentActivity;
        public void SetActivity(Activity activity)
        {
            _currentActivity = activity;
        }

        public byte[] Capture()
        {
            var rootView = _currentActivity.Window.DecorView.RootView;

            using (var screenshot = Bitmap.CreateBitmap(
                                    rootView.Width,
                                    rootView.Height,
                                    Bitmap.Config.Argb8888))
            {
                var canvas = new Canvas(screenshot);
                rootView.Draw(canvas);

                using (var stream = new MemoryStream())
                {
                    screenshot.Compress(Bitmap.CompressFormat.Png, 100, stream);
                    return stream.ToArray();
                }
            }
        }


        public string Capture(string filename)
        {
            var rootView = _currentActivity.Window.DecorView.RootView;
            string fullpath = string.Empty;

            using (var screenshot = Bitmap.CreateBitmap(
                                    rootView.Width ,
                                    rootView.Height,
                                    Bitmap.Config.Argb8888))
            {
                var canvas = new Canvas(screenshot);
                rootView.Draw(canvas);

                using (var stream = new MemoryStream())
                {
                    screenshot.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);

                    File picturesDirectory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures);
                    

                    using (File bitmapFile = new File(picturesDirectory, filename))
                    {
                        bitmapFile.CreateNewFile();

                        using (FileOutputStream outputStream = new FileOutputStream(bitmapFile))
                        {
                            outputStream.Write(stream.ToArray());
                        }
                        fullpath = bitmapFile.AbsolutePath;
                    }
                    return fullpath;
                }
            }
        }
    }
}