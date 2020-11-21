using System.IO;
using Android.Content.Res;
using SmartApp.Droid;
using SmartApp.Interface;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(DataAndroid))]
namespace SmartApp.Droid
{
    public class DataAndroid : IData
    {
        public string GetData(string filename)
        {
            string content;
            using (StreamReader sr = new StreamReader(Forms.Context.Assets.Open(filename)))
            {
                content = sr.ReadToEnd();
            }
            return content;
        }

        public byte[] ReadBinary(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            byte[] byteData = new byte[fs.Length];
            fs.Read(byteData, 0, byteData.Length);
            fs.Close();
            return byteData;
        }
    }
}