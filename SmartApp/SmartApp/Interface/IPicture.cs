using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SmartApp.Interface
{
    public interface IPicture
    {
        Task<bool> SaveImage(string directory, string filename, ImageSource img);
    }
}
