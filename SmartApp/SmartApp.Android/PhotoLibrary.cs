using Android.App;
using Android.Content;
using Android.Provider;
using SmartApp.Droid;
using SmartApp.FilePicker;
using SmartApp.Interface;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: UsesPermission(Android.Manifest.Permission.ReadExternalStorage)]
[assembly: UsesPermission(Android.Manifest.Permission.WriteExternalStorage)]
[assembly: UsesPermission(Android.Manifest.Permission.WriteGservices)]
[assembly: Dependency(typeof(PhotoLibrary))]
namespace SmartApp.Droid
{
    public class PhotoLibrary : IPhotoLibrary
    {

        /// <summary>
        /// Android context to use for picking
        /// </summary>
        private readonly Context context;

        /// <summary>
        /// Request ID for current picking call
        /// </summary>
        private int requestId;

        /// <summary>
        /// Task completion source for task when finished picking
        /// </summary>
        private TaskCompletionSource<Android.Net.Uri> completionSource;

        // Saving photos requires android.permission.WRITE_EXTERNAL_STORAGE in AndroidManifest.xml

        public PhotoLibrary()
        {
            this.context = Android.App.Application.Context;
        }

        public bool SavePhotoAsync(byte[] data, string directory, string folder, string filename, ref string fullpath)
        {
            try
            {
                //File picturesDirectory = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures);
                Java.IO.File folderDirectory = null;

                if (!string.IsNullOrEmpty(folder))
                {
                    folderDirectory = new Java.IO.File(directory, folder);
                    folderDirectory.Mkdirs();
                }

                using (Java.IO.File bitmapFile = new Java.IO.File(folderDirectory, filename))
                {
                    bitmapFile.CreateNewFile();

                    using (Java.IO.FileOutputStream outputStream = new Java.IO.FileOutputStream(bitmapFile))
                    {
                        outputStream.Write(data);
                    }
                    fullpath = bitmapFile.AbsolutePath;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool SaveJsonAsync(string data, string directory, string folder, string filename, ref string fullpath)
        {
            try
            {
                Java.IO.File folderDirectory = null;

                if (!string.IsNullOrEmpty(folder))
                {
                    folderDirectory = new Java.IO.File(directory, folder);
                    folderDirectory.Mkdirs();
                }

                FileStream fileOS = new FileStream(directory + "/" + folder + "/" + filename, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);

                Java.IO.BufferedWriter buf1 = new Java.IO.BufferedWriter(new Java.IO.OutputStreamWriter(fileOS));
                buf1.Write(data, 0, data.Length);
                buf1.Flush();
                buf1.Close();

            }
            catch
            {
                return false;
            }
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

            var intent = new Intent(Intent.ActionSend);
            intent.SetType("application/zip");

            //intent.PutExtra(Intent.ExtraStream, imageData);

            var zipFile = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), title + ".zip");

            if (File.Exists(zipFile))
            {
                File.Delete(zipFile);
            }

            using (ZipArchive archive = ZipFile.Open(zipFile, ZipArchiveMode.Create))
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

            intent.PutExtra(Intent.ExtraStream, Android.Support.V4.Content.FileProvider.GetUriForFile(context, context.PackageName + ".fileprovider", new Java.IO.File(zipFile)));
            intent.PutExtra(Intent.ExtraText, string.Empty);
            intent.PutExtra(Intent.ExtraSubject, title + ".zip");

            var chooserIntent = Intent.CreateChooser(intent, "Export to");
            chooserIntent.SetFlags(ActivityFlags.ClearTop);
            chooserIntent.SetFlags(ActivityFlags.NewTask);
            this.context.StartActivity(chooserIntent);


            return Task.FromResult(1);
        }

        public async Task<int> SaveAsync(string title, string pickedData, byte[] imageData)
        {
            // var uri = await PickFolderAsync();
            var uri = await CreateDocumentAsync("application/zip", title + ".zip");

            if (uri == null)
            {
                return 1;
            }

            try
            {
                //var zipUri = DocumentsContract.CreateDocument(this.context.ContentResolver, uri, "application/zip", title + ".zip");
                var zipUri = uri;

                using (var stream = this.context.ContentResolver.OpenOutputStream(zipUri, "w"))
                {

                    using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Create))
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
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Exception file: " + ex.ToString());
                return -1;
            }

            return 0;
        }

