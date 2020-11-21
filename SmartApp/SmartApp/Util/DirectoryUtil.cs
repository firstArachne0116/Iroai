using SmartApp.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SmartApp
{
    public class DirectoryUtil
    {
        static DirectoryUtil baseDirecotry;
        public static DirectoryUtil Current
        {
            get { return baseDirecotry ?? (baseDirecotry = new DirectoryUtil()); }
        }
        private static readonly string directory;
        static DirectoryUtil()
        {
            if (directory == null)
                directory = DependencyService.Get<IDirectory>().GetDirectory();
        }

        public string GetDir()
        {
            return directory;
        }
    }
}
