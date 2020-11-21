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
using System.Runtime.InteropServices;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(ScreenshotService))]
namespace SmartApp.iOS
{
    public class ScreenshotService : IScreenshotService
    {
        public byte[] Capture()
        {
            //var capture = UIScreen.MainScreen.Capture();
            //using (NSData data = capture.AsPNG())
            //{
            //    var bytes = new byte[data.Length];
            //    Marshal.Copy(data.Bytes, bytes, 0, Convert.ToInt32(data.Length));
            //    return bytes;
            //}
            return null;
        }

        public string Capture(string filename)
        {
            //return string.Empty;
            var capture = UIScreen.MainScreen.Capture();

            var imagePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            imagePath = Path.Combine(imagePath, "share");
            if (!System.IO.Directory.Exists(imagePath.ToString()))
            {
                Directory.CreateDirectory(imagePath);
            }
            imagePath = Path.Combine(imagePath, filename);

            using (NSData dataTemp = capture.AsPNG())
            {
                var bytes = new byte[dataTemp.Length];
                Marshal.Copy(dataTemp.Bytes, bytes, 0, Convert.ToInt32(dataTemp.Length));
                //byte[] imageBytes = System.Convert.FromBase64String(momentResponse.imagedata);
                NSData data = NSData.FromArray(bytes);
                NSError err = null;
                data.Save(imagePath, false, out err);
            }

            return imagePath;
        }
    }
    
}