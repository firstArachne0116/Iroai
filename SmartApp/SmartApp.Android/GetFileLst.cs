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
using SmartApp.Model;

[assembly: Xamarin.Forms.Dependency(typeof(GetFileLst))]//DirectoryAndroid

namespace SmartApp.Droid
{
    public class GetFileLst : IGetFileLst
    {
        public List<string> GetFoderList()
        {
            List<string> retList = new List<string>();

            try
            {
                string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

                if (!System.IO.Directory.Exists(documentsPath+"/iroai"))
                {
                    System.IO.Directory.CreateDirectory(documentsPath + "/iroai");
                }

                if (!System.IO.Directory.Exists(documentsPath + "/iroai/Export"))
                {
                    System.IO.Directory.CreateDirectory(documentsPath + "/iroai/Export");
                }

                if (!System.IO.Directory.Exists(documentsPath + "/iroai/Import"))
                {
                    System.IO.Directory.CreateDirectory(documentsPath + "/iroai/Import");
                }

                foreach(string filepath in System.IO.Directory.GetDirectories(documentsPath + "/iroai"))
                {
                    retList.Add(filepath);
                }
            }
            catch
            {

            }

            return retList;
        }
    }
}