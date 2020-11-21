using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation;
using SmartApp.iOS;
using UIKit;
using SQLite;
using System.IO;


[assembly: Xamarin.Forms.Dependency(typeof(SQLiteIOS))]
namespace SmartApp.iOS
{
    public class SQLiteIOS : ISQLite
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

                    path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), sqliteFilename);

                    //var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
                    //var libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
                    //path = Path.Combine(libraryPath, sqliteFilename);
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