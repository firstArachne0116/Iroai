using SmartApp.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SmartApp.Util
{
    public class DataUtil
    {
        static DataUtil baseData;
        public static DataUtil Current
        {
            get { return baseData ?? (baseData = new DataUtil()); }
        }
        static DataUtil()
        {
        }

        public string GetData(string fileName)
        {
            string data = DependencyService.Get<IData>().GetData(fileName);
            return data;
        }
    }
}
