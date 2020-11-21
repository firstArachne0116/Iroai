using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation;
using UIKit;

using System.IO;
using SmartApp.iOS;
using SmartApp.Interface;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(DataIOS))]
namespace SmartApp.iOS
{
    public class DataIOS : IData
    {
        public string GetData(string filename)
        {
            string content;
            using (StreamReader sr = new StreamReader(filename))
            {
                content = sr.ReadToEnd();
            }
            return content;
        }

        public byte[] ReadBinary(string filename)
        {
            return null;
        }
    }
}