        private Task<Android.Net.Uri> CreateDocumentAsync(string docType, string docName)
        {

            var id = this.GetRequestId();

            var ntcs = new TaskCompletionSource<Android.Net.Uri>(id);

            var previousTcs = Interlocked.Exchange(ref this.completionSource, ntcs);
            if (previousTcs != null)
            {
                previousTcs.TrySetResult(null);
            }

            try
            {
                var createDocIntent = new Intent(this.context, typeof(CreateDocumentActivity));
                createDocIntent.SetFlags(ActivityFlags.NewTask);
                createDocIntent.PutExtra(CreateDocumentActivity.ExtraDocType, docType);
                createDocIntent.PutExtra(CreateDocumentActivity.ExtraDocName, docName);

                this.context.StartActivity(createDocIntent);

                EventHandler<Android.Net.Uri> handler = null;
                EventHandler<Exception> cancelledHandler = null;

                handler = (s, e) =>
                {
                    var tcs = Interlocked.Exchange(ref this.completionSource, null);

                    CreateDocumentActivity.FolderPickCancelled -= cancelledHandler;
                    CreateDocumentActivity.FolderPicked -= handler;

                    tcs?.SetResult(e);
                };

                cancelledHandler = (s, e) =>
                {
                    var tcs = Interlocked.Exchange(ref this.completionSource, null);

                    CreateDocumentActivity.FolderPickCancelled -= cancelledHandler;
                    CreateDocumentActivity.FolderPicked -= handler;

                    if (e != null)
                    {
                        tcs?.SetException(e);
                    }
                    else
                    {

                        tcs?.SetResult(null);
                    }

                };

                CreateDocumentActivity.FolderPickCancelled += cancelledHandler;
                CreateDocumentActivity.FolderPicked += handler;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
                this.completionSource.SetException(ex);
            }

            return this.completionSource.Task;
        }

        private Task<Android.Net.Uri> PickFolderAsync()
        {

            var id = this.GetRequestId();

            var ntcs = new TaskCompletionSource<Android.Net.Uri>(id);

            var previousTcs = Interlocked.Exchange(ref this.completionSource, ntcs);
            if (previousTcs != null)
            {
                previousTcs.TrySetResult(null);
            }

            try
            {
                var pickerIntent = new Intent(this.context, typeof(FolderPickerActivity));
                pickerIntent.SetFlags(ActivityFlags.NewTask);

                this.context.StartActivity(pickerIntent);

                EventHandler<Android.Net.Uri> handler = null;
                EventHandler<Exception> cancelledHandler = null;

                handler = (s, e) =>
                {
                    var tcs = Interlocked.Exchange(ref this.completionSource, null);

                    FolderPickerActivity.FolderPickCancelled -= cancelledHandler;
                    FolderPickerActivity.FolderPicked -= handler;

                    tcs?.SetResult(e);
                };

                cancelledHandler = (s, e) =>
                {
                    var tcs = Interlocked.Exchange(ref this.completionSource, null);

                    FolderPickerActivity.FolderPickCancelled -= cancelledHandler;
                    FolderPickerActivity.FolderPicked -= handler;

                    if (e != null)
                    {
                        tcs?.SetException(e);
                    }
                    else
                    {

                        tcs?.SetResult(null);
                    }

                };

                FolderPickerActivity.FolderPickCancelled += cancelledHandler;
                FolderPickerActivity.FolderPicked += handler;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
                this.completionSource.SetException(ex);
            }

            return this.completionSource.Task;
        }

        /// <summary>
        /// Returns a new request ID for a new call to PickFile()
        /// </summary>
        /// <returns>new request ID</returns>
        private int GetRequestId()
        {
            int id = this.requestId;

            if (this.requestId == int.MaxValue)
            {
                this.requestId = 0;
            }
            else
            {
                this.requestId++;
            }

            return id;
        }
    }
}