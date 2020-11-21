using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SmartApp.Interface
{
    public interface IShare
    {
        Task Show(string title, string message, string filePath);

        Task Show(string title, string message, byte[] fileArray);
    }
}
