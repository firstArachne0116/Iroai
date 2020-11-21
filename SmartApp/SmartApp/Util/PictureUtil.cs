using SmartApp.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SmartApp.Util
{
    public class PictureUtil
    {
        static PictureUtil basePicture;
        public static PictureUtil Current
        {
            get { return basePicture ?? (basePicture = new PictureUtil()); }
        }
        
        static PictureUtil()
        {
        }

        public async System.Threading.Tasks.Task<bool> SaveImage(string strDir,string fileName,ImageSource imageinfo)
        {
           bool rest =await DependencyService.Get<IPicture>().SaveImage(strDir, fileName, imageinfo);
           return rest;
        }
    }
}
