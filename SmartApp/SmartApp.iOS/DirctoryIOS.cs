using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation;
using UIKit;

using SmartApp.iOS;
using SmartApp;
using System.IO;
using SmartApp.Interface;


[assembly: Xamarin.Forms.Dependency(typeof(DirctoryIOS))]//DirectoryAndroid
namespace SmartApp.iOS
{
    public class DirctoryIOS : IDirectory
    {
        public string GetDirectory()
        {
            //string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments); // Documents folder
            string picturesDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            return picturesDirectory;
        }
    }
}