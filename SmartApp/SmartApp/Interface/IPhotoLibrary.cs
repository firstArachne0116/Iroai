using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace SmartApp.Interface
{
    public interface IPhotoLibrary
    {
        bool SavePhotoAsync(byte[] data, string directory, string folder, string filename, ref string fullpath);

        bool SaveJsonAsync(string data, string directory, string folder, string filename, ref string fullpath);

        Task<int> SaveAsync(string title, string pickedData, byte[] imageData);

        Task<int> ShareAsync(string title, string pickedData, byte[] imageData);

    }
}
