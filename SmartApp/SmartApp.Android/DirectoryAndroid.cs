using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SmartApp.Droid;
using SmartApp;
using SQLite;
using System.IO;
using SmartApp.Interface;
using Android.OS;
using Java.IO;
using Environment = Android.OS.Environment;
using File = Java.IO.File;

[assembly: Xamarin.Forms.Dependency(typeof(DirectoryAndroid))]//DirectoryAndroid
namespace SmartApp.Droid
{
    public class DirectoryAndroid : IDirectory
    {
        public string GetDirectory()
        {
            //string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments); // Documents folder
            File picturesDirectory = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures);

            return picturesDirectory.ToString();
        }
    }
}