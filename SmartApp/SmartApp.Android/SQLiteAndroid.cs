using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SmartApp.Droid;
using SmartApp;
using SQLite;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(SQLiteAndroid))]//注入SQLiteAndroid
namespace SmartApp.Droid
{
    public class SQLiteAndroid : ISQLite
    {
        private static string path;

        private static SQLiteAsyncConnection connectionAsync;

        private static readonly object locker = new object();
        private static readonly object pathLocker = new object();

        private static string GetDatabasePath()
        {
            lock (pathLocker)
            {
                if (path == null)
                {
                    const string sqliteFilename = "iroai.db3";
                    string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments); // Documents folder
                    path = Path.Combine(documentsPath, sqliteFilename);
                }
            }
            return path;
        }

        public SQLiteAsyncConnection GetAsyncConnection()
        {
            lock (locker)
            {
                if (connectionAsync == null)
                {
                    var dbPath = GetDatabasePath();
                    connectionAsync = new SQLiteAsyncConnection(dbPath);
                }
            }
            return connectionAsync;
        }
    }
}