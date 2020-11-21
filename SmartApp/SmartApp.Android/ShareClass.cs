using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using SmartApp.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using SmartApp.Interface;
using System.Threading.Tasks;
using Android.Net;
using Java.IO;

[assembly: Dependency(typeof(ShareClass))]
namespace SmartApp.Droid
{
    public class ShareClass : IShare
    {
        private readonly Context _context;
        public ShareClass()
        {
            _context = Android.App.Application.Context;
        }

        public Task Show(string title, string message, string filePath)
        {
            var extension = filePath.Substring(filePath.LastIndexOf(".") + 1).ToLower();
            var contentType = string.Empty;

            // You can manually map more ContentTypes here if you want.
            switch (extension)
            {
                case "pdf":
                    contentType = "application/pdf";
                    break;
                case "png":
                    contentType = "image/png";
                    break;
                case "jpg":
                    contentType = "image/*";
                    break;
                default:
                    contentType = "application/octetstream";
                    break;
            }

            var intent = new Intent(Intent.ActionSend);
            intent.SetType(contentType);
            //intent.PutExtra(Intent.ExtraStream, Uri.Parse("content://" + filePath));
            intent.PutExtra(Intent.ExtraStream, Android.Support.V4.Content.FileProvider.GetUriForFile(_context, _context.PackageName + ".fileprovider", new File(filePath)));
            intent.PutExtra(Intent.ExtraText, string.Empty);
            intent.PutExtra(Intent.ExtraSubject, message ?? string.Empty);

            var chooserIntent = Intent.CreateChooser(intent, title ?? string.Empty);
            chooserIntent.SetFlags(ActivityFlags.ClearTop);
            chooserIntent.SetFlags(ActivityFlags.NewTask);
            _context.StartActivity(chooserIntent);

            return Task.FromResult(true);
        }


        public Task Show(string title, string message, byte[] filearray)
        {
            var intent = new Intent(Intent.ActionSend);
            intent.SetType("image/*");
            intent.PutExtra(Intent.ExtraStream, filearray);
            intent.PutExtra(Intent.ExtraText, string.Empty);
            intent.PutExtra(Intent.ExtraSubject, message ?? string.Empty);

            var chooserIntent = Intent.CreateChooser(intent, title ?? string.Empty);
            chooserIntent.SetFlags(ActivityFlags.ClearTop);
            chooserIntent.SetFlags(ActivityFlags.NewTask);
            _context.StartActivity(chooserIntent);

            return Task.FromResult(true);
        }
    }
}