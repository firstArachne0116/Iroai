using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using System.Threading.Tasks;
using Xamarin.Forms;
using SmartApp.Interface;
using SmartApp.iOS;
using System.Threading;
using ObjCRuntime;
using System.IO;
using SmartApp.iOS.Classes;
using AssetsLibrary;
using Plugin.Media.Abstractions;
using System.IO.Compression;
using MobileCoreServices;

[assembly: Dependency(typeof(PhotoLibrary))]
namespace SmartApp.iOS
{
    public class PhotoLibrary : IPhotoLibrary
    {
        public bool SavePhotoAsync(byte[] data, string directory, string folder, string filename, ref string fullpath)
        {
            try
            {
                ////code1
                //NSData nsData = NSData.FromArray(data);
                //UIImage image = new UIImage(nsData);
                //TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

                //image.SaveToPhotosAlbum((UIImage img, NSError error) =>
                //{                   
                //    taskCompletionSource.SetResult(error == null);
                //});

                //return taskCompletionSource.Task;


                //Code2
                var AbsolutePath = string.Empty;
                // First, check to see if we have initially asked the user for permission 
                // to access their photo album.
                if (Photos.PHPhotoLibrary.AuthorizationStatus ==
                    Photos.PHAuthorizationStatus.NotDetermined)
                {
                    var status =
                        Plugin.Permissions.CrossPermissions.Current.RequestPermissionsAsync(
                            Plugin.Permissions.Abstractions.Permission.Photos);
                }
                if (Photos.PHPhotoLibrary.AuthorizationStatus ==
                    Photos.PHAuthorizationStatus.Authorized)
                {
                    NSData nsData = NSData.FromArray(data);
                    UIImage image = new UIImage(nsData);
                    TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
                    var dict = new NSDictionary();
                    // This bit of code saves to the Photo Album with metadata
                    ALAssetsLibrary library = new ALAssetsLibrary();
                    library.WriteImageToSavedPhotosAlbum(image.CGImage, dict, (assetUrl, error) =>
                    {
                        AbsolutePath = assetUrl.ToString();
                        library.AssetForUrl(assetUrl, delegate (ALAsset asset)
                        {
                            AbsolutePath = assetUrl.LastPathComponent;
                            ALAssetRepresentation representation = asset.DefaultRepresentation;
                            if (representation != null)
                            {
                                string fileName = representation.Filename != null ? representation.Filename : string.Empty;
                                var filePath = assetUrl.ToString();
                                var extension = filePath.Split('.')[1].ToLower();
                                var mimeData = string.Format("image/{0}", extension);
                                var mimeType = mimeData.Split('?')[0].ToLower();
                                var documentName = assetUrl.Path.ToString().Split('/')[1];
                                taskCompletionSource.SetResult(error == null);
                            }
                        }, delegate (NSError err)
                        {
                            Console.WriteLine("User denied access to photo Library... {0}", err);
                        });
                    });
                    fullpath = AbsolutePath;
                    return true;
                }
                else
                {
                    return false;
                }


                //Code3
                //bool success = false;

                //NSData imgData = NSData.FromArray(data);
                //UIKit.UIImage photo = new UIImage(imgData);

                //var savedImageFilename = System.IO.Path.Combine(directory, "temp");
                //savedImageFilename = System.IO.Path.Combine(savedImageFilename, filename);
                //NSFileManager.DefaultManager.CreateDirectory(savedImageFilename, true, null);

                //if (System.IO.Path.GetExtension(filename).ToLower() == ".png")
                //    imgData = photo.AsPNG();
                //else
                //    imgData = photo.AsJPEG(100);

                ////File.WriteAllBytes(savedImageFilename, data);

                //NSError err = null;
                //success = imgData.Save(savedImageFilename, NSDataWritingOptions.Atomic, out err);

                //return Task.FromResult(success);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SaveJsonAsync(string data, string directory, string folder, string filename, ref string fullpath)
        {
            return true;
        }

        /// <summary>
        /// save image data to zip file
        /// </summary>
        /// <param name="title"></param>
        /// <param name="pickedData"></param>
        /// <param name="imageData"></param>
        /// <returns></returns>
        public Task<int> ShareAsync(string title, string pickedData, byte[] imageData)
        {
            return Task.FromResult(1);
        }


        public async Task<int> SaveAsync(string title, string pickedData, byte[] imageData)
        {

            using (var memStream = new MemoryStream())
            {

                using (ZipArchive archive = new ZipArchive(memStream, ZipArchiveMode.Create))
                {
                    var entry1 = archive.CreateEntry("data.json");
                    using (StreamWriter writer = new StreamWriter(entry1.Open()))
                    {
                        writer.WriteLine(pickedData);
                    }

                    var entry2 = archive.CreateEntry("image.png");
                    using (Stream writer = entry2.Open())
                    {
                        writer.Write(imageData);
                    }
                }

                var filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                filePath = Path.Combine(filePath, "iroai");
                if (!System.IO.Directory.Exists(filePath.ToString()))
                {
                    Directory.CreateDirectory(filePath);
                }
                filePath = Path.Combine(filePath, title + ".zip");

                System.Diagnostics.Debug.Write(filePath);

                NSData data = NSData.FromArray(memStream.ToArray());
                NSError err = null;
                data.Save(filePath, false, out err);



                var items = new NSObject[] { NSUrl.FromFilename(filePath) };
                var activityController = new UIActivityViewController(items, null);
                var vc = GetActiveViewController();

                NSString[] excludedActivityTypes = null;

                if (excludedActivityTypes != null && excludedActivityTypes.Length > 0)
                    activityController.ExcludedActivityTypes = excludedActivityTypes;

                await vc.PresentViewControllerAsync(activityController, true);



            }

            return 0;
        }


        /// <summary>
        /// Finds active view controller to use to present document picker
        /// </summary>
        /// <returns>view controller to use</returns>
        private static UIViewController GetActiveViewController()
        {
            UIWindow window = UIApplication.SharedApplication.KeyWindow;
            UIViewController viewController = window.RootViewController;

            while (viewController.PresentedViewController != null)
            {
                viewController = viewController.PresentedViewController;
            }

            return viewController;
        }
    }
}