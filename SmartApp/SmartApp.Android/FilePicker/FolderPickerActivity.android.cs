using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using System;
using System.Net;

namespace SmartApp.FilePicker
{
    /// <summary>
    /// Activity that is shown in order to start Android file picking using ActionGetContent
    /// intent.
    /// </summary>
    [Activity(ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    [Preserve(AllMembers = true)]
    public class FolderPickerActivity : Activity
    {

        /// <summary>
        /// Event that gets signaled when file has successfully been picked
        /// </summary>
        internal static event EventHandler<Android.Net.Uri> FolderPicked;

        /// <summary>
        /// Event that gets signaled when file picking has been cancelled by the user
        /// </summary>
        internal static event EventHandler<Exception> FolderPickCancelled;

        /// <summary>
        /// This variable gets passed when the request for the permission to access storage
        /// gets send and then gets again read whne the request gets answered.
        /// </summary>
        private const int RequestStorage = 1;


        /// <summary>
        /// Android context to be used for opening file picker
        /// </summary>
        private Context context;


        /// <summary>
        /// Called when activity is about to be created; immediately starts file picker intent
        /// when permission is available, otherwise requests permission on API level >= 23 or
        /// throws an error if the API level is below.
        /// </summary>
        /// <param name="savedInstanceState">saved instance state; unused</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.context = Application.Context;

            if (this.context.PackageManager.CheckPermission(
                Manifest.Permission.ReadExternalStorage,
                this.context.PackageName) == Permission.Granted)
            {
                this.StartPicker();
            }
            else
            {
                if ((int)Build.VERSION.SdkInt >= 23)
                {
                    this.RequestPermissions(new string[] { Manifest.Permission.ReadExternalStorage }, RequestStorage);
                }
                else
                {
                    throw new InvalidOperationException(
                        "Android permission READ_EXTERNAL_STORAGE is missing and API level lower than 23, so it can't be requested");
                }
            }
        }

        /// <summary>
        /// Receives the answer from the dialog that asks for the READ_EXTERNAL_STORAGE permission
        /// and starts the FilePicker if it's granted or otherwise closes this activity.
        /// </summary>
        /// <param name="requestCode">requestCode; shows us that the dialog we requested is responsible for this answer</param>
        /// <param name="permissions">permissions; unused</param>
        /// <param name="grantResults">grantResults; contains the result of the dialog to request the permission</param>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == RequestStorage)
            {
                if (grantResults.Length > 0 &&
                    grantResults[0] == Permission.Granted)
                {
                    this.StartPicker();
                }
                else
                {
                    OnFolderPickCancelled();
                    this.Finish();
                }
            }
        }

        /// <summary>
        /// Sends an intent to start the FilePicker
        /// </summary>
        private void StartPicker()
        {
            var intent = new Intent(Intent.ActionOpenDocumentTree);
            try
            {
                this.StartActivityForResult(intent, 8000);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        /// <summary>
        /// Called when activity started with StartActivityForResult() returns.
        /// </summary>
        /// <param name="requestCode">request code used in StartActivityForResult()</param>
        /// <param name="resultCode">result code</param>
        /// <param name="data">intent data from file picking</param>
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Canceled)
            {
                // Notify user file picking was cancelled.
                OnFolderPickCancelled();
                this.Finish();
            }
            else
            {
                try
                {
                    if (data?.Data == null)
                    {
                        throw new Exception("Folder picking returned no valid data");
                    }

                    System.Diagnostics.Debug.Write(data.Data);


                    var uri = data.Data;
                    var docUri = DocumentsContract.BuildDocumentUriUsingTree(uri, DocumentsContract.GetTreeDocumentId(uri));
                    OnFolderPicked(docUri);
                }
                catch (Exception readEx)
                {
                    System.Diagnostics.Debug.Write(readEx);

                    // Notify user file picking failed.
                    FolderPickCancelled?.Invoke(this, readEx);
                }
                finally
                {
                    this.Finish();
                }
            }
        }

        /// <summary>
        /// Signals event that file picking was cancelled
        /// </summary>
        private static void OnFolderPickCancelled()
        {
            FolderPickCancelled?.Invoke(null, null);
        }

        /// <summary>
        /// Signals event that file picking has finished
        /// </summary>
        /// <param name="args">file picker event args</param>
        private static void OnFolderPicked(Android.Net.Uri args)
        {
            FolderPicked?.Invoke(null, args);
        }
    }
}